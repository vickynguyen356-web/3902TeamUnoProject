// connects game components and runs them together
// loads texture and fonts, creates player, factories, level, controllers and renderer
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
    public class MarioGame : BaseGame
    {
        private readonly LevelDefinition _levelDefinition;
        private Texture2D _whitePixelTexture;
        private GameSession _gameSession;
        private GameRenderer _gameRenderer;
        private CombinedController _controller;

        public MarioGame() : this(DemoLevel.CreateDefinition())
        {
        }

        private MarioGame(LevelDefinition levelDefinition)
            : base("Sprint 2 Player Demo", levelDefinition.Width, levelDefinition.Height, false)
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
            SpriteFont controlsFont = Content.Load<SpriteFont>("MyFont");

            _whitePixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _whitePixelTexture.SetData(new Color[] { Color.White });

            MarioPlayer player = new MarioPlayer(
                MarioSpriteFactory.Create(marioTexture),
                _levelDefinition.PlayerSpawnPosition,
                PlayerForm.Fire);

            //BowserFireballSpriteFactory fireballSpriteFactory = new BowserFireballSpriteFactory(enemyTexture);
            IProjectileFactory projectileFactory = new ProjectileFactory(
                delegate ()
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
            DemoLevel level = new DemoLevel(_levelDefinition, enemyFactory, itemFactory, projectileFactory);
            // the session updates and resets the level, including its items
            _gameSession = new GameSession(
                player,
                level,
                new DemoMovement(level.Definition.Width, level.Definition.FloorY),
                projectileFactory);

            KeyboardInput input = new KeyboardInput();
            _controller = new CombinedController(
                input,
                _gameSession,
                new KeyboardController(player, _gameSession, input),
                // Handles item cycling, number-key selection, and block and enemy demo controls
                new DemoController(level, input));
            _gameRenderer = new GameRenderer(backgroundTexture, _whitePixelTexture, blockTexture, controlsFont);
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
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _gameRenderer.Draw(SpriteBatch, GraphicsDevice.Viewport.Bounds, _gameSession);
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
