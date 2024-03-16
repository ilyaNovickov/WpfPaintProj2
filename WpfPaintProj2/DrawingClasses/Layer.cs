using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfPaintProj2.DrawingClasses
{
    public class Layer : PositionObject
    {
        private readonly ObservableCollection<Figure> figures = new ObservableCollection<Figure>();

        private Figure selectedFigure = null;

        private Brush fill = Brushes.Red;
        private string name;
        private bool isVisible = true;



        public Figure SelectedFigure
        {
            get => selectedFigure;
            set
            {
                selectedFigure = value;
                SelectedFigureChanged?.Invoke(this, new SelectedFigureChangedEventArgs(value));
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Figure> Figures => figures;

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

        public event EventHandler<FigureAddedEventArgs> FigureAdded;

        public event EventHandler<FigureRemovedEventArgs> FigureRemoved;

        public event EventHandler<SelectedFigureChangedEventArgs> SelectedFigureChanged;

        public void AddFigure(Figure figure)
        {
            figures.Add(figure);
            FigureAdded?.Invoke(this, new FigureAddedEventArgs(figure));
        }

        public void RemoveFigure(Figure figure)
        {
            int index = figures.IndexOf(figure);
            figures.Remove(figure);
            FigureRemoved?.Invoke(this, new FigureRemovedEventArgs(figure, index));
        }

        public void RemoveAt(int index)
        {
            Figure figure = figures[index];

            figures.RemoveAt(index);

            FigureRemoved?.Invoke(this, new FigureRemovedEventArgs(figure, index));
        }
    }
}
