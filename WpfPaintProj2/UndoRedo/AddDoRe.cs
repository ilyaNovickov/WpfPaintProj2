using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfPaintProj2.DrawingClasses;

namespace WpfPaintProj2.UndoRedo
{
    public class AddDoRe : IUndoRedo
    {
        public DrawingField DrawingField { get; }
        public Layer Owner { get; private set; }
        public AddRemoveDo Args { get; private set; }

        public AddDoRe(Layer layer, AddRemoveDo args, DrawingField drawingField)
        {
            Args = args;
            Owner = layer;
            DrawingField = drawingField;
        }

        public void Invoke()
        {
            this.DrawingField.RemoveFigureInSelectedLayer_Internal(Args.Shape, Owner);
        }

        public IUndoRedo GetInversedAction()
        {
            return new RemoveDoRe(this.Owner, this.Args, this.DrawingField);
        }
    }
}
