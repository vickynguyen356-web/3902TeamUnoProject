using TeamUno.Mario.Entities.Enemies;
using TeamUno.Mario.Entities.Player;
using TeamUno.Mario.Interfaces;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Projectiles;

namespace TeamUno.Mario.Collision.Handlers
{
    /// <summary>MarioPlayer vs IEnemy. 
    /// CHECKLIST:
    /// info.Side is relative to the MarioPlayer. (ex: in MarioEnemyCollisionHandler,
    /// Side == Bottom means a stomp.
    /// Mark dead entities dead/expired at the top of Handle() because Level.Update()
    /// takes care of that (cleans it up). TryDetect() in CollisionSystem makes handlers fire for objects that touch so
    /// OverlapWidth and OverlapHeight can be 0.</summary>
    internal class MarioEnemyCollisionHandler : ICollisionHandler<MarioPlayer, IEnemy>
    {
        private const float StompBounceVelocity = -350f;

        public void Handle(MarioPlayer first, IEnemy second, CollisionInfo info)
        {
            if (first.IsDead || first.IsChangingForm || second.IsDead)
            {
                return;
            }

            if (first.IsStarActive)
            {
                Enemy enemy = second as Enemy;
                if (enemy != null && !enemy.IsDead)
                {
                    enemy.Kill();
                }
                return;
            }

            Rectangle enemyBounds = second.Bounds;
            bool isStomping = info.Side == CollisionSide.Bottom
                && first.Velocity.Y > 0f
                && info.FirstPreviousBounds.Bottom <= enemyBounds.Top
                && first.Bounds.Bottom >= enemyBounds.Top;

            if (!isStomping || second is Bowser || second is PiranhaPlant) // Take damage if hitting enemy from the side/bottom, or enemy is Bowser/Piranha Plant
            {
                first.TakeDamage();
                return;
            }

            Koopa koopa = second as Koopa;
            if (koopa != null)
            {
                KoopaShell shell = koopa.Stomp();
                if (shell != null)
                {
                    first.AddProjectile(shell);
                }
            }
            else
            {
                second.Kill();
            }

            Vector2 position = first.Position;
            position.Y = enemyBounds.Top - first.Bounds.Height;
            Vector2 velocity = first.Velocity;
            velocity.Y = StompBounceVelocity;
            first.ApplyMotion(position, velocity, isGrounded: false);
        }
    }
}
