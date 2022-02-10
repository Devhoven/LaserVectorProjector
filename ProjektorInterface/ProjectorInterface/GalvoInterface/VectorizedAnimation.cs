using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectorInterface.Helper;
using ProjectorInterface.GalvoInterface;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;

namespace ProjectorInterface
{
    
    class VectorizedAnimation
    {
        public List<VectorizedImage> img;
        public VectorizedImage currentImage;

        public VectorizedAnimation(List<VectorizedImage> list, UIElement p)
        {
            img = list;
            currentImage = getImageAt(39);
            ((StackPanel)p).Children.Add(new RenderedImage(this));
        }

        public void addImage(VectorizedImage vImage)
            => img.Add(vImage);


       public VectorizedImage getImageAt(int i)
       {
            return img.ElementAt(i);
       }


        VectorizedImage getCurrent()
        {
            return currentImage;
        }

    }
}
