using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plotliner.Manager;
using Plotliner.Utils;
using Plotliner.Windows;

namespace Plotliner
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public int screenWidth = 1280;
        public int screenHeight = 720;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        NetworkManager network;
        PlotlineManager plotline;
        ServerConnectWindow connectWindow;

        KeyboardListener keyboard;
        MouseListener mouse;
        EventManager eventManager;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;

            IsMouseVisible = true;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ContentLoader.setContent(Content);

            MouseListenerSettings mouseSettings = new MouseListenerSettings() { DoubleClickMilliseconds = 100 };
            mouse = new MouseListener(mouseSettings);
            keyboard = new KeyboardListener();

            connectWindow = new ServerConnectWindow(this);
            connectWindow.Active = true;

            plotline = new PlotlineManager(this);
            network = new NetworkManager(plotline);
            plotline.setNetwork(network);

            eventManager = new EventManager(mouse, keyboard, plotline, connectWindow);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            mouse.Update(gameTime);
            keyboard.Update(gameTime);

            network.update();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            plotline.draw(spriteBatch);

            if(connectWindow.Active)
                connectWindow.draw(spriteBatch);
        }

        public void startServer()
        {
            network.createServer();
            network.createClient("127.0.0.1");
            plotline.Active = true;
            connectWindow.Active = false;
        }

        public void connectToServer(string ip)
        {
            network.createClient(ip);
            plotline.Active = true;
            connectWindow.Active = false;
        }

        public KeyboardListener Keyboard
        {
            get
            {
                return keyboard;
            }
        }

        public MouseListener Mouse
        {
            get
            {
                return mouse;
            }
        }
    }
}
