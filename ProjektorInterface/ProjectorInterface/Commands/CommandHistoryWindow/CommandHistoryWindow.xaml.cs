using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectorInterface.Commands
{
    public partial class CommandHistoryWindow : UserControl
    {
        public CommandHistory History 
        {
            get => _History;
            set
            {
                _History = value;

                // With these 4 events this window is synchronizing the history display with the commands 
                _History.Executed += HistoryExecuted;
                _History.Deleted += HistoryDeleted;
                _History.Undid += HistoryUndid;
                _History.Redid += HistoryRedid;
            }
        }

        CommandHistory _History = null!;

        public CommandHistoryWindow()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        private void HistoryDeleted(object? sender, int index)
            => HistoryPanel.Children.RemoveRange(index, HistoryPanel.Children.Count - index);

        private void HistoryExecuted(object? sender, CanvasCommand command)
            => HistoryPanel.Children.Add(new CommandRecord(command));

        private void HistoryUndid(object? sender, int index)
        {
            CommandRecord current = (CommandRecord)HistoryPanel.Children[index];
            current.Undo();
            current.BringIntoView();
        }

        private void HistoryRedid(object? sender, int index)
        {
            CommandRecord current = (CommandRecord)HistoryPanel.Children[index];
            current.Redo();
            current.BringIntoView();
        }

        private void UndoClick(object sender, RoutedEventArgs e)
            => History.Undo();

        private void RedoClick(object sender, RoutedEventArgs e)
            => History.Redo();
    }
}
