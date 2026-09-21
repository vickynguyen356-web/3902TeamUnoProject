using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sprint0
{
    public abstract class Core : Game
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        protected SpriteBatch SpriteBatch { get; private set; }

        protected Core(string title, int windowWidth, int windowHeight, bool isFullScreen)
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            _graphicsDeviceManager.PreferredBackBufferWidth = windowWidth;
            _graphicsDeviceManager.PreferredBackBufferHeight = windowHeight;
            _graphicsDeviceManager.IsFullScreen = isFullScreen;
            _graphicsDeviceManager.ApplyChanges();

            Window.Title = title;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            SpriteBatch.Dispose();
            base.UnloadContent();
        }
    }
}
