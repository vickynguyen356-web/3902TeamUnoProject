# Creating a level

Each level lives in a folder named for its area ID. Add a matching
`<area-id>.levelproperties` file and a required `<area-id>_Foreground.csv`
grid. Background, above-foreground, underground, enemy, collectible, and item
grids are optional. Every present grid must have the same number of rows and
columns as the foreground.

CSV rows run from top to bottom, and comma-separated cells run from left to
right. Coordinates in `.levelproperties` use zero-based `(column, row)` tile
coordinates from the top-left corner. Each tile is 48 pixels in the game.

Empty cells can use `.` or `empty` instead of `-1`. Common foreground and
underground map tiles and gameplay markers can use names instead of atlas IDs:

| Map tile name | Meaning |
| --- | --- |
| `ground` | Ground tile |
| `brick`, `brick-alt` | Brick blocks |
| `underground-brick`, `underground-floor` | Underground brickwork and floor |
| `question`, `used`, `invisible` | Question, used, and invisible blocks |
| `pipe-left-top`, `pipe-right-top` | Pipe top tiles |
| `pipe-left-body`, `pipe-right-body` | Pipe body tiles |
| `horizontal-pipe-top-left`, `horizontal-pipe-top-inner-left`, `horizontal-pipe-top-inner-right`, `horizontal-pipe-top-right` | Top row of a sideways pipe |
| `horizontal-pipe-bottom-left`, `horizontal-pipe-bottom-inner-left`, `horizontal-pipe-bottom-inner-right`, `horizontal-pipe-bottom-right` | Bottom row of a sideways pipe |
| `flagpole`, `flagpole-alt`, `flag` | Flagpole and flag |
| `coin`, `platform`, `spring`, `cannon` | Coin, moving platform, spring, and cannon |

The `_Underground.csv` layer resolves pipe and coin names to its underground
palette, preserving its appearance without requiring raw tile IDs.

Enemy grids accept `goomba`, `goomba-alt`, `koopa`, `koopa-alt`,
`piranha-plant`, `blooper`, `cheep-cheep`, `cheep-cheep-alt`, `lakitu`,
`hammer-bro`, `bowser`, and `bullet-bill`.

Collectible and item grids accept `fire-flower`, `coin`, `one-up`, `star`,
`vine`, and `mushroom`. The same word can mean different things in different
layers; for example, `blooper` is an enemy marker while `fire-flower` is an
item marker.

Numeric tile IDs remain supported in every grid. Use them for decorative
tiles, alternate atlas artwork, or any tile without a named alias. Background
and above-foreground grids are primarily visual and usually use numeric atlas
IDs.

For example, a small foreground grid can be written as:

```csv
.,.,.,.,.
.,.,brick,question,.
ground,ground,ground,ground,ground
```

Keep the named property formats in `<area-id>.levelproperties` for player and
camera starts, spawn points, exits, pipes, and special moving objects. Existing
properties and numeric CSV grids continue to work.
