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
        public Layer Owner { get; private set; }
        public AddRemoveDo Args { get; private set; }
        public RemoveDoRe(Layer layer, AddRemoveDo args, DrawingField drawingField)
        {
            Args = args;
            Owner = layer;
            DrawingField = drawingField;
        }
        public void Invoke()
        {
            DrawingField.AddFigureToSelectedLayer_Internal(Args.Shape, Owner);
        }

        public IUndoRedo GetInversedAction()
        {
            return new AddDoRe(this.Owner, this.Args, this.DrawingField);
        }
    }
}
