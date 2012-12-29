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
using System.Windows.Shapes;

namespace ColoursInSpace
{
#if DEBUG5
	/// <summary>
	/// Interaction logic for TargetBoxPreview.xaml
	/// </summary>
	public partial class TargetBoxPreview : Window
	{
		public TargetBoxPreview()
		{
			InitializeComponent();
		}

		public void DoThaThang(Byte[] arr, int i, int dimension)
		{
			this.Dispatcher.Invoke((Action)(() =>
			{
				i = 4 - i;
				if (i == 0)
					bitmaps.box1.WritePixels(
						new Int32Rect(0, 0, dimension, dimension),
						arr,
						bitmaps.box1.PixelWidth * sizeof(int),
						0);
				if (i == 1)
					bitmaps.box2.WritePixels(
						new Int32Rect(0, 0, dimension, dimension),
						arr,
						bitmaps.box1.PixelWidth * sizeof(int),
						0);
				if (i == 2)
					bitmaps.box3.WritePixels(
						new Int32Rect(0, 0, dimension, dimension),
						arr,
						bitmaps.box1.PixelWidth * sizeof(int),
						0);
				if (i == 3)
					bitmaps.box4.WritePixels(
						new Int32Rect(0, 0, dimension, dimension),
						arr,
						bitmaps.box1.PixelWidth * sizeof(int),
						0);
				if (i == 4)
					bitmaps.box5.WritePixels(
						new Int32Rect(0, 0, dimension, dimension),
						arr,
						bitmaps.box1.PixelWidth * sizeof(int),
						0);
			}));
		}
	}
#endif
}
