// connects game components and runs them together
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TeamUno.Mario.Audio;
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
        private readonly IReadOnlyDictionary<string, LevelDefinition> _areas;
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private GameSession _gameSession;
        private GameRenderer _gameRenderer;
        private KeyboardController _controller;
        private KeyboardState _previousKeys;
        private MenuScreen _menu;

        public MarioGame()
        {
            _areas = LevelLayouts.CreateAreas();
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
            _font = Content.Load<SpriteFont>("MyFont");

            Texture2D marioTexture = Content.Load<Texture2D>("Sprites/mario");
            Texture2D enemyTexture = Content.Load<Texture2D>("Sprites/enemiesSprites");
            Texture2D itemTexture = Content.Load<Texture2D>("Sprites/items");
            Texture2D backgroundTexture = Content.Load<Texture2D>("background");
            Texture2D blockTexture = Content.Load<Texture2D>("Sprites/blocksspritesheetbg");

            ProjectileFactory.Initialize(enemyTexture, itemTexture);
            EnemyFactory.Initialize(enemyTexture);
            ItemFactory.Initialize(itemTexture);
            SoundPlayer.Initialize(Content);

            LevelDefinition firstArea = _areas["overworld"];
            Vector2 spawn = firstArea.GetSpawnPoint("start");
            MarioPlayer player = new MarioPlayer(
                MarioSpriteFactory.Create(marioTexture),
                new Vector2(spawn.X, spawn.Y - MarioPlayer.StandingHeight),
                PlayerForm.Fire);

            Level level = new Level(firstArea);
            Camera camera = new Camera(GraphicsDevice.Viewport.Width, level.Definition.Width);
            MusicPlayer music = new MusicPlayer(Content);
            // the session updates and resets the level, including its items
            _gameSession = new GameSession(player, level, camera, _areas, music);

            KeyBindings bindings = new KeyBindings();
            _controller = new KeyboardController(player, _gameSession, bindings);
            _menu = new MenuScreen(_gameSession, bindings, _gameSession.AreaIds);
            _gameRenderer = new GameRenderer(GraphicsDevice, backgroundTexture, blockTexture);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keys = Keyboard.GetState();

            if (_gameSession.State == GameSessionState.Menu)
            {
                _menu.Update();
                if (_gameSession.State != GameSessionState.Menu)
                {
                    _controller.ResetInput();   // keys held in the menu aren't new presses
                }
            }
            else if (keys.IsKeyDown(Keys.Escape) && _previousKeys.IsKeyUp(Keys.Escape))
            {
                _menu.Open();
            }
            else
            {
                _controller.Update();
            }

            _previousKeys = keys;

            if (_gameSession.ShouldExit)
            {
                Exit();
                return;
            }

            _gameSession.Update(gameTime, _controller.IsJumpHeld);
            Window.Title = "Team Uno Mario - " + _gameSession.Level.Definition.Id;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _gameRenderer.DrawBackground(GraphicsDevice, _spriteBatch, GraphicsDevice.Viewport.Bounds);
            _spriteBatch.End();

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _gameSession.Camera.Transform);
            _gameRenderer.DrawWorld(_spriteBatch, _gameSession);
            _spriteBatch.End();

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _gameRenderer.DrawHud(_spriteBatch, _gameSession, _menu, _font, GraphicsDevice.Viewport.Bounds);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            _spriteBatch.Dispose();
            _gameRenderer?.Dispose(); // clean up the game renderer: its pixel texture, ? incase it was never initialized
            base.UnloadContent();
        }
    }
}
