using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotliner.Utils
{
    public static class ContentLoader
    {
        static ContentManager content;

        public static void setContent(ContentManager contentB)
        {
            content = contentB;
        }

        public static Texture2D loadTexture2D(string filePath)
        {
            return content.Load<Texture2D>(filePath);
        }

        public static SpriteFont loadSpriteFont(string filePath)
        {
            return content.Load<SpriteFont>(filePath);
        }

        public static void unload()
        {
            content.Unload();
        }
    }
}
