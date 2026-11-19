using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Sprites;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities.Player
{
    internal static class MarioSpriteFactory
    {
<<<<<<< Updated upstream
        private const float WalkingFrameDurationSeconds = 0.12f;
=======
        private const float WalkingFrameDurationSeconds = 0.125f;
>>>>>>> Stashed changes

        public static ISprite Create(Texture2D spriteSheetTexture)
        {
            return new SpriteSheetSprite(spriteSheetTexture, 3f);
        }

        internal static SpriteAnimation CreateFormTransitionAnimation(PlayerForm sourceForm, PlayerForm targetForm)
        {
            SpriteFrame sourceFrame = CreateStandingFrame(sourceForm);
            SpriteFrame targetFrame = CreateStandingFrame(targetForm);
            SpriteFrame[] frames = new SpriteFrame[]
            {
                sourceFrame,
                targetFrame,
                sourceFrame,
                targetFrame,
                sourceFrame,
                targetFrame,
                targetFrame
            };

            float frameDuration = PlayerStateMachine.FormTransitionDurationSeconds / frames.Length;
            return new SpriteAnimation(frameDuration, false, frames);
        }

        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateSuperAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(0.80f, CreateStandingFrame(PlayerForm.Super));
<<<<<<< Updated upstream

            SpriteFrame[] walkingFrames = new SpriteFrame[3];
            walkingFrames[0] = CreateFrame(150, 52, 16, 32, 239, 52);
            walkingFrames[1] = CreateFrame(121, 52, 14, 31, 270, 52);
            walkingFrames[2] = CreateFrame(90, 53, 16, 30, 299, 53);
            SpriteAnimation run = new SpriteAnimation(WalkingFrameDurationSeconds, walkingFrames);
=======
            SpriteAnimation run = CreateRunAnimation(PlayerForm.Super, WalkingFrameDurationSeconds);
>>>>>>> Stashed changes

            SpriteAnimation crouch = new SpriteAnimation(1f, CreateFrame(0, 57, 16, 22, 389, 57));
            SpriteAnimation jump = new SpriteAnimation(0.20f, CreateFrame(30, 52, 16, 32, 359, 52));
            SpriteAnimation fall = new SpriteAnimation(0.28f, CreateFrame(30, 52, 16, 32, 359, 52));
            SpriteAnimation dead = new SpriteAnimation(0.28f, CreateFrame(0, 16, 15, 14, 390, 16));
            SpriteAnimation throwFireball = new SpriteAnimation(1f, CreateStandingFrame(PlayerForm.Super));
            SpriteAnimation swim = new SpriteAnimation(1f / 16f,
                CreateFrame(52, 88, 16, 30, 337, 88),
                CreateFrame(78, 88, 14, 30, 313, 88));
            SpriteAnimation swimJump = new SpriteAnimation(1f / 16f, false,
                CreateFrame(52, 88, 16, 30, 337, 88),
                CreateFrame(78, 88, 14, 30, 313, 88),
                CreateFrame(103, 88, 14, 30, 288, 88),
                CreateFrame(127, 88, 16, 29, 262, 88),
                CreateFrame(152, 88, 16, 29, 237, 88),
                CreateFrame(180, 88, 16, 29, 209, 88));
            SpriteAnimation swimWalk = CreateRunAnimation(PlayerForm.Super, 0.25f);
            SpriteAnimation climb = new SpriteAnimation(0.15f,
                CreateFrame(1, 88, 14, 30, 390, 88),
                CreateFrame(28, 89, 14, 27, 363, 89));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Idle, idle },
                { EntityAnimationState.Run, run },
                { EntityAnimationState.Crouch, crouch },
                { EntityAnimationState.Jump, jump },
                { EntityAnimationState.Fall, fall },
                { EntityAnimationState.Dead, dead },
                { EntityAnimationState.ThrowFireball, throwFireball },
                { EntityAnimationState.Swim, swim },
                { EntityAnimationState.SwimWalk, swimWalk },
                { EntityAnimationState.SwimJump, swimJump },
                { EntityAnimationState.Drift, new SpriteAnimation(
                    1f,
                    CreateFrame(60, 52, 16, 32, 329, 52)) },
                { EntityAnimationState.Climb, climb }
            };
        }

        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateSmallAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(0.80f, CreateStandingFrame(PlayerForm.Small));
