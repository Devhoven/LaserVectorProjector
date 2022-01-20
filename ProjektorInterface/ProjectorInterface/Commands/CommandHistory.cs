using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectorInterface.Commands
{
    // Wrapper for a do / undo history
    class CommandHistory
    {
        // Holds all of the commands
        List<DrawCommand> Commands;
        // Holds the index of the command which was executed last
        int CurrentIndex;
        public CommandHistory()
        {
            Commands = new List<DrawCommand>();
            CurrentIndex = 0;
        }

        // Adds the command to the list and executes it 
        public void Execute(DrawCommand command)
        {
            // If some commands have been undone, the CurrentIndex variable is going to be smaller than the number of the elements
            // The commands that were undone, have to be deleted from the list
            if (CurrentIndex < Commands.Count)
            {
                Commands.RemoveRange(CurrentIndex, Commands.Count - CurrentIndex);
            }

            // Adds the new command to the list, executes it and increses the index
            Commands.Add(command);
            command.Execute();
            CurrentIndex++;
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
        }

        // Redoes the last undone command
        public void Redo()
        {
            if (CurrentIndex == Commands.Count)
                return;
            Commands[CurrentIndex].Execute();
            CurrentIndex++;
        }
    }
}
