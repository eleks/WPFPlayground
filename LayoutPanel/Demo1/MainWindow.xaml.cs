using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demo1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Array docks = Enum.GetValues(typeof(Dock));
            var random = new Random();
            for (int i = 0; i < 10; i++)
            {
                var button = new Button() { Content = "Button" + i.ToString() };
                LayoutPanel.Children.Add(button);
                if ((i + 1) % 2 == 0)
                {
                    Grid.SetColumn(button, 1);
                }
                Canvas.SetLeft(button, random.NextDouble() * LayoutPanel.ActualWidth);
                Canvas.SetTop(button, random.NextDouble() * LayoutPanel.ActualHeight);
                Grid.SetRow(button, i / 2);
                DockPanel.SetDock(button, (Dock)docks.GetValue(random.Next(0, docks.Length)));
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            LayoutPanel.Children.Add(new Button() {Content = "Button"});
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            LayoutPanel.Children.RemoveAt(0);
        }
    }
}
