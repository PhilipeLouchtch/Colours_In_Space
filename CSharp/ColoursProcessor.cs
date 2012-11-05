using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace ColoursInSpace
{
	class ColoursProcessor
	{
		/// <summary>
		/// Bitmap that will hold a single frame
		/// </summary>
		private WriteableBitmap colourBitmap;

		private byte[] pixels;

		public ushort amntTargetBoxes { set; get; }

		/// <summary>
		/// Initializes the ColoursProcessor class
		/// </summary>
		/// <param name="FrameWidth">FrameWidth from the KinectSensor.ColorStream.FrameWidth property</param>
		/// <param name="FrameHeight">FrameHeight from the KinectSensor.ColorStream.FrameHeight property</param>
		public ColoursProcessor(ushort amntTargetBoxes = 3)
		{
			this.amntTargetBoxes = amntTargetBoxes;
			pixels = new byte[(640 * 480 * 4)];  //Hardcoded size? Not very nice, yes
		}
			

		/// <summary>
		/// Handles a new frame from the Kinect sensor
		/// </summary>
		/// <param name="colourBitmap">A cloned colourBitmap</param>
		public void ProcessColourBitmap(WriteableBitmap colourBitmap)
		{
			colourBitmap.CopyPixels(this.pixels, colourBitmap.PixelWidth * sizeof(int), 0);

			//TODO: Pixels byte data to colours (without killing performance)
		}


		public Colours GetTargetBoxColours()
		{
			Colours colours = new Colours(amntTargetBoxes);

			switch (amntTargetBoxes)
			{
				case 3:
					break;
				case 5:
					break;
				case 7:
					break;
			}

			return colours;
		}
		
	}
}
