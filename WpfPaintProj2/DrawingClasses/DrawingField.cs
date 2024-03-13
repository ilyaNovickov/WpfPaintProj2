using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace WpfPaintProj2.DrawingClasses
{
    class DrawingField : UserControl, INotifyPropertyChanged
    {
        private Layer selectedLayer = null;

        private ObservableCollection<Layer> layers = new ObservableCollection<Layer>();

        public Canvas mainCanvas = null;

        public DrawingField()
        {
            mainCanvas = new Canvas();
            mainCanvas.Background = Brushes.Gray;
            mainCanvas.Width = 500;
            mainCanvas.Height = 500;
            mainCanvas.ClipToBounds = true;
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
            layer.Width = 100;
            layer.Height = 100;
            layer.SizeChanged += Layer_SizeChanged;
            //mainCanvas.Children.Add(layer);//drawingCanvas.Last());
            UpdateSize(layer);
            //drawingCanvas.Add(new LayerItem("test" + drawingCanvas.Count, layer));
            layers.Add(layer);
        }

        public void AddLayer(Layer layer)
        {
            //this.canvas.Children.Add(canvas);
            layer.SizeChanged += Layer_SizeChanged;
            UpdateSize(layer);
            //drawingCanvas.Add(new LayerItem("test" + drawingCanvas.Count, canvas));
            layers.Add(layer);
        }

        public void RemoveLayer(Layer layer)
        {
            layers.Remove(layer);
            //mainCanvas.Children.Remove(layer);
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
    }
}
