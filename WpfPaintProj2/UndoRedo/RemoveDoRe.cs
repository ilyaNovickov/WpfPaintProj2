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
        public Layer Owner { get; private set; }
        public AddRemoveDo Args { get; private set; }
        public RemoveDoRe(Layer layer, AddRemoveDo args)
        {
            Args = args;
            Owner = layer;
        }
        public void Invoke()
        {
            Owner.__AddShape(Args.Shape);
        }

        public IUndoRedo GetInversedAction()
        {
            return new AddDoRe(this.Owner, this.Args);
        }
    }
}
