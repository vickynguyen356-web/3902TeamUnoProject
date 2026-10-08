// connects game components and runs them together
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities.Enemies;
using TeamUno.Mario.Entities.Items;
using TeamUno.Mario.Entities.Player;
using TeamUno.Mario.Entities.Projectiles;
using TeamUno.Mario.Input;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.World;

namespace TeamUno.Mario
{
    internal class MarioGame : Game
    {
        private const int WindowWidth = 1280;
        private const int WindowHeight = 720;
        private readonly LevelDefinition _levelDefinition;
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private GameSession _gameSession;
        private GameRenderer _gameRenderer;
        private IController _controller;

        public MarioGame()
        {
            _levelDefinition = LevelLayouts.CreateFirstLevel();
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

            MarioPlayer player = new MarioPlayer(
                MarioSpriteFactory.Create(marioTexture),
                _levelDefinition.PlayerSpawnPosition,
                PlayerForm.Fire);

            Level level = new Level(_levelDefinition);
            Camera camera = new Camera(GraphicsDevice.Viewport.Width, level.Definition.Width);
            // the session updates and resets the level, including its items
            _gameSession = new GameSession(player, level, camera);

            _controller = new KeyboardController(player, _gameSession);
            _gameRenderer = new GameRenderer(backgroundTexture, blockTexture);
        }

        protected override void Update(GameTime gameTime)
        {
            _controller.Update();
            if (_gameSession.ShouldExit)
            {
                Exit();
                return;
            }

            _gameSession.Update(gameTime, _controller.IsJumpHeld);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _gameRenderer.DrawBackground(_spriteBatch, GraphicsDevice.Viewport.Bounds);
            _spriteBatch.End();

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _gameSession.Camera.Transform);
            _gameRenderer.DrawWorld(_spriteBatch, _gameSession);
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
