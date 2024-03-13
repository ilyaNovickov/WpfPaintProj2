using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
using WpfPaintProj2.Helpers;
using WpfPaintProj2.OwnShapes;

namespace WpfPaintProj2.DrawingClasses
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class DrawingField : UserControl, INotifyPropertyChanged
    {
        private Layer selectedLayer = null;

        private ObservableCollection<Layer> layers = new ObservableCollection<Layer>();

        private List<Canvas> canvases = new List<Canvas>();

        private Dictionary<Layer, List<Shape>> shapes = new Dictionary<Layer, List<Shape>>();


        public DrawingField()
        {
            InitializeComponent();
        }

        public ObservableCollection<Layer> Layers => layers;

        public Layer SelectedLayer
        {
            get => selectedLayer;
            set
            {
                selectedLayer = value;
                OnPropertyChanged();
            }
        }

        public Canvas LinkedCanvas
        {
            get
            {
                if (selectedLayer == null)
                    return null;

                return canvases[layers.IndexOf(selectedLayer)];
            }
        }

        public void AddFigure(Figure figure)
        {
            if (SelectedLayer == null)
                return;

            SelectedLayer.AddFigure(figure);
        }

        public void RemoveFigure(Figure figure)
        {
            if (SelectedLayer == null)
                return;

            RemoveShape(figure);
            SelectedLayer.RemoveFigure(figure);
        }

        private void RemoveShape(Figure figure)
        {
            if (SelectedLayer == null)
                return;

            int index = SelectedLayer.Figures.IndexOf(figure);

            Shape shape = shapes[SelectedLayer][index];

            shapes[SelectedLayer].RemoveAt(index);

            LinkedCanvas.Children.Remove(shape);
        }

        public void AddLayer()
        {
            Layer layer = new Layer();
            layer.Width = 500;
            layer.Height = 500;
            layer.Fill = Brushes.Transparent;

            AddLayer(layer);
        }

        public void AddLayer(Layer layer)
        {
            layer.SizeChanged += Layer_SizeChanged;
            layer.VisibleChanged += Layer_VisibleChanged;
            layer.FigureAdded += Layer_FigureAdded;
            layer.FigureRemoved += Layer_FigureRemoved;
            shapes.Add(layer, new List<Shape>());
            layer.Name = "Layer #" + layers.Count;
            UpdateSize(layer);
            layers.Add(layer);
            AddCanvas(layer);
        }

        private void Layer_FigureRemoved(object sender, EventArgs e)
        {
            //Layer layer = (Layer)sender;

            //List<Shape> shapesList = shapes[layer];

            //shapesList.

            //Shape shape = GetShape(layer.Figures.Last());

            //shapes[layer].Add(shape);

            //canvases[layers.IndexOf(layer)].Children.Add(shape);
        }

        private void Layer_FigureAdded(object sender, EventArgs e)
        {
            Layer layer = (Layer)sender;

            Shape shape = GetShape(layer.Figures.Last());

            shapes[layer].Add(shape);

            canvases[layers.IndexOf(layer)].Children.Add(shape);
        }

        private void Layer_VisibleChanged(object sender, EventArgs e)
        {
            int index = layers.IndexOf((Layer)sender);

            if (!(0 <= index && index < layers.Count))
                return;

            switch (((Layer)sender).IsVisible)
            {
                case true:
                    canvases[index].Visibility = Visibility.Visible;
                    break;
                case false:
                    canvases[index].Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }

        public void RemoveLayer()
        {
            if (SelectedLayer == null)
                return;

            RemoveLayer(SelectedLayer);
        }

        public void RemoveLayer(Layer layer)
        {
            RemoveCanvas(layer);
            layers.Remove(layer);    

            UpdateSize();
        }

        public void RemoveLayerAt(int index)
        {
            RemoveCanvas(index);
            layers.RemoveAt(index);
            UpdateSize();
        }

        private void Layer_SizeChanged(object sender, EventArgs e)
        {
            Layer layer = (Layer)sender;

            UpdateSize();
        }

        public void UpdateSize(Layer layer)
        {
            if (layer.Width > mainCanvas.Width)
            {
                mainCanvas.Width = layer.Width;
            }
            if (layer.Height > mainCanvas.Height)
            {
                mainCanvas.Height = layer.Height;
            }
        }

        public void UpdateSize()
        {
            Size maxSize = new Size(0, 0);

            foreach (Layer layer in this.layers)
            {
                if (layer.Width > maxSize.Width)
                {
                    maxSize.Width = layer.Width;
                }
                if (layer.Height > maxSize.Height)
                {
                    maxSize.Height = layer.Height;
                }
            }

            this.mainCanvas.Width = maxSize.Width;
            this.mainCanvas.Height = maxSize.Height;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void SetSelectedLayer(int index)
        {
            if (!(0 <= index && index < Layers.Count))
                return;
            SelectedLayer = Layers[index];
        }

        public void SetSelectedLayer(Layer layer)
        {
            if (!layers.Contains(layer))
                return;
            SelectedLayer = layer;
        }

        private void AddCanvas(Layer layer)
        {
            Canvas canvas = GetCanvas(layer);

            mainCanvas.Children.Add(canvas);

            canvases.Add(canvas);
        }

        private void RemoveCanvas(Layer layer)
        {
            int index = layers.IndexOf(layer);
            mainCanvas.Children.Remove(canvases[index]);
            canvases.RemoveAt(index);
        }

        private void RemoveCanvas(int index)
        {
            mainCanvas.Children.Remove(canvases[index]);
            canvases.RemoveAt(index);
        }

        private Canvas GetCanvas(Layer layer)
        {
            Canvas canvas = new Canvas();

            canvas.Background = layer.Fill;
            canvas.Width = layer.Width;
            canvas.Height = layer.Height;
            canvas.SetCanvasPoint(layer.X, layer.Y);
            return canvas;
        }

        private Shape GetShape(Figure figure)
        {
            Shape shape;

            switch (figure.Type)
            {
                case StandartShapes.Ellipse:
                    shape = new Ellipse();
                    break;
                case StandartShapes.Triangle:
                    shape = new Triangle();
                    break;
                case StandartShapes.Rhomb:
                    shape = new Rhomb();
                    break;
                default:
                case StandartShapes.Rectangele:
                    shape = new Rectangle();
                    break;
            }

            shape.Stroke = figure.Fore;
            shape.Fill = figure.Fill;
            shape.Width = figure.Width;
            shape.Height = figure.Height;
            shape.SetCanvasCenterPoint(figure.X, figure.Y);

            return shape;
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);



            Point pt = e.GetPosition(LinkedCanvas);

            // Perform the hit test against a given portion of the visual object tree.
            HitTestResult result = VisualTreeHelper.HitTest(LinkedCanvas, pt);

            if (result != null)
            {
                if (result.VisualHit is Shape)
                {

                }
            }





            if (LinkedCanvas == null)
                return;

            Figure figure = new Figure()
            {
                Fill = Brushes.Red,
                Fore = Brushes.Black,
                Width = 50d,
                Height = 50d,
                Location = e.GetPosition(LinkedCanvas)
            };

            SelectedLayer.AddFigure(figure);

            
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
           
        }
    }
}
