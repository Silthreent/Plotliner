using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotliner.Entities
{
    class ColorChanger
    {
        Color[,] colors = { { Color.Blue, Color.Red, Color.Black },
                             { Color.Brown, Color.Green, Color.Yellow },
                           {Color.DarkBlue, Color.Maroon, Color.DarkGreen }};
        int size = 150;
        int margin;
        int boxSize;

        Texture2D rect;

        TextBox box;
        bool enabled;
        Point colorPos;

        public ColorChanger(Game1 gameRef)
        {
            rect = new Texture2D(gameRef.GraphicsDevice, 1, 1);
            rect.SetData(new[] { Color.White });

            margin = size / 15;
            boxSize = size / 4;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            colorPos = new Point(box.Position.X + box.Size.X + 15, box.Position.Y + (box.Size.Y / 2) - (size / 2));
            spriteBatch.Draw(rect, new Rectangle(colorPos.X, colorPos.Y, size, size), Color.Black);

            for(int x = 1; x <= 3; x++)
            {
                for(int y = 1; y <= 3; y++)
                {
                    spriteBatch.Draw(rect, new Rectangle(colorPos.X + (margin * x) + (boxSize * (x - 1)), colorPos.Y + (margin * y) + (boxSize * (y - 1)), boxSize, boxSize), colors[x - 1, y - 1]);
                }
            }
        }

        public Color checkClicked(Point mousePos)
        {
            if(new Rectangle(colorPos, new Point(size, size)).Contains(mousePos))
            {
                for(int x = 1; x <= 3; x++)
                {
                    for(int y = 1; y <= 3; y++)
                    {
                        if(new Rectangle(colorPos.X + (margin * x) + (boxSize * (x - 1)), colorPos.Y + (margin * y) + (boxSize * (y - 1)), boxSize, boxSize).Contains(mousePos))
                        {
                            return colors[x - 1, y - 1];
                        }
                    }
                }
            }

            return Color.Transparent;
        }

        public void enable(TextBox box)
        {
            this.box = box;
            enabled = true;
        }

        public void disable()
        {
            box = null;
            enabled = false;
        }

        public bool Enabled
        {
            get
            {
                return enabled;
            }
        }

        public TextBox Box
        {
            get
            {
                return box;
            }
        }
    }
}
