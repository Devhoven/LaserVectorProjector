using ProjectorInterface.Helper;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProjectorInterface.DrawingCommands
{
    // Used to display one command in the command history
    class CommandRecord : Border
    {
        // Height of each record
        public const int RECORD_HEIGHT = 20;

        static BitmapFrame UndoIcon;
        static BitmapFrame RedoIcon;

        static CommandRecord()
        {
            UndoIcon = AssetManager.GetBmpFrame("UndoIcon.png");
            RedoIcon = AssetManager.GetBmpFrame("RedoIcon.png");
        }

        public static CommandRecord CreateNew(CanvasCommand command)
        {
            CommandRecord result = new CommandRecord();

            StackPanel contentPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };

            Image commandImg = new Image()
            {
                Source = command.GetBmpFrame(),
                Height = RECORD_HEIGHT
            };
            // Otherwise the ellipse image would look weird
            RenderOptions.SetBitmapScalingMode(commandImg, BitmapScalingMode.Fant);

            Label commandDesc = new Label()
            {
                Content = command.ToString(),
                VerticalAlignment = VerticalAlignment.Center
            };

            contentPanel.Children.Add(commandImg);
            contentPanel.Children.Add(commandDesc);

            result.Child = contentPanel;

            return result;
        }

        public static CommandRecord CreateNewUndid(CanvasCommand command)
        {
            CommandRecord result = CreateNew(command);

            ((StackPanel)result.Child).Children.Insert(0, new Image()
            {
                Source = UndoIcon,
                Width = RECORD_HEIGHT
            });

            return result;
        }

        public static CommandRecord CreateNewRedid(CanvasCommand command)
        {
            CommandRecord result = CreateNew(command);

            ((StackPanel)result.Child).Children.Insert(0, new Image()
            {
                Source = RedoIcon,
                Width = RECORD_HEIGHT
            });

            return result;
        }
    }
}