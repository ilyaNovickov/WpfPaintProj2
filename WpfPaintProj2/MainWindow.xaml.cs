﻿using System;
using System.Collections.Generic;
using System.Linq;
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
using WpfPaintProj2.DrawingClasses;

namespace WpfPaintProj2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Layer layer = new Layer();
            layer.Width = 500;
            layer.Height = 500;
            layer.Fill = Brushes.White;
            drawingField.AddLayer(layer);
            drawingField.SelectedLayer = drawingField.Layers.Last();
        }

        private void drawingField_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            drawingField.RemoveLayer();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            drawingField.AddLayer();
        }
    }
}
