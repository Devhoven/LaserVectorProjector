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
        static HashSet<HotkeyEditor> HotkeyEditors = new HashSet<HotkeyEditor>();

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
                Hotkey = Keybinds.GetHotkey(KeyName);
                HotkeyTextBox.Text = Hotkey.ToString();
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

            HotkeyEditors.Add(this);
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
                UnsetHotkey();
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
            Hotkey tmpHotkey = new Hotkey(key, modifiers);

            if (HotkeyOccupied(tmpHotkey))
            {
                HotkeyEditor otherHotkeyEditor = GetHotkeyEditor(tmpHotkey);
                MessageBoxResult result = MessageBox.Show($"\"{otherHotkeyEditor.KeyDisplayName}\" already occupies that key combination. Do you want to override it?", 
                    "Key combination taken", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                    otherHotkeyEditor.UnsetHotkey();
                else if (result == MessageBoxResult.No)
                    tmpHotkey = Hotkey;
            }
            Hotkey = tmpHotkey;
            HotkeyTextBox.Text = Hotkey.ToString();
        }

        void UnsetHotkey()
        {
            Hotkey = new Hotkey(Key.None, ModifierKeys.None);
            HotkeyTextBox.Text = "-- not set --";
        }

        bool HotkeyOccupied(Hotkey hotkey)
            => HotkeyEditors.Any(h => h.Hotkey.Equals(hotkey) && h != this);

        HotkeyEditor GetHotkeyEditor(Hotkey hotkey)
            => HotkeyEditors.First(h => h.Hotkey.Equals(hotkey) && h != this);
    }
}