<<<<<<< Updated upstream

            SpriteFrame[] walkingFrames = new SpriteFrame[3];
            walkingFrames[0] = CreateFrame(150, 0, 14, 15, 241, 0);
            walkingFrames[1] = CreateFrame(121, 0, 12, 16, 272, 0);
            walkingFrames[2] = CreateFrame(89, 0, 16, 16, 300, 0);
            SpriteAnimation run = new SpriteAnimation(WalkingFrameDurationSeconds, walkingFrames);
=======
            SpriteAnimation run = CreateRunAnimation(PlayerForm.Small, WalkingFrameDurationSeconds);
>>>>>>> Stashed changes

            SpriteAnimation crouch = new SpriteAnimation(1f, CreateStandingFrame(PlayerForm.Small));
            SpriteAnimation jump = new SpriteAnimation(0.20f, CreateFrame(29, 0, 17, 16, 359, 0));
            SpriteAnimation fall = new SpriteAnimation(0.28f, CreateFrame(29, 0, 17, 16, 359, 0));
            SpriteAnimation dead = new SpriteAnimation(0.28f, CreateFrame(0, 16, 15, 14, 390, 16));
            SpriteAnimation throwFireball = new SpriteAnimation(1f, CreateStandingFrame(PlayerForm.Small));
            SpriteAnimation swim = new SpriteAnimation(1f / 16f,
                CreateFrame(90, 30, 14, 15, 301, 30),
                CreateFrame(120, 30, 14, 15, 271, 30));
            SpriteAnimation swimJump = new SpriteAnimation(1f / 16f, false,
                CreateFrame(90, 30, 14, 15, 301, 30),
                CreateFrame(120, 30, 14, 15, 271, 30),
                CreateFrame(150, 30, 14, 15, 241, 30),
                CreateFrame(180, 30, 15, 15, 210, 30));
            SpriteAnimation swimWalk = CreateRunAnimation(PlayerForm.Small, 0.25f);
            SpriteAnimation climb = new SpriteAnimation(0.15f,
                CreateFrame(30, 30, 14, 16, 361, 30),
                CreateFrame(61, 30, 13, 15, 331, 30));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Idle, idle },
                { EntityAnimationState.Run, run },
                { EntityAnimationState.Crouch, crouch },
                { EntityAnimationState.Jump, jump },
                { EntityAnimationState.Fall, fall },
                { EntityAnimationState.Dead, dead },
                { EntityAnimationState.ThrowFireball, throwFireball },
                { EntityAnimationState.Swim, swim },
                { EntityAnimationState.SwimWalk, swimWalk },
                { EntityAnimationState.SwimJump, swimJump },
                { EntityAnimationState.Drift, new SpriteAnimation(
                    1f,
                    CreateFrame(60, 0, 16, 16, 329, 0)) },
                { EntityAnimationState.Climb, climb }
            };
        }

        internal static IReadOnlyDictionary<EntityAnimationState, SpriteAnimation> CreateFireAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(0.80f, CreateStandingFrame(PlayerForm.Fire));
<<<<<<< Updated upstream

            SpriteFrame[] walkingFrames = new SpriteFrame[3];
            walkingFrames[0] = CreateFrame(152, 122, 16, 32, 237, 122);
            walkingFrames[1] = CreateFrame(128, 122, 14, 31, 263, 122);
            walkingFrames[2] = CreateFrame(102, 123, 16, 30, 287, 123);
            SpriteAnimation run = new SpriteAnimation(WalkingFrameDurationSeconds, walkingFrames);
=======
            SpriteAnimation run = CreateRunAnimation(PlayerForm.Fire, WalkingFrameDurationSeconds);
