using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Interfaces;

namespace Sprint0.Entities
{
    public class SpriteSheetSprite : ISprite
    {
        private readonly Texture2D _spriteSheetTexture;
        private readonly float _spriteScale;
        private SpriteAnimation _currentAnimation;
        private float _frameTimer;
        private int _frameIndex;

        public SpriteSheetSprite(Texture2D spriteSheetTexture, float spriteScale)
        {
            if (spriteSheetTexture == null)
            {
                throw new ArgumentNullException(nameof(spriteSheetTexture));
            }

            _spriteSheetTexture = spriteSheetTexture;
            _spriteScale = spriteScale;
        }

        public void Reset()
        {
            _currentAnimation = null;
            _frameIndex = 0;
            _frameTimer = 0;
        }

        public void Update(GameTime gameTime, SpriteAnimation animation)
        {
            if (animation == null)
            {
                throw new ArgumentNullException(nameof(animation));
            }

            if (_currentAnimation != animation)
            {
                // Start a new animation at frame zero
                _currentAnimation = animation;
                _frameIndex = 0;
                _frameTimer = 0;
            }

            // Use the same time limit as movement
            float elapsedSeconds = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            _frameTimer += elapsedSeconds;
            while (_frameTimer >= animation.FrameDuration)
            {
                // Keep any leftover time for the next frame
                _frameTimer -= animation.FrameDuration;
                _frameIndex++;
                if (_frameIndex >= animation.Frames.Count)
                {
                    _frameIndex = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle bounds, SpriteEffects facingDirection)
        {
            // Update supplies the animation before the sprite can be drawn
            if (_currentAnimation == null)
            {
                return;
            }

            SpriteFrame selectedFrame = _currentAnimation.Frames[_frameIndex];
            Rectangle sourceRectangle = selectedFrame.GetSourceRectangle(facingDirection);

            // Line up the bottom of the picture with the entity's feet
            Vector2 drawingOrigin = new Vector2(sourceRectangle.Width / 2f, sourceRectangle.Height);
            Vector2 feetPosition = new Vector2(bounds.Center.X, bounds.Bottom);
            Vector2 frameOffset = new Vector2(selectedFrame.OffsetX, selectedFrame.OffsetY) * _spriteScale;

            // Both directions are already on the sheet
            spriteBatch.Draw(
                _spriteSheetTexture,
                feetPosition + frameOffset,
                sourceRectangle,
                Color.White,
                0,
                drawingOrigin,
                _spriteScale,
                SpriteEffects.None,
                0);
        }
    }
}
