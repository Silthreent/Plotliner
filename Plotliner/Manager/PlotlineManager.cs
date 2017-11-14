using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plotliner.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotliner.Manager
{
    class PlotlineManager
    {
        Game1 gameRef;
        NetworkManager network;

        Camera camera;
        List<TextBox> textBoxes;

        TextBox focus;

        public PlotlineManager(Game1 gameRef, NetworkManager network)
        {
            this.gameRef = gameRef;
            this.network = network;

            camera = new Camera(gameRef.GraphicsDevice);

            textBoxes = new List<TextBox>();

            gameRef.Keyboard.KeyReleased += onKeyReleased;
        }

        public void update(GameTime gameTime)
        {

        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(camera: camera);
            {
                foreach(TextBox box in textBoxes)
                {
                    box.draw(spriteBatch);
                }
            }
            spriteBatch.End();
        }

        public void setNetwork(NetworkManager network)
        {
            this.network = network;
        }

        public void createTextBox(int x, int y)
        {
            //Vector2 world = camera.ToWorld(Mouse.GetState().Position.ToVector2());

            textBoxes.Add(new TextBox(new Point(x, y), gameRef));
        }

        void onKeyReleased(object sender, KeyboardEventArgs args)
        {
            if(focus != null)
                return;

            if(args.Key == Keys.R)
            {
                network.createServer();
                network.createClient("127.0.0.1");
            }
            if(args.Key == Keys.F)
            {
                network.createClient("71.203.217.238");
            }

            if(args.Key == Keys.Q)
            {
                Console.WriteLine("Q pressed");

                Point world = camera.ToWorld(Mouse.GetState().Position.ToVector2()).ToPoint();
                network.sendMessage(0, world.X, world.Y);
            }
        }
    }
}
