using LvpStudio.DrawingTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace LvpStudio.Helper
{
    // Holds the ToolButtons and visually selects them
    public class ButtonPanel : WrapPanel
    {
        static readonly Thickness UNSELECTED_THICKNESS = (Thickness)Application.Current.FindResource("UnselectedBtnThickness");
        static readonly SolidColorBrush UNSELECTED_BORDER_COLOR = (SolidColorBrush)Application.Current.FindResource("UnselectedBtnBorderColor");

        static readonly Thickness SELECTED_THICKNESS = (Thickness)Application.Current.FindResource("SelectedBtnThickness");
        static readonly SolidColorBrush SELECTED_BORDER_COLOR = (SolidColorBrush)Application.Current.FindResource("SelectedBtnBorderColor");

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
            {
                button.BorderThickness = UNSELECTED_THICKNESS;
                button.BorderBrush = UNSELECTED_BORDER_COLOR;
            }

            b.BorderThickness = SELECTED_THICKNESS;
            b.BorderBrush = SELECTED_BORDER_COLOR;
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

        // This should be reworked
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Image img)
            {
                Button? tmp = img.Parent as Button;
                if (tmp != null)
                    SelectButton(tmp);
            }
        }
    }
}
