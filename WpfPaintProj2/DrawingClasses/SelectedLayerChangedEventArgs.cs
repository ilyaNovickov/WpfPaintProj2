using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPaintProj2.DrawingClasses
{
    public class SelectedLayerChangedEventArgs : EventArgs
    {
        public Layer Layer { get; }
        public Layer OldLayer { get; }
        public SelectedLayerChangedEventArgs(Layer layer, Layer oldLayer = null)
        {
            Layer = layer;
            OldLayer = oldLayer;
        }
    }
}
