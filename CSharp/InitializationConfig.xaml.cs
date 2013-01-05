using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Net;

namespace ColoursInSpace
{
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
			// Show the splash just a wee bit longer
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
                settings = sender as RuntimeSettings;
                this.Targets.Value  = this.settings.amntTargetBoxes;
                this.Zoom.IsChecked = this.settings.zoom;
				this.VolumeSlider.Value = (double) this.settings.volume;
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

			this.Algorithm.SelectedIndex = (int)settings.algorithm;
			this.SynthType.SelectedIndex = (int)settings.synthType;
        }

		void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Slider slider	= (Slider)sender;
			settings.volume = (short)slider.Value;
		}

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            osc.Dispose();
        }

        private void startToggle_Click(object sender, RoutedEventArgs e)
        {
			Button button = (Button)sender;
            //Very hacky...
            if (button.Content != "Terminate Application")
            {
                //We have lift off
                kinectThread.Start();
				button.Content = "Terminate Application";
            }
            else
            {
                base.Close();
                this.Close();
            }
        }

		private void Algorithm_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			ColourAveragingAlgorithms algorithm = (ColourAveragingAlgorithms)comboBox.SelectedIndex;
			settings.algorithm = algorithm;
		}

		private void SynthType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			SCSynthType synthType = (SCSynthType)comboBox.SelectedIndex;
			settings.synthType = synthType;
		}

		private void voiceHelp_Click(object sender, RoutedEventArgs e)
		{
			VoiceCommandsHelp voiceHelpWindow = new VoiceCommandsHelp();
			voiceHelpWindow.Show();
		}
    }
}
