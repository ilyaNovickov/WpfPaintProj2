using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
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
        private Layer selectedLayer = null;

        private readonly ObservableCollection<Layer> layers = new ObservableCollection<Layer>();

        private readonly List<Canvas> canvases = new List<Canvas>();

        private readonly Dictionary<Layer, List<Shape>> shapes = new Dictionary<Layer, List<Shape>>();

        private DrawingMode mode = DrawingMode.None;

        private readonly ControlPoints controlPoints = new ControlPoints();

        private ResizeDirection resizeDirection = ResizeDirection.None;

        private Point oldPoint = new Point(0d, 0d);

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
                Layer oldLayer = selectedLayer;
                selectedLayer = value;
                OnSelectedLayerChanged(new SelectedLayerChangedEventArgs(value, oldLayer));
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

        public event EventHandler<SelectedLayerChangedEventArgs> SelectedLayerChanged;

        protected virtual void OnSelectedLayerChanged(SelectedLayerChangedEventArgs e)
        {
            if (e.OldLayer != null)
            {
                e.OldLayer.SelectedFigure = null;
                canvases[Layers.IndexOf(e.OldLayer)].IsEnabled = false;
                //Canvas.SetZIndex(canvases[Layers.IndexOf(e.OldLayer)], 0);
            }

            canvases[Layers.IndexOf(e.Layer)].IsEnabled = true;
            //Canvas.SetZIndex(canvases[Layers.IndexOf(e.Layer)], 1);

            SelectedLayerChanged?.Invoke(this, e);
        }
        #endregion
        #region AddRemoveLayerCanvas
        public event EventHandler<LayerAddedEventArgs> LayerAdded;

        public event EventHandler<LayerRemovedEventArgs> LayerRemoved;

        public void AddLayer()
        {
            Layer layer = new Layer
            {
                Width = 500,
                Height = 500,
                Fill = Brushes.Transparent
            };

            AddLayer(layer);
        }

        public void AddLayer(Layer layer)
        {
            if (layer == null)
                return;

            layers.Add(layer);

            layer.SelectedFigureChanged += Layer_SelectedFigureChanged;

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
            Canvas canvas = new Canvas
            {
                Background = layer.Fill,
                Width = layer.Width,
                Height = layer.Height
            };
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

            SelectedLayer = layer;

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

            undoManager.RegistrAction(new AddDoRe(SelectedLayer, new AddRemoveDoArgs(figure), this));

            AddFigureToSelectedLayer_Internal(figure, SelectedLayer);
        }

        internal void AddFigureToSelectedLayer_Internal(Figure figure, Layer layer)
        {
            if (layer == null || figure == null)
                return;

            figure.PropertyChanged += Figure_PropertyChanged;
            figure.Moved += Figure_Moved;
            figure.SizeChanged += Figure_SizeChanged;

            layer.AddFigure(figure);
            //SelectedLayer.AddFigure(figure);

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

            undoManager.RegistrAction(new RemoveDoRe(SelectedLayer, new AddRemoveDoArgs(figure), this));

            RemoveFigureInSelectedLayer_Internal(figure, SelectedLayer);
        }

        internal void RemoveFigureInSelectedLayer_Internal(Figure figure, Layer layer)
        {
            if (layer == null || figure == null)
                return;

            layer.RemoveFigure(figure);
        }

        private void RemoveShape(object sender, FigureRemovedEventArgs e)
        {
            Layer layer = (Layer)sender;

            Shape shape = shapes[layer][e.Index];

            shapes[layer].RemoveAt(e.Index);

            //LinkedCanvas.Children.Remove(shape);
            canvases[layers.IndexOf(layer)].Children.Remove(shape);
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
            shape.SetCanvasPoint(figure.X, figure.Y);

            return shape;
        }
        #endregion

        #region UpdateSize
        private void Layer_SizeChanged(object sender, EventArgs e)
        {
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
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

            Point pt = e.GetPosition(LinkedCanvas);

            List<DependencyObject> hitResultsList = new List<DependencyObject>();

            HitTestFilterBehavior MyHitTestFilter(DependencyObject o)
            {
                // Test for the object value you want to filter.
                if (o.GetType() == typeof(Canvas))
                {
                    // Visual object and descendants are NOT part of hit test results enumeration.
                    return HitTestFilterBehavior.ContinueSkipSelf;
                }
                else
                {
                    // Visual object is part of hit test results enumeration.
                    return HitTestFilterBehavior.Continue;
                }
            }
            HitTestResultBehavior MyHitTestResult(HitTestResult result1)
            {
                // Add the hit test result to the list that will be processed after the enumeration.
                hitResultsList.Add(result1.VisualHit);

                // Set the behavior to return visuals at all z-order levels.
                return HitTestResultBehavior.Continue;
            }

            VisualTreeHelper.HitTest(LinkedCanvas,
                      new HitTestFilterCallback(MyHitTestFilter),
                      new HitTestResultCallback(MyHitTestResult),
                      new PointHitTestParameters(pt));


            foreach (Shape shape1 in hitResultsList.Cast<Shape>())
            {
                if (shape1 == controlPoints.DecoRectange)
                    continue;
                else if (shape1 == controlPoints.MoveRectange)
                {
                    this.DrawingMode = DrawingMode.Dragging;
                    this.Cursor = Cursors.SizeAll;
                    oldShapePosition = SelectedLayer.SelectedFigure.Location;
                    oldPoint = pt;
                    break;
                }
                else if (controlPoints.ResizeRecrangeles.Contains(shape1))
                {
                    this.DrawingMode = DrawingMode.Resizing;
                    OnResizePointClicked(shape1);
                    oldShapePosition = SelectedLayer.SelectedFigure.Location;
                    oldSize = SelectedLayer.SelectedFigure.Size;
                    oldPoint = pt;
                    break;
                }
                else
                {
                    SelectedLayer.SelectedFigure = SelectedLayer.Figures[shapes[SelectedLayer].IndexOf(shape1)];
                    break;
                }
            }  
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (mode == DrawingMode.Dragging)
                undoManager.RegistrAction(new MoveDoRe(SelectedLayer, new MoveDoArgs(SelectedLayer.SelectedFigure, oldShapePosition,
                        SelectedLayer.SelectedFigure.Location)));
            else if (mode == DrawingMode.Resizing)
                undoManager.RegistrAction(new ResizeDoRe(SelectedLayer, new ResizeDoArgs(SelectedLayer.SelectedFigure, oldShapePosition,
                    SelectedLayer.SelectedFigure.Location, oldSize, 
                    new Size(SelectedLayer.SelectedFigure.Width, SelectedLayer.SelectedFigure.Height))));

            DrawingMode = DrawingMode.Selecting;

            this.Cursor = Cursors.Arrow;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Point pos = e.GetPosition(LinkedCanvas);

            switch (mode)
            {

                case DrawingMode.None:
                case DrawingMode.Adding:
                case DrawingMode.Selecting:
                default:
                    break;
                case DrawingMode.Dragging:
                    

                    double dx = pos.X - oldPoint.X;
                    double dy = pos.Y - oldPoint.Y;

                    SelectedLayer.SelectedFigure.Offset(dx, dy);

                    foreach (Shape shapes in controlPoints.GetControlPoints())
                    {
                        shapes.Offset(dx, dy);
                    }

                    //ShapeMoved?.Invoke(this, new ShapeMovedArgs(SelectedShape, SelectedShape.GetCanvasPoint(), oldPoint));

                    oldPoint = pos;
                    break;
                case DrawingMode.Resizing:

                    ResizeFigure(SelectedLayer.SelectedFigure, pos.X - oldPoint.X, pos.Y - oldPoint.Y);

                    //ShapeMoved?.Invoke(this, new ShapeMovedArgs(SelectedShape, SelectedShape.GetCanvasPoint(), oldPoint));

                    oldPoint = pos;
                    break;
            }
        }

        public void ResetSelectedFigure()
        {
            if (SelectedLayer == null)
                return;

            SelectedLayer.SelectedFigure = null;
        }

        private void Figure_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Figure figure = sender as Figure;

            Layer layer = GetLayerByFigure(figure);

            //Shape shape = shapes[SelectedLayer][SelectedLayer.Figures.IndexOf(figure)];
            Shape shape = shapes[layer][layer.Figures.IndexOf(figure)];

            shape.SetCanvasPoint(e.NewLocation);

            shape.Width = e.NewSize.Width;

            shape.Height = e.NewSize.Height;
        }

        private void Figure_Moved(object sender, MovedEventArgs e)
        {
            Figure figure = sender as Figure;

            Layer layer = GetLayerByFigure(figure);

            //Shape shape = shapes[SelectedLayer][SelectedLayer.Figures.IndexOf(figure)];
            Shape shape = shapes[layer][layer.Figures.IndexOf(figure)];

            shape.SetCanvasPoint(e.NewLocation);
        }

        private void Figure_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Figure figure = (Figure)sender;

            Layer layer = null;

            foreach (Layer layer1 in layers)
            {
                if (!layer1.Figures.Contains(figure))
                    continue;
                layer = layer1;
                break;
            }

            //Shape shape = shapes[SelectedLayer][SelectedLayer.Figures.IndexOf(figure)];
            Shape shape = shapes[layer][layer.Figures.IndexOf(figure)];

            switch (e.PropertyName)
            {
                //case nameof(figure.Height):
                //    shape.Height = figure.Height;
                //    break;
                //case nameof(figure.Width):
                //    shape.Width = figure.Width;
                //    break;
                //case nameof(figure.Y):
                //case nameof(figure.X):
                //case nameof(figure.Location):
                //    shape.SetCanvasPoint(figure.X, figure.Y);
                //    break;
                case nameof(figure.Fill):
                    shape.Fill = figure.Fill;
                    break;
                case nameof(figure.Fore):
                    shape.Stroke = figure.Fore;
                    break;
                default:
                    return;
            }
            
        }

        private void Layer_SelectedFigureChanged(object sender, SelectedFigureChangedEventArgs e)
        {
            Layer layer = (Layer)sender;

            Canvas canvas = canvases[this.Layers.IndexOf(layer)];

            if (controlPoints.MoveRectange != null)
            {
                canvas.Children.Remove(controlPoints.MoveRectange);
                canvas.Children.Remove(controlPoints.DecoRectange);
                foreach (Rectangle rect in controlPoints.ResizeRecrangeles)
                {
                    canvas.Children.Remove(rect);
                }
                controlPoints.ResizeRecrangeles.Clear();
            }

            if (e.SelectedFigure == null)
                return;

            Shape shape = shapes[layer][layer.Figures.IndexOf(e.SelectedFigure)];

            //Точка для декора
            Rectangle decoRect = new Rectangle()
            {
                Width = shape.Width,
                Height = shape.Height,
                Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                Fill = Brushes.Transparent,
                StrokeDashArray = new DoubleCollection() { 4, 4 }
            };
            decoRect.SetCanvasPoint(shape.GetCanvasPoint());
            controlPoints.DecoRectange = decoRect;
            canvas.Children.Add(decoRect);
            //---
            foreach (Rectangle shape1 in shape.GetShapeControlPoints().Cast<Rectangle>())
            {
                
                controlPoints.ResizeRecrangeles.Add(shape1);
                canvas.Children.Add(shape1);
            }
            //---
            Rectangle moveRect = new Rectangle
            {
                Width = 10,
                Height = 10,
                Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0)),
                Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0))
            };
            Canvas.SetLeft(moveRect, Canvas.GetLeft(shape) + shape.Width / 2d - moveRect.Width / 2d);
            Canvas.SetTop(moveRect, Canvas.GetTop(shape) + shape.Height / 2d - moveRect.Height / 2d);
            controlPoints.MoveRectange = moveRect;
            canvas.Children.Add(moveRect);
        }

        private void OnResizePointClicked(Shape shape)
        {
            switch (shape.Name)
            {
                case "TOP":
                    this.Cursor = Cursors.ScrollN;
                    resizeDirection = ResizeDirection.Top;
                    break;
                case "BOTTOM":
                    this.Cursor = Cursors.ScrollS;
                    resizeDirection = ResizeDirection.Bottom;
                    break;
                case "LEFT":
                    this.Cursor = Cursors.ScrollW;
                    resizeDirection = ResizeDirection.Left;
                    break;
                case "RIGHT":
                    this.Cursor = Cursors.ScrollE;
                    resizeDirection = ResizeDirection.Right;
                    break;
                case "TOPLEFT":
                    this.Cursor = Cursors.ScrollNW;
                    resizeDirection = ResizeDirection.TopLeft;
                    break;
                case "TOPRIGHT":
                    this.Cursor = Cursors.ScrollNE;
                    resizeDirection = ResizeDirection.TopRight;
                    break;
                case "BOTTOMLEFT":
                    this.Cursor = Cursors.ScrollSW;
                    resizeDirection = ResizeDirection.BottomLeft;
                    break;
                case "BOTTOMRIGHT":
                    this.Cursor = Cursors.ScrollSE;
                    resizeDirection = ResizeDirection.BottomRight;
                    break;
                default:
                    throw new Exception("Неизвесная точка");

            }
        }

        private void ResizeFigure(Figure figure, double dx, double dy)
        {
            try
            {
                double newWidth = figure.Width;
                double newHeight = figure.Height;
                Point newLocation = figure.Location;

                switch (this.resizeDirection)
                {
                    case ResizeDirection.Top:
                        newHeight -= dy;
                        newLocation.Offset(0, dy);
                        break;
                    case ResizeDirection.Bottom:
                        newHeight += dy;
                        newLocation.Offset(0, 0);
                        break;
                    case ResizeDirection.Left:
                        newWidth -= dx;
                        newLocation.Offset(dx, 0);
                        break;
                    case ResizeDirection.Right:
                        newWidth += dx;
                        newLocation.Offset(0, 0);
                        break;
                    case ResizeDirection.TopRight:
                        newWidth += dx;
                        newHeight -= dy;
                        newLocation.Offset(0, dy);
                        break;
                    case ResizeDirection.BottomRight:
                        newWidth += dx;
                        newHeight += dy;
                        newLocation.Offset(0, 0);
                        break;
                    case ResizeDirection.TopLeft:
                        newWidth -= dx;
                        newHeight -= dy;
                        newLocation.Offset(dx, dy);
                        break;
                    case ResizeDirection.BottomLeft:
                        newWidth -= dx;
                        newHeight += dy;
                        newLocation.Offset(dx, 0);
                        break;
                    case ResizeDirection.None:
                    default:
                        return; ;
                }

                if (newWidth <= 0 || newHeight <= 0)
                    return;

                figure.Location = newLocation;
                figure.Width = newWidth;
                figure.Height = newHeight;

                Layer layer = GetLayerByFigure(figure);

                //Shape shape = shapes[SelectedLayer][SelectedLayer.Figures.IndexOf(figure)];
                Shape shape = shapes[layer][layer.Figures.IndexOf(figure)];

                int index = 0;
                foreach (KeyValuePair<string, Point> pair in shape.GetPointsofBorderControlPoints())
                {
                    controlPoints.ResizeRecrangeles[index].SetCanvasCenterPoint(pair.Value.X, pair.Value.Y);
                    index++;
                }
                controlPoints.MoveRectange.SetCanvasCenterPoint(Canvas.GetLeft(shape) + shape.Width / 2d,
                    Canvas.GetTop(shape) + shape.Height / 2d);
                controlPoints.DecoRectange.SetCanvasPoint(shape.GetCanvasPoint().X, shape.GetCanvasPoint().Y);
                controlPoints.DecoRectange.Width = shape.Width;
                controlPoints.DecoRectange.Height = shape.Height;
            }
            catch { }
        }

        private Layer GetLayerByFigure(Figure figure)
        {
            Layer layer = null;

            foreach (Layer layer1 in layers)
            {
                if (!layer1.Figures.Contains(figure))
                    continue;
                layer = layer1;
                break;
            }

            return layer;
        }

        private class ControlPoints
        {
            public Canvas Canvas { get; set; }

            private List<Rectangle> resizeRectangles = new List<Rectangle>();

            public Rectangle MoveRectange { get; set; }
            public Rectangle DecoRectange { get; set; }
            public List<Rectangle> ResizeRecrangeles
            {
                get => resizeRectangles;
                set => resizeRectangles = value;
            }

            public List<Shape> GetControlPoints()
            {
                List<Shape> list = new List<Shape>() { MoveRectange, DecoRectange };
                list.AddRange(ResizeRecrangeles);
                return list;
            }
        }

        #region UndoRedo
        private readonly UndoRedoManager undoManager;

        private Point oldShapePosition = new Point(0d, 0d);

        private Size oldSize = new Size(0d, 0d);

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
