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
        public Layer layer { get; }
        public MoveDoArgs Args { get; }

        public MoveDoRe(Layer layer, MoveDoArgs args)
        {
            Args = args;
            this.layer = layer;
        }

        public void Invoke()
        {
            Args.Figure.Location = Args.OldPosition;
            //Args.Shape.SetCanvasPoint(Args.OldPosition);
        }

        public IUndoRedo GetInversedAction()
        {
            return new MoveDoRe(this.layer, new MoveDoArgs(Args.Figure, Args.NewPosition, Args.OldPosition));
        }
    }
}
