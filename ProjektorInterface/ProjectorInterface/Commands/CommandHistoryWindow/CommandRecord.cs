using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProjectorInterface.Commands
{
    // Used to display one command in the command history
    class CommandRecord : DockPanel
    {
        // Height of each record
        const int RECORD_HEIGHT = 20;
        // Colors for each state
        static readonly Brush ActivatedForeground = new SolidColorBrush(Colors.Black);
        static readonly Brush DeactivatedForeground = new SolidColorBrush(Colors.Gray);

        Label CommandDesc;

        public CommandRecord(CanvasCommand command)
        {
            Image commandImg = new Image()
            {
                Source = BitmapFrame.Create(command.GetImgFile()),
                Height = RECORD_HEIGHT
            };
            // Otherwise the ellipse image would look weird
            RenderOptions.SetBitmapScalingMode(commandImg, BitmapScalingMode.Fant);

            CommandDesc = new Label()
            {
                Content = command.ToString(),
                VerticalAlignment = VerticalAlignment.Center
            };

            Children.Add(commandImg);
            Children.Add(CommandDesc);

            SetDock(CommandDesc, Dock.Left);
        }

        public void Undo()
            => CommandDesc.Foreground = DeactivatedForeground;

        public void Redo()
            => CommandDesc.Foreground = ActivatedForeground;
    }
}
