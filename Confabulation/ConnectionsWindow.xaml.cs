using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Confabulation
{
    /// <summary>
    /// Interaction logic for ConnectionsWindow.xaml
    /// </summary>
    public partial class ConnectionsWindow : Window
    {
        public ConnectionsWindow()
        {
            InitializeComponent();

            App app = (App)App.Current;
            connectionListBox.DataContext = app.Connections;
        }
    }
}
