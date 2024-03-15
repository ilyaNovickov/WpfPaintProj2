using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfPaintProj2.Helpers;
using WpfPaintProj2.OwnShapes;
using WpfPaintProj2.UndoRedo;

namespace WpfPaintProj2.DrawingClasses
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class DrawingField : UserControl, INotifyPropertyChanged
    {
        #region foo
        private class FigureShapeLinking
        {            
            public Figure Figure { get; }
            public Shape Shape { get; }
            public int IndexInCanvas { get; set; }

            public FigureShapeLinking(Figure figure, Shape shape, int index)
            {
                Figure = figure;
                Shape = shape;
                IndexInCanvas = index;
            }
        }

        private class LayerCanvasLinking
        {
            public Layer Layer { get; }
            public Canvas Canvas { get; }
            public int IndexInMainCanvas { get; set; }

            public LayerCanvasLinking(Layer layer, Canvas canvas, int index)
            {
                Layer = layer;
                Canvas = canvas;
                IndexInMainCanvas = index;
            }
        }

        private class asd
        {
            public void doo()
            {
                
            }
        }
        #endregion
        private Layer selectedLayer = null;

        private ObservableCollection<Layer> layers = new ObservableCollection<Layer>();

        private List<Canvas> canvases = new List<Canvas>();

        private Dictionary<Layer, List<Shape>> shapes = new Dictionary<Layer, List<Shape>>();

        private DrawingMode mode = DrawingMode.None;

        public DrawingField()
        {
            InitializeComponent();

            undoManager = new UndoRedoManager(null);
        }

        #region Properties
        public DrawingMode DrawingMode
        {
            get => mode;
            set
            {
                mode = value;
                OnPropertyChanged();
            }
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
        #endregion
        #region AddRemoveLayerCanvas
        public event EventHandler<LayerAddedEventArgs> LayerAdded;

        public event EventHandler<LayerRemovedEventArgs> LayerRemoved;

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
            if (layer == null)
                return;

            layers.Add(layer);

            SelectedLayer = layer;

            OnLayerAdded(new LayerAddedEventArgs(layer));
        }

        public void RemoveSelectedLayer()
        {
            if (SelectedLayer == null)
                return;

            RemoveLayer(SelectedLayer);
        }

        public void RemoveLayer(Layer layer)
        {
            int index = layers.IndexOf(layer);
            layers.Remove(layer);
            UpdateSize();

            OnLayerRemoved(new LayerRemovedEventArgs(layer, index));
        }

        public void RemoveLayerAt(int index)
        {
            Layer layer = layers[index];
            layers.RemoveAt(index);
            UpdateSize();

            OnLayerRemoved(new LayerRemovedEventArgs(layer, index));
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

        protected virtual void OnLayerAdded(LayerAddedEventArgs e)
        {
            Layer layer = e.Layer;

            layer.SizeChanged += Layer_SizeChanged;
            layer.VisibleChanged += Layer_VisibleChanged;
            layer.FigureAdded += AddShape;
            layer.FigureRemoved += RemoveShape;
            layer.Name = "Layer #" + layers.Count;

            shapes.Add(layer, new List<Shape>());

            UpdateSize(layer);

            Canvas canvas = GetCanvas(layer);

            mainCanvas.Children.Add(canvas);

            canvases.Add(canvas);

            LayerAdded?.Invoke(this, e);
        }

        protected virtual void OnLayerRemoved(LayerRemovedEventArgs e)
        {
            canvases.RemoveAt(e.Index);

            mainCanvas.Children.RemoveAt(e.Index);

            LayerRemoved?.Invoke(this, e);
        }
        #endregion
        #region AddRemoveShapeFigure
        public void AddFigureToSelectedLayer(Figure figure)
        {
            if (SelectedLayer == null || figure == null)
                return;

            SelectedLayer.AddFigure(figure);
        }

        private void AddShape(object sender, FigureAddedEventArgs e)
        {
            Layer layer = (Layer)sender;

            Shape shape = GetShape(e.AddedFigure);

            shapes[layer].Add(shape);

            canvases[layers.IndexOf(layer)].Children.Add(shape);
        }

        public void RemoveFigureInSelectedLayer(Figure figure)
        {
            if (SelectedLayer == null || figure == null)
                return;

            SelectedLayer.RemoveFigure(figure);
        }

        private void RemoveShape(object sender, FigureRemovedEventArgs e)
        {
            Layer layer = (Layer)sender;

            Shape shape = shapes[layer][e.Index];

            shapes[layer].RemoveAt(e.Index);

            LinkedCanvas.Children.Remove(shape);
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
        #endregion

        #region UpdateSize
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
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
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

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            return;

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

        public void ResetSelectedFigure()
        {
            if (SelectedLayer == null)
                return;

            SelectedLayer.SelectedFigure = null;
        }

        #region UndoRedo
        private UndoRedoManager undoManager;

        public void Undo()
        {
            if (SelectedLayer == null)
                return;

            ResetSelectedFigure();
            undoManager.Undo();
        }

        public void Redo()
        {
            if (SelectedLayer == null)
                return;

            ResetSelectedFigure();
            undoManager.Redo();
        }
        #endregion
    }
}
