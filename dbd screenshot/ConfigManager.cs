using System;
using Microsoft.Win32;

namespace dbd_screenshot
{
    public class ConfigManager
    {
        private const string RegistryKeyPath = "Software\\sldw.de\\dbd_screenshot";

        public void SaveConfig(string key, string value)
        {
            try
            {
                using RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(RegistryKeyPath);
                registryKey?.SetValue(key, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving configuration: " + ex.Message);
            }
        }

        public string LoadConfig(string key, string defaultValue)
        {
            try
            {
                using RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
                if (registryKey != null)
                {
                    object? value = registryKey.GetValue(key);
                    if (value != null)
                    {
                        return value.ToString() ?? defaultValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading configuration: " + ex.Message);
            }
            return defaultValue;
        }
    }
}
