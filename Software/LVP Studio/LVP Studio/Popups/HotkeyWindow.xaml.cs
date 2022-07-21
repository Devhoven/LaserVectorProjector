using LvpStudio.Helpler;
using LvpStudio.HotkeyHelper;
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

namespace LvpStudio
{
    public partial class HotkeyWindow : Window
    {
        public HotkeyWindow(Window owner)
        {
            InitializeComponent();

            Owner = owner;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
            => Close();

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void ConfirmClick(object sender, RoutedEventArgs e)
        {
            foreach (HotkeyEditor hkEditor in HotkeyEditor.HotkeyEditors)
            {
                string hotkeyText = hkEditor.GetHotkeyTxt();
                if (hotkeyText == "-- not set --")
                {
                    MessageBox.Show("Not all keybinds are set!", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                else if (hotkeyText != Keybinds.GetHotkey(hkEditor.KeyName).ToString())
                {
                    Keybinds.UpdateKeyEntry(hkEditor.KeyName, hkEditor.Hotkey);
                }
            }
            Close();
        }
    }
}
