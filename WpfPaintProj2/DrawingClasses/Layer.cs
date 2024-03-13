using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfPaintProj2.DrawingClasses
{
    public class Layer : PositionObject
    {
        private Brush fill = Brushes.Red;
        private string name;
        private bool isVisible = true;

        public bool IsVisible
        {
            get => isVisible;
            set
            {
                isVisible = value;
                VisibleChanged?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public Brush Fill
        {
            get => fill;
            set
            {
                fill = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler VisibleChanged;
    }
}
