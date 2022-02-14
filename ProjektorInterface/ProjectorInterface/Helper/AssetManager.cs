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
        public static Stream GetStream(string assetName)
        {
            return System.Windows.Application.GetResourceStream(new Uri(@"/Assets/CommandImages/" + assetName, UriKind.Relative)).Stream;
        }
    }
}
