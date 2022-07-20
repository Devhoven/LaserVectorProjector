using LvpStudio.Helpler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LvpStudio.Helper
{
    // static class, which holds all of the constant variables needed for the GUI
    public partial class Settings
    {
        public static readonly int SCAN_SPEED = 30_000;

        public static readonly int SCAN_FPS = 30;

        // Duration of how long one frame needs to be in ms to achieve the given frame rate
        public static int INV_SCAN_FPS_MS { private set; get; } = (int)(1000.0 / SCAN_FPS);
        // Holds the maximum voltage (in mV) 
        public static readonly short MAX_VOLTAGE = 4095;
        // You can't give the galvos too much change in voltage per command, this limits it to 150mV per command
        public static readonly short MAX_STEP_SIZE = 150;
        // The MAX_STEP_SIZE for points which are turned off
        public static readonly short OFF_LINE_MAX_STEP_SIZE = (short)(MAX_STEP_SIZE / 3.0);
        // The angle at which more points are going to be added to the edge 
        // Currently at 45 degrees in radians
        public static readonly double ADJUST_ANGLE = 0.785398;

        // The epsilon for the RDP algorithm (the maximal distance a point can be from a line)
        public static readonly double RDP_EPSILON = 2;

        public static float IMG_SECTION 
        {
            get => IMG_SECTION_CACHED;
            set 
            {
                IMG_SECTION_CACHED = value;
                IMG_SECTION_SIZE = (short)(MAX_VOLTAGE * IMG_SECTION_CACHED);
                IMG_OFFSET = (short)((MAX_VOLTAGE - IMG_SECTION_SIZE) / 2.0);
            }
        }
        // Holds how much the full image section is going to be reduced to
        static float IMG_SECTION_CACHED = 3 / 4f;
        // The new maximum size of the image section (in mV)
        public static short IMG_SECTION_SIZE = (short)(MAX_VOLTAGE * IMG_SECTION_CACHED);
        // The offset which needs to be added to any coord, otherwise it won't be centered
        public static short IMG_OFFSET = (short)((MAX_VOLTAGE - IMG_SECTION_SIZE) / 2.0);

        public static readonly int SHAPE_THICKNESS = 5;
        public static readonly Brush SHAPE_COLOR = Brushes.Red;

        public static readonly int RENDERED_IMG_BMP_SIZE = 500;

        public static bool DEBUG_MODE = false;
    }
}
