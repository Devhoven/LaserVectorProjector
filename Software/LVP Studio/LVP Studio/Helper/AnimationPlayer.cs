using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace ProjectorInterface.Helper
{
    static class AnimationPlayer
    {
        public static void FadeIn(FrameworkElement elem)
            => ((Storyboard)Application.Current.FindResource("FadeIn")).Begin(elem);

        public static void FadeOut(FrameworkElement elem)
            => ((Storyboard)Application.Current.FindResource("FadeOut")).Begin(elem);
    }
}
