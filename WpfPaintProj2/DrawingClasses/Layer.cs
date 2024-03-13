using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfPaintProj2.DrawingClasses
{
    class Layer : PositionObject
    {
        private Brush fill = Brushes.White;
        private string name;

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
    }
}
