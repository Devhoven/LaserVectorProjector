using ProjectorInterface.Helper;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ProjectorInterface.DrawingCommands
{
    // Superclass for the Command Design Pattern
    public abstract class CanvasCommand
    {
        // A static reference to the canvas which is currently being drawn on 
        public static Canvas Parent = null!;

        protected static Dictionary<string, BitmapFrame> Icons = new Dictionary<string, BitmapFrame>();

        protected CanvasCommand(string iconFileName)
        {
            if (!Icons.ContainsKey(iconFileName))
                Icons.Add(iconFileName, AssetManager.GetBmpFrame(iconFileName));
        }

        // Has to be implemented by the specific command
        public abstract void Execute();

        // Does exactly the opposite of Execute()
        public abstract void Undo();

        public abstract BitmapFrame GetBmpFrame();
    }
}
