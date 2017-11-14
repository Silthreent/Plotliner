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
        Camera camera;
        List<TextBox> textBoxes;

        TextBox focus;

        public PlotlineManager(Game1 gameRef)
        {
            this.gameRef = gameRef;
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

        void createTextBox()
        {
            Vector2 world = camera.ToWorld(Mouse.GetState().Position.ToVector2());

            textBoxes.Add(new TextBox(world.ToPoint(), gameRef));
        }

        void onKeyReleased(object sender, KeyboardEventArgs args)
        {
            if(focus != null)
                return;

            if(args.Key == Keys.Q)
            {
                Console.WriteLine("Q pressed");
                createTextBox();
            }
        }
    }
}
