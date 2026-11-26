using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.World
{
    internal static class LevelProjectileManager
    {
        public static void Update(
            GameTime gameTime,
            LevelDefinition definition,
            IList<IProjectile> projectiles,
            IReadOnlyList<IBlock> blocks,
            IList<IEnemy> enemies)
        {
            for (int index = projectiles.Count - 1; index >= 0; index--)
            {
                IProjectile projectile = projectiles[index];
                if (!projectile.IsDead)
                {
                    projectile.Update(gameTime);
                }

                if (LevelBounds.IsOutsideLevel(projectile.Bounds, definition))
                {
                    projectile.Kill();
                }

                if (projectile is KoopaShell shell
                    && shell.ShouldRevive)
                {
                    LevelEnemyManager.TryReviveShell(
                        shell,
                        blocks,
                        blocks,
                        enemies);
                }

                if (projectile.IsDead)
                {
                    projectiles.RemoveAt(index);
                }
            }
        }
    }
}
