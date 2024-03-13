using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class DrawingField : UserControl, INotifyPropertyChanged
    {
        private Layer selectedLayer = null;

        private ObservableCollection<Layer> layers = new ObservableCollection<Layer>();


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

        public void AddLayer()
        {
            Layer layer = new Layer();
            layer.Width = 500;
            layer.Height = 500;
            layer.Fill = Brushes.White;
            layer.Name = "Layer #" + layers.Count;
            layer.SizeChanged += Layer_SizeChanged;

            AddLayer(layer);
        }

        public void AddLayer(Layer layer)
        {
            layer.SizeChanged += Layer_SizeChanged;
            UpdateSize(layer);
            layers.Add(layer);
            AddCanvas(layer);
        }

        public void RemoveLayer(Layer layer)
        {
            layers.Remove(layer);
            
            UpdateSize();
        }

        public void RemoveLayerAt(int index)
        {
            layers.RemoveAt(index);
            //mainCanvas.Children.RemoveAt(index);
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
        }

        private void RemoveCanvas()
        {

        }

        private Canvas GetCanvas(Layer layer)
        {
            Canvas canvas = new Canvas();

            canvas.Background = layer.Fill;
            canvas.Width = layer.Width;
            canvas.Height = layer.Height;
            //Canvas(canvas.
            return canvas;
        }
    }
}
