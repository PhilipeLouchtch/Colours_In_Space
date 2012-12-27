using System;
using System.Drawing.Imaging;
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
using System.Windows.Shapes;
using System.Net;

namespace ColoursInSpace
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// 


    public partial class InitConfigWindow : Window
    {

        private OSC osc;
        private FrameProcessor coloursProcessor;
        private Kinect kinectLogic;

        //Thread where the KinectLogic start from
        private System.Threading.Thread kinectThread;

        private ResourceDictionary previewImages;
        private RuntimeSettings settings;

        public InitConfigWindow()
        {
			// Not very nice yes, makes the splash stay visible a bit longer
            Thread.Sleep(500);

            previewImages = new ResourceDictionary();
            previewImages.Source = new Uri("/Resources/PreviewImagesDictionary.xaml", UriKind.Relative);
            settings = RuntimeSettings.Instance;
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ushort value = (ushort)((Slider)sender).Value;
			settings.amntTargetBoxes = (ushort)value;

            bool zoom = (bool)this.Zoom.IsChecked;            

            ChangePreviewImage(value, zoom);
        }

        private void ProcessSettingsChanges(object sender)
        {           
            this.Dispatcher.Invoke((Action)(() =>
            {
                settings = (RuntimeSettings) sender;
                this.Targets.Value  = this.settings.amntTargetBoxes;
                this.Zoom.IsChecked = this.settings.zoom;
            }));
        }

        public void ChangePreviewImage(int numBoxes, bool zoom)
        {
            string zoomMode = zoom ? "Zoom" : "NoZoom";
            this.PreviewImagebox.Source = (previewImages[numBoxes.ToString() + zoomMode] as System.Windows.Controls.Image).Source;
        }

        private void Zoom_Checked(object sender, RoutedEventArgs e)
        {
            int boxes = (int)this.Targets.Value;
            bool zoom = (bool)((CheckBox)sender).IsChecked;
            settings.zoom = zoom;

            ChangePreviewImage(boxes, zoom);
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            osc = new OSC(IPAddress.Loopback.ToString());
			RuntimeSettings settings = RuntimeSettings.Instance;
            coloursProcessor = new FrameProcessor(this.osc.SendMsg, this.osc.SendBoxes, settings);
            kinectLogic = new Kinect(this.coloursProcessor.ProcessPixelData);
            kinectThread = new Thread(new ThreadStart(this.kinectLogic.ConnectToSensor));


            /* Set interface controls to values stored in RuntimeSettings */

			/* Get the proper image by building its name from the settings */
            string boxes = settings.amntTargetBoxes.ToString();
            string zoom = settings.zoom ? "Zoom" : "NoZoom";
            this.PreviewImagebox.Source = (previewImages[boxes + zoom] as System.Windows.Controls.Image).Source;

			this.VolumeSlider.Value = settings.volume;
            this.Targets.Value = settings.amntTargetBoxes;
            this.Targets.ValueChanged += Slider_ValueChanged;

			this.VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            this.settings.settingsChanged  += this.ProcessSettingsChanges;
        }

		void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Slider slider	= (Slider)sender;
			settings.volume = (ushort)slider.Value;
		}

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            osc.Dispose();
        }

        private void startToggle_Click(object sender, RoutedEventArgs e)
        {
			Button button = (Button)sender;
            //Very hacky...
            if (button.Content != "Kill it")
            {
                //We have lift off
                kinectThread.Start();
				button.Content = "Kill it";
            }
            else
            {
                base.Close();
                this.Close();
            }
        }
    }
}
