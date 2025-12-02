using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CrawlGate : MonoBehaviour
{
    [Header("References")]
    public BoxCollider gateColliderFront;
    public BoxCollider gateColliderBack;
    public string playerTag = "Player";

    [Header("Crawl")]
    [Tooltip("CharacterController.height <= 此阈值视为匍匐/钻洞")]
    public float crawlHeight = 0.35f;

    BoxCollider triggerBox;

    void Reset()
    {
        // 自动把自己改成 Trigger
        triggerBox = GetComponent<BoxCollider>();
        triggerBox.isTrigger = true;
    }

    void OnValidate()
    {
        triggerBox = GetComponent<BoxCollider>();
        if (triggerBox) triggerBox.isTrigger = true;
    }

    void Awake()
    {
        triggerBox = GetComponent<BoxCollider>();
        if (!triggerBox.isTrigger)
        {
            //Debug.LogWarning($"[CrawlGate] {name}: BoxCollider was not Trigger, fixing.");
            triggerBox.isTrigger = true;
        }

        var rb = GetComponent<Rigidbody>();
        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"[CrawlGate] ENTER by {other.name} (tag={other.tag})");
    }

    void OnTriggerStay(Collider other)
    {
        Transform root = other.transform.root;
        if (!root.CompareTag(playerTag)) return;

        // Tell the player to stay crouched while inside this trigger
        var playerCtrl = root.GetComponent<FPSPlayerControllerIS>();
        if (playerCtrl) playerCtrl.SetCrawlLock(true);

        var cc = root.GetComponent<CharacterController>();
        if (!cc) return;

        bool isCrawling = cc.height <= crawlHeight;

        if (gateColliderFront && gateColliderBack)
        {
            bool shouldEnable = !isCrawling; // block passage when not crawling
            // (Use OR here so either collider out-of-sync still gets corrected)
            if (gateColliderFront.enabled != shouldEnable || gateColliderBack.enabled != shouldEnable)
            {
                gateColliderFront.enabled = shouldEnable;
                gateColliderBack.enabled  = shouldEnable;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.transform.root.CompareTag(playerTag)) return;

        // Allow standing again after fully leaving the hole
        var playerCtrl = other.transform.root.GetComponent<FPSPlayerControllerIS>();
        if (playerCtrl) playerCtrl.SetCrawlLock(false);

        if (gateColliderFront && gateColliderBack)
        {
            gateColliderFront.enabled = true;
            gateColliderBack.enabled  = true;
        }
    }

}
