using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotliner.Entities
{
    class BoxConnection
    {
        TextBox box1;
        TextBox box2;
        Texture2D rect;

        public BoxConnection(TextBox one, TextBox two, Game1 gameRef)
        {
            box1 = one;
            box2 = two;

            rect = new Texture2D(gameRef.GraphicsDevice, 1, 1);
            rect.SetData(new[] { Color.White });
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(rect, box1.Position.ToVector2() + (box1.Size.ToVector2() / 2), null, Color.Black,
                (float)Math.Atan2(box2.Position.Y - box1.Position.Y, box2.Position.X - box1.Position.X), new Vector2(0f, 0f), new Vector2(Vector2.Distance(box1.Position.ToVector2(), box2.Position.ToVector2()), 3f), SpriteEffects.None, 0f);
        }

        public TextBox Box1
        {
            get
            {
                return box1;
            }
        }

        public TextBox Box2
        {
            get
            {
                return box2;
            }
        }
    }
}
