using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plotliner.Entities;
using Plotliner.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotliner.Manager
{
    class PlotlineManager
    {
        bool active = false;
        
        Game1 gameRef;
        NetworkManager network;
        ColorChanger colorPicker;

        Camera camera;
        List<TextBox> textBoxes;
        List<BoxConnection> boxLines;
        Texture2D rect;
        SpriteFont font;

        TextBox focus;
        TextBox dragging;
        TextBox connecting;

        string lastAction = "";

        public PlotlineManager(Game1 gameRef)
        {
            this.gameRef = gameRef;

            camera = new Camera(gameRef.GraphicsDevice);
            colorPicker = new ColorChanger(gameRef);

            textBoxes = new List<TextBox>();
            boxLines = new List<BoxConnection>();

            font = ContentLoader.loadSpriteFont("Font");

            rect = new Texture2D(gameRef.GraphicsDevice, 1, 1);
            rect.SetData(new[] { Color.White });
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
                    box.draw(spriteBatch, focus == box);
                }

                if(colorPicker.Enabled)
                {
                    colorPicker.draw(spriteBatch);
                }

                if(connecting != null)
                {
                    Vector2 world = camera.ToWorld(Mouse.GetState().Position.ToVector2());
                    spriteBatch.Draw(rect, connecting.Position.ToVector2() + (connecting.Size.ToVector2() / 2), null, Color.Black,
                        (float)Math.Atan2(world.Y - connecting.Position.Y, world.X - connecting.Position.X), new Vector2(0f, 0f), new Vector2(Vector2.Distance(connecting.Position.ToVector2(), world - (connecting.Size.ToVector2() / 2)), 3f), SpriteEffects.None, 0f);
                }

            }
            spriteBatch.End();

            spriteBatch.Begin();
            {
                spriteBatch.DrawString(font, lastAction, new Vector2(0, 0), Color.White);
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

        public void deleteTextBox(int index)
        {
            int numFound = 0;

            foreach(BoxConnection connection in boxLines)
            {
                if(connection.Box1 == textBoxes[index] || connection.Box2 == textBoxes[index])
                {
                    network.sendMessage(6, boxLines.IndexOf(connection) - numFound);
                    numFound++;
                }
            }
            if(colorPicker.Box == textBoxes[index])
                colorPicker.disable();

            textBoxes.RemoveAt(index);
        }

        public void deleteBoxConnection(int index)
        {
            boxLines.RemoveAt(index);
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

        public void updateTextBox(int index, byte r, byte g, byte b)
        {
            textBoxes[index].Color = new Color(r, g, b);
            lastAction = "Changed Box Color";
        }

        public void savePlotline(string fileName)
        {
            Console.WriteLine("Saving");

            try
            {
                using(StreamWriter file = new StreamWriter(@"plotlines/" + fileName + ".txt"))
                {
                    foreach(TextBox box in textBoxes)
                    {
                        box.save(file);
                    }
                    foreach(BoxConnection connection in boxLines)
                    {
                        file.WriteLine("!");
                        file.WriteLine(textBoxes.IndexOf(connection.Box1));
                        file.WriteLine(textBoxes.IndexOf(connection.Box2));
                    }
                }

                lastAction = "Saved";
                Console.WriteLine("Saved");
            }
            catch(Exception e)
            {
                lastAction = e.GetType().ToString();
                Console.WriteLine(e.Message);
            }

        }

        public void loadPlotline(string loadString)
        {
            Console.WriteLine("Loading");

            dragging = null;
            focus = null;
            connecting = null;
            textBoxes.Clear();
            boxLines.Clear();

            TextBox tempBox = null;
            string line = "";

                using(StringReader file = new StringReader(loadString))
                {
                    while((line = file.ReadLine()) != null)
                    {
                        if(line.Length == 0)
                            continue;

                        if(line == "#")
                        {
                            tempBox = new TextBox(0, 0, gameRef);
                            textBoxes.Add(tempBox);
                        }
                        else if(line[0] == '@')
                        {
                            string[] split = file.ReadLine().Split(',');
                            tempBox.updatePosition(int.Parse(split[0]), int.Parse(split[1]));
                        }
                        else if(line == "!")
                        {
                            boxLines.Add(new BoxConnection(textBoxes[int.Parse(file.ReadLine())], textBoxes[int.Parse(file.ReadLine())], gameRef));
                        }
                        else if(line == "%")
                        {
                            string[] split = file.ReadLine().Split(',');
                            tempBox.Color = new Color(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
                        }
                        else
                        {
                            tempBox.Text = line;
                        }
                    }
                }

                lastAction = "Loaded";
                Console.WriteLine("Loaded");
           
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

        public void onKeyReleased(object sender, KeyboardEventArgs args)
        {
            if(focus != null)
                return;

            if(args.Key == Keys.Q)
            {
                Console.WriteLine("Creating Box");
                Point world = camera.ToWorld(Mouse.GetState().Position.ToVector2()).ToPoint();
                network.sendMessage(0, world.X, world.Y);
                lastAction = "Created Box";
            }
            if(args.Key == Keys.A)
            {
                TextBox box = checkBoxClick();
                if(box != null)
                {
                    Console.WriteLine("Deleting box");
                    network.sendMessage(4, textBoxes.IndexOf(box));
                    lastAction = "Deleted Box";
                }
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
                        lastAction = "Drawing Line";
                    }
                    else
                    {
                        Console.WriteLine("Connected Boxes");
                        network.sendMessage(3, textBoxes.IndexOf(connecting), textBoxes.IndexOf(box));
                        lastAction = "Drew line";
                        connecting = null;
                    }
                }
                else
                {
                    connecting = null;
                    lastAction = "Cancelled line";
                }
            }
        }

        public void onKeyTyped(object sender, KeyboardEventArgs args)
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

                if(args.Modifiers == KeyboardModifiers.Control)
                {
                    if(args.Key == Keys.S)
                    {
                        lastAction = "Saving...";
                        savePlotline(focus.Text);
                        return;
                    }

                    if(args.Key == Keys.L)
                    {
                        try
                        {
                            using(StreamReader file = new StreamReader(@"plotlines/" + focus.Text + ".txt"))
                            {
                                network.sendMessage(5, file.ReadToEnd());
                                lastAction = "Loading...";
                            }
                        }
                        catch(FileNotFoundException e)
                        {
                            focus = null;
                            lastAction = "Failed load: Plotline not found";
                            Console.WriteLine(e.Message);
                        }

                        return;
                    }

                    if(args.Key == Keys.C)
                    {
                        lastAction = "Connecting...";
                        network.shutdownServer();
                        network.createClient(focus.Text);
                        lastAction = "Connected";
                    }
                    if(args.Key == Keys.V)
                    {
                        lastAction = "Creating server...";
                        network.createServer(int.Parse(focus.Text));
                        lastAction = "Created server";
                    }
                }

                if(args.Character.HasValue)
                {
                    network.sendMessage(1, textBoxes.IndexOf(focus), args.Character.Value.ToString());
                }
            }
        }

        public void onMouseClick(object sender, MouseEventArgs args)
        {
            if(args.Button == MouseButton.Left)
            {
                TextBox box = checkBoxClick();
                if(box == null)
                {
                    if(colorPicker.Enabled)
                    {
                        Color color;
                        if((color = colorPicker.checkClicked(camera.ToWorld(Mouse.GetState().Position.ToVector2()).ToPoint())) != Color.Transparent)
                        {
                            network.sendMessage(7, textBoxes.IndexOf(colorPicker.Box), color.R, color.G, color.B);
                            return;
                        }
                    }

                    focus = null;
                    colorPicker.disable();
                }
            }
            
            if(args.Button == MouseButton.Right)
            {
                TextBox box = checkBoxClick();
                if(box != null)
                {
                    colorPicker.enable(box);
                }
            }
        }

        public void onMouseDoubleClick(object sender, MouseEventArgs args)
        {
            if(args.Button == MouseButton.Left)
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
        }

        public void onMouseDragStart(object sender, MouseEventArgs args)
        {
            if(args.Button == MouseButton.Left)
            {
                TextBox box = checkBoxClick();
                if(box != null)
                {
                    dragging = box;
                }
            }
        }

        public void onMouseDrag(object sender, MouseEventArgs args)
        {
            if(args.Button == MouseButton.Left)
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
        }

        public void onMouseDragEnd(object sender, MouseEventArgs args)
        {
            if(args.Button == MouseButton.Left)
            {
                dragging = null;
            }
        }

        public void onMouseWheelMove(object sender, MouseEventArgs args)
        {
            if(args.ScrollWheelDelta > 0)
                camera.Scale += .1f;
            else
            {
                camera.Scale -= .1f;
                if(camera.Scale < .2f)
                {
                    camera.Scale = .2f;
                }
            }
        }

        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }
    }
}
