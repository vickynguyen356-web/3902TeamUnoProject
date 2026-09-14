using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;

// importing interfaces
using MonoGameLibrary.Interfaces;
using MonoGameLibrary.Input;
using MonoGameLibrary.Entities;

namespace Sprint0
{
    public class Game1 : Core
    {
        private Texture2D _spriteSheet;
        private Texture2D _backgroundTexture;

        // char reference 
        private KeyboardPlayer _player;
        private CombinedController _controller;

        // font
        private SpriteFont _font;
        private Vector2 _textPos;
        Color backgroundColor;
        public Game1() : base("Sprint0", 1280, 720, false)
        {

        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        { 
            base.LoadContent();

            // load sprite sheet & background texture
            _spriteSheet = Content.Load<Texture2D>("images/spritesheet");
            _backgroundTexture = Content.Load<Texture2D>("images/background");

            _controller = new CombinedController();

            // physical char logic types
            Vector2 center = new Vector2(
                Window.ClientBounds.Width / 2f,
                Window.ClientBounds.Height / 2f
                );
           
            // making cat sprite spawn on the floor 
            float backgroundFloorY = 500f;
            float calcGroundY = backgroundFloorY - (_spriteSheet.Height / 5f);
            Vector2 spawnPos = new Vector2(center.X, calcGroundY);

            _player = new KeyboardPlayer(_spriteSheet, spawnPos, calcGroundY);

            _font = Content.Load<SpriteFont>("MyFont.spritefont");
            _textPos = new Vector2(160, 575);

            backgroundColor = Color.LightPink;
        }

        protected override void Update(GameTime gameTime)
        {
            // update InputManager first
            base.Update(gameTime);

            _controller.Update();

            if(_controller.EscQuit())
            {
                Exit();
            }

            _player.Update(gameTime, _controller);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);

            // begin sprite batch for rendering
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // draw background and then cat sprite
            Core.SpriteBatch.Draw(_backgroundTexture, GraphicsDevice.Viewport.Bounds, Color.White);
            _player.Draw(SpriteBatch);
            Core.SpriteBatch.DrawString(_font, "Credits\nProgram Made By: Vy Nguyen\nSprites from: https://opengameart.org/content/cat-sprites\nBackground from: https://dribbble.com/shots/27573605-2D-Game-Background-Design", _textPos, Color.White);

            // end sprite batch
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
