using LvpStudio.Helper;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LvpStudio.DrawingCommands
{
    // Used to display one command in the command history
    class CommandRecord : StackPanel
    {
        // Height of each record
        public const int RECORD_HEIGHT = 20;

        static BitmapFrame UndoIcon = AssetManager.GetBmpFrame("CommandImages/UndoIcon.png");
        static BitmapFrame RedoIcon = AssetManager.GetBmpFrame("CommandImages/RedoIcon.png");

        public static CommandRecord CreateNew(CanvasCommand command)
        {
            CommandRecord result = new CommandRecord();

            Image commandImg = new Image()
            {
                Source = command.GetBmpFrame(),
                Height = RECORD_HEIGHT
            };

            Label commandDesc = new Label()
            {
                Content = command.ToString(),
                VerticalAlignment = VerticalAlignment.Center
            };

            result.Children.Add(commandImg);
            result.Children.Add(commandDesc);

            result.Orientation = Orientation.Horizontal;

            return result;
        }

        // Adds a undo picture in front of the record
        public static CommandRecord CreateNewUndid(CanvasCommand command)
        {
            CommandRecord result = CreateNew(command);

            result.Children.Insert(0, new Image()
            {
                Source = UndoIcon,
                Width = RECORD_HEIGHT
            });

            return result;
        }

        // Adds a redo picture in front of the record
        public static CommandRecord CreateNewRedid(CanvasCommand command)
        {
            CommandRecord result = CreateNew(command);

            result.Children.Insert(0, new Image()
            {
                Source = RedoIcon,
                Width = RECORD_HEIGHT
            });

            return result;
        }
    }
}