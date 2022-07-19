using System.Text;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectorInterface.HotkeyHelper
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
