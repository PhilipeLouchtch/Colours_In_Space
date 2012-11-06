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
		private byte[] pixelBgraData;

		public ushort amntTargetBoxes { set; get; }

		/// <summary>
		/// Initializes the ColoursProcessor class
		/// </summary>
        /// <param name="amntTargetBoxes">Amount of colours to be stored (the amount of targets set)</param>
		public ColoursProcessor(ushort amntTargetBoxes = 3)
		{
			this.amntTargetBoxes = amntTargetBoxes;
            pixelBgraData = new byte[(640 * 480 * 4)];  //Hardcoded size? Not very nice, yes
		}

		/// <summary>
		/// Handles a new frame from the Kinect sensor
		/// </summary>
		/// <param name="colourBitmap">A cloned colourBitmap</param>
		public void ProcessColourBitmap(WriteableBitmap colourBitmap)
		{
            colourBitmap.CopyPixels(this.pixelBgraData, colourBitmap.PixelWidth * sizeof(int), 0);

			//TODO: Pixels byte data to colours (without killing performance)
		}

        /// <summary>
        /// Handles a new frame from the Kinect sensor
        /// </summary>
        /// <param name="colourBitmap">A cloned colourBitmap</param>
        public void ProcessPixelData(byte[] colourPixels)
        {

            Colours colours = new Colours();
            colours.ProcessPixelBgraData(ref colourPixels);
        }


		public TargetColours GetTargetBoxColours()
		{
			TargetColours targetColours = new TargetColours(amntTargetBoxes);

			switch (amntTargetBoxes)
			{
				case 3:
					break;
				case 5:
					break;
				case 7:
					break;
			}

			return targetColours;
		}
		
	}
}
