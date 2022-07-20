using System.Text;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LvpStudio.HotkeyHelper
{
    public class Hotkey
    {
        public Key Key;

        public ModifierKeys Modifiers;

        public Hotkey(Key key, ModifierKeys modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        // Parses the data from the string to a hotkey
        public Hotkey(string hotkeyStr)
        {
            ModifierKeysConverter conv = new ModifierKeysConverter();

            string modifierKeysStr = GetModifierKeysStr(hotkeyStr);
            string keysStr = hotkeyStr.Substring(modifierKeysStr.Length);

            Modifiers = (ModifierKeys?)conv.ConvertFromString(modifierKeysStr) ?? ModifierKeys.None;

            Enum.TryParse(Key.GetType(), keysStr, out object? result);
            Key = ((Key?)result).GetValueOrDefault(Key.None);
        }

        static readonly List<string> ModifierKeyStr = new List<string>() { "CTRL", "ALT", "SHIFT" };
        // Returns a substring of the input, which only contains modifiers
        static string GetModifierKeysStr(string hotkeyStr)
        {
            string[] hotkeySplit = hotkeyStr.Trim().Split('+');
            for (int i = 0; i < hotkeySplit.Length; i++)
            {
                if (!ModifierKeyStr.Contains(hotkeySplit[i].Trim().ToUpper()))
                {
                    return hotkeyStr.Substring(0, hotkeyStr.LastIndexOf(hotkeySplit[i]));
                }
            }
            return hotkeyStr;
        }

        public bool IsPressed()
            => (Keyboard.GetKeyStates(Key) & KeyStates.Down) > 0 && Keyboard.Modifiers == Modifiers;

        // return string with pressed commands + buttons
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            if (Modifiers.HasFlag(ModifierKeys.Control))
                str.Append("Ctrl + ");
            if (Modifiers.HasFlag(ModifierKeys.Shift))
                str.Append("Shift + ");
            if (Modifiers.HasFlag(ModifierKeys.Alt))
                str.Append("Alt + ");

            str.Append(Key);

            return str.ToString();
        }
    }
}
