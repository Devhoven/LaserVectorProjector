using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LvpStudio.HotkeyHelper
{
    public partial class HotkeyEditor : Grid
    {
        Label NameLabel;
        TextBox HotkeyTextBox;

        Hotkey _Hotkey = null!;

        public Hotkey Hotkey
        {
            get => _Hotkey;
            set => _Hotkey = value;
        }

        public string KeyDisplayName
        {
            get => (string)NameLabel.Content;
            set => NameLabel.Content = value;
        }

        string _KeyName = "";
        public string KeyName
        {
            get => _KeyName;
            set
            {
                _KeyName = value;
                HotkeyTextBox.Text = Keybinds.GetHotkey(KeyName).ToString();
            } 
        }

        public string GetHotkeyTxt()
            => HotkeyTextBox.Text;

        public HotkeyEditor()
        {
            NameLabel = new Label();

            HotkeyTextBox = new TextBox();

            HotkeyTextBox.PreviewKeyDown += UpdateUI;

            Children.Add(NameLabel);
            Children.Add(HotkeyTextBox);

            SetColumn(NameLabel, 0); 
            SetColumn(HotkeyTextBox, 1);
        }

        public void UpdateUI(object sender, KeyEventArgs e)
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
                Hotkey = new Hotkey(Key.None, ModifierKeys.None);
                HotkeyTextBox.Text = "-- not set --";
                return;
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
                HotkeyTextBox.Text = "";
                return;
            }

            // Update the value
            Hotkey = new Hotkey(key, modifiers);

            HotkeyTextBox.Text = Hotkey.ToString();
        }
    }
}
