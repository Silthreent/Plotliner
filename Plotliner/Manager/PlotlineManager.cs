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
            gameRef.Keyboard.KeyTyped += onKeyTyped;

            gameRef.Mouse.MouseClicked += onMouseClick;
            gameRef.Mouse.MouseDoubleClicked += onMouseDoubleClick;
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
            textBoxes.Add(new TextBox(x, y, gameRef));
        }

        public void updateTextBox(int index, string text)
        {
            textBoxes[index].Text += text;
        }

        TextBox checkBoxClick()
        {
            Point world = camera.ToWorld(Mouse.GetState().Position.ToVector2()).ToPoint();

            foreach(TextBox box in textBoxes)
            {
                if(box.checkClick(world))
                {
                    return box;
                }
            }

            return null;
        }

        void onMouseClick(object sender, MouseEventArgs args)
        {
            TextBox box = checkBoxClick();
            if(box == null)
            {
                focus = null;
            }
        }

        void onMouseDoubleClick(object sender, MouseEventArgs args)
        {
            TextBox box = checkBoxClick();
            if(box != null)
            {
                focus = box;
            }
            else
            {
                focus = null;
            }
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
                Console.WriteLine("Creating Box");
                Point world = camera.ToWorld(Mouse.GetState().Position.ToVector2()).ToPoint();
                network.sendMessage(0, world.X, world.Y);
            }
        }

        void onKeyTyped(object sender, KeyboardEventArgs args)
        {
            if(focus != null)
            {
                if(args.Key == Keys.Back)
                {
                    if(focus.Text.Length < 1)
                        return;

                    //text = text.Remove(text.Length - 1);
                    return;
                }
                if(args.Key == Keys.Enter)
                {
                    network.sendMessage(1, textBoxes.IndexOf(focus), "\n");
                    return;
                }

                if(args.Character.HasValue)
                {
                    network.sendMessage(1, textBoxes.IndexOf(focus), args.Character.Value.ToString());
                }
            }
        }
    }
}
