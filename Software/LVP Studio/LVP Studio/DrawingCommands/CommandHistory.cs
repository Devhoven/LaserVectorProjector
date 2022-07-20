using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LvpStudio.DrawingCommands
{
    // Wrapper for a do / undo history
    public class CommandHistory
    {
        // Used for the history display
        public delegate void CommandEventHandler(CanvasCommand command);
        // Event which fires, when a new command got executed
        public event CommandEventHandler? Executed;

        public delegate void ActionEventHandler(int index);
        // When a command got undone, sends the command which was undone with it
        public event CommandEventHandler? Undid;
        // When a command was redid, sends the command which was redid with it
        public event CommandEventHandler? Redid;

        // Holds all of the commands
        List<CanvasCommand> Commands;
        // Holds the index of the command which was executed last
        int CurrentIndex;

        public CommandHistory()
        {
            Commands = new List<CanvasCommand>();
            CurrentIndex = 0;
        }

        // Adds the command to the list and executes it 
        public void Execute(CanvasCommand command)
        {
            // If some commands have been undone, the CurrentIndex variable is going to be smaller than the number of the elements
            // The commands that were undone, have to be deleted from the list
            if (CurrentIndex < Commands.Count)
                Commands.RemoveRange(CurrentIndex, Commands.Count - CurrentIndex);

            // Adds the new command to the list, executes it and increases the index
            Commands.Add(command);
            command.Execute();
            CurrentIndex++;

            Executed?.Invoke(command);
        }

        // Lowers CurrentIndex and undos the last command, but does not delete it yet
        public void Undo()
        {
            CurrentIndex--;
            if (CurrentIndex < 0)
            {
                CurrentIndex = 0;
                return;
            }
            Commands[CurrentIndex].Undo();

            Undid?.Invoke(Commands[CurrentIndex]);
        }

        // Redoes the last undone command
        public void Redo()
        {
            if (CurrentIndex == Commands.Count)
                return;
            Commands[CurrentIndex].Execute();

            Redid?.Invoke(Commands[CurrentIndex]);

            CurrentIndex++;
        }
    }
}
