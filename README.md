# Game Design Document (GDD)

## 1. Game Overview :dizzy:

### Core Concept

- Our game is a first-person survival horror experience set in a vast, nightmarish maze. The player takes the role of a rag doll inhabited by a trapped human soul, cast into the labyrinth as part of a cruel game devised by a god.

- The player’s role is to navigate the maze, manage their sanity, and evade monstrous hunters that roam the corridors. The doll’s small, fragile body emphasizes the Miniature theme—the world feels overwhelmingly large and oppressive, while the player themselves is small, vulnerable, and constantly pursued.

- The main idea is survival under pressure:
    - Explore the maze to find the exit.
    - Avoid or outmaneuver the towering monster.
    - Maintain sanity, which decreases over time and when encountering horrors.
    - Escape before being consumed by the maze or losing the last fragment of the puppet’s soul.

- The core experience focuses on fear, tension, and scale, where ordinary walls feel like looming giants, and every step carries the risk of discovery.

### Related Genre

- Our game falls into the first-person 3D survival horror genre, with elements of psychological thriller woven into its design.

<p align="center">
  <img src="Assets\ReadMeImages\imgs\imgs\related genre.jpg" width="1200"><br>
</p>   

- Inspirations
    - Dark Deception
    - Don't Starve
    - Amnesia
    - Little Nightmares
    - Silent Hill 2
    - Outlast

- What makes our game different:
    - Miniature Perspective - focus on scale and immersion, as players must navigate mazes and environments inhabited by entities many times larger than themselves, creating a strong sense of vulnerability and tension.

    - A unique narrative background - players' story gradually unfolds throughout the survival process.

    - Sanity System - replaces traditional time-based survival mechanics, the lower the sanity, the harder the game becomes.


### Target Audience

- Our game is aimed at high school and university students (ages 16–26) who enjoy horror, survival, and psychological thriller experiences. This demographic is familiar with popular survival horror titles and is drawn to games that offer unique twists on familiar mechanics, such as sanity management and mythos-inspired storytelling.

- The game is designed for PC players, particularly those who appreciate short, intense gameplay sessions that deliver tension, atmosphere, and replayability within a compact experience.

### Unique Selling Points

- We combine Cthulhu-inspired elements with horror themes, creating a deeply immersive atmosphere. Unlike traditional survival games where progress is measured by time, our game redefines this mechanic by introducing Sanity as the core system, making psychological endurance the key to survival.

## 2. Story and Narrative :speech_balloon:

### Backstory

  An ancient god delights in the struggle of the weak.
  For its amusement, it created a living maze — a writhing construct of failed experiments, each one thirsting for souls.

  One night, the god stole a human soul and sealed it inside a small rag doll. The rag doll was cast before the maze’s gate. The moment it awoke, a towering creature — the god’s most “successful failure” — forced it inside.

  Within the shifting corridors, the rag doll encounters other beings. They were once souls like itself, shattered by the god’s cruel trials. Now they wander, unable to leave, driven only by the instinct to hunt — to seize a new soul to patch the ruin of their own.

  Out of twisted mercy, the god granted the rag doll an all-seeing vision and a handful of tools to preserve its sanity. After all, it would be far too dull if the rag doll perished too quickly. The god wishes to watch hope bloom in its eyes… before crushing it.

  As time bleeds away, the puppet’s sanity falters. The whispers grow louder, the maze hungrier. It must find the exit before its mind collapses or the devoured ones catch it. Somewhere in the northwest lies a door — a promised escape.

  But even if it reaches it… will the god truly let it go?
  And what of the monsters that prowl the halls — why do their eyes flicker with pity, and pain?

  You are Subject 072. You are not the first soul to walk these halls. Seventy-one have come before you. Some hid from the god’s gaze, leaving faint messages and safe rooms behind.

  Hold on to your sanity.
  Follow the traces of the lost.
  And uncover the truth buried beneath the maze.
### Characters

- #### Player - The Rag Doll
    - The player character is a small rag doll inhabited by a trapped human soul. Its body resembles an artist’s mannequin, with stiff, jerky movements that highlight its unnatural existence. Compared to the towering enemies of the maze, the doll’s steps are tiny, reinforcing the sense of vulnerability and insignificance within the vast environment. Despite its fragile body, the doll carries a strong motivation: to escape the labyrinth and reclaim its stolen freedom. The puppet’s personality is defined by resilience and desperation—haunted by fear, yet unwilling to surrender to the cruel fate imposed upon it

- #### Villain - The Gatekeeper Monster
    - The primary antagonist, known as the Gatekeeper Monster, is a skeletal giant with grotesquely elongated limbs that dwarf the doll. It moves slowly and heavily, each step resonating like a tremor through the maze. Although less agile than the player, it is immensely powerful, and once it closes the distance, escape becomes nearly impossible. The Gatekeeper’s motivation is primal: to hunt and devour the souls of those cast into the maze. As one of the god’s failed experiments, it embodies hunger and despair, existing only to destroy.

