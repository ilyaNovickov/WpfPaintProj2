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
    public struct ResizeDo
    {
        public ResizeDo(Figure figure, Point old, Point point, Size oldSize, Size newSize)
        {
            Figure = figure;
            OldPosition = old;
            NewPosition = point;
            OldSize = oldSize;
            NewSize = newSize;
        }
        public Figure Figure { get; }

        public Point OldPosition { get; }

        public Point NewPosition { get; }

        public Size OldSize { get; }

        public Size NewSize { get; }
    }
}
