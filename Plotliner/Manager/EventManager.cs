using MonoGame.Extended.Input.InputListeners;
using Plotliner.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotliner.Manager
{
    class EventManager
    {
        PlotlineManager plotline;
        ServerConnectWindow serverConnect;

        public EventManager(MouseListener mouse, KeyboardListener keyboard, PlotlineManager plotline, ServerConnectWindow serverConnect)
        {
            this.plotline = plotline;
            this.serverConnect = serverConnect;

            keyboard.KeyReleased += onKeyReleased;
            keyboard.KeyTyped += onKeyTyped;

            mouse.MouseClicked += onMouseClick;
            mouse.MouseDoubleClicked += onMouseDoubleClick;
            mouse.MouseDragStart += onMouseDragStart;
            mouse.MouseDrag += onMouseDrag;
            mouse.MouseDragEnd += onMouseDragEnd;
            mouse.MouseWheelMoved += onMouseWheelMove;
        }

        void onKeyReleased(object sender, KeyboardEventArgs args)
        {
            if(plotline.Active)
            {
                plotline.onKeyReleased(sender, args);
            }
        }

        void onKeyTyped(object sender, KeyboardEventArgs args)
        {
            if(plotline.Active)
            {
                plotline.onKeyTyped(sender, args);
            }

            if(serverConnect.Active)
            {
                serverConnect.onKeyTyped(sender, args);
            }
        }

        void onMouseClick(object sender, MouseEventArgs args)
        {
            if(plotline.Active)
            {
                plotline.onMouseClick(sender, args);
            }

            if(serverConnect.Active)
            {
                serverConnect.onMouseClick(sender, args);
            }
        }

        void onMouseDoubleClick(object sender, MouseEventArgs args)
        {
            if(plotline.Active)
            {
                plotline.onMouseDoubleClick(sender, args);
            }
        }

        void onMouseDragStart(object sender, MouseEventArgs args)
        {
            if(plotline.Active)
            {
                plotline.onMouseDragStart(sender, args);
            }
        }

        void onMouseDrag(object sender, MouseEventArgs args)
        {
            if(plotline.Active)
            {
                plotline.onMouseDrag(sender, args);
            }
        }

        void onMouseDragEnd(object sender, MouseEventArgs args)
        {
            if(plotline.Active)
            {
                plotline.onMouseDragEnd(sender, args);
            }
        }

        void onMouseWheelMove(object sender, MouseEventArgs args)
        {
            if(plotline.Active)
            {
                plotline.onMouseWheelMove(sender, args);
            }
        }
    }
}
