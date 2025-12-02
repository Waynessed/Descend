using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class MonsterChase : MonoBehaviour
{
    // ========== Static Monster Registry ==========
    private static HashSet<MonsterChase> AllMonsters = new HashSet<MonsterChase>();
    public static int ActiveMonsterCount => AllMonsters.Count;
    [Header("Links")]
    public Transform player;

    [Header("Detection")]
    public float chaseRange = 20f;    // Distance within which monster will chase player

    [Header("Speeds")]
    public float patrolSpeed = 0.7f;  // Patrol
    public float chaseSpeed = 1.45f;  // Direct chase pursuit
    public float minTurnAngleForAnim = 60f;  // Minimum turn angle for animation
    public float turnAroundAngle = 140f;     // Turn around angle

    [Header("Combat")]
    public float grabRange = 1.15f;

    [Header("Animation")]
    public float patrolAnimSpeed = 1f;      // Patrol animation speed
    public float chaseAnimSpeed = 1.2f;     // Chase animation speed

    [Header("Patrol")]
    public float patrolMaxDistance = 12f;    // Patrol radius
    public float preferOriginalPatrolChance = 0.6f; // Chance to prefer original patrol area
    
    [Header("Territory")]
    public float territoryRadius = 20f;      // Monster's territory radius from spawn point
    public bool useTerritorySystem = true;   // Enable territory restrictions

    [Header("Control")]
    public bool aiEnabled = false; // Whether AI starts executing logic
    public GameObject monsterModel; // Monster visual model to hide/show
    public bool hideModelUntilAIEnabled = false; // Hide model until aiEnabled becomes true

    [Header("Multi-Monster Cooperation")]
    public float avoidanceDistance = 4f;           // Minimum distance from other monsters (increased)
    public float neighborDetectionRadius = 8f;     // Detection range for other monsters (increased)
    public float patrolAvoidanceRadius = 10f;      // Avoid other monsters during patrol
    public float avoidanceStrength = 2f;           // How strong the avoidance force is
    public bool enableCooperation = true;          // Enable monster cooperation
    public int maxCooperatingMonsters = 2;         // Maximum number of cooperating monsters

    [Header("Stuck Prevention (Emergency Escape)")]
    public bool enableStuckPrevention = true;      // Enable stuck detection and escape
    public float stuckDetectionTime = 3f;          // Time in seconds to detect stuck (velocity < threshold)
    public float stuckVelocityThreshold = 0.05f;   // Velocity below this is considered stuck
    public float stuckDistanceThreshold = 0.3f;    // Position change below this is considered stuck
    public float escapeForce = 5f;                 // Force to apply when escaping
    public float escapeDuration = 1.5f;            // How long to apply escape force
    public float escapeCooldown = 5f;              // Cooldown after escape before checking again
    public bool useTeleportEscape = false;         // If true, teleport instead of force push

    private enum State { Patrol, Chase }
    private State state;
    private NavMeshAgent agent;
    private Animator anim;
    
    // 巡逻路径记忆
    private Vector3 patrolStartPos;
    private float chaseStartTime = -1f;  // Track when this monster started chasing
    
    // 领地系统
    private Vector3 territoryCenter;  // Center of monster's territory
    
    // 卡模检测和逃脱系统
    private float stuckTimer = 0f;              // Timer for stuck detection
    private Vector3 lastPosition;               // Last position for stuck detection
    private float lastPositionUpdateTime = 0f;  // Time when last position was recorded
    private bool isEscaping = false;            // Whether currently escaping from stuck
    private float escapeTimer = 0f;             // Timer for escape duration
    private float escapeCooldownTimer = 0f;     // Cooldown timer after escape
    private Vector3 escapeDirection = Vector3.zero; // Direction to escape

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        if (anim) anim.applyRootMotion = false;
        
        // Register this monster to the static list
        AllMonsters.Add(this);
    }

    void OnDestroy()
    {
        // Unregister when destroyed
        AllMonsters.Remove(this);
    }

    void Start()
    {
        // Start with patrol
        state = State.Patrol;
        patrolStartPos = transform.position; // Record initial patrol start point
        territoryCenter = transform.position; // Set territory center at spawn position

        // Ensure we start on NavMesh
        if (NavMesh.SamplePosition(transform.position, out var hit, 2f, NavMesh.AllAreas))
            agent.Warp(hit.position);
            
        // Hide monster model initially if configured to do so
        if (hideModelUntilAIEnabled && monsterModel != null && !aiEnabled)
        {
            monsterModel.SetActive(false);
        }
        
        // Initialize stuck detection
        lastPosition = transform.position;
        lastPositionUpdateTime = Time.time;
    }

    void Update()
    {
        // Show monster model when AI becomes enabled
        if (hideModelUntilAIEnabled && monsterModel != null && aiEnabled && !monsterModel.activeSelf)
        {
            monsterModel.SetActive(true);
            Debug.Log("👹 怪物模型已显示");
        }
        
        if (!aiEnabled) return;  // Learning phase doesn't enable AI

        if (!player || !agent.isOnNavMesh) return;

        // Set animation speed based on state
        UpdateAnimationSpeed();

        if (anim) anim.SetFloat("Speed", agent.velocity.magnitude);

        // Simple distance-based detection
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (state)
        {
            case State.Patrol:
                agent.speed = patrolSpeed;
                PatrolTick();
                // Check if player is within chase range
                if (distanceToPlayer <= chaseRange)
                {
                    TransitionToChase();
                    Debug.Log("👹 怪物开始追击玩家！");
                }
                break;

            case State.Chase:
                ChaseTick();
                break;
        }
        
        // Apply emergency avoidance when monsters are too close (only if they're about to collide)
        ApplyEmergencyAvoidance();
        
        // 保底机制：检测卡模并强制脱离
        if (enableStuckPrevention)
        {
            HandleStuckDetectionAndEscape();
        }
    }

    // Update animation speed based on current state
    void UpdateAnimationSpeed()
    {
        if (!anim) return;

        float targetSpeed = 1f;
        switch (state)
        {
            case State.Patrol:
                targetSpeed = patrolAnimSpeed;
                break;
            case State.Chase:
                targetSpeed = chaseAnimSpeed;
                break;
        }
        anim.speed = targetSpeed;
    }

    // ========== State Management Helpers ==========
    
    /// <summary>
    /// Transition to Chase state when player is detected
    /// </summary>
    void TransitionToChase()
    {
        state = State.Chase;
        chaseStartTime = Time.time;
    }
    
    /// <summary>
    /// Transition to Patrol state and reset chase-related variables
    /// </summary>
    void TransitionToPatrolFromChase()
    {
        state = State.Patrol;
        anim?.SetBool("Chasing", false);
        chaseStartTime = -1f;
    }
    
    // State implementations

    void PatrolTick()
    {
        // Smarter patrol: prefer returning to original area sometimes
        if (!agent.hasPath || Arrived())
        {
            Vector3 targetPos;
            
            // Chance to prefer original patrol area
            if (Random.value < preferOriginalPatrolChance)
            {
                // Try to find a point near original start position
                Vector3 offset = Random.insideUnitSphere * patrolMaxDistance;
                offset.y = 0;
                Vector3 preferredPos = patrolStartPos + offset;
                
                if (NavMesh.SamplePosition(preferredPos, out var hit, 5f, NavMesh.AllAreas))
                    targetPos = hit.position;
                else
                    targetPos = RandomPatrolPoint();
            }
            else
            {
                // Completely random patrol
                targetPos = RandomPatrolPoint();
            }
            
            // Apply patrol avoidance to stay away from other monsters
            targetPos = ApplyPatrolAvoidance(targetPos);
            
            // Ensure patrol stays within territory
            if (useTerritorySystem)
            {
                targetPos = ClampToTerritory(targetPos);
            }
            
            agent.SetDestination(targetPos);
        }
    }
    
    /// <summary>
    /// Check if player is within monster's territory
    /// </summary>
    bool IsPlayerInTerritory()
    {
        if (!player) return false;
        float distanceToTerritoryCenter = Vector3.Distance(player.position, territoryCenter);
        return distanceToTerritoryCenter <= territoryRadius;
    }
    
    /// <summary>
    /// Clamp patrol target position to stay within territory
    /// </summary>
    Vector3 ClampToTerritory(Vector3 targetPos)
    {
        float distanceFromTerritoryCenter = Vector3.Distance(targetPos, territoryCenter);
        
        // If target is outside territory, clamp it to the boundary
        if (distanceFromTerritoryCenter > territoryRadius)
        {
            Vector3 directionToTarget = (targetPos - territoryCenter).normalized;
            return territoryCenter + directionToTarget * territoryRadius;
        }
        
        return targetPos;
    }
    
    /// <summary>
    /// Apply avoidance to patrol target to keep monsters separated
    /// Now avoids ALL nearby monsters regardless of state
    /// </summary>
    Vector3 ApplyPatrolAvoidance(Vector3 targetPos)
    {
        Vector3 avoidanceOffset = CalculateAvoidanceOffsetAtRadius(patrolAvoidanceRadius);
        
        // Average the force and apply to target
        if (avoidanceOffset.magnitude > 0.01f)
        {
            targetPos += avoidanceOffset;
            
            // Ensure the adjusted position is still on NavMesh
            if (NavMesh.SamplePosition(targetPos, out var hit, 5f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        
        return targetPos;
    }

    void ChaseTick()
    {
        anim?.SetBool("Chasing", true);

        // Territory check: if player left territory, give up chase
        if (useTerritorySystem && !IsPlayerInTerritory())
        {
            Debug.Log($"👹 玩家离开领地，放弃追踪");
            TransitionToPatrolFromChase();
            return;
        }

        // Speed-based turn factor
        float angle = SignedAngleTo(agent, player.position);
        float turnFactor = Mathf.InverseLerp(15f, 80f, Mathf.Abs(angle));      // No slowdown within 15, 80 deg is 20%
        agent.speed = Mathf.Lerp(chaseSpeed, chaseSpeed * 0.2f, turnFactor);

        // Trigger turn animations
        TriggerTurnAnimations(angle);

        // Cooperative flanking: calculate offset position if multiple monsters are chasing
        Vector3 targetPosition = CalculateCooperativeTarget(player.position);

        // Path reachability check (doors/obstacles)
        SetSmartDestination(targetPosition, out bool reachable);

        if (!reachable)
        {
            // Path unreachable, cancel chase and go to patrol
            Debug.Log($"👹 目标不可达，取消追击");
            TransitionToPatrolFromChase();
            return;
        }

        // If player gets too far away, stop chasing
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > chaseRange)
        {
            Debug.Log($"👹 玩家超出追踪范围，放弃追踪");
            TransitionToPatrolFromChase();
            return;
        }

        // Grab
        if (reachable && Vector3.Distance(transform.position, player.position) <= grabRange)
        {
            TryGrab();
        }
    }

    // Pathfinding

    void SetSmartDestination(Vector3 target, out bool reachable)
    {
        NavMeshPath path = new NavMeshPath();
        reachable = agent.CalculatePath(target, path) && path.status == NavMeshPathStatus.PathComplete;
        agent.SetPath(path);
    }

    bool Arrived() =>
        !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f;

    Vector3 RandomPatrolPoint()
    {
        for (int i = 0; i < 8; i++)
        {
            Vector3 rnd = transform.position + Random.insideUnitSphere * 6f;
            
            // If territory system is enabled, clamp the random point to territory
            if (useTerritorySystem)
            {
                rnd = ClampToTerritory(rnd);
            }
            
            if (NavMesh.SamplePosition(rnd, out var hit, 6f, NavMesh.AllAreas))
                return hit.position;
        }
        return transform.position;
    }

    float SignedAngleTo(NavMeshAgent ag, Vector3 worldTarget)
    {
        Vector3 desired = ag.steeringTarget - transform.position;
        if (desired.sqrMagnitude < 1e-2f) desired = worldTarget - transform.position;
        desired.y = 0;
        float angle = Vector3.SignedAngle(transform.forward, desired.normalized, Vector3.up);
        anim?.SetFloat("TurnAngle", angle);
        return angle;
    }

    void TriggerTurnAnimations(float angle)
    {
        if (Mathf.Abs(agent.velocity.magnitude) > 0.25f) return; // Don't trigger turns when moving too fast
        if (Mathf.Abs(angle) > turnAroundAngle) { anim?.SetTrigger("TurnAround"); return; }
        if (angle > minTurnAngleForAnim) anim?.SetTrigger("TurnRight");
        if (angle < -minTurnAngleForAnim) anim?.SetTrigger("TurnLeft");
    }

    void TryGrab()
    {
        agent.isStopped = true;
        anim?.SetTrigger("Grab");
    }

    // Animation event callbacks
    // Animation events on Monster_grab animation will call these functions
    public void OnGrabHit()
    {
        // Check if hit, trigger damage/defeat/other logic
        // Example:
        // if (Vector3.Distance(transform.position, player.position) < grabRange + 0.2f && HasLOS()) { ... }
    }

    public void OnGrabEnd()
    {
        agent.isStopped = false;
    }

    // ========== Multi-Monster Cooperation & Avoidance ==========

    /// <summary>
    /// Active avoidance: Continuously push monsters apart when they're too close
    /// This is called every frame to prevent monsters from getting stuck together
    /// </summary>
    void ApplyEmergencyAvoidance()
    {
        // Only apply if moving and not recently adjusted
        if (agent.velocity.magnitude < 0.1f) return;
        
        // Check if any monsters are too close
        Collider[] neighbors = Physics.OverlapSphere(transform.position, avoidanceDistance);
        
        Vector3 avoidanceOffset = Vector3.zero;
        bool needsAvoidance = false;
        
        foreach (var col in neighbors)
        {
            MonsterChase otherMonster = col.GetComponent<MonsterChase>();
            if (otherMonster == null || otherMonster == this || !otherMonster.aiEnabled) continue;
            
            Vector3 directionToOther = otherMonster.transform.position - transform.position;
            float distance = directionToOther.magnitude;
            
            // Apply force when too close
            if (distance < avoidanceDistance && distance > 0.1f)
            {
                // Calculate strong separation force
                float forceStrength = avoidanceStrength * (1f - distance / avoidanceDistance);
                
                // If extremely close, apply even stronger force
                if (distance < avoidanceDistance * 0.5f)
                {
                    forceStrength *= 5f;  // Much stronger when very close
                }
                
                Vector3 avoidanceDirection = -directionToOther.normalized;
                avoidanceDirection.y = 0;
                
                avoidanceOffset += avoidanceDirection * forceStrength;
                needsAvoidance = true;
            }
        }
        
        // Apply avoidance by slightly modifying current destination
        if (needsAvoidance && avoidanceOffset.magnitude > 0.1f)
        {
            Vector3 currentDestination = agent.destination;
            Vector3 avoidanceTarget = currentDestination + avoidanceOffset * 2f;
            
            // Check if the avoidance target is on NavMesh
            if (NavMesh.SamplePosition(avoidanceTarget, out var hit, avoidanceDistance, NavMesh.AllAreas))
            {
                // Only update destination if it's significantly different
                if (Vector3.Distance(currentDestination, hit.position) > 0.5f)
                {
                    agent.SetDestination(hit.position);
                }
            }
        }
    }

    /// <summary>
    /// Calculate avoidance offset from nearby monsters
    /// Uses standard avoidance distance
    /// </summary>
    Vector3 CalculateAvoidanceOffset()
    {
        return CalculateAvoidanceOffsetAtRadius(avoidanceDistance);
    }
    
    /// <summary>
    /// Core avoidance calculation with configurable radius
    /// </summary>
    Vector3 CalculateAvoidanceOffsetAtRadius(float maxDistance)
    {
        Vector3 totalForce = Vector3.zero;
        int neighborCount = 0;

        // Check for nearby monsters using sphere cast
        Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborDetectionRadius);
        
        foreach (var col in neighbors)
        {
            MonsterChase otherMonster = col.GetComponent<MonsterChase>();
            if (otherMonster == null || otherMonster == this || !otherMonster.aiEnabled) continue;

            Vector3 directionToOther = otherMonster.transform.position - transform.position;
            float distance = directionToOther.magnitude;

            // Only avoid if too close
            if (distance < maxDistance && distance > 0.1f)
            {
                // Force increases as distance decreases
                float forceMagnitude = avoidanceStrength * (1f - distance / maxDistance);
                Vector3 force = -directionToOther.normalized * forceMagnitude;
                force.y = 0; // Keep horizontal
                totalForce += force;
                neighborCount++;
            }
        }

        // Average the force
        if (neighborCount > 0)
            totalForce /= neighborCount;

        return totalForce;
    }

    Vector3 CalculateCooperativeTarget(Vector3 baseTarget)
    {
        if (!enableCooperation) return baseTarget;

        Vector3 finalTarget = baseTarget;

        // Apply cooperative flanking if multiple monsters chasing (limited to max cooperators)
        List<MonsterChase> chasingMonsters = GetChasingMonsters();
        int cooperatingCount = Mathf.Min(chasingMonsters.Count, maxCooperatingMonsters);
        
        if (cooperatingCount >= 2)
        {
            // Find our index in the cooperating group
            int myIndex = chasingMonsters.IndexOf(this);
            
            // Only apply flanking if we're in the top cooperators
            if (myIndex >= 0 && myIndex < maxCooperatingMonsters)
            {
                // Calculate flanking angle based on our position in the group
                float totalAngle = 90f; // Total spread angle
                float anglePerMonster = totalAngle / cooperatingCount;
                float myOffsetAngle = (myIndex - (cooperatingCount - 1) * 0.5f) * anglePerMonster;

                // Convert angle to world direction
                Vector3 toPlayer = baseTarget - transform.position;
                toPlayer.y = 0;
                Vector3 perpendicular = Vector3.Cross(toPlayer.normalized, Vector3.up);
                
                // Rotate perpendicular by our offset angle
                Quaternion rotation = Quaternion.AngleAxis(myOffsetAngle, Vector3.up);
                Vector3 flankingDirection = rotation * perpendicular;

                // Apply flanking offset
                float flankDistance = 3f;
                finalTarget = baseTarget + flankingDirection * flankDistance;
            }
        }

        // Apply avoidance offset if other monsters nearby
        Vector3 avoidanceOffset = CalculateAvoidanceOffset();
        finalTarget += avoidanceOffset;

        // Ensure the target is on NavMesh
        if (NavMesh.SamplePosition(finalTarget, out var hit, 8f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return baseTarget;
    }

    List<MonsterChase> GetChasingMonsters()
    {
        List<MonsterChase> chasingList = new List<MonsterChase>();
        
        foreach (var monster in AllMonsters)
        {
            if (monster != null && monster.state == State.Chase && monster.aiEnabled)
            {
                chasingList.Add(monster);
            }
        }

        // Sort by chase start time (earliest first) for priority-based cooperation
        chasingList.Sort((a, b) => 
        {
            float timeA = a.chaseStartTime > 0 ? a.chaseStartTime : float.MaxValue;
            float timeB = b.chaseStartTime > 0 ? b.chaseStartTime : float.MaxValue;
            return timeA.CompareTo(timeB);
        });

        return chasingList;
    }
    
    // ========== Stuck Detection and Emergency Escape ==========
    
    /// <summary>
    /// 检测怪物是否卡住，如果卡住则执行逃脱机制
    /// 这是保底机制，确保100%避免怪物被永久卡住
    /// </summary>
    void HandleStuckDetectionAndEscape()
    {
        // 如果正在逃脱中，处理逃脱逻辑
        if (isEscaping)
        {
            escapeTimer -= Time.deltaTime;
            
            if (escapeTimer <= 0f)
            {
                // 逃脱结束，进入冷却期
                isEscaping = false;
                escapeCooldownTimer = escapeCooldown;
                escapeDirection = Vector3.zero;
                
                // 重置路径，恢复正常行为
                if (state == State.Chase && player != null)
                {
                    agent.SetDestination(player.position);
                }
                else if (state == State.Patrol)
                {
                    agent.SetDestination(RandomPatrolPoint());
                }
                
                Debug.Log($"👹 怪物 {gameObject.name} 逃脱完成，恢复正常行为");
                return;
            }
            
            // 执行逃脱移动
            ExecuteEscape();
            return;
        }
        
        // 如果在冷却期，不检测卡模
        if (escapeCooldownTimer > 0f)
        {
            escapeCooldownTimer -= Time.deltaTime;
            return;
        }
        
        // 检测是否卡住
        bool isStuck = CheckIfStuck();
        
        if (isStuck)
        {
            Debug.LogWarning($"⚠️ 怪物 {gameObject.name} 检测到卡模！开始强制脱离...");
            EscapeFromStuck();
        }
    }
    
    /// <summary>
    /// 检测怪物是否处于卡住状态
    /// </summary>
    bool CheckIfStuck()
    {
        float currentVelocity = agent.velocity.magnitude;
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        
        // 每0.5秒更新一次位置记录
        if (Time.time - lastPositionUpdateTime >= 0.5f)
        {
            // 如果速度很低且移动距离很小，认为是卡住
            if (currentVelocity < stuckVelocityThreshold && distanceMoved < stuckDistanceThreshold)
            {
                stuckTimer += Time.time - lastPositionUpdateTime;
            }
            else
            {
                // 如果移动了，重置计时器
                stuckTimer = 0f;
            }
            
            lastPosition = transform.position;
            lastPositionUpdateTime = Time.time;
        }
        
        // 如果卡住时间超过阈值，确认卡住
        return stuckTimer >= stuckDetectionTime;
    }
    
    /// <summary>
    /// 执行逃脱：强制脱离卡住位置
    /// </summary>
    void EscapeFromStuck()
    {
        isEscaping = true;
        escapeTimer = escapeDuration;
        stuckTimer = 0f; // 重置卡模计时器
        
        if (useTeleportEscape)
        {
            // 传送逃脱：传送到附近一个安全位置
            TeleportEscape();
        }
        else
        {
            // 力推逃脱：强制向某个方向移动
            ForcePushEscape();
        }
    }
    
    /// <summary>
    /// 力推逃脱：计算一个安全的逃脱方向并强制移动
    /// </summary>
    void ForcePushEscape()
    {
        // 尝试多个方向，找到最安全的逃脱方向
        Vector3 bestDirection = Vector3.zero;
        float maxDistance = -1f;
        
        // 测试8个方向（包括后退和侧向）
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f; // 每45度一个方向
            Vector3 testDirection = Quaternion.Euler(0, angle, 0) * -transform.forward;
            
            // 检查这个方向上是否有其他怪物阻挡
            bool hasObstacle = false;
            Collider[] obstacles = Physics.OverlapSphere(transform.position + testDirection * 2f, 1f);
            foreach (var col in obstacles)
            {
                if (col.GetComponent<MonsterChase>() != null && col.GetComponent<MonsterChase>() != this)
                {
                    hasObstacle = true;
                    break;
                }
            }
            
            if (!hasObstacle)
            {
                // 检查这个方向是否在NavMesh上
                Vector3 testPos = transform.position + testDirection * escapeForce;
                if (NavMesh.SamplePosition(testPos, out var hit, 5f, NavMesh.AllAreas))
                {
                    float distance = Vector3.Distance(transform.position, hit.position);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        bestDirection = testDirection;
                    }
                }
            }
        }
        
        // 如果找不到好方向，就向后推
        if (bestDirection == Vector3.zero)
        {
            bestDirection = -transform.forward;
        }
        
        escapeDirection = bestDirection.normalized;
        
        // 计算逃脱目标点
        Vector3 escapeTarget = transform.position + escapeDirection * escapeForce;
        
        // 确保目标在NavMesh上
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(escapeTarget, out navHit, escapeForce, NavMesh.AllAreas))
        {
            escapeTarget = navHit.position;
        }
        else
        {
            // 如果不在NavMesh上，尝试在当前方向找一个最近的点
            if (NavMesh.SamplePosition(escapeTarget, out navHit, escapeForce * 2f, NavMesh.AllAreas))
            {
                escapeTarget = navHit.position;
            }
        }
        
        // 设置目标点
        agent.SetDestination(escapeTarget);
        
        Debug.Log($"👹 怪物 {gameObject.name} 使用力推逃脱，方向: {escapeDirection}, 目标: {escapeTarget}");
    }
    
    /// <summary>
    /// 传送逃脱：直接传送到附近安全位置
    /// </summary>
    void TeleportEscape()
    {
        // 尝试找到一个安全位置
        for (int attempt = 0; attempt < 10; attempt++)
        {
            // 在附近随机找一个点
            Vector3 randomOffset = Random.insideUnitSphere * (escapeForce * 2f);
            randomOffset.y = 0;
            Vector3 teleportPos = transform.position + randomOffset;
            
            // 检查这个位置是否安全（没有其他怪物）
            bool isSafe = true;
            Collider[] nearby = Physics.OverlapSphere(teleportPos, 2f);
            foreach (var col in nearby)
            {
                if (col.GetComponent<MonsterChase>() != null && col.GetComponent<MonsterChase>() != this)
                {
                    isSafe = false;
                    break;
                }
            }
            
            // 检查是否在NavMesh上
            NavMeshHit teleportHit;
            if (isSafe && NavMesh.SamplePosition(teleportPos, out teleportHit, 5f, NavMesh.AllAreas))
            {
                // 传送怪物
                agent.Warp(teleportHit.position);
                transform.position = teleportHit.position;
                
                Debug.Log($"👹 怪物 {gameObject.name} 使用传送逃脱，传送到: {teleportHit.position}");
                return;
            }
        }
        
        // 如果找不到安全位置，降级为力推逃脱
        Debug.LogWarning($"⚠️ 传送逃脱失败，降级为力推逃脱");
        ForcePushEscape();
    }
    
    /// <summary>
    /// 执行逃脱移动（在逃脱期间持续调用）
    /// </summary>
    void ExecuteEscape()
    {
        if (escapeDirection == Vector3.zero) return;
        
        // 持续更新目标点，确保逃脱方向正确
        Vector3 escapeTarget = transform.position + escapeDirection * escapeForce;
        
        NavMeshHit escapeHit;
        if (NavMesh.SamplePosition(escapeTarget, out escapeHit, escapeForce, NavMesh.AllAreas))
        {
            // 检查目标点是否仍然合理（距离不能太远）
            if (Vector3.Distance(transform.position, escapeHit.position) <= escapeForce * 1.5f)
            {
                agent.SetDestination(escapeHit.position);
            }
        }
        
        // 如果已经移动了一定距离，提前结束逃脱
        if (Vector3.Distance(transform.position, lastPosition) > stuckDistanceThreshold * 3f)
        {
            // 成功移动，可以提前结束
            escapeTimer = 0.1f; // 快速结束
        }
    }
    
    // ========== Gizmos for Editor Visualization ==========
    
    /// <summary>
    /// Draw debug gizmos in the Unity editor
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draw territory radius (red wireframe sphere)
        if (useTerritorySystem && territoryRadius > 0f)
        {
            Gizmos.color = Color.red;
            Vector3 center = Application.isPlaying ? territoryCenter : transform.position;
            Gizmos.DrawWireSphere(center, territoryRadius);
        }
        
        // Draw chase range (yellow wireframe sphere)
        if (chaseRange > 0f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }
        
        // Draw patrol area (cyan wireframe sphere)
        if (patrolMaxDistance > 0f)
        {
            Gizmos.color = Color.cyan;
            Vector3 patrolCenter = Application.isPlaying ? patrolStartPos : transform.position;
            Gizmos.DrawWireSphere(patrolCenter, patrolMaxDistance);
        }
        
        // Draw grab range (magenta wireframe sphere)
        if (grabRange > 0f)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, grabRange);
        }
        
        // Draw avoidance radius (green wireframe sphere)
        if (avoidanceDistance > 0f)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, avoidanceDistance);
        }
    }
}
