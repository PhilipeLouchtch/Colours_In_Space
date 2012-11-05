using System;
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
using System.Net;

namespace ColoursInSpace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OSC osc;
        public MainWindow()
        {
            osc = new OSC(IPAddress.Loopback.ToString(), 57120);
            InitializeComponent();
        }

        private void SendMsg_Click(object sender, RoutedEventArgs e)
        {
            osc.SendMsg();
        }
    }
}
