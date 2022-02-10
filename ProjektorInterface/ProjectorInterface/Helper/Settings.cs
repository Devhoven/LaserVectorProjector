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

        public static readonly int ZOOM_IMG_SPEED = 15;
        public static readonly int MOVE_IMG_SPEED = 15;
        
        public static readonly int CANVAS_RESOLUTION = 1080;

        public static readonly int SHAPE_THICKNESS = CANVAS_RESOLUTION / 200;
        public static readonly Brush SHAPE_COLOR = new SolidColorBrush(Colors.Red);
    }
}
