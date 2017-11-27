using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plotliner.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotliner.Windows
{
    class ServerConnectWindow
    {
        Game1 gameRef;
        bool active = false;

        TextBox createServer;
        TextBox connectServer;
        TextBox enterIPWindow;
        bool connectClicked = false;

        public ServerConnectWindow(Game1 gameRef)
        {
            this.gameRef = gameRef;

            createServer = new TextBox(gameRef.screenWidth / 2, gameRef.screenHeight / 5, gameRef);
            createServer.Text = "Create a server";

            connectServer = new TextBox(gameRef.screenWidth / 2, gameRef.screenHeight / 3, gameRef);
            connectServer.Text = "Connect to a server";

            enterIPWindow = new TextBox(gameRef.screenWidth / 2, gameRef.screenHeight / 2, gameRef);
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            {
                createServer.draw(spriteBatch, false);
                connectServer.draw(spriteBatch, false);

                if(connectClicked)
                {
                    enterIPWindow.draw(spriteBatch, false);
                }
            }
            spriteBatch.End();

            
        }

        public void onMouseClick(object sender, MouseEventArgs args)
        {
            if(createServer.checkClick(Mouse.GetState().Position))
            {
                gameRef.startServer();
            }

            if(connectServer.checkClick(Mouse.GetState().Position))
            {
                connectClicked = true;
            }
        }

        public void onKeyTyped(object sender, KeyboardEventArgs args)
        {
            if(connectClicked)
            {
                if(args.Key >= Keys.D0 && args.Key <= Keys.D9)
                {
                    enterIPWindow.Text += args.Character;
                }
                if(args.Key == Keys.OemPeriod)
                {
                    enterIPWindow.Text += ".";
                }
                if(args.Key == Keys.Back)
                {
                    if(enterIPWindow.Text.Length > 0)
                    {
                        enterIPWindow.Text = enterIPWindow.Text.Remove(enterIPWindow.Text.Length - 1);
                    }
                }
                if(args.Key == Keys.Enter)
                {
                    gameRef.connectToServer(enterIPWindow.Text);
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
                connectClicked = false;
            }
        }
    }
}
