using System;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Items;

namespace TeamUno.Mario.World
{
    public class DemoLevel : Level, IDemoControls
    {
        private static readonly BlockType[] _blockTypes =
        {
            BlockType.Brick,
            BlockType.Question,
            BlockType.Used,
            BlockType.Ground,
            BlockType.Solid,
            BlockType.Pipe,
            BlockType.FlagPole
        };
        private static readonly ItemType[] _itemTypes =
        {
            ItemType.Mushroom,
            ItemType.FireFlower,
            ItemType.FloatingCoin,
            ItemType.Star,
            ItemType.OneUpMushroom,
            ItemType.BlockCoin
        };
        private static readonly EnemyType[] _enemyTypes =
        {
            EnemyType.Goomba,
            EnemyType.Koopa,
            EnemyType.PiranhaPlant,
            EnemyType.HammerBro,
            EnemyType.Bowser
        };
        private int _currentBlockIndex;
        private int _currentItemIndex;
        private int _currentEnemyIndex;

        public ItemType SelectedItem
        {
            get
            {
                return Items[0].Type;
            }
        }

        public DemoLevel(LevelDefinition definition, IEnemyFactory enemyFactory, IItemFactory itemFactory, IProjectileFactory projectileFactory)
            : base(ValidateDefinition(definition), enemyFactory, itemFactory, projectileFactory)
        {
            ResetSelectionIndices();
        }

        public static LevelDefinition CreateDefinition()
        {
            const int width = 1280;
            const int height = 720;
            const int floorY = 528;

            return new LevelDefinition(
                width,
                height,
                floorY,
                new Vector2(96, floorY - MarioPlayer.StandingHeight),
                new BlockSpawnDefinition[]
                {
                    new BlockSpawnDefinition(new Vector2(256, 420), BlockType.Brick, 48, 48)
                },
                new ItemSpawnDefinition[]
                {
                    new ItemSpawnDefinition(ItemType.Mushroom, new Vector2(32, floorY))
                },
                new EnemySpawnDefinition[]
                {
                    new EnemySpawnDefinition(EnemyType.Goomba, new Vector2(480, floorY - Goomba.GoombaHeight))
                });
        }

        public override void Reset()
        {
            ResetSelectionIndices();
            base.Reset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Items[0].Position.X > Definition.Width + 16)
            {
                Items[0].Reset();
            }
        }

        public void PreviousBlock()
        {
            _currentBlockIndex = (_currentBlockIndex - 1 + _blockTypes.Length) % _blockTypes.Length;
            ReplaceSelectedBlock();
        }

        public void NextBlock()
        {
            _currentBlockIndex = (_currentBlockIndex + 1) % _blockTypes.Length;
            ReplaceSelectedBlock();
        }

        public void PreviousEnemy()
        {
            _currentEnemyIndex = (_currentEnemyIndex - 1 + _enemyTypes.Length) % _enemyTypes.Length;
            ReplaceSelectedEnemy();
        }

        public void NextEnemy()
        {
            _currentEnemyIndex = (_currentEnemyIndex + 1) % _enemyTypes.Length;
            ReplaceSelectedEnemy();
        }

        public void Select(ItemType itemType)
        {
            int index = Array.IndexOf(_itemTypes, itemType);
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(itemType));
            }

            _currentItemIndex = index;
            ReplaceSelectedItem();
        }

        public void CycleItem(int direction)
        {
            _currentItemIndex = _currentItemIndex + direction;
            if (_currentItemIndex < 0)
            {
                _currentItemIndex = _itemTypes.Length - 1;
            }
            else if (_currentItemIndex >= _itemTypes.Length)
            {
                _currentItemIndex = 0;
            }

            ReplaceSelectedItem();
        }

        private void ResetSelectionIndices()
        {
            _currentBlockIndex = Array.IndexOf(_blockTypes, Definition.Blocks[0].Type);
            _currentItemIndex = Array.IndexOf(_itemTypes, Definition.Items[0].Type);
            _currentEnemyIndex = Array.IndexOf(_enemyTypes, Definition.Enemies[0].Type);
        }

        private void ReplaceSelectedBlock()
        {
            BlockType type = _blockTypes[_currentBlockIndex];
            int width = 48;
            int height = 48;
            Vector2 position = Definition.Blocks[0].Position;
            if (type == BlockType.Pipe)
            {
                width = 64;
                height = 64;
            }
            else if (type == BlockType.FlagPole)
            {
                height = 498;
                position = new Vector2(position.X, Definition.FloorY - height);
            }

            ReplaceBlock(0, new BlockSpawnDefinition(position, type, width, height));
        }

        private void ReplaceSelectedItem()
        {
            ReplaceItem(0, new ItemSpawnDefinition(_itemTypes[_currentItemIndex], Definition.Items[0].Position));
        }

        private void ReplaceSelectedEnemy()
        {
            ReplaceEnemy(0, new EnemySpawnDefinition(_enemyTypes[_currentEnemyIndex], Definition.Enemies[0].Position));
        }

        private static LevelDefinition ValidateDefinition(LevelDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (definition.Blocks.Count != 1 || definition.Items.Count != 1 || definition.Enemies.Count != 1)
            {
                throw new ArgumentException("The demo requires one block, one item, and one enemy", nameof(definition));
            }

            if (Array.IndexOf(_blockTypes, definition.Blocks[0].Type) < 0)
            {
                throw new ArgumentException("The initial demo block is not in the block roster", nameof(definition));
            }

            if (Array.IndexOf(_itemTypes, definition.Items[0].Type) < 0)
            {
                throw new ArgumentException("The initial demo item is not in the item roster", nameof(definition));
            }

            if (Array.IndexOf(_enemyTypes, definition.Enemies[0].Type) < 0)
            {
                throw new ArgumentException("The initial demo enemy is not in the enemy roster", nameof(definition));
            }

            return definition;
        }
    }
}
