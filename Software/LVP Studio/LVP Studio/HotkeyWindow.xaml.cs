using ProjectorInterface.Helpler;
using ProjectorInterface.HotkeyHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectorInterface
{
    public partial class HotkeyWindow : Window
    {
        
        public HotkeyWindow()
        {
            InitializeComponent();
            ProjectFrameBox.Text = Keybinds.GetHotkey("ProjectFrame").ToString();
        }


        private void CancelClick(object sender, RoutedEventArgs e)
            => Close();
        
        private void ConfirmClick(object sender, RoutedEventArgs e)
        {
            //Keybind.Add(ProjectFrameBox,((HotkeyEditor)ProjectFrameBox.Parent).Hotkey);
            foreach(TextBox)

            Close();
        }

        private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox TextBox = (TextBox)sender;
            TextBox.Text = ((HotkeyEditor)TextBox.Parent).UpdateUI(sender, e);
        }
    }
}
