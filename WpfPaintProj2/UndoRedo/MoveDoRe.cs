using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfPaintProj2.DrawingClasses;
using WpfPaintProj2.Helpers;

namespace WpfPaintProj2.UndoRedo
{
    public class MoveDoRe : IUndoRedo
    {
        public Layer Owner { get; private set; }
        public MoveDo Args { get; private set; }

        public MoveDoRe(Layer layer, MoveDo args)
        {
            Args = args;
            Owner = layer;
        }

        public void Invoke()
        {
            Args.Figure.Location = Args.OldPosition;
            //Args.Shape.SetCanvasPoint(Args.OldPosition);
        }

        public IUndoRedo GetInversedAction()
        {
            return new MoveDoRe(this.Owner, new MoveDo(Args.Figure, Args.NewPosition, Args.OldPosition));
        }
    }
}
