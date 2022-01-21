using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ProjectorInterface.Helper
{
    static class AssetManager
    {
        static Assembly Assembly;

        static string[] ResourceNames;
        static AssetManager()
        {
            Assembly = Assembly.GetExecutingAssembly();
            ResourceNames = Assembly.GetManifestResourceNames();
        }

        public static Stream GetStream(string assetName)
        {
            string fullName = ResourceNames.Single(str => str.EndsWith(assetName));
            Stream? result = Assembly.GetManifestResourceStream(fullName);
            if (result == null)
                throw new FileNotFoundException("This embedded resource does not exist");
            return result;
        }
    }
}
