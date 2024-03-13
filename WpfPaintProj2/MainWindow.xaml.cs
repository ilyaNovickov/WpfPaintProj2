using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfPaintProj2.DrawingClasses;

namespace WpfPaintProj2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StandartShapes shapeToAdd = StandartShapes.Rectangele;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            Layer layer = new Layer();
            layer.Width = 500;
            layer.Height = 500;
            layer.Fill = Brushes.White;
            drawingField.AddLayer(layer);
            drawingField.SelectedLayer = drawingField.Layers.Last();
        }

        private void removeLayerButton_Click(object sender, RoutedEventArgs e)
        {
            drawingField.RemoveLayer();
        }

        private void addLayerButton_Click(object sender, RoutedEventArgs e)
        {
            drawingField.AddLayer();
        }

        private void addShapeButton_Click(object sender, RoutedEventArgs e)
        {
            drawingField.DrawingMode = DrawingMode.Adding;

            if (sender == rectButton)
                shapeToAdd = StandartShapes.Rectangele;
            else if (sender == ellipseButton)
                shapeToAdd = StandartShapes.Ellipse;
            else if (sender == triangleButton)
                shapeToAdd = StandartShapes.Triangle;
            else if (sender == rhombeButton)
                shapeToAdd = StandartShapes.Rhomb;
        }

        private void drawingField_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (drawingField.DrawingMode != DrawingMode.Adding || drawingField.SelectedLayer == null)
                return;

            Figure figure = new Figure()
            {
                Fill = Brushes.White,
                Fore = Brushes.Black,
                Size = new Size(50d, 50d),
                Location = e.GetPosition(drawingField.LinkedCanvas),
                Type = shapeToAdd
            };

            drawingField.AddFigure(figure);

            drawingField.SelectedLayer.SelectedFigure = figure;

            drawingField.DrawingMode = DrawingMode.Selecting;
        }

        private void removeFigureButton_Click(object sender, RoutedEventArgs e)
        {
            drawingField.RemoveFigure(drawingField.SelectedLayer.SelectedFigure);
        }

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void redoButton_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
