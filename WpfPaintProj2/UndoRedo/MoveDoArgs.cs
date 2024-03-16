using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using WpfPaintProj2.DrawingClasses;

namespace WpfPaintProj2.UndoRedo
{
    public readonly struct MoveDoArgs
    {
        public MoveDoArgs(Figure figure, Point old, Point point)
        {
            Figure = figure;
            OldPosition = old;
            NewPosition = point;
        }
        public Figure Figure { get; }

        public Point OldPosition { get; }

        public Point NewPosition { get; }
    }
}
