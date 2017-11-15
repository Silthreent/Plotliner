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
        List<BoxConnection> boxLines;

        TextBox focus;
        TextBox dragging;
        TextBox connecting;

        public PlotlineManager(Game1 gameRef, NetworkManager network)
        {
            this.gameRef = gameRef;
            this.network = network;

            camera = new Camera(gameRef.GraphicsDevice);

            textBoxes = new List<TextBox>();
            boxLines = new List<BoxConnection>();

            gameRef.Keyboard.KeyReleased += onKeyReleased;
            gameRef.Keyboard.KeyTyped += onKeyTyped;

            gameRef.Mouse.MouseClicked += onMouseClick;
            gameRef.Mouse.MouseDoubleClicked += onMouseDoubleClick;
            gameRef.Mouse.MouseDragStart += onMouseDragStart;
            gameRef.Mouse.MouseDrag += onMouseDrag;
            gameRef.Mouse.MouseDragEnd += onMouseDragEnd;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(camera: camera);
            {
                foreach(BoxConnection line in boxLines)
                {
                    line.draw(spriteBatch);
                }
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

        public void createBoxConnect(int box1, int box2)
        {
            boxLines.Add(new BoxConnection(textBoxes[box1], textBoxes[box2], gameRef));
        }

        public void updateTextBox(int index, string text)
        {
            if(text == "")
            {
                textBoxes[index].Text = textBoxes[index].Text.Remove(textBoxes[index].Text.Length - 1);
                return;
            }

            textBoxes[index].Text += text;
        }

        public void updateTextBox(int index, int x, int y)
        {
            textBoxes[index].updatePosition(x, y);
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

            if(args.Key == Keys.W)
            {
                TextBox box = checkBoxClick();
                if(box != null)
                {
                    Console.WriteLine("Creating Connection");
                    if(connecting == null)
                    {
                        if(connecting == box)
                        {
                            Console.WriteLine("Same box connected twice");
                            connecting = null;
                            return;
                        }
                        connecting = box;
                    }
                    else
                    {
                        Console.WriteLine("Connected Boxes");
                        network.sendMessage(3, textBoxes.IndexOf(connecting), textBoxes.IndexOf(box));
                        connecting = null;
                    }
                }
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

                    network.sendMessage(1, textBoxes.IndexOf(focus), "");
                    return;
                }
                if(args.Key == Keys.Enter)
                {
                    network.sendMessage(1, textBoxes.IndexOf(focus), "\n");
                    return;
                }

                if(args.Character.HasValue)
                {
                    Console.WriteLine(":" + args.Character.Value + ":");
                    Console.WriteLine(":" + "" + ":");
                    network.sendMessage(1, textBoxes.IndexOf(focus), args.Character.Value.ToString());
                }
            }
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

        void onMouseDragStart(object sender, MouseEventArgs args)
        {
            TextBox box = checkBoxClick();
            if(box != null)
            {
                dragging = box;
            }
        }

        void onMouseDrag(object sender, MouseEventArgs args)
        {
            if(dragging == null)
            {
                camera.Position -= args.DistanceMoved;
            }
            else
            {
                Point world = camera.ToWorld(Mouse.GetState().Position.ToVector2()).ToPoint();
                network.sendMessage(2, textBoxes.IndexOf(dragging), world.X, world.Y);
            }
        }

        void onMouseDragEnd(object sender, MouseEventArgs args)
        {
            dragging = null;
        }
    }
}
