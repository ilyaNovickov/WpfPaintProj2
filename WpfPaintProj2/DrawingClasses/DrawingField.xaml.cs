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
using WpfPaintProj2.Helpers;

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

        public void AddLayer()
        {
            Layer layer = new Layer();
            layer.Width = 500;
            layer.Height = 500;
            layer.Fill = Brushes.Transparent;
            layer.SizeChanged += Layer_SizeChanged;

            AddLayer(layer);
        }

        public void AddLayer(Layer layer)
        {
            layer.SizeChanged += Layer_SizeChanged;
            layer.VisibleChanged += Layer_VisibleChanged;
            layer.Name = "Layer #" + layers.Count;
            UpdateSize(layer);
            layers.Add(layer);
            AddCanvas(layer);
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

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (LinkedCanvas == null)
                return;

            Rectangle rect = new Rectangle();
            rect.Width = 50;
            rect.Height = 50;
            rect.SetCanvasCenterPoint(e.GetPosition(LinkedCanvas));

            rect.Fill = Brushes.Red;

            Point p = rect.GetCanvasPoint();

            LinkedCanvas.Children.Add(rect);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
           
        }
    }
}
