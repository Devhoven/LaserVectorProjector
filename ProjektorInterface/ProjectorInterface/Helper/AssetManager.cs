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
        public static BitmapFrame GetBmpFrame(string fileName)
            => BitmapFrame.Create(GetStream(fileName));

        public static Stream GetStream(string assetName)
            => System.Windows.Application.GetResourceStream(new Uri(@"/Assets/CommandImages/" + assetName, UriKind.Relative)).Stream;
    }
}