Together, these characters form a dynamic of predator and prey, locked in a narrative of survival and hopelessness. The doll seeks escape, the monster seeks to consume, and the god watches with detached cruelty. Their contrasting designs—small and fragile, towering and skeletal, vast and all-seeing—reflect the central themes of vulnerability, scale, and dread that define the game.

## 3. Gameplay and Mechanics :walking_man:

- ### Player Perspective
    - The game is experienced entirely from a first-person perspective, placing the player directly into the fragile body of the doll. The camera is fixed to follow the puppet’s movements closely. 

- ### Controls
    - WASD -> movement
    - C -> crouch (Ctrl was previously used for crouch, but was changed to C to avoid WebGL browser shortcut conflicts.)
    - Shift -> sprint
    - Esc -> pause menu
    - E -> interaction with objects
    - Space -> Skip the text

- ### Progression and Game Mechanics
    - The rag doll awakens at the gate of the maze. Before its stitched eyes, divine words shimmer — the god’s gift of “guidance.”

    - On the screen, a message appears:
    Subject No.072.
    Learn to move. Learn to hide.
    Find the exit before your sanity reaches zero… or before they find you.

    - A faint bar marks your sanity — a fragile thread of light.
    It drains slowly, inevitably, with every passing moment within the maze.
    The longer you wander, the deeper the madness seeps.

    - If the player reaches the gate while sanity remains above zero — and without being caught — a false ending unfolds.
    The camera rises.
    The towering walls shrink away.
    The rag doll’s fabric twists and tightens.
    You become one of the hunters.

    - If you are caught, the screen fades to black, whispering:
    “Your soul has been devoured.”

    - But if you uncover every hidden safe room, follow the scattered words of those who came before, and discover the true exit —
    the maze trembles,
    the god’s eye closes for the first time, and the final words appear:
    “You saved your soul.”

- ### Gameplay Mechanics
    - When the player encounters a monster, sanity decreases more rapidly.

    - As the monster draws closer, the sound of the heartbeat grows faster and louder.

    - After collecting a clockwork gear, the player can perform a short sprint, though each sprint requires time to recharge.

    - The player can crouch and crawl through holes in the walls, allowing faster movement through the maze and access to safe rooms.

    - There are six safe rooms in total, each accessible only by crawling through a small opening. Monsters, due to their massive bodies, cannot enter these rooms.

    - Inside each safe room, there are interactive objects and readable notes left behind by previous souls.

    - A hidden maze section can only be unlocked after all six safe rooms have been discovered.


## 4. Levels and World Design :house:

- ### Game World
    - The game is presented in 3D, using a first-person camera to immerse the player directly in the rag doll’s perspective. Throughout the entire game, the camera follows the player’s point of view, reflecting every movement, breath, and tremor of fear.

<p align="center">
  <img src="Assets\ReadMeImages\imgs\imgs\game world.jpg" width="1300"><br>
</p>    

- ### Maze Level

    - The maze is the central stage — vast, shifting, and alive.

    <p align="center">
    <img src="Assets\ReadMeImages\imgs\imgs\MazeDesign.png" width="1300"><br>
    </p>

    - A mini-map is provided to assist navigation, though it offers only partial clarity within the maze's distortions.

    - On the mini-map:
    Yellow Arrow — represents the player’s position.
    Green Dots — mark safe rooms and sanity-restoring items.
    Blue Dots — indicate safe rooms containing sprint-related clockwork items.
    Red Dots — show the monsters roaming throughout the labyrinth.

The mini-map flickers occasionally — as if something unseen is interfering with it — reminding the player that even their guidance may be watched.
- ### Objects
    - Maze walls – block sight and movement, creating tension and disorientation.
    - Interactive Elements (notes, clockwork gears, toys) – may restore sanity, unlock sprint abilities, or reveal fragments of the maze’s backstory.

- ### Physics
    - The game uses simple collision-based physics. The doll collides with walls and objects, restricting movement within the maze, also can crouch or squeeze through tighter spaces. Monsters can collide with the maze wall. 

## 5. Art and Audio :art:

- ### Art Style
    - Art style - dark, realistic, horror, reflecting the Cthulhu mythos.
    - Colours - black and white, with occasional contrasting eerie highlights.

<p align="center">
  <img src="Assets\ReadMeImages\silenthill.webp" width="500"><br>
  <em>Silent Hill 2 (2001). Used here as a reference for atmospheric inspiration.</em>
</p>

- ### Shapes and textures: 
    - Monster Design: Extremely tall and thin, with elongated limbs that emphasize unnatural proportions. This exaggeration of scale highlights the player's vulnerability when navigating the maze.

        <p align="center">
        <img src="Assets\ReadMeImages\imgs\imgs\monster-draft.jpg" width="1300"><br>
        </p>

    - Player Design: The player takes the role of a fragile rag doll, standing 0.55 meters tall. Its body is stitched together with visible seams, the fabric worn and uneven — a creation both delicate and unsettling.
    The rag doll’s body is soft and pliable, allowing subtle movements and distortions.
    This flexibility, however, also makes it easy to alter, reflecting the god’s cruel experiments and the doll’s unstable nature.

