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
        private readonly SpriteAnimations _superAnimations;
        private readonly SpriteAnimations _smallAnimations;
        private readonly SpriteAnimations _fireAnimations;
        private SpriteAnimation _currentAnimation;
        private float _frameTimer;
        private int _frameIndex;

        public SpriteSheetSprite(
            Texture2D spriteSheetTexture,
            float spriteScale,
            SpriteAnimations superAnimations,
            SpriteAnimations smallAnimations,
            SpriteAnimations fireAnimations)
        {
            if (spriteSheetTexture == null)
            {
                throw new ArgumentNullException(nameof(spriteSheetTexture));
            }

            _spriteSheetTexture = spriteSheetTexture;
            if (superAnimations == null)
            {
                throw new ArgumentNullException(nameof(superAnimations));
            }

            _superAnimations = superAnimations;
            if (smallAnimations == null)
            {
                throw new ArgumentNullException(nameof(smallAnimations));
            }

            _smallAnimations = smallAnimations;
            if (fireAnimations == null)
            {
                throw new ArgumentNullException(nameof(fireAnimations));
            }

            _fireAnimations = fireAnimations;
            _spriteScale = spriteScale;
        }

        public void Reset()
        {
            _currentAnimation = null;
            _frameIndex = 0;
            _frameTimer = 0;
        }

        public void Update(GameTime gameTime, ISpriteState spriteState)
        {
            SpriteAnimation selectedAnimation = GetAnimation(spriteState);
            if (_currentAnimation != selectedAnimation)
            {
                // Start a new animation at frame zero.
                _currentAnimation = selectedAnimation;
                _frameIndex = 0;
                _frameTimer = 0;
            }

            // Use the same time limit as movement.
            float elapsedSeconds = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);
            _frameTimer += elapsedSeconds;
            while (_frameTimer >= selectedAnimation.FrameDuration)
            {
                // Keep any leftover time for the next frame.
                _frameTimer -= selectedAnimation.FrameDuration;
                _frameIndex++;
                if (_frameIndex >= selectedAnimation.Frames.Count)
                {
                    _frameIndex = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, ISpriteState spriteState)
        {
            SpriteAnimation selectedAnimation = GetAnimation(spriteState);
            int frameIndex = 0;
            if (_currentAnimation == selectedAnimation)
            {
                frameIndex = _frameIndex;
            }

            SpriteFrame selectedFrame = selectedAnimation.Frames[frameIndex];
            Rectangle sourceRectangle = selectedFrame.GetSourceRectangle(spriteState.FacingDirection);

            // Line up the bottom of the picture with Mario's feet.
            Vector2 drawingOrigin = new Vector2(sourceRectangle.Width / 2f, sourceRectangle.Height);
            Vector2 feetPosition = new Vector2(spriteState.Bounds.Center.X, spriteState.Bounds.Bottom);
            Vector2 frameOffset = new Vector2(selectedFrame.OffsetX, selectedFrame.OffsetY) * _spriteScale;

            // Both directions are already on the sheet.
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

        private SpriteAnimation GetAnimation(ISpriteState spriteState)
        {
            SpriteAnimations formAnimations;
            switch (spriteState.Form)
            {
                case PlayerForm.Small:
                    formAnimations = _smallAnimations;
                    break;
                case PlayerForm.Fire:
                    formAnimations = _fireAnimations;
                    break;
                default:
                    formAnimations = _superAnimations;
                    break;
            }

            return formAnimations.Get(spriteState.AnimationState);
        }
    }
}
