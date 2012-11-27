using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Demo2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IList<MenuItemViewModel> m_MenuItems = new ObservableCollection<MenuItemViewModel>(); 

        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < 6; i++)
            {
                m_MenuItems.Add(new MenuItemViewModel() { Row = i });
            }

            m_MenuItems[0].Image = "Images\\Applications.png";
            m_MenuItems[1].Image = "Images\\browser.png";
            m_MenuItems[2].Image = "Images\\cal.png";
            m_MenuItems[3].Image = "Images\\calc.png";
            m_MenuItems[4].Image = "Images\\clock.png";
            m_MenuItems[5].Image = "Images\\Downloads.png";
            m_MenuItems[0].Header = "Applications";
            m_MenuItems[1].Header = "Browser";
            m_MenuItems[2].Header = "Calendar";
            m_MenuItems[3].Header = "Calculator";
            m_MenuItems[4].Header = "Clock";
            m_MenuItems[5].Header = "Downloads";

            DataContext = this;
        }

        public IList<MenuItemViewModel> Items
        {
            get { return m_MenuItems; }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
                MenuListBox.SelectedItem = null;
                e.Handled = true;
            }
        }
    }

    public class MenuItemViewModel
    {
        public int Row { get; set; }
        public string Header { get; set; }
        public string Image { get; set; }
    }
}
