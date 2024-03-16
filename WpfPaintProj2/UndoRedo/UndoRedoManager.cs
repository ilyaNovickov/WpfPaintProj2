using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfPaintProj2.DrawingClasses;

namespace WpfPaintProj2.UndoRedo
{
    public class UndoRedoManager
    {
        private readonly Stack<IUndoRedo> undo = new Stack<IUndoRedo>(1);
        private readonly Stack<IUndoRedo> redo = new Stack<IUndoRedo>(1);

        public Layer Owner { get; private set; }

        public UndoRedoManager(Layer layer)
        {
            Owner = layer;
        }

        public void Undo()
        {
            if (undo.Count == 0)
                return;

            IUndoRedo action = undo.Pop();

            action.Invoke();

            redo.Push(action.GetInversedAction());
        }

        public void Redo()
        {
            if (redo.Count == 0)
                return;

            IUndoRedo action = redo.Pop();

            action.Invoke();

            undo.Push(action.GetInversedAction());
        }

        public void RegistrAction(IUndoRedo action)
        {
            undo.Push(action);
            redo.Clear();
        }

    }
}
