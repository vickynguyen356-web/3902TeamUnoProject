# CSE 3902 — Team Uno Mario

## Sprint 3

Root namespace: `TeamUno.Mario`

Our project is a Mario game built with C# and MonoGame. This sprint, we connected the objects from our earlier demo so Mario can collect items, fight enemies, interact with blocks, and travel between areas. The game includes a scrolling camera, coins and lives, and separate areas for testing.

The game starts in the overworld with Fire Mario. Mario can change forms through item pickups and damage.

## Controls

| Key | Action |
| --- | --- |
| A / Left Arrow | Move left |
| D / Right Arrow | Move right |
| W / Up Arrow / Space | Jump |
| S / Down Arrow | Crouch, or enter a connected pipe while standing on top |
| W / Up Arrow | Climb up a vine |
| S / Down Arrow | Climb down a vine |
| Z / N | Throw a fireball in Fire form |
| P | Pause or resume |
| R | Restart the current area and reset Mario, coins, and lives |
| Q / Escape | Quit |
| F1 | Go to the overworld |
| F2 | Go to the underground area |
| F3 | Go to the start of the test area |
| F4 | Go to the item section of the test area |
| F5 | Go to the combat section of the test area |
| F6 | Go to the Bowser section of the test area |
| F7 | Go to the pipe and vine section of the test area |
| F8 | Go to Bowser Party |

Hold the movement and climbing keys to move. Press jump, fireball, pause, reset, and area-selection keys once for each action. To enter a connected pipe, stand near the center of its top and press Down or S, but not every pipe has a destination.

## Features

**Mario:** Mario can move, jump, fall, crouch, climb vines, and throw fireballs in Fire form. He speeds up while moving and slows down when movement keys are released. His animations change with his action, facing direction, and form. Mario can only jump when grounded and not crouching. Small Mario cannot crouch.

Mario lands on blocks and stops at their sides. A mushroom grows Small Mario without downgrading Fire Mario. A fire flower changes him to Fire Mario. Damage changes a larger Mario to Small Mario, and Small Mario dies when hit. A short protection timer prevents repeated damage immediately after a hit.

**Items:** The game includes 6 item types:

- Mushroom and 1-Up Mushroom move, land on blocks, and turn at walls.
- Fire Flower and Floating Coin stay at their positions and animate.
- Star moves up and down, bounces on blocks, and turns at walls.
- Block Coin moves up and down, then disappears after its short animation.

Collected items disappear and apply their effect once. Coins add to the coin count, and 1-ups add a life. A block coin gives a coin when the block releases it, so touching its animation does not count it again.

A star gives Mario 10 seconds of invincibility. He changes colors to signal it is active and defeats enemies when in contact. Another star restarts the timer. The effect ends when its timer runs out or the game resets. Items that leave the level are removed.

**Blocks:** The types are Brick, Question, Used, Ground, Pipe, Solid, FlagPole, Empty, Platform, Vine, and Invisible. Question blocks release their item when hit from below and become Used. Invisible blocks only respond from below until revealed. Larger Mario can break bricks, which show a breaking animation before being removed.

Platforms can be landed on from above and passed through from below or the sides. Mario can climb vines with Up and Down. Connected pipes take Mario to another area. The flagpole has a climb and slide sequence that finishes the area.

**Enemies:** The game includes Goomba, Koopa, Piranha Plant, Hammer Bro, and Bowser. Walking enemies turn at walls, ledges, and their patrol limits. They use gravity, land on blocks, and fall if their supporting block is removed. Dead enemies and enemies below the level are removed.

Mario can stomp Goombas and Koopas and bounce afterward. Stomping a Koopa creates a shell. Mario can kick a stopped shell by touching its side, and stomping a moving shell stops it. Moving shells can hurt Mario after a short delay following a kick. Bowser and Piranha Plant cannot be safely stomped. Piranha Plant uses its open/closed animation and stays at its pipe position.

Hammer Bro stops to throw hammers before turning. Bowser fires during patrol and takes 3 fireball hits to defeat. Mario's fireballs and moving shells defeat enemies. Moving shells do not damage Bowser.

**Projectiles:** Mario's fireballs bounce on the floor and disappear against walls. Bowser's fire travels horizontally, and hammers follow an arc. Enemy projectiles damage Mario and disappear with contact unless star power protects him. Moving shells turn at walls and can break bricks. Fireballs can destroy shells, and shell collisions remove both shells when at least 1 is moving.

**Areas and Camera:** The overworld has blocks, pickups, enemies, a floor gap, vines, and pipes connected to the underground area. The underground area has its own objects and return pipes. Completing the overworld flagpole leads to Bowser Party, an extra area with rows of platforms and Bowsers. The flagpole leads to the win screen.

The camera follows Mario horizontally. F1–F8 let the user reach areas or test sections without walking through the whole level.

**Progress and Game States:** The display shows the current area, coins, and lives. The game starts with 0 coins and 3 lives. Collecting 100 coins resets the coin count to 0 and gives an extra life. Death takes 1 life, pauses, and respawns Small Mario at the level's starting area. Losing all lives shows Game Over. The game also has pause, level-clear, and win messages.

## Code Design and Tools

The project uses interfaces to define common actions for players, items, enemies, and sprites. Commands connect keyboard input to game actions. Factories create items, enemies, projectiles, and sprites.

`GameSession` owns Mario, the camera, game progress, the collision system, and the session state. It handles area changes, death, pause, and level completion. `Level` owns the blocks, items, enemies, and projectiles for the active area. Area layouts and spawn points are defined in `World/LevelLayouts.cs`.

The collision system checks object pairs and calls the matching handler. Handlers change the objects through their methods. Shared behavior stays in base classes, while each item or enemy keeps its own movement and animation behavior. Mario's state machine handles forms, transitions, and the star timer. A Draw overload lets Mario use changing colors while other objects keep their normal draw calls.

The project uses Git/GitHub and MonoGame's Content Builder. Code reviews are stored in CodeReviews. The Documentation folder includes Sprint 2 code analysis results and a Sprint 3 reflection.

## Known Bugs and Limitations

- Mario can get stuck or appear to twitch at the floor gap's edge. The landing check can treat edge contact as support
