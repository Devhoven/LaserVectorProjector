using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ProjectorInterface.Helper
{
    // static class, which holds all of the constant variables needed for the GUI
    static class Settings
    {
        // Holds the maximum voltage (in mV) 
        public static readonly short MAX_VOLTAGE = 4096;
        // You can't give the galvos too much change in voltage per command, this limits it to 100mV per command
        public static readonly short MAX_STEP_SIZE = 100;
        // Holds how much the full image section is going to be reduced to
        public static float IMG_SECTION = 5 / 8f;

        public static readonly int ZOOM_IMG_SPEED = 15;
        public static readonly int MOVE_IMG_SPEED = 15;
        
        public static readonly int SHAPE_THICKNESS = 5;
        public static readonly Brush SHAPE_COLOR = new SolidColorBrush(Colors.Red);

        public static readonly int RENDERED_IMG_BMP_SIZE = 500;
        public static readonly int RENDERED_IMG_SIZE = 270;

    }
}
