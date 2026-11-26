using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Blocks;
using TeamUno.Mario.Entities.Enemies;
using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    internal sealed class LevelEnemyManager
    {
        public static bool Update(
            GameTime gameTime,
            LevelDefinition definition,
            Vector2? listenerPosition,
            Rectangle? listenerBounds,
            IReadOnlyList<IBlock> readOnlyBlocks,
            List<IEnemy> enemies,
            List<IProjectile> projectiles,
            bool hasBowser)
        {
            bool isBowserDefeated = false;

            for (int index = enemies.Count - 1; index >= 0; index--)
            {
                IEnemy enemy = enemies[index];
                Enemy movingEnemy = enemy as Enemy;

                if (enemy.IsDead)
                {
                    if (movingEnemy != null)
                    {
                        movingEnemy.UpdateDeathFall(gameTime);
                    }

                    if (movingEnemy == null
                        || LevelBounds.IsBelowLevel(enemy.Bounds, definition))
                    {
                        enemies.RemoveAt(index);
                        continue;
                    }
                }

                if (enemy is Bowser bowser)
                {
                    bowser.ListenerPosition = listenerPosition;
                }

                if (enemy is Blooper blooper)
                {
                    blooper.ListenerPosition = listenerPosition;
                }

                Lakitu lakitu = enemy as Lakitu;
                if (lakitu != null)
                {
                    lakitu.ListenerPosition = listenerPosition;
                    lakitu.FlagPosition = FindFlagPosition(readOnlyBlocks);
                }

                if (enemy is PiranhaPlant piranha)
                {
                    piranha.PlayerBounds = listenerBounds;
                }

                if (enemy is BulletBillCannon cannon)
                {
                    cannon.ListenerPosition = listenerPosition;
                }

                enemy.Update(gameTime);

                if (lakitu != null)
                {
                    foreach (Spiny spiny in lakitu.DrainSpinySpawns())
                    {
                        spiny.SetWorldBlocks(readOnlyBlocks);
                        enemies.Add(spiny);
                    }

                    if (lakitu.ShouldDespawn)
                    {
                        enemies.RemoveAt(index);
                        continue;
                    }
                }

                if (enemy is BulletBillCannon activeCannon)
                {
                    foreach (BulletBill bulletBill in activeCannon.DrainSpawns())
                    {
                        bulletBill.SetWorldBlocks(readOnlyBlocks);
                        enemies.Add(bulletBill);
                    }
                }

                if (movingEnemy != null)
                {
                    movingEnemy.UpdateGravity(gameTime);
                }

                if (LevelBounds.IsBelowLevel(enemy.Bounds, definition))
                {
                    enemies.RemoveAt(index);
                    continue;
                }

                if (enemy is IProjectileEmitter emitter)
                {
                    foreach (IProjectile projectile in emitter.Projectiles)
                    {
                        projectiles.Add(projectile);
                    }

                    emitter.ClearProjectiles();
                }
            }

            if (hasBowser)
            {
                isBowserDefeated = true;
                foreach (IEnemy enemy in enemies)
                {
                    if (enemy is Bowser bowser && !bowser.IsDead)
                    {
                        isBowserDefeated = false;
                        break;
                    }
                }
            }

            return isBowserDefeated;
        }

        public static bool TryReviveShell(
            KoopaShell shell,
            IReadOnlyList<IBlock> blocks,
            IReadOnlyList<IBlock> readOnlyBlocks,
            IList<IEnemy> enemies)
        {
            float x = shell.Position.X
                + (KoopaShell.ShellWidth - Koopa.KoopaWidth) / 2f;
            float y = shell.Position.Y
                + KoopaShell.ShellHeight - Koopa.KoopaHeight;
            Rectangle spawnBounds = new Rectangle(
                (int)x,
                (int)y,
                Koopa.KoopaWidth,
                Koopa.KoopaHeight);

            foreach (IBlock block in blocks)
            {
                if (block.IsDeleted
                    || block.Type == BlockType.Empty
                    || block.Type == BlockType.Invisible
                    || block.Type == BlockType.Vine
                    || block.Type == BlockType.FlagPole
                    || block.Type == BlockType.Flag
                    || block.Type == BlockType.Platform)
                {
                    continue;
                }

                if (spawnBounds.Intersects(block.Bounds))
                {
                    return false;
                }
            }

            IEnemy newEnemy = EnemyFactory.Create(
                EnemyType.Koopa,
                new Vector2(x, y));
            if (newEnemy is Enemy walkingEnemy)
            {
                walkingEnemy.SetWorldBlocks(readOnlyBlocks);
            }

            enemies.Add(newEnemy);
            shell.Kill();
            return true;
        }

        private static Vector2? FindFlagPosition(IReadOnlyList<IBlock> blocks)
        {
            foreach (IBlock block in blocks)
            {
                if (block.Type == BlockType.Flag)
                {
                    return new Vector2(
                        block.Bounds.Center.X,
                        block.Bounds.Center.Y);
                }
            }

            return null;
        }
    }
}
