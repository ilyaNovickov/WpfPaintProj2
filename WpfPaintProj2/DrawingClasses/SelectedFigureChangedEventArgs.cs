using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPaintProj2.DrawingClasses
{
    public class SelectedFigureChangedEventArgs : EventArgs
    {
        public Figure SelectedFigure { get; }

        public SelectedFigureChangedEventArgs(Figure figure)
        {
            SelectedFigure = figure;
        }
    }
}
