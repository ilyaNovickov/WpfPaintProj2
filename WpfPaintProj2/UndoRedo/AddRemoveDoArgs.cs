using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using WpfPaintProj2.DrawingClasses;

namespace WpfPaintProj2.UndoRedo
{
    public readonly struct AddRemoveDoArgs
    {
        public AddRemoveDoArgs(Figure shape)
        {
            Shape = shape;
        }
        public Figure Shape { get; }
    }
}
