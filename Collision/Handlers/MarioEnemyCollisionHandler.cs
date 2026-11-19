using TeamUno.Mario.Entities.Enemies;
using TeamUno.Mario.Entities.Player;
using TeamUno.Mario.Interfaces;
using Microsoft.Xna.Framework;
using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Audio;
using TeamUno.Mario.World;

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
        private readonly GameProgress _progress;

        public MarioEnemyCollisionHandler(GameProgress progress)
        {
            System.ArgumentNullException.ThrowIfNull(progress);
            _progress = progress;
        }

        public void Handle(MarioPlayer first, IEnemy second, CollisionInfo info)
        {
            if (first.IsDead || first.IsChangingForm || first.IsSlidingOnFlagpole || second.IsDead)
            {
                return;
            }

            // Star-powered Mario defeats every enemy immediately.
            if (first.IsStarActive)
            {
                if (second is Bowser bowser)
                {
                    while (!bowser.IsDead)
                    {
                        bowser.TakeDamage();
                    }

                    _progress.AddScore(100);
                    return;
                }

                if (second is FireBarSegment || second is BulletBillCannon)
                {
                    return;
                }

                Enemy enemy = second as Enemy;
                if (enemy != null && !enemy.IsDead)
                {
                    enemy.Kill();
                    _progress.AddScore(100);
                }

                return;
            }
            
            Rectangle enemyBounds = second.CollisionBounds;
            bool isStomping = first.Velocity.Y > 0f
                && info.FirstPreviousBounds.Bottom <= enemyBounds.Top
                && first.Bounds.Bottom >= enemyBounds.Top
                && first.Bounds.Right > enemyBounds.Left
                && first.Bounds.Left < enemyBounds.Right;
            // take damage if hitting enemy from the side/bottom, or enemy is Bowser/Piranha Plant
            if (!isStomping || second is Bowser || second is PiranhaPlant
                || second is Spiny
                || (second is CheepCheep cheepCheep
                    && cheepCheep.MovementMode == CheepCheepMovementMode.Swimming)
                || second is FireBarSegment || second is BulletBillCannon)
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
                    _progress.AddScore(100);
                }
            }
            else
            {
                second.Kill();
                _progress.AddScore(100);
            }

            SoundPlayer.Play("Audio/SoundEffects/stompswim");

            Vector2 position = first.Position;
            position.Y = enemyBounds.Top - first.Bounds.Height;
            Vector2 velocity = first.Velocity;
            velocity.Y = StompBounceVelocity;
            first.ApplyMotion(position, velocity, isGrounded: false);
        }
    }
}
