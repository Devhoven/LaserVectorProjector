using ProjectorInterface;
using ProjectorInterface.Helpler;
using ProjectorInterface.HotkeyHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectorInterface
{
    class Keybinds
    {
        public static Dictionary<string, Hotkey> KeybindDictionary = LoadDictionary();

        static Dictionary<string, Hotkey> LoadDictionary()
        {
            Dictionary<string, Hotkey> keybinds = new Dictionary<string, Hotkey>();

            AddKeyEntry(keybinds, "ProjectFrame", Key.P, ModifierKeys.Control);
            AddKeyEntry(keybinds, "LoadBgImg", Key.B, ModifierKeys.Control);
            AddKeyEntry(keybinds, "SaveAnimation", Key.S, ModifierKeys.Control);
            AddKeyEntry(keybinds, "Line", Key.D1, ModifierKeys.None);
            AddKeyEntry(keybinds, "Rectangle", Key.D2, ModifierKeys.None);
            AddKeyEntry(keybinds, "Ellipse", Key.D3, ModifierKeys.None);
            AddKeyEntry(keybinds, "Path", Key.D4, ModifierKeys.None);
            AddKeyEntry(keybinds, "PlayPause", Key.Space, ModifierKeys.None);
            AddKeyEntry(keybinds, "SkipAnimation", Key.Right, ModifierKeys.Shift);
            AddKeyEntry(keybinds, "RevertAnimation", Key.Left, ModifierKeys.Shift);
            AddKeyEntry(keybinds, "LoadAnimations", Key.O, ModifierKeys.Control);

            return keybinds;
        }

        static void AddKeyEntry(Dictionary<string, Hotkey> keybinds, string name, Key key, ModifierKeys modifier)
            => keybinds.Add(name, RegistryManager.GetVal(name + "Key", new Hotkey(key, modifier)));

        public static Hotkey GetHotkey(string name)
            => KeybindDictionary[name];

        public static bool IsPressed(string name)
            => KeybindDictionary[name].IsPressed();

    }
}
