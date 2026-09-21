using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Entities
{
    public class MarioSpriteFactory
    {
        public static ISprite Create(Texture2D spriteSheetTexture)
        {
            SpriteAnimations superAnimations = CreateSuperAnimations();
            SpriteAnimations smallAnimations = CreateSmallAnimations();
            SpriteAnimations fireAnimations = CreateFireAnimations();

            return new SpriteSheetSprite(spriteSheetTexture, 2f, superAnimations, smallAnimations, fireAnimations);
        }

        private static SpriteAnimations CreateSuperAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(0.80f, CreateFrame(180, 52, 16, 32, 209, 52));

            SpriteFrame[] walkingFrames = new SpriteFrame[3];
            walkingFrames[0] = CreateFrame(150, 52, 16, 32, 239, 52);
            walkingFrames[1] = CreateFrame(121, 52, 14, 31, 270, 52);
            walkingFrames[2] = CreateFrame(90, 53, 16, 30, 299, 53);
            SpriteAnimation run = new SpriteAnimation(0.28f, walkingFrames);

            SpriteAnimation crouch = new SpriteAnimation(1f, CreateFrame(0, 57, 16, 22, 389, 57));
            SpriteAnimation jump = new SpriteAnimation(0.20f, CreateFrame(30, 52, 16, 32, 359, 52));
            SpriteAnimation fall = new SpriteAnimation(0.28f, CreateFrame(30, 52, 16, 32, 359, 52));
            SpriteAnimation dead = new SpriteAnimation(0.28f, CreateFrame(0, 16, 15, 14, 390, 16));
            SpriteAnimation throwFireball = new SpriteAnimation(1f, CreateFrame(180, 52, 16, 32, 209, 52));

            return new SpriteAnimations(idle, run, crouch, jump, fall, dead, throwFireball);
        }

        private static SpriteAnimations CreateSmallAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(0.80f, CreateFrame(181, 0, 13, 16, 211, 0));

            SpriteFrame[] walkingFrames = new SpriteFrame[3];
            walkingFrames[0] = CreateFrame(150, 0, 14, 15, 241, 0);
            walkingFrames[1] = CreateFrame(121, 0, 12, 16, 272, 0);
            walkingFrames[2] = CreateFrame(89, 0, 16, 16, 300, 0);
            SpriteAnimation run = new SpriteAnimation(0.28f, walkingFrames);

            SpriteAnimation crouch = new SpriteAnimation(1f, CreateFrame(181, 0, 13, 16, 211, 0));
            SpriteAnimation jump = new SpriteAnimation(0.20f, CreateFrame(29, 0, 17, 16, 359, 0));
            SpriteAnimation fall = new SpriteAnimation(0.28f, CreateFrame(29, 0, 17, 16, 359, 0));
            SpriteAnimation dead = new SpriteAnimation(0.28f, CreateFrame(0, 16, 15, 14, 390, 16));
            SpriteAnimation throwFireball = new SpriteAnimation(1f, CreateFrame(181, 0, 13, 16, 211, 0));

            return new SpriteAnimations(idle, run, crouch, jump, fall, dead, throwFireball);
        }

        private static SpriteAnimations CreateFireAnimations()
        {
            SpriteAnimation idle = new SpriteAnimation(0.80f, CreateFrame(180, 122, 16, 32, 209, 122));

            SpriteFrame[] walkingFrames = new SpriteFrame[3];
            walkingFrames[0] = CreateFrame(152, 122, 16, 32, 237, 122);
            walkingFrames[1] = CreateFrame(128, 122, 14, 31, 263, 122);
            walkingFrames[2] = CreateFrame(102, 123, 16, 30, 287, 123);
            SpriteAnimation run = new SpriteAnimation(0.28f, walkingFrames);

            SpriteAnimation crouch = new SpriteAnimation(1f, CreateFrame(0, 127, 16, 22, 389, 127));
            SpriteAnimation jump = new SpriteAnimation(0.20f, CreateFrame(27, 122, 16, 32, 362, 122));
            SpriteAnimation fall = new SpriteAnimation(0.28f, CreateFrame(27, 122, 16, 32, 362, 122));
            SpriteAnimation dead = new SpriteAnimation(0.28f, CreateFrame(0, 16, 15, 14, 390, 16));
            SpriteAnimation throwFireball = new SpriteAnimation(1f, CreateFrame(77, 123, 16, 30, 312, 123));

            return new SpriteAnimations(idle, run, crouch, jump, fall, dead, throwFireball);
        }

        // The sheet has separate left and right frames, with uneven spacing between poses.
        private static SpriteFrame CreateFrame(
            int leftFrameX, int leftFrameY, int width, int height, int rightFrameX, int rightFrameY)
        {
            Rectangle leftFrame = new Rectangle(leftFrameX, leftFrameY, width, height);
            Rectangle rightFrame = new Rectangle(rightFrameX, rightFrameY, width, height);
            return new SpriteFrame(leftFrame, rightFrame);
        }
    }
}
