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
        private string name = $"{StandartShapes.Rectangele.ToString()} [{0}, {0}]";
        private Brush fill = Brushes.White;
        private Brush fore = Brushes.Black;
        private StandartShapes shapeType = StandartShapes.Rectangele;

        public StandartShapes Type
        {
            get => shapeType;
            set
            {
                shapeType = value;
                Name = $"{this.Type.ToString()} [{X}, {Y}]";
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

        public Brush Fore
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

            Name = $"{this.Type.ToString()} [{X}, {Y}]";
        }
    }
}
