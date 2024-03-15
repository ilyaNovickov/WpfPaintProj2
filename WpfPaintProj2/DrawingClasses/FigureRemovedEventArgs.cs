using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPaintProj2.DrawingClasses
{
    public class FigureRemovedEventArgs : EventArgs
    {
        public Figure RemovedFigure { get; }

        public FigureRemovedEventArgs(Figure figure)
        {
            RemovedFigure = figure;
        }
    }
}
