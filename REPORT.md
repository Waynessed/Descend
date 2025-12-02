# Project 2 Report

Read the [project 2
specification](https://github.com/feit-comp30019/project-2-specification) for
details on what needs to be covered here. You may modify this template as you
see fit, but please keep the same general structure and headings.

Remember that you should maintain the Game Design Document (GDD) in the
`README.md` file (as discussed in the specification). We've provided a
placeholder for it [here](README.md).

## Table of Contents

- [Evaluation Plan](#evaluation-plan)
- [Evaluation Report](#evaluation-report)
- [Shaders and Special Effects](#shaders-and-special-effects)
- [Summary of Contributions](#summary-of-contributions)
- [References and External Resources](#references-and-external-resources)

## Evaluation Plan

## 🔍Custom Questionnaire

### 👤Participants

- We will test with at least 5 players who have never seen this game build
- They will be 18+ years old and are comfortable with horror themes to avoid distress
- They have used PC before, and anyone who is prone to motion sickness will be avoided
- We will recruit classmates and friends who are not in the team
- We will keep all data anonymous

### ❓Why custom questionnaire as querying technique

- Efficient to distribute and collect data
- Comparable across players as the standardised items allows clear analysis patterns
- Mixed question types (scale from 1-5, multiple choice, open-ended response) for both quantitative and qualitative measures)

---

### 🔄Process

1. Share the playable build (itch.io) and questionnaire link (google form) with participants who have provided consent to the evaluation. 
2. Ask each participant to finish one uninterrupted first playthrough without any help before answering the first section so we capture first time clarity of UI cues and core mechanics, then allow them to replay as many times as they like before completing the remaining open-ended questions about difficulty, balance, theme, and overall art style. 
3. Gather responses and review the scaled questions first using auto generated charts to spot patterns, focusing on any low scoring areas. 
4. Then, summarise open-ended responses and follow up with participants if a response is unclear or requires further discussion.
5. Meet as a team to prioritise fixes by impact versus effort. Then, assign changes for the next build. 

---

### 📶Data Collection

1. **Tool**
    - We use Google Form to collect ratings and comments in one place and we auto store results in the linked Google Sheet.
2. **Data quality and consistency**
    - We make the rating questions required so we avoid missing values in the core metrics.
    - We keep open-ended questions optional so participants are not forced to write filler text.
    - We limit the form to one submission per participant so we reduce duplicates.
    - We log date and approximate session length so we can spot outliers and rushed entries.
    - We keep the build version fixed so results are comparable.
3. **Ethics/consent**
    - All responses are anonymous
    - Participant can withdraw anytime
    - No sensitive content
4. **What we collect**
    - Controls and Learnability
        - We collect how players discovered crouch, sprint, pick-ups and whether WASD and E felt natural so we can confirm the tutorial at the beginning actually teaches the basics to navigate the game.
        - This matters for our game as confusion about E interact or sprint breaks the horror pacing and leads to false difficulty.
        - We use the results to tighten prompts, improve tutorial timing, and adjust when and where the E icon appears or is occluded by walls.
        - We use simple recognition questions to verify real understanding and 5-point scale questions to give comparable scores that show whether changes improve learnability across builds.
    - Navigation and minimap
        - We collect whether players understand green, red, and yellow markers and whether the minimap actually helps them move.
        - This matters for our game because the maze can feel unfair if players cannot read the legend.
        - We use the results to fix/increase icons on the minimap.
        - We use quick multiple choice checks to measure actual comprehension and ratings to quantify usefulness.
    - Cues and attention
        - We collect perceptions of the heartbeat proximity cue, the green leak around crawl holes, and the timing and visibility of interaction prompts.
        - This matters for our game because players need strong, readable affordances to choose between hiding, sprinting, or investigating without breaking immersion.
        - We use the results to tune audio ranges, light placement, brightness, and prompt gating so cues pop at the right moment.
    - Overall experience
        - We collect ratings for enjoyment, tension, immersion, fairness, readability, and perceived difficulty to judge whether the core loop lands as intended.
        - This matters for our game because we have to balance sustained pressure with genuine fun.
        - We use the results to decide whether to adjust sanity drain, movement speed, stamina, monster movement etc.

### The Final Questionnaire

https://docs.google.com/forms/d/e/1FAIpQLSfUvOSdpt1mZ_9V1RY1036tXAJwNEOtq-G7E_D5CPjtx7lxnw/viewform?usp=sharing&ouid=117018929149020335883

---

### ✏️Data analysis

1. **Quantitative (scale from 1 to 5, where 1 means strongly disagree and 5 means strongly agree)**
    - The responses will be visualised as simple bar/pie charts to show the distribution for each item so trends and any outliers are easy to spot.
    - A score of 4 or 5 will be treated as a positive signal for clarity or experience, while a score of 1 or 2 will flag a likely issue.
    - If at least 75% of players choose 4 or 5 for an item, we will consider that area acceptable for this build and focus effort elsewhere.
    - If more than 25% of players choose 1 or 2 for an item, we will mark it as a candidate for change and follow a short fix loop.
        1. We will check if the problem could be related to device or setup such as resolution or FOV.
        2. We will identify the specific cause in the game such as unclear prompts, weak contrast, or timing.
        3. We will list two or three practical solutions and estimate effort versus impact.
        4. We will implement the safest and most efficient option first and retest on a small group.
2. **Qualitative (open-ended answers and observation notes)**
    - The open-ended answers will be coded into a small set of themes such as unclear prompts, getting lost, minimap issues, speed or sanity tuning, and monster fairness so we can see patterns without losing player voice.
    - We will mark “critical incidents” where players got stuck and note the location or moment so fixes can target exact places in the maze.
    - We will check whether the qualitative themes line up with low scoring items so we know which issues are both common and strongly felt.
3. **Decision rules (how we turn data into changes)**
    - We will prioritise issues that fail the 75% positive rule or exceed the 25% negative rule and that also appear in the top qualitative themes so our changes address the biggest pain points.
    - We will choose at most three to five changes per iteration so we can isolate effects and avoid creating new confounds.

---

### ⌛Timeline

October 28: Finalise the questionnaire and find participants

October 29-30: Run play sessions and collect questionnaire responses

November 1-4: Analyse responses and implement the selected changes

---

## 💬Post-task Walkthrough

### 👤Participants

- We will test with at least 5 different players who have never seen this game build
- They will be 18+ years old and are comfortable with horror themes to avoid distress
- They have used PC before, and anyone who is prone to motion sickness will be avoided
- We will recruit classmates and friends who are able to meet in person/share screen while playing the game
- We will keep all data anonymous

### 🔒Consistent Environments

- Same PC build for all
- 1080p windowed
- Default mouse sensitivity
- Headphones
- Quiet room

### ❓Why post-task walkthrough as the observational technique

- We use post-task walkthrough because it captures what the player was thinking at key moments immediately after play, giving us insight without interrupting immersion during the horror experience.
- We prefer this method over pure think-aloud because speaking during tense sequences can blunt fear and change behaviour, whereas a guided recap preserves the game’s pacing and emotion.
- We can anchor questions to concrete moments such as the first time they noticed the crawl hole glow, the point they chose to sprint, and the path they took toward the supposed exit so fixes target exact locations.
- We gather both what happened and why it happened and asking the player to explain expectations, confusions, and alternatives they considered.
- We keep sessions lightweight and repeatable so at least five players can complete them within our timeline, producing comparable notes that feed directly into design changes.

---

### 📶Process and Data collection

**Method Overview**

- We run one session per participant with a short briefing, one uninterrupted play, and a guided post-task walkthrough.
- We take brief notes as we watch participants play and we revisit in the walkthrough.

**Setup**

- We confirm the participant is comfortable with WASD and mouse and adjust sensitivity or FOV if needed.
- We ask them to play as they normally would and to ignore us unless they want to stop.

**Gameplay Observation**

1. Stage 1 - Game tutorial
    - We note whether the player moves immediately or hesitates on spawn
    - We note any wrong key attempts to move forward or repeated key mashing for interact
    - We note whether they enter the maze directly or wander without a goal
2. Stage 2 - Free exploration
    - We note when they first notice the sanity bar and what behaviour changes after that
    - We note when they first notice the minimap and how often they check it
    - We note visible hesitations, repeated backtracking, or circling behaviours
    - We note any minimap confusion about legend or orientation
3. Stage 3 - Crawl holes and green light reaction
    - We note whether crawl holes are seen and ignored or not seen at all
    - We note the first attempt to crawl and what triggered the attempt
    - We note whether green light near holes attracts attention or is avoided purposely
4. Stage 4 - Safe room interaction
    - We note "E" presses that fail because of distance, angle, or occlusion
    - We note whether the player reads the wall hint or skips it immediately
    - We note whether they understand the toy and winding key mechanic and how long that takes
    - We note whether sanity restore and stamina feedback are noticed and whether Shift sprint is used
5. Stage 5 - Threat encounter
    - We note the first reaction to heartbeat such as stopping, looking around, or sprinting
    - We note whether they combine audio and minimap red signals to decide a direction
    - We note what information seems missing if they are caught
6. Stage 6 - Progress and endings
    - We note whether they realise room writing relates to the true exit
    - We note whether they seek out more rooms after the first few and what strategy they choose

**Post-task walkthrough** 

- We reference the moments and ask what the player was trying to do at each point
- We ask what told them an object was interactive and whether any prompt felt mistimed or hidden.
- We ask how the audio at key moments influenced their decisions
- We ask how the minimap colours were interpreted and whether they ever contradicted world cues
- We ask what first made them try crawling
- We ask which mechanic felt unclear or unfair and what single change would have helped
- We ask what they expected near the “exit” and what convinced them to keep exploring rooms
- We ask if any cues should be added for clearer game interaction

**What data we collect after play**

- We ask short, targeted questions about moments where the player showed confusion or hesitation so we can understand what they were trying to do and why.
- We record outcome markers such as safe rooms reached, deaths, total session length, and whether an ending was reached so difficulty and progress are measurable.
- We capture events such as failed "E" presses, incorrect distance or angle for interact, prompts that were visible but ignored, and repeated backtracking so specific usability issues are clear.

**Tools we use**

- We use simple digital notes to write down noticeable events so our walkthrough questions can target to exact moments.
- We use timer to keep timing consistent across sessions.

**Consistency**

- The same process will be applied to all participants so the data stays comparable.

---

### ✏️Data analysis

We combine what we saw during play with what the player said in the walkthrough

**Decision rules**

- We note suggested fixes and mark whether they are feasible within our scope/time frame.
- We summarise each session with three “top pain points” and one “bright spot” so the team sees what to fix and what to keep.
- We prioritise a theme when at least two players mention it.
- We defer a fix when the theme is minor, rare, or purely stylistic unless it conflicts with the game’s core tone.
- We limit each iteration to three to five changes so we can see the impact and avoid hidden side effects.

---

### ⌛Timeline

October 28: Find participants

October 29-30: Run play sessions and conduct post-talk walkthrough sessions

November 1-4: Analyse responses and implement the selected changes

## Evaluation Report

## 🔍Custom Questionnaire

**Link to responses:** 

https://docs.google.com/forms/d/1oOXGNukHfeCSxy3wO36Xs--2yOViW31DFk9MimTQufw/edit#responses

---

### **Controls and Learnability**

**Question 1:**

How did you learn how to crouch in the game?

**Data analysis:**

2 of 9 participants did not know how to crouch during play. Some tried to look for controls in Settings, which is not intuitive for first time players. This indicates that there needs to be a direct cue.

**Change:**

After the tutorial message **“Press WASD to move”**, add a new hint **“Press C to crouch”** and keep it visible longer for better readability.  

> 💡 *Reason:*  
> The previous version used **Ctrl** as the crouch key, but in the WebGL browser build this triggered the system shortcut **“Ctrl + W”**, which closes the browser tab.  
> To prevent this conflict and improve usability, the crouch key has been changed to **C**.  

This change aligns with standard PC game control conventions (WASD for movement, C for crouch, Shift for sprint) and ensures safe, stable gameplay within a browser environment.

---

**Question 2:**

How did you learn how to sprint in the game?

**Data analysis:**

6 of 9 participants did not know sprinting was available.This is a major design gap because not sprinting affects pacing, chase difficulty, and overall fairness.

**Change:**

After the player collects the winding key in a safe room, show a clear on screen hint: “Press Shift to sprint.” Trigger this for 2–3 seconds so it is readable under pressure.

---

**Question 3:**

How did you learn that you can collect items in the game?

**Data analysis:**

When players reached an interactable item, the “E” icon appeared. 8 of 9 participants correctly understood that “E” means interact/pick up. This behaviour is working as intended.

---

**Question 4:**

The default controls (WASD + mouse) felt natural to navigate the game.

**Data analysis:**

WASD + mouse felt natural to most participants, with a mean rating of 4.11 out of 5. This matches common PC control conventions (left hand on WASD, right hand on mouse).

---

**Question 5:**

After finding the winding key, sprint (Shift key) was easy to discover and use appropriately.

**Data analysis:**

Only 2 of 9 participants discovered sprint after collecting the winding key. This confirms the connection between the item and the action is unclear.

**Change:** 

When the winding key is collected, follow it with a contextual hint that ties the item to the action. This message should appear once on first pickup and clearly explain what the player can do next.

---

### **Navigation and Minimap**

**Question 6:**

What does each of the markers on the minimap represent?

**Data analysis:**

Around 78% participants correctly understood each legend on the minimap and used them as intended. No change needed.  

---

**Question 7:**

I often rely on the minimap when monsters are nearby

**Data analysis:**

7 of 9 participants found the minimap helpful for navigating the maze, and 8 of 9 relied on it when monsters were nearby.

**Change:**

The minimap will be kept visible at all times to support navigation and threat awareness. This improves overall player experience.

---

### Cues and Attention

Overall, there was no issues with cues such as 

- Heartbeat audio to indicate monsters getting closer
- Green light leaks near crawl holes indicate safe rooms/potential areas to explore
- Prompt timing and visibility

So no change needed

---

### Overall Experience

**Question 8:**

The game felt fair. I could tell what to do to survive, even if I failed. 

**Data Analysis:**

7 of 9 players said they understood what to do to survive, which shows the core goal is clear for first time players.

---

**Question 9:**

Colours/contrast/fonts were readable. I experienced no/little eye strain or motion sickness. 

**Data Analysis:** 

Overall control feel worked well for most players.

---

**Question 10:**

Overall, how difficult was the game for first playthrough?

**Data Analysis:**

3 out of 9 people found the game to be some what difficult. 

**Change:**

Despite most responded the sanity drain is fair, this could be a potential feature to adjust to lower the game difficulty. Other things like monster speed, chase logic, and strengthen key cues and hints should be considered to reduce confusion and game difficulty. 

---

### Open-ended Questions

Note: we selected only feedback that is both significant and feasible to implement within the remaining development timeframe.

**Question:**

Describe one moment you felt very lost or stuck. What would have made it clearer (UX clarity, prompts, tutorial, labelling)?  

**Participant 1:**

“Since the demo game runs in a web browser, pressing “Ctrl” to crouch while moving forward with “W” sometimes triggers the “Ctrl + W” shortcut, which closes the web page. Perhaps switching to a different platform could solve this issue…? Or maybe disabling certain shortcuts?”

**Change:**

The crouch key has been remapped to **C** to prevent browser shortcut conflicts and improve playability in WebGL builds.

**Participant 2:**

”Mouse way too sensitive, I just moved 1cm with my mouse and started rotating in game”

**Change:**

Lower the default mouse sensitivity by 30% and add a Sensitivity slider in Settings.

---

**Question:**

What did you expect from this type of game that you couldn't find or couldn't figure out how to do there? 

**Participant:**

“I thought I could sprint already, but then I knew that I have to find it on my own, and I found it hard to learn the sprint and there wasn't a guide of it.”

**Change:**

Highlight the safe room that contains the winding key on the minimap in a different colour from the other safe rooms.

---

**Question:**

How did the player speed, sanity drain, and stamina feel together? Would you change any of them, and why (drain rate, stamina recharge, player speed)? 

**Participant 1:**

”Sanity already dropped by ten points before I even started running.”

**Participant 2:**

“Sanity seems to drop a bit fast. If you don’t quickly find a safe room it can drain away”

**Change:**

Reduce the drain slightly or increase safe room restore amount.

---

**Question:**

In your words, how did the monster feel to play against (speed, route blocking, fairness)? 

**Participant:**

“All good, except the route blocking. When I entered a new area, the monster will automatically find and chase the player. When there is a one-way path, the monster will block the path.”

**Change:**

Adjust the monster’s chase logic so it doesn’t aggro immediately when the player enters a new area, adding a short delay or minimum distance check before pursuit begins.

## 💬Post-task Walkthrough

Note: repeated changes from the questionnaire will not be included here.


**Issue:**

During the game play, 4 participants routed along the outer boundary and reached the false exit without encountering monsters. Some did this out of curiosity to test the map limits, and others did it as the first run felt difficult and they sought an easier path. However, that’s not what we intended. 

**Change:**

Add an invisible wall around the maze to prevent leaving the intended play area.

---

**Issue:**

2 participants triggered a bug where pressing Escape key immediately at the start to either open settings or by accident, allowed free movement during the tutorial phase.

**Change:**

Fix the shortcut bug so the player cannot move while tutorial lock is active, regardless of key pressed.

---

**Issue:**

Some players never found the room with the winding key and therefore never unlocked sprint. A few avoided safe rooms entirely, treating them as risky/unknown areas.

This led to fast sanity drain, and therefore unable to even complete 1/4 of the maze, or simply not enough time to visit all safe rooms within 10 minutes. 

**Change:**

Give the winding key safe room a blue legend on the minimap, which is distinct from all other safe rooms, monsters, and player.

Also add an early hint right at the start of the game, encouraging exploration of green and blue areas. 

---

**Issue:** 

Many players focused only on the minimap, but the square player icon made it hard to tell facing direction. Noticeably, some circled in safe rooms to figure out which way they should leave.

**Change:**

Replace the square with a directional arrow icon so facing is immediately clear.

---

**Issue:**

Most participants played multiple rounds due to early deaths or aiming to unlock both endings. However, rewatching the full tutorial each time caused frustration.

**Change:**

Allow players to press space to skip each tutorial line, speeding up repeat runs.

Overall, key usability issues identified through player testing—such as unclear sprint discovery and browser shortcut conflicts—have been resolved in this build. The updated control scheme (WASD + C + Shift + E) provides a smoother and safer experience for WebGL users.


## Shaders and Special Effects

### Shader Implementations

We have implemented two custom-written, non-trivial HLSL shader programs that enhance the game's visuals and contribute to performance optimization.

#### Selection of Shaders for Assessment

If more than two shaders are developed, the team must explicitly choose exactly two that they wish to be assessed. This selection must be clearly stated in the report.

**We have implemented multiple shaders, but for assessment purposes, we explicitly select the following two shaders:**

1. **FogParticleURP.shader** - Volumetric Fog Particle Shader
2. **TriplanarConcrete.shader** - Triplanar Mapping Surface Shader

---

#### Shader 1: Volumetric Fog Particle Shader

**File Path:** [Assets/Materials/Fog/FogParticleURP.shader](Assets/Materials/Fog/FogParticleURP.shader)

**Description:**
This shader implements a sophisticated volumetric fog effect for particle systems, creating atmospheric depth and visual richness in the game environment. The shader uses custom HLSL code to achieve complex visual effects including:

- **Multi-layer noise sampling**: Combines main noise and detail noise textures with configurable tiling and scrolling for organic, natural-looking fog patterns
- **Parallax/volume ray marching**: Implements a simplified ray marching technique with configurable step counts (1-4 steps) to create volumetric depth perception
- **Soft particle rendering**: Uses depth buffer sampling to create smooth blending between fog particles and scene geometry
- **Height-based density falloff**: Implements vertical gradient control to fade fog at specific world-space heights
- **Blue noise dithering**: Applies blue noise texture sampling for temporal dithering, reducing banding artifacts and improving visual quality
- **Edge feathering**: Uses circular masks with configurable power functions to eliminate visible square edges on billboard particles
- **Camera distance fade**: Implements distance-based fade-out to improve performance and visual quality at long ranges

**Rationale:**
The fog effect is crucial for creating the horror atmosphere in our maze-based game. It obscures visibility, adds tension, and creates a sense of unease as players navigate through the environment. Rather than using simple additive blending, this shader implements advanced techniques that:

1. **Enhance visual quality**: The multi-layer noise and parallax effects create realistic, volumetric fog that feels natural and immersive
2. **Optimize performance**: By using GPU-accelerated depth sampling and efficient ray marching (with WebGL optimizations), the shader reduces CPU load while maintaining visual fidelity
3. **Support gameplay**: The fog serves both aesthetic and gameplay purposes, limiting visibility strategically to enhance the horror experience

The shader includes platform-specific optimizations (WebGL build defines) to ensure compatibility across different deployment targets while maintaining visual quality.

---

#### Shader 2: Triplanar Mapping Surface Shader

**File Path:** [Assets/Materials/Concrete/Concrete008/TriplanarConcrete.shader](Assets/Materials/Concrete/Concrete008/TriplanarConcrete.shader)

**Description:**
This shader implements triplanar texture mapping, a technique that projects textures onto surfaces from three orthogonal directions (X, Y, Z axes) and blends them based on surface normals. This eliminates UV mapping requirements and creates seamless texture application on any geometry.

Key features include:

- **Triplanar projection**: Samples textures from three axes (XZ, XY, YZ planes) and blends them using weighted normal-based mixing
- **PBR integration**: Fully integrated with Unity's Universal Render Pipeline (URP) lighting system, supporting shadows, global illumination, and additional lights
- **Secondary texture blending**: Allows mixing of two different albedo textures using noise-based blending for visual variation
- **Normal mapping support**: Implements triplanar normal mapping with proper tangent space conversion for each projection axis
- **Roughness map support**: Includes configurable roughness mapping with gamma correction and remapping controls
- **Ambient occlusion mapping**: Supports AO texture sampling with intensity control
- **Configurable blend sharpness**: Allows fine-tuning of the triplanar blend transitions for optimal visual quality

**Rationale:**
Triplanar mapping is essential for our game because:

1. **Performance optimization**: By eliminating UV mapping requirements, we can use procedurally generated meshes or modify geometry without worrying about UV coordinates, reducing CPU load during level generation
2. **Visual consistency**: The technique ensures seamless texture application across complex geometry, preventing visible seams and tiling artifacts that would break immersion
3. **Flexibility**: The secondary texture blending and noise-based mixing allow for natural variation in materials like concrete walls, making the maze environment feel more organic and less repetitive
4. **Professional quality**: The PBR integration ensures proper lighting and shadow behavior, maintaining visual quality standards expected in modern games

This shader is particularly valuable for maze environments where walls may be generated or modified dynamically, as it maintains visual quality regardless of geometry complexity.

---

### Particle System Implementation

#### Particle System Selection

**Particle System for Assessment:** Fog Particle System

**Location:** The particle system can be found in the Unity project at:
- **Prefab Path:** [Assets/Prefab/Fog/Fog.prefab](Assets/Prefab/Fog/Fog.prefab)

**How to Locate:**
1. Open the Unity Editor
2. Navigate to the Project window
3. Open the folder structure: `Assets` → `Prefab` → `Fog`
4. Select the `Fog.prefab` file
5. In the Inspector, you will see the ParticleSystem component with all configured modules

Alternatively, if the prefab is instantiated in a scene:
1. Open any game scene (e.g., `Assets/Scenes/`)
2. Search for "Fog" in the Hierarchy window
3. Select the GameObject and examine the ParticleSystem component in the Inspector

---

#### Particle System Description

**Purpose:**
This particle system creates atmospheric fog effects throughout the maze environment. It enhances the horror atmosphere by reducing visibility, creating tension, and adding visual depth to the game world.

**Key Attributes Varied:**

1. **Emission Rate**
   - **Configuration:** Rate over time set to 30 particles per second (with minMaxState allowing variation)
   - **Rationale:** A moderate emission rate ensures dense fog coverage without overwhelming the rendering pipeline. The particles are large billboards, so fewer particles are needed compared to small particle effects.

2. **Start Lifetime**
   - **Configuration:** Lifetime set to 10 seconds (scalar), with minMaxState allowing variation between 5-10 seconds
   - **Rationale:** Longer lifetimes allow particles to accumulate and create dense fog banks. The variation prevents uniform appearance and creates natural fog density variations.

3. **Start Speed**
   - **Configuration:** Speed range between 1-2 units (minMaxState: 3, indicating random between min and max)
   - **Rationale:** Slow-moving particles create a subtle drift effect, simulating natural air movement. The random speed variation adds organic motion to the fog.

4. **Start Size**
   - **Configuration:** Size range between 15-25 units (minMaxState: 3, random between min and max)
   - **Rationale:** Large particle sizes create billboard quads that effectively cover areas. The size variation prevents uniform fog appearance and creates natural density variations.

5. **Shape Module**
   - **Configuration:** Box shape emitter
   - **Rationale:** Box emission allows uniform fog distribution across the maze area, ensuring consistent atmospheric coverage.

**Use of Randomness:**

Randomness is strategically utilized throughout the particle system to create natural, organic fog behavior:

1. **Lifetime Randomization**: Each particle has a random lifetime between 5-10 seconds, preventing synchronized particle death and creating smooth fog density transitions.

2. **Speed Randomization**: Random speeds between 1-2 units create varied movement patterns, simulating natural air currents and preventing uniform fog drift.

3. **Size Randomization**: Random sizes between 15-25 units create natural density variations. Larger particles create denser fog patches, while smaller ones add subtle atmospheric haze.

4. **Auto Random Seed**: Enabled (`autoRandomSeed: 1`) ensures that each instance of the particle system produces different patterns, preventing identical fog appearance across multiple instances.

5. **Particle Color**: Uses vertex colors from the particle system (inherited through the shader), allowing for per-particle color variation based on the particle's lifecycle and position.

**Rationale for Design Choices:**

- **Large Billboard Particles**: Using large billboards (15-25 units) with the custom fog shader is more efficient than using many small particles. Each particle covers a significant area, reducing the total particle count needed.

- **Slow Movement**: The slow speed (1-2 units) creates subtle, atmospheric drift rather than fast-moving clouds, which suits the horror game's tense atmosphere.

- **Random Variation**: Extensive use of randomness prevents the fog from appearing artificial or repetitive, which is crucial for maintaining immersion in a horror game.

- **Custom Shader Integration**: The particle system is designed to work with our custom `FogParticleURP.shader`, which handles the complex visual effects. The particle system itself focuses on emission, lifetime, and basic movement, while the shader handles all the sophisticated rendering techniques.

- **Looped Emission**: The system loops continuously (`looping: 1`) to maintain fog throughout gameplay, creating consistent atmospheric conditions.

- **Play on Awake**: Enabled (`playOnAwake: 1`) ensures fog is immediately present when the scene loads, maintaining atmosphere from the start.

This particle system, combined with the custom fog shader, creates a cohesive atmospheric effect that significantly enhances the game's visual quality and horror atmosphere while maintaining acceptable performance.

## Summary of Contributions

### Yuting Ying
**Role:** Environment and art designer  
- Modelled and textured all **characters**, **monsters**, and **environmental assets**.  
- Designed and built the **maze map layout** in Unity.  
- Implemented custom **shader effects** for lighting and fog.  
- [View shader → `Assets/Shaders/Fog.shader`](./Assets\Materials\Fog\FogParticleURP.shader)  
- [View scene → `Assets/Scripts\MonsterChase.cs`](./Assets\Scripts\MonsterChase.cs)
---

### Jiayi Liu
**Role:** UI and audio designer  
- Designed and implemented the **main menu UI**, **pause menu**, and **settings menu** (sound, resolution, brightness).  
- Created and scripted the **in-game tutorial** system (`TutorialManager.cs`).  
- Integrated **sound effects** and **background music** through Unity’s audio system.  
- [View code → `Assets\Scripts\MainMenuManager.cs`](./Assets\Scripts\MainMenuManager.cs)  
- [View code → `Assets\Scripts\AudioManager.cs`](./Assets\Scripts\AudioManager.cs)

---

### Wayne Chen
**Role:** Programmer & gameplay designer  
- Designed and implemented the **game logic** and **framework**, including player movement and item interaction (`IInteractable.cs`), and player state systems.  
- Contributed to performance optimization and WebGL build setup.  
- [View code → `Assets\Scripts\PlayerController.cs`](./Assets\Scripts\PlayerController.cs)  
- [View code → `Assets\Scripts\SprintUnlockPickup.cs`](./Assets\Scripts\SprintUnlockPickup.cs)

---

### Katherine Xin
**Role:** Documentation and evaluation lead  
- Authored the **Game Design Document (GDD)** and **REPORT.md**.  
- Led the creation of the **game evaluation plan**, coordinating **playtesting** and collection of **player feedback**.  
- Contributed to the designing of the maze.  
- [View document → `README.md`](./README.md)  
- [View document → `REPORT.md`](./REPORT.md)


## References and External Resources
- 3D Models and Textures:
    - Player Model: Source: https://sketchfab.com/3d-models/low-poly-rag-doll-6d97443dd03c485b96376d51942ceca7, with Mixamo for rigging and animation assets.
        
    - Toys Model: Source: https://sketchfab.com/3d-models/stuffed-dino-toy-d69e9bb7bfc6451993bf84f3e763a28a
        
    - Clockwork Gears Model: Source: https://sketchfab.com/3d-models/winding-key-1cfb0ba4245647d29bad1940ad876e46

    - Maze Wall Texture: Source: https://ambientcg.com/view?id=Concrete008

    - Plane texture: Source: https://ambientcg.com/view?id=Concrete012
- Music and sound effects: 
    - Home page BGM: Source: Background music generated with Suno AI (https://suno.com)
        
    - In-game BGM: Source: https://freesound.org/people/ChristmasKrumble666/sounds/726368/
        
    - Monster Roar: Source: https://freesound.org/people/aarontheonly/sounds/614007/
        
    - Monster Footsteps: Source: https://www.epidemicsound.com/sound-effects/tracks/b860994f-e5e9-464d-b746-2f915e9b7635/
        
    - Player Footsteps: Source: https://freesound.org/people/modusmogulus/sounds/827593/
        
    - Player Heartbeat: Source: https://freesound.org/people/patobottos/sounds/369017/
- Typography：
    - Theme typography: Source: https://www.dafont.com/i-still-know.font

    - Setting typography: Source: https://www.dafont.com/neck-romancer.font






