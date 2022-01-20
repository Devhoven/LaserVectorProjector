using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectorInterface.Commands
{
    // Superclass for the Command Design Pattern
    abstract class DrawCommand
    {
        // A static reference to the canvas which is currently being drawn on 
        public static Canvas Parent = null!;

        // Has to be implemented by the specific command
        public abstract void Execute();

        // Does exactly the opposite of Execute()
        public abstract void Undo();
    }
}
