using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plotliner.Manager;
using Plotliner.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotliner.Entities
{
    class TextBox
    {
        SpriteFont font;
        Texture2D rect;
        string text = "";
        Point position;
        Point size;

        public TextBox(int x, int y, Game1 gameRef)
        {
            position = new Point(x, y);
            size = new Point(25, 25);

            font = ContentLoader.loadSpriteFont("Font");

            rect = new Texture2D(gameRef.GraphicsDevice, 1, 1);
            rect.SetData(new[] { Color.White });
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(rect, new Rectangle(position, size), Color.White);
            spriteBatch.DrawString(font, text, position.ToVector2(), Color.Black);
        }

        public bool checkClick(Point mousePos)
        {
            if(new Rectangle(position, size).Contains(mousePos))
                return true;

            return false;
        }
        
        public void updatePosition(int x, int y)
        {
            position.X = x;
            position.Y = y;
        }

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                if(text != "")
                {
                    size = font.MeasureString(text).ToPoint();
                }
            }
        }
    }
}
