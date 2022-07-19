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
        }

        private void CancelClick(object sender, RoutedEventArgs e)
            => Close();
        
        private void ConfirmClick(object sender, RoutedEventArgs e)
        {
            foreach(UIElement uIElement in MainGrid.Children)
            {
                if (uIElement is Grid grid)
                {
                    foreach (UIElement uie in grid.Children)
                    {
                        if (uie is HotkeyEditor hkeditor)
                        {
                            string hotkeyText = hkeditor.GetHotkeyTxt();
                            if (hotkeyText == Keybinds.GetHotkey(hkeditor.KeyName).ToString())
                            {
                                continue;
                            }
                            else if(hotkeyText == "-- not set --")
                            {
                                MessageBox.Show("Not all keybinds are set!", "Warning");
                                return;
                            }
                            else
                            {
                                Keybinds.KeybindDictionary[hkeditor.KeyName] = hkeditor.Hotkey;
                            }
                        }
                    }
                }
            }
            Close();
        }
    }
}
