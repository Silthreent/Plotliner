using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotliner.Entities
{
    class TextBox
    {
        Texture2D rect;
        string text;
        Point position;

        public TextBox(int x, int y, Game1 gameRef)
        {
            position = new Point(x, y);

            rect = new Texture2D(gameRef.GraphicsDevice, 1, 1);
            rect.SetData(new[] { Color.White });
        }

        public TextBox(Point pos, Game1 gameRef)
        {
            position = pos;

            rect = new Texture2D(gameRef.GraphicsDevice, 1, 1);
            rect.SetData(new[] { Color.White });
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(rect, new Rectangle(position, new Point(25, 25)), Color.White);
        }
    }
}
