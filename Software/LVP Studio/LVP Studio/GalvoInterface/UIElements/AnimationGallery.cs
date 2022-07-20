using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LvpStudio.GalvoInterface.UIElements
{
    class AnimationGalleryPanel : StackPanel
    {
        public AnimationGalleryPanel()
            => AnimationManager.GetAnimation(AnimationManager.Source.AnimationGallery).OnImgIndexChanged += OnImgIndexChanged;
        
        public void Clear()
            => Children.Clear();

        public void AddBorder(RenderedItemBorder newElement)
            => Children.Add(newElement);

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
