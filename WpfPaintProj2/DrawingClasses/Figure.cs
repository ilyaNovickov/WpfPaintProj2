using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfPaintProj2.DrawingClasses
{
    public class Figure : PositionObject
    {
        private string name = $"{StandartShapes.Rectangele} [{0}, {0}]";
        private Color fill = Colors.White;// Brushes.White;
        private Color fore = Colors.Black;// Brushes.Black;
        private StandartShapes shapeType = StandartShapes.Rectangele;

        public StandartShapes Type
        {
            get => shapeType;
            set
            {
                shapeType = value;
                Name = $"{this.Type} [{X}, {Y}]";
                OnPropertyChanged();
            }
        }

        public Color Fill
        {
            get => fill;
            set
            {
                fill = value;
                OnPropertyChanged();
            }
        }

        public Color Fore
        {
            get => fore;
            set
            {
                fore = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => name;
            private set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public override void OnMoved(MovedEventArgs e)
        {
            base.OnMoved(e);

            Name = $"{this.Type} [{X}, {Y}]";
        }
    }
}
