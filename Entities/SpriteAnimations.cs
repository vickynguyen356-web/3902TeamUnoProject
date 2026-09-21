using Sprint0.Interfaces;

namespace Sprint0.Entities
{
    public class SpriteAnimations
    {
        public SpriteAnimation Idle { get; private set; }
        public SpriteAnimation Run { get; private set; }
        public SpriteAnimation Crouch { get; private set; }
        public SpriteAnimation Jump { get; private set; }
        public SpriteAnimation Fall { get; private set; }
        public SpriteAnimation Dead { get; private set; }
        public SpriteAnimation ThrowFireball { get; private set; }

        public SpriteAnimations(
            SpriteAnimation idle,
            SpriteAnimation run,
            SpriteAnimation crouch,
            SpriteAnimation jump,
            SpriteAnimation fall,
            SpriteAnimation dead,
            SpriteAnimation throwFireball)
        {
            Idle = idle;
            Run = run;
            Crouch = crouch;
            Jump = jump;
            Fall = fall;
            Dead = dead;
            ThrowFireball = throwFireball;
        }

        public SpriteAnimation Get(EntityAnimationState animationState)
        {
            switch (animationState)
            {
                case EntityAnimationState.Idle:
                    return Idle;
                case EntityAnimationState.Run:
                    return Run;
                case EntityAnimationState.Crouch:
                    return Crouch;
                case EntityAnimationState.Jump:
                    return Jump;
                case EntityAnimationState.Fall:
                    return Fall;
                case EntityAnimationState.Dead:
                    return Dead;
                case EntityAnimationState.ThrowFireball:
                    return ThrowFireball;
                default:
                    return Idle;
            }
        }
    }
}
