using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using WpfPaintProj2.DrawingClasses;
using WpfPaintProj2.Helpers;

namespace WpfPaintProj2.UndoRedo
{
    public class ResizeDoRe : IUndoRedo
    {
        public Layer Layer { get; }
        public ResizeDoArgs Args { get; }

        public ResizeDoRe(Layer layer, ResizeDoArgs args)
        {
            Args = args;
            Layer = layer;
        }

        public void Invoke()
        {
            Args.Figure.Location = Args.OldPosition;
            //Args.Shape.SetCanvasPoint(Args.OldPosition);
            Args.Figure.Width = Args.OldSize.Width;
            Args.Figure.Height = Args.OldSize.Height;
        }

        public IUndoRedo GetInversedAction()
        {
            return new ResizeDoRe(this.Layer,
                new ResizeDoArgs(Args.Figure,
                Args.NewPosition, Args.OldPosition,
                Args.NewSize, Args.OldSize));
        }
    }
}
