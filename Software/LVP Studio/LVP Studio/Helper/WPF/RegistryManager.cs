using Microsoft.Win32;

namespace LvpStudio.Helpler
{
    // Small wrapper for the Registry class, so I don't have to specify the KeyName all of the time
    static class RegistryManager
    {
        static readonly string KeyName = @"SOFTWARE\LvpStudio";

        public static void SetValue<T>(string valName, T value)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(KeyName))
                key.SetValue(valName, value);
        }

        public static T GetVal<T>(string valName, T defaultVal)
        {
            // Create SubKey creates a new subkey or opens it if it exists
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(KeyName))
                return (T?)key.GetValue(valName, defaultVal) ?? defaultVal;
        }

        public static string GetValStr(string valName, string defaultVal)
        {
            // Create SubKey creates a new subkey or opens it if it exists
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(KeyName))
                return (string?)key.GetValue(valName, defaultVal) ?? defaultVal;
        }
    }
}