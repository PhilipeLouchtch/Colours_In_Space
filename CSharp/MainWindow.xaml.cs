using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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
		private FrameProcessor coloursProcessor;
		private Kinect kinectLogic;

		//Thread where the KinectLogic start from
		private System.Threading.Thread kinectThread;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //osc = new OSC(IPAddress.Loopback.ToString());
            osc = new OSC("132.229.130.152");
			RuntimeSettings settings = new RuntimeSettings();
            coloursProcessor = new FrameProcessor(this.osc.SendMsg, this.osc.SendColourBoxes5, settings);
			kinectLogic = new Kinect(this.coloursProcessor.ProcessPixelData);
            kinectThread = new Thread(new ThreadStart(this.kinectLogic.ConnectToSensor));
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            osc.Dispose();
        }

        private void zeButton_Click(object sender, RoutedEventArgs e)
        {
			//Very hacky...
			if (((Button)sender).Content != "Kill it")
			{
				//We have lift off
                //InitConfigWindow configWindow = new InitConfigWindow();
				//configWindow.Show(); //Just for the show...
				kinectThread.Start();
				((Button)sender).Content = "Kill it";
			}
			else
			{
				base.Close();
				this.Close();
			}

        }

    }
}
