using LvpStudio;
using LvpStudio.Helpler;
using LvpStudio.HotkeyHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LvpStudio
{
    class Keybinds
    {
        public static Dictionary<string, Hotkey> KeybindDictionary = LoadDictionary();

        static Dictionary<string, Hotkey> LoadDictionary()
        {
            Dictionary<string, Hotkey> keybinds = new Dictionary<string, Hotkey>();

            AddKeyEntry(keybinds, "ShowPortSelectWindow",    "C");
            AddKeyEntry(keybinds, "AddFrame",                "CTRL + S");
            AddKeyEntry(keybinds, "ProjectFrame",            "CTRL + P"); 
            AddKeyEntry(keybinds, "LoadBgImg",               "CTRL + B");
            AddKeyEntry(keybinds, "SaveAnimation",           "SHIFT + S");
            AddKeyEntry(keybinds, "Line",                    "D1");
            AddKeyEntry(keybinds, "Rectangle",               "D2");
            AddKeyEntry(keybinds, "Ellipse",                 "D3");
            AddKeyEntry(keybinds, "Path",                    "D4");
            AddKeyEntry(keybinds, "Selection",               "D5");
            AddKeyEntry(keybinds, "PlayPause",               "Space");
            AddKeyEntry(keybinds, "SkipAnimation",           "PageDown");
            AddKeyEntry(keybinds, "RevertAnimation",         "PageUp");
            AddKeyEntry(keybinds, "LoadAnimations",          "CTRL + O");

            return keybinds;
        }

        public static void AddKeyEntry(Dictionary<string, Hotkey> keybinds, string name, string defaultHotkey)
            => keybinds.Add(name, new Hotkey(RegistryManager.GetValStr(name + "Key", defaultHotkey)));

        public static Hotkey GetHotkey(string name)
            => KeybindDictionary[name];

        public static bool IsPressed(string name)
            => KeybindDictionary[name].IsPressed();

        public static void UpdateKeyEntry(string keyName, Hotkey newHotkey)
        {
            KeybindDictionary[keyName] = newHotkey;
            RegistryManager.SetValue(keyName + "Key", newHotkey.ToString());
        }
    }
}
