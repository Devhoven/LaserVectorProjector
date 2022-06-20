using ProjectorInterface.DrawingTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ProjectorInterface.Helper
{
    // Holds the ToolButtons and visually selects them
    public class ButtonPanel : WrapPanel
    {
        Thickness selected = new Thickness(3);
        Thickness normal = new Thickness(1);

        public ButtonPanel() : base()
        {
            VerticalAlignment = VerticalAlignment.Center;
            Orientation = Orientation.Vertical;
            HorizontalAlignment = HorizontalAlignment.Center;

            // Waiting for all children to be loaded
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => { SelectButton((Button)Children[0]); }));
        }

        public void SelectButton(Button b)
        {
            foreach (Button button in Children)
                button.BorderThickness = normal;

            b.BorderThickness = selected;
            Keyboard.ClearFocus();
        }

        public void ToolChanged(DrawingCanvas canvas)
        {
            Button tmp = new Button();

            
            if (canvas.CurrentTool is LineTool)
                tmp = (Button)Children[0];
            else if (canvas.CurrentTool is RectTool)
                tmp = (Button)Children[1];
            else if (canvas.CurrentTool is CircleTool)
                tmp = (Button)Children[2];
            else if (canvas.CurrentTool is PathTool)
                tmp = (Button)Children[3];

            SelectButton(tmp);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Image)
            {
                Button? tmp = ((Image)e.OriginalSource).Parent as Button;

                SelectButton(tmp);
            }
        }
    }
}
