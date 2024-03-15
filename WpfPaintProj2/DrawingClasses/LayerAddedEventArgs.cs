using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPaintProj2.DrawingClasses
{
    public class LayerAddedEventArgs : EventArgs
    {
        public Layer Layer;

        public LayerAddedEventArgs(Layer layer)
        {
            Layer = layer;
        }
    }
}
