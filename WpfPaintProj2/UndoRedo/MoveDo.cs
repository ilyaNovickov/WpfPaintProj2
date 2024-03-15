using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;

namespace WpfPaintProj2.UndoRedo
{
    public struct MoveDo
    {
        public MoveDo(Shape shape, Point old, Point point)
        {
            Shape = shape;
            OldPosition = old;
            NewPosition = point;
        }
        public Shape Shape { get; }

        public Point OldPosition { get; }

        public Point NewPosition { get; }
    }
}
