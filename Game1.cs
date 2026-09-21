using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprint0.Entities;
using Sprint0.Input;
using Sprint0.Interfaces;
using Sprint0.World;

namespace Sprint0
{
    public class Game1 : Core
    {
        private const int WindowWidth = 1280;
        private const int WindowHeight = 720;
        private const int DemoFloorY = 528;

        private Texture2D _backgroundTexture;
        private Texture2D _whitePixelTexture;
        private GameSession _gameSession;
        private GameRenderer _gameRenderer;
        private IController _keyboardController;

        public Game1() : base("Sprint 2 Player Demo", WindowWidth, WindowHeight, false)
        {
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Texture2D marioTexture = Content.Load<Texture2D>("mario");
            SpriteFont controlsFont = Content.Load<SpriteFont>("MyFont");
            _backgroundTexture = Content.Load<Texture2D>("background");

            _whitePixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            // Stretch this pixel to draw the demo floor 
            _whitePixelTexture.SetData(new Color[] { Color.White });

            MarioPlayer player = new MarioPlayer(
                MarioSpriteFactory.Create(marioTexture),
                new Vector2(96, DemoFloorY - MarioPlayer.StandingHeight),
                PlayerForm.Fire);
            Level level = new Level();

            _gameSession = new GameSession(
                player,
                level,
                new DemoMovement(WindowWidth, DemoFloorY),
                new CollisionSystem(level));
            _keyboardController = new KeyboardController(player, _gameSession);
            _gameRenderer = new GameRenderer(_whitePixelTexture, controlsFont);
        }

        protected override void Update(GameTime gameTime)
        {
            _keyboardController.Update();
            if (_gameSession.ShouldExit)
            {
                Exit();
                return;
            }

            _gameSession.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // PointClamp keeps the scaled sprites sharp.
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            SpriteBatch.Draw(_backgroundTexture, GraphicsDevice.Viewport.Bounds, Color.White * 0.45f);
            _gameRenderer.DrawDemoFloor(SpriteBatch, GraphicsDevice.Viewport.Bounds, DemoFloorY);
            _gameRenderer.DrawWorld(SpriteBatch, _gameSession.Level, _gameSession.Player);
            _gameRenderer.DrawControls(SpriteBatch);
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            _whitePixelTexture.Dispose();
            base.UnloadContent();
        }
    }
}
