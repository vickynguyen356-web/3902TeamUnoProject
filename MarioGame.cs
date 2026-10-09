// connects game components and runs them together
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Enemies;
using TeamUno.Mario.Entities.Items;
using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Input;
using TeamUno.Mario.World;

namespace TeamUno.Mario
{
    internal class MarioGame : Game
    {
        private const int WindowWidth = 1280;
        private const int WindowHeight = 720;
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private GameSession _gameSession;
        private GameRenderer _gameRenderer;

        public MarioGame()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            _graphicsDeviceManager.PreferredBackBufferWidth = WindowWidth;
            _graphicsDeviceManager.PreferredBackBufferHeight = WindowHeight;
            _graphicsDeviceManager.IsFullScreen = false;
            _graphicsDeviceManager.ApplyChanges();

            Window.Title = "Team Uno Mario";
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Texture2D marioTexture = Content.Load<Texture2D>("mario");
            Texture2D enemyTexture = Content.Load<Texture2D>("enemiesSprites");
            Texture2D itemTexture = Content.Load<Texture2D>("items");
            Texture2D backgroundTexture = Content.Load<Texture2D>("background");
            Texture2D blockTexture = Content.Load<Texture2D>("blocksspritesheetbg");

            ProjectileFactory.Initialize(enemyTexture, itemTexture);
            EnemyFactory.Initialize(enemyTexture);
            ItemFactory.Initialize(itemTexture);

            // the session updates and resets the level, including its items
            _gameSession = new GameSession(marioTexture, GraphicsDevice.Viewport.Width, new KeyboardController());
            _gameRenderer = new GameRenderer(backgroundTexture, blockTexture);
        }

        protected override void Update(GameTime gameTime)
        {
            _gameSession.Update(gameTime);
            if (_gameSession.ShouldExit)
            {
                Exit();
                return;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _gameRenderer.DrawBackground(_spriteBatch, GraphicsDevice.Viewport.Bounds);
            _spriteBatch.End();

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _gameSession.Level.Camera.Transform);
            _gameRenderer.DrawWorld(_spriteBatch, _gameSession.Level);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            _spriteBatch.Dispose();
            base.UnloadContent();
        }
    }
}
