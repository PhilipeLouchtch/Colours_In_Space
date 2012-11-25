using System;
using System.Drawing.Imaging;
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
using System.Windows.Shapes;

namespace ColoursInSpace
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	/// 


	public partial class InitConfigWindow : Window
	{

		private ResourceDictionary previewImages;

		private RuntimeSettings settings;

		public InitConfigWindow()
		{
			previewImages = new ResourceDictionary();
			previewImages.Source = new Uri("/Resources/PreviewImagesDictionary.xaml", UriKind.Relative);
			settings = new RuntimeSettings();
			InitializeComponent();
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ushort value = (ushort)((Slider)sender).Value;

			bool zoom = (bool)this.Zoom.IsChecked; 

			settings.amntTargetBoxes = (ushort)value;
			ChangePreviewImage(value, zoom);
		}

		private void ChangePreviewImage(int numBoxes, bool zoom)
		{
			string zoomMode = zoom ? "Zoom" : "NoZoom";
			this.PreviewImagebox.Source = (previewImages[numBoxes.ToString() + zoomMode] as System.Windows.Controls.Image).Source;
		}

		private void Zoom_Checked(object sender, RoutedEventArgs e)
		{
			int boxes = (int)this.boxSelector.Value;
			bool zoom = (bool)((CheckBox)sender).IsChecked;
			settings.zoom = zoom;
		
			ChangePreviewImage(boxes, zoom);
		}

		private void WindowLoaded(object sender, RoutedEventArgs e)
		{
			// Set everything up to the way settings are initialized
			string boxes = settings.amntTargetBoxes.ToString();
			string zoom = settings.zoom ? "Zoom" : "NoZoom";
			this.PreviewImagebox.Source = (previewImages[boxes + zoom] as System.Windows.Controls.Image).Source;
			this.boxSelector.Value = settings.amntTargetBoxes;
			this.boxSelector.ValueChanged += Slider_ValueChanged;
		}
	}
}
