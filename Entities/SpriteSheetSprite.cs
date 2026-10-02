using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Interfaces;

namespace TeamUno.Mario.Entities
{
    public class SpriteSheetSprite : ISprite
    {
        private readonly Texture2D _spriteSheetTexture;
        private readonly float _spriteScale;
        private SpriteAnimation _currentAnimation;
        private readonly bool _useSpriteEffects;
        private float _frameTimer;
        private int _frameIndex;

        public SpriteSheetSprite(Texture2D spriteSheetTexture, float spriteScale, bool useSpriteEffects = false)
        {
            if (spriteSheetTexture == null)
            {
                throw new ArgumentNullException(nameof(spriteSheetTexture));
            }

            _spriteSheetTexture = spriteSheetTexture;
            _spriteScale = spriteScale;
            _useSpriteEffects = useSpriteEffects;
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
                _currentAnimation = animation;
                _frameIndex = 0;
                _frameTimer = 0;
            }

            // Limit animation catch-up after a slow frame
            float elapsedSeconds = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            _frameTimer = _frameTimer + elapsedSeconds;
            while (_frameTimer >= animation.FrameDuration)
            {
                // Keep any leftover time for the next frame
                _frameTimer = _frameTimer - animation.FrameDuration;
                _frameIndex = _frameIndex + 1;
                if (_frameIndex >= animation.Frames.Count)
                {
                    _frameIndex = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle bounds, SpriteEffects facingDirection, float rotation = 0f, bool rotateAroundCenter = false)
        {
            if (_currentAnimation == null)
            {
                return;
            }

            SpriteFrame selectedFrame = _currentAnimation.Frames[_frameIndex];
            Rectangle sourceRectangle;
            SpriteEffects spriteEffects;

            if (_useSpriteEffects)
            {
                // Mirror a sheet that contains only one facing direction
                sourceRectangle = selectedFrame.LeftSource;
                spriteEffects = facingDirection;
            }
            else
            {
                sourceRectangle = selectedFrame.GetSourceRectangle(facingDirection);
                spriteEffects = SpriteEffects.None;
            }

            // Anchor frames at the feet, even when their heights differ
            Vector2 drawingOrigin;
            if (rotateAroundCenter)
            {
                drawingOrigin = new Vector2(sourceRectangle.Width / 2f,
                    sourceRectangle.Height / 2f);
            }
            else
            {
                drawingOrigin = new Vector2(sourceRectangle.Width / 2f,
                sourceRectangle.Height);
            }
            Vector2 feetPosition = new Vector2(bounds.Center.X, 
                bounds.Bottom);
            Vector2 frameOffset = new Vector2(selectedFrame.OffsetX, 
                selectedFrame.OffsetY) * _spriteScale;

            spriteBatch.Draw(
                _spriteSheetTexture,
                feetPosition + frameOffset,
                sourceRectangle,
                Color.White,
                rotation,
                drawingOrigin,
                _spriteScale,
                spriteEffects,
                0);
        }
    }
}
