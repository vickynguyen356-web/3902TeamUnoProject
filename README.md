# CSE 3902 — Team Uno Mario
## Sprint 2
Root namespace: `TeamUno.Mario`

Our project is a Mario demo built with C# and MonoGame. For Sprint 2, the demo includes player movement and animations, different blocks, items, and enemies. One block, item, and enemy are shown at a time, and keyboard controls let the user switch between them.

## Controls

| Key | Action |
| --- | --- |
| A / Left Arrow | Move left |
| D / Right Arrow | Move right |
| W / Up Arrow / Space | Jump |
| S / Down Arrow | Hold to crouch; release to stand |
| Z / N | Show the throwing animation in Fire form |
| E | Damage Mario |
| R | Reset the demo |
| Q / Escape | Quit |
| T / Y | Previous / next block |
| O / P | Previous / next enemy |
| U / I | Previous / next item |
| 1 | Mushroom |
| 2 | Fire Flower |
| 3 | Floating Coin |
| 4 | Star |
| 5 | 1-Up Mushroom |
| 6 | Block Coin |

Hold the movement keys to move, and press the other action keys once for each action.

## Features

**Mario:** Mario can move left and right, jump, fall, and crouch. Mario speeds up as he moves, slows down when movement keys are released, and falls back to the floor after jumping. His animation changes based on his current action and facing direction. He stays within the sides of the screen and lands on the floor.

The demo starts with Fire Mario. Pressing E changes him from Fire to Super, then Small, then dead. Small Mario cannot crouch. Mario can only jump when grounded and not crouching. Pressing R restores his starting form and position and resets the demo selections.

**Items:** The demo includes six item types:

- Mushroom and 1-Up Mushroom move to the right.
- Fire Flower and Floating Coin display their animations at a fixed position.
- Star moves to the right while bouncing up and down.
- Block Coin moves up, moves down, and disappears after a short timer.

Moving items return to their starting position after passing the right side of the screen. Pressing 6 again replays the Block Coin effect.

**Blocks:** The available types are Brick, Question, Used, Ground, Solid, FlagPole, Empty and Pipe.

The question block is animated to change between the different question block type dependent on the time.

**Enemies:** The available types are Goomba, Koopa, Piranha Plant, Hammer Bro, and Bowser. The selection controls let the user view each enemy and its current behavior.

The game also displays a background, floor, control instructions, and the selected item's name.

## Code Design and Tools

The project uses interfaces to define common actions for players, items, enemies, and sprites. Commands connect keyboard input to game actions. Factories handle creating items, enemies, and sprites.

Shared code is kept in base classes. For example, `Item` handles position, bounds, drawing, animation updates, and resetting. Specific items inherit this behavior and add their own movement when needed. State classes help organize player and enemy behavior and animations.

The project uses Git/GitHub for source management and MonoGame's Content Builder to prepare game assets. The selection, damage, and reset controls make it easier to check individual features without restarting the program.

## Known Bugs and Limitations

- Collisions with the displayed blocks, items, and enemies are not connected yet. Mario can pass through them, and items do not grant power-ups, points, or extra lives.
- The fireball keys currently show the throwing animation but do not create a moving fireball.
- Enemy damage and defeat behavior will be built up more in the following sprint.
