using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectorInterface.Commands
{
    public partial class CommandHistoryWindow : Window
    {
        CommandHistory History;

        public CommandHistoryWindow(CommandHistory history)
        {
            InitializeComponent();

            History = history;

            // With these 4 events this window is synchronizing the history display with the commands 
            History.Executed += HistoryExecuted;
            History.Deleted += HistoryDeleted;
            History.Undid += HistoryUndid;
            History.Redid += HistoryRedid;
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
