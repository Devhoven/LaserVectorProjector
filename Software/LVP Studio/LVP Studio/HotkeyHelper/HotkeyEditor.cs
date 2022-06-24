using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectorInterface.HotkeyHelper
{
    public partial class HotkeyEditor : StackPanel
    {
        public static readonly DependencyProperty HotkeyProperty =
            DependencyProperty.Register("Hotkey", typeof(Hotkey), typeof(HotkeyEditor));

        public Hotkey Hotkey
        {
            get => (Hotkey)GetValue(HotkeyProperty);
            set => SetValue(HotkeyProperty, value);
        }

        public string UpdateUI(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            // get modifiers and key data
            var modifiers = Keyboard.Modifiers;
            var key = e.Key;

            // when alt is pressed, SystemKey is used instead
            if (key == Key.System)
                key = e.SystemKey;

            // pressing delete, backspace or escape without modifiers clears the current value
            if (modifiers == ModifierKeys.None && (key == Key.Delete || key == Key.Back || key == Key.Escape))
            {
                Hotkey = null!;
                return "-- not set --";
            }

            // return if no actual key was pressed
            if (key == Key.LeftCtrl ||
                key == Key.RightCtrl ||
                key == Key.LeftAlt ||
                key == Key.RightAlt ||
                key == Key.LeftShift ||
                key == Key.RightShift ||
                key == Key.LWin ||
                key == Key.RWin ||
                key == Key.Clear)
            {
                return "";
            }

            // Update the value
            Hotkey = new Hotkey(key, modifiers);

            return Hotkey.ToString();
        }
    }
}
