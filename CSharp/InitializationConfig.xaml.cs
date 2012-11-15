﻿using System;
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


	public partial class Window1 : Window
	{

		private ResourceDictionary previewImages;

		public Window1()
		{
			previewImages = new ResourceDictionary();
			previewImages.Source = new Uri("/Resources/PreviewImagesDictionary.xaml", UriKind.Relative);
			InitializeComponent();
			this.PreviewImagebox.Source = (previewImages["3NoZoom"] as System.Windows.Controls.Image).Source;
		}

		//TODO: Events onZoom and onSliderChange to change the preview picture!
	}
}
