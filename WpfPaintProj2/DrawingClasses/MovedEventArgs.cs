using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfPaintProj2.DrawingClasses
{
    public class MovedEventArgs : EventArgs
    {
        public PositionObject Sender { get; }
        public Point OldLocation { get; }

        public Point NewLocation { get; }

        public double Dx => NewLocation.X - OldLocation.X;

        public double Dy => NewLocation.Y - OldLocation.Y;

        public MovedEventArgs(PositionObject sender, Point old, Point newLoc)
        {
            Sender = sender;
            OldLocation = old;
            NewLocation = newLoc;
        }
    }
}
