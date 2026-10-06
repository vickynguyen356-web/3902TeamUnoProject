using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamUno.Mario.Entities;
using TeamUno.Mario.Input;
using TeamUno.Mario.Interfaces;
using TeamUno.Mario.Items;
using TeamUno.Mario.Projectiles;
using TeamUno.Mario.World;

namespace TeamUno.Mario
{
    internal class MarioGame : BaseGame
    {
        private const int WindowWidth = 1280;
        private const int WindowHeight = 720;
        private readonly LevelDefinition _levelDefinition;
        private GameSession _gameSession;
        private GameRenderer _gameRenderer;
        private CombinedController _controller;
        private Camera _camera;

        public MarioGame() : this(LevelLayouts.CreateFirstLevel())
        {
        }

        private MarioGame(LevelDefinition levelDefinition)
            : base("Team Uno Mario", WindowWidth, WindowHeight, false)
        {
            _levelDefinition = levelDefinition;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Texture2D marioTexture = Content.Load<Texture2D>("mario");
            Texture2D enemyTexture = Content.Load<Texture2D>("enemiesSprites");
            Texture2D itemTexture = Content.Load<Texture2D>("items");
            Texture2D backgroundTexture = Content.Load<Texture2D>("background");
            Texture2D blockTexture = Content.Load<Texture2D>("blocksspritesheetbg");

            MarioPlayer player = new MarioPlayer(
                MarioSpriteFactory.Create(marioTexture),
                _levelDefinition.PlayerSpawnPosition,
                PlayerForm.Fire);

            IProjectileFactory projectileFactory = new ProjectileFactory(
                delegate()
                {
                    return ProjectileSpriteFactory.Create(enemyTexture);
                });

            IEnemyFactory enemyFactory = new EnemyFactory(
                delegate()
                {
                    return GoombaSpriteFactory.Create(enemyTexture);
                },
                delegate()
                {
                    return KoopaSpriteFactory.Create(enemyTexture);
                },
                delegate()
                {
                    return PiranhaSpriteFactory.Create(enemyTexture);
                },
                delegate()
                {
                    return HammerBroSpriteFactory.Create(enemyTexture);
                },
                delegate()
                {
                    return BowserSpriteFactory.Create(enemyTexture);
                },
                projectileFactory);
            IItemFactory itemFactory = new ItemFactory(
                delegate()
                {
                    return ItemSpriteFactory.Create(itemTexture);
                });

            Level level = new Level(_levelDefinition, enemyFactory, itemFactory, projectileFactory);
            _gameSession = new GameSession(player, level);

            KeyboardInput input = new KeyboardInput();
            _controller = new CombinedController(
                input,
                _gameSession,
                new KeyboardController(player, _gameSession, input));
            _gameRenderer = new GameRenderer(backgroundTexture, blockTexture);
            _camera = new Camera(GraphicsDevice.Viewport.Width, level.Definition.Width);
            _camera.Follow(player.Bounds);
        }

        protected override void Update(GameTime gameTime)
        {
            _controller.Update();
            if (_gameSession.ShouldExit)
            {
                Exit();
                return;
            }

            if (!_controller.ResetThisFrame)
            {
                _gameSession.Update(gameTime);
            }

            _camera.Follow(_gameSession.Player.Bounds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _gameRenderer.DrawBackground(SpriteBatch, GraphicsDevice.Viewport.Bounds);
            SpriteBatch.End();

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.Transform);
            _gameRenderer.DrawWorld(SpriteBatch, _gameSession);
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
