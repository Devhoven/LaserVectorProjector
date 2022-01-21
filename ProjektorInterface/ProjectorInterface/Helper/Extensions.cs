using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace ProjectorInterface.Helper
{
    static class Extensions
    {
        // Extension for shapes, returns a string representation of the given shape
        public static string StrRep(this Shape shape)
        {
            if (shape is Line)
                return "Line";
            if (shape is Rectangle)
                return "Rectangle";
            if (shape is Ellipse)
                return "Ellipse";
            if (shape is Path)
                return "Path";
            return "Nothing";
        }
    }
}
