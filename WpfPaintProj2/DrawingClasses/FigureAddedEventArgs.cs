using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPaintProj2.DrawingClasses
{
    public class FigureAddedEventArgs : EventArgs
    {
        public Figure AddedFigure { get; }

        public FigureAddedEventArgs(Figure figure)
        {
            AddedFigure = figure;
        }
    }
}