<p align="center">
  <img src="Assets\ReadMeImages\imgs\imgs\characters.jpg" width="1200"><br>
</p>    


- ### Sound and Music

    - Planned sound effects include:
        - Footsteps.
        - Distant whispers.
        - Heartbeat sounds that intensify with lower sanity.
        - Monster cues: low-frequency rumbles, distorted growls, and heavy stomps.

    - Planned music include:
        - General Exploration – features minimalist, ambient horror soundscapes that emphasize isolation and unease. Subtle mechanical hums, distant echoes, and low-frequency drones create a constant feeling of tension as the player navigates the maze.
        - Main Menu – accompanied by a haunting background ambience, faintly resembling wind passing through hollow fabric or strings being slowly tightened. It sets the tone of quiet dread before the game begins.

- ### Assets
    We plan to use a combination of self-created assets and royalty-free resources.
    - References Images: We are sourcing concept art and design inspiration from Pinterest to guide the overall aesthetic of our game, including monster shapes, labyrinth structures, and atmosphere design.
    - 3D Models and Textures:
        - Monster Model: Created using Blender for modeling and texturing, with Mixamo for rigging and animation assets.

            <p align="center">
            <img src="Assets\ReadMeImages\imgs\imgs\MonsterFinalModel.png" width="1300"><br>
            </p>

        - Player Model: Source: https://sketchfab.com/3d-models/low-poly-rag-doll-6d97443dd03c485b96376d51942ceca7, with Mixamo for rigging and animation assets.
        
        - Toys Model: Source: https://sketchfab.com/3d-models/stuffed-dino-toy-d69e9bb7bfc6451993bf84f3e763a28a
        
        - Clockwork Gears Model: Source: https://sketchfab.com/3d-models/winding-key-1cfb0ba4245647d29bad1940ad876e46
    - Music and sound effects: Our resources will be sourced from the following platforms: Freesound, OpenGameArt, Pixabay Music, Youtube Audio-Library
        - Home page BGM: Source: Background music generated with Suno AI (https://suno.com)
        
        - In-game BGM: Source: https://freesound.org/people/ChristmasKrumble666/sounds/726368/
        
        - Monster Roar: Source: https://freesound.org/people/aarontheonly/sounds/614007/
        
        - Monster Footsteps: Source: https://www.epidemicsound.com/sound-effects/tracks/b860994f-e5e9-464d-b746-2f915e9b7635/
        
        - Player Footsteps: Source: https://freesound.org/people/modusmogulus/sounds/827593/
        
        - Player Heartbeat: Source: https://freesound.org/people/patobottos/sounds/369017/
    Additionally, the main menu theme was created using Figma, designed to evoke an eerie, dreamlike stillness.
## 6. User Interface :desktop_computer:
<p align="center">
  <img src="Assets\ReadMeImages\UI\UI\UI.jpg" width="1000"><br>
</p>

## 7. Technology and Tools :wrench:
- Unity 6.1.x
    - Core engine for building the game and supports WebGL builds for browser play.

- Github
	- Repository for version control and collaboration

- VS code
	- Popular IDE for C# scripting in Unity, supports integrated debugging and code editing tools


- Art and Sound tools
    - Blender used for 3D modelling of the monster
    - Audacity for editing and mixing sound effects
    - OpenGameArt.org for royalty free sound effects



## 8. Possible Challenges :anger:

- Technical Complexity (Monster AI and Sanity System)

    - Challenge: Implementing pathfinding and believable chase mechanics for the monster, as well as linking the sanity system to gameplay effects (e.g., distorted vision, misleading signs).

    - Plan: Begin with a simple prototype of AI (basic chase using Unity NavMesh) and gradually add complexity. Sanity effects will first be tested with visual filters before tying into level design.

- Level Design Balance

    - Challenge: Designing the maze so it is challenging but not frustrating, ensuring players feel tension without becoming stuck.

    - Plan: Build multiple small prototypes of maze layouts and conduct early playtests with peers to refine pacing and difficulty.

- Art and Atmosphere Creation

    - Challenge: Achieving a dark, immersive horror aesthetic with limited time and resources.

    - Plan: Use a mix of custom low-poly models and free assets (from Unity Asset Store/OpenGameArt) while focusing on lighting, fog, and sound design to reinforce mood.

- Time Constraints

    - Challenge: With only one semester, scope creep is a risk, especially if we aim for features beyond core requirements.


    - Plan: Prioritise the minimum viable product (MVP)—basic maze, player movement, monster chase, and sanity bar. Extra features (e.g., environmental interactions, multiple monster types) will only be added if time permits.

