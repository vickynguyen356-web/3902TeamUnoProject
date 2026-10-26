using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Player;
using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Collision.Handlers
{
    /// <summary>Marioplayer vs IProjectile. 
    /// CHECKLIST:
    /// info.Side is relative to the IEnemy. (ex: in MarioEnemyCollisionHandler,
    /// Side == Bottom means a stomp.
    /// Mark dead entities dead/expired at the top of Handle() because Level.Update()
    /// takes care of that (cleans it up). TryDetect() in CollisionSystem makes handlers fire for objects that touch so
    /// OverlapWidth and OverlapHeight can be 0.</summary>
    internal class MarioProjectileCollisionHandler : ICollisionHandler<MarioPlayer, IProjectile>
    {
        public void Handle(MarioPlayer first, IProjectile second, CollisionInfo info)
        {
            if (second.IsDead || first.IsDead || first.IsChangingForm)
            {
                return;
            }

            KoopaShell shell = second as KoopaShell;
            if (shell != null)
            {
                if (shell.IsEnemyProjectile && first.IsStarActive)
                {
                    second.Kill();
                    return;
                }

                HandleShellCollision(first, shell, info);
                return;
            }

            if (!second.IsEnemyProjectile || first.IsStarActive)
            {
                return;
            }

            first.TakeDamage();
            second.Kill();
        }

        private static void HandleShellCollision(MarioPlayer player, KoopaShell shell, CollisionInfo info)
        {
            Rectangle shellBounds = shell.Bounds;
            bool isStomping = info.Side == CollisionSide.Bottom
                && player.Velocity.Y > 0f
                && info.FirstPreviousBounds.Bottom <= shellBounds.Top
                && player.Bounds.Bottom >= shellBounds.Top;

            if (isStomping)
            {
                if (shell.IsMoving)
                {
                    shell.Stop();
                }
                else
                {
                    float kickDirection;
                    if (player.Bounds.Center.X < shellBounds.Center.X)
                    {
                        kickDirection = 1f;
                    }
                    else
                    {
                        kickDirection = -1f;
                    }

                        shell.Kick(kickDirection);
                }

                BouncePlayer(player, shellBounds);
                return;
            }

            if (info.Side != CollisionSide.Left && info.Side != CollisionSide.Right)
            {
                return;
            }

            if (shell.IsMoving)
            {
                if (!shell.IsEnemyProjectile)
                {
                    return;
                }

                player.TakeDamage();
                return;
            }

            float direction;
            if (info.Side == CollisionSide.Right)
            {
                direction = 1f;
            }
            else
            {
                direction = -1f;
            }

            shell.Kick(direction);
        }

        private static void BouncePlayer(MarioPlayer player, Rectangle shellBounds)
        {
            Vector2 position = player.Position;
            position.Y = shellBounds.Top - player.Bounds.Height;
            Vector2 velocity = player.Velocity;
            velocity.Y = -350f;
            player.ApplyMotion(position, velocity, isGrounded: false);
        }
    }
}
