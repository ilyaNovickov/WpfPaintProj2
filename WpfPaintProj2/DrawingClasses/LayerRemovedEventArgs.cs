using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPaintProj2.DrawingClasses
{
    public class LayerRemovedEventArgs : EventArgs
    {
        public Layer Layer { get; }
        public int Index { get; }

        public LayerRemovedEventArgs(Layer layer, int index)
        {
            Layer = layer;
            Index = index;
        }
    }
}
