using Microsoft.Win32;

namespace ProjectorInterface.Helpler
{
    // Small wrapper for the Registry class, so I don't have to specify the KeyName all of the time
    static class RegistryManager
    {
        static readonly string KeyName = @"SOFTWARE\ProjectorInterface";

        public static void SetValue(string ValName, object Value)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(KeyName))
                key.SetValue(ValName, Value);
        }

        public static T GetVal<T>(string ValName, T DefaultVal)
        {
            // Create SubKey creates a new subkey or opens it if it exists
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(KeyName))
                return (T?)key.GetValue(ValName, DefaultVal) ?? DefaultVal;
        }
    }
}