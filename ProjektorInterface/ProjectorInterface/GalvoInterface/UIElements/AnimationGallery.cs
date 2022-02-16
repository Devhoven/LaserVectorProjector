using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectorInterface.GalvoInterface.UIElements
{
    class AnimationGalleryPanel : StackPanel
    {
        public AnimationGalleryPanel()
            => SerialManager.OnImgIndexChanged += OnImgIndexChanged;

        private void OnImgIndexChanged(int oldVal, int newVal)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (oldVal < Children.Count)
                    ((RenderedItemBorder)Children[oldVal]).Deselect();
                if (newVal < Children.Count)
                    ((RenderedItemBorder)Children[newVal]).Select();
            }));
        }
    }
}
