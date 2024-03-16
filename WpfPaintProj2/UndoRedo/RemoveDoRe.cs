using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfPaintProj2.DrawingClasses;

namespace WpfPaintProj2.UndoRedo
{
    public class RemoveDoRe : IUndoRedo
    {
        public DrawingField DrawingField { get; }
        public Layer Layer { get; }
        public AddRemoveDoArgs Args { get; }
        public RemoveDoRe(Layer layer, AddRemoveDoArgs args, DrawingField drawingField)
        {
            Args = args;
            Layer = layer;
            DrawingField = drawingField;
        }
        public void Invoke()
        {
            DrawingField.AddFigureToSelectedLayer_Internal(Args.Shape, Layer);
        }

        public IUndoRedo GetInversedAction()
        {
            return new AddDoRe(this.Layer, this.Args, this.DrawingField);
        }
    }
}
