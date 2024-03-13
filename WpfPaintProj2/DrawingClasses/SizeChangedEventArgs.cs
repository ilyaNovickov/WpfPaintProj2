using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfPaintProj2.DrawingClasses
{
    public class SizeChangedEventArgs : MovedEventArgs
    {
        public Size OldSize { get; }

        public Size NewSize { get; }

        public SizeChangedEventArgs(PositionObject sender, Point old, Point newLoc, Size oldSize, Size newSize)
            : base(sender, old, newLoc)
        {
            OldSize = oldSize;
            NewSize = newSize;
        }
    }
}