>>>>>>> Stashed changes

            SpriteAnimation crouch = new SpriteAnimation(1f, CreateFrame(0, 127, 16, 22, 389, 127));
            SpriteAnimation jump = new SpriteAnimation(0.20f, CreateFrame(27, 122, 16, 32, 362, 122));
            SpriteAnimation fall = new SpriteAnimation(0.28f, CreateFrame(27, 122, 16, 32, 362, 122));
            SpriteAnimation dead = new SpriteAnimation(0.28f, CreateFrame(0, 16, 15, 14, 390, 16));
            SpriteAnimation throwFireball = new SpriteAnimation(1f, CreateFrame(77, 123, 16, 30, 312, 123));
            SpriteAnimation swim = new SpriteAnimation(1f / 16f,
                CreateFrame(52, 158, 16, 30, 337, 158),
                CreateFrame(78, 158, 14, 30, 313, 158));
            SpriteAnimation swimJump = new SpriteAnimation(1f / 16f, false,
                CreateFrame(52, 158, 16, 30, 337, 158),
                CreateFrame(78, 158, 14, 30, 313, 158),
                CreateFrame(103, 158, 14, 30, 288, 158),
                CreateFrame(127, 158, 16, 29, 262, 158),
                CreateFrame(152, 158, 16, 29, 237, 158),
                CreateFrame(180, 158, 16, 29, 209, 158));
            SpriteAnimation swimWalk = CreateRunAnimation(PlayerForm.Fire, 0.25f);
            SpriteAnimation climb = new SpriteAnimation(0.15f,
                CreateFrame(1, 158, 14, 30, 390, 158),
                CreateFrame(28, 159, 14, 27, 363, 159));

            return new Dictionary<EntityAnimationState, SpriteAnimation>
            {
                { EntityAnimationState.Idle, idle },
                { EntityAnimationState.Run, run },
                { EntityAnimationState.Crouch, crouch },
                { EntityAnimationState.Jump, jump },
                { EntityAnimationState.Fall, fall },
                { EntityAnimationState.Dead, dead },
                { EntityAnimationState.ThrowFireball, throwFireball },
                { EntityAnimationState.Swim, swim },
                { EntityAnimationState.SwimWalk, swimWalk },
                { EntityAnimationState.SwimJump, swimJump },
                { EntityAnimationState.Drift, new SpriteAnimation(
                    1f,
                    CreateFrame(60, 122, 16, 32, 329, 122)) },
                { EntityAnimationState.Climb, climb }
            };
        }

        internal static SpriteAnimation CreateSprintRunAnimation(PlayerForm form)
        {
            return CreateRunAnimation(form, WalkingFrameDurationSeconds / 1.5f);
        }

        private static SpriteAnimation CreateRunAnimation(PlayerForm form, float frameDuration)
        {
            SpriteFrame[] frames;
            switch (form)
            {
                case PlayerForm.Small:
                    frames = new SpriteFrame[]
                    {
                        CreateFrame(150, 0, 14, 15, 241, 0),
                        CreateFrame(121, 0, 12, 16, 272, 0),
                        CreateFrame(89, 0, 16, 16, 300, 0)
                    };
                    break;
                case PlayerForm.Fire:
                    frames = new SpriteFrame[]
                    {
                        CreateFrame(152, 122, 16, 32, 237, 122),
                        CreateFrame(128, 122, 14, 31, 263, 122),
                        CreateFrame(102, 123, 16, 30, 287, 123)
                    };
                    break;
                default:
                    frames = new SpriteFrame[]
                    {
                        CreateFrame(150, 52, 16, 32, 239, 52),
                        CreateFrame(121, 52, 14, 31, 270, 52),
                        CreateFrame(90, 53, 16, 30, 299, 53)
                    };
                    break;
            }

            return new SpriteAnimation(frameDuration, frames);
        }

        private static SpriteFrame CreateStandingFrame(PlayerForm form)
        {
            switch (form)
            {
                case PlayerForm.Small:
                    return CreateFrame(181, 0, 13, 16, 211, 0);
                case PlayerForm.Super:
                    return CreateFrame(180, 52, 16, 32, 209, 52);
                case PlayerForm.Fire:
                    return CreateFrame(180, 122, 16, 32, 209, 122);
                default:
                    throw new ArgumentOutOfRangeException(nameof(form));
            }
        }

        private static SpriteFrame CreateFrame(
            int leftFrameX, int leftFrameY, int width, int height, int rightFrameX, int rightFrameY)
        {
            Rectangle leftFrame = new Rectangle(leftFrameX, leftFrameY, width, height);
            Rectangle rightFrame = new Rectangle(rightFrameX, rightFrameY, width, height);
            return new SpriteFrame(leftFrame, rightFrame);
        }
    }
}
