using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfPaintProj2.DrawingClasses
{
    public abstract class PositionObject : INotifyPropertyChanged
    {
        private Rect rect = new Rect(0d, 0d, 100d, 100d);

        public Point Location
        {
            get => rect.Location;
            set
            {
                Point old = rect.Location;
                rect.Location = value;
                OnMoved(new MovedEventArgs(this, old, rect.Location));
                OnPropertyChanged();
            }
        }

        public double X
        {
            get => rect.X;
            set
            {
                Point old = rect.Location;
                rect.X = value;
                OnMoved(new MovedEventArgs(this, old, rect.Location));
                OnPropertyChanged();
            }
        }

        public double Y
        {
            get => rect.Y;
            set
            {
                Point old = rect.Location;
                rect.Y = value;
                OnMoved(new MovedEventArgs(this, old, rect.Location));
                OnPropertyChanged();
            }
        }

        public Size Size
        {
            get => rect.Size;
            set
            {
                Point old = rect.Location;
                Size oldSize = rect.Size;
                rect.Size = value;
                OnSizeChanged(new SizeChangedEventArgs(this, old,
                    rect.Location, oldSize, rect.Size));
                OnPropertyChanged();
            }
        }

        public double Width
        {
            get => rect.Width;
            set
            {
                Point old = rect.Location;
                Size oldSize = rect.Size;
                rect.Width = value;
                OnSizeChanged(new SizeChangedEventArgs(this, old,
                    rect.Location, oldSize, rect.Size));
                OnPropertyChanged();
            }
        }
        public double Height
        {
            get => rect.Height;
            set
            {
                Point old = rect.Location;
                Size oldSize = rect.Size;
                rect.Height = value;
                OnSizeChanged(new SizeChangedEventArgs(this, old,
                    rect.Location, oldSize, rect.Size));
                OnPropertyChanged();
            }
        }


        public event EventHandler<MovedEventArgs> Moved;

        public event EventHandler<SizeChangedEventArgs> SizeChanged;



        public virtual void OnMoved(MovedEventArgs e)
        {
            Moved?.Invoke(this, e);
        }

        public virtual void OnSizeChanged(SizeChangedEventArgs e)
        {
            SizeChanged?.Invoke(this, e);
        }

        public void Offset(double dx, double dy)
        {
            this.Location = new Point(Location.X + dx, Location.Y + dy);
        }

        public void SetSize(double dx, double dy, ResizeDirection resizeDirection)
        {
            try
            {
                switch (resizeDirection)
                {
                    case ResizeDirection.Top:
                        this.Height -= dy;
                        this.Offset(0, dy);
                        break;
                    case ResizeDirection.Bottom:
                        this.Height += dy;
                        this.Offset(0, 0);
                        break;
                    case ResizeDirection.Left:
                        this.Width -= dx;
                        this.Offset(dx, 0);
                        break;
                    case ResizeDirection.Right:
                        this.Width += dx;
                        this.Offset(0, 0);
                        break;
                    case ResizeDirection.TopRight:
                        this.Width += dx;
                        this.Height -= dy;
                        this.Offset(0, dy);
                        break;
                    case ResizeDirection.BottomRight:
                        this.Width += dx;
                        this.Height += dy;
                        this.Offset(0, 0);
                        break;
                    case ResizeDirection.TopLeft:
                        this.Width -= dx;
                        this.Height -= dy;
                        this.Offset(dx, dy);
                        break;
                    case ResizeDirection.BottomLeft:
                        this.Width -= dx;
                        this.Height += dy;
                        this.Offset(dx, 0);
                        break;
                    case ResizeDirection.None:
                    default:
                        break;
                }
            }
            catch { }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }   
     
}
