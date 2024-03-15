using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPaintProj2.UndoRedo
{
    public interface IUndoRedo
    {
        IUndoRedo GetInversedAction();
        void Invoke();
    }
}
