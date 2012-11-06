using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.Threading;

namespace ColoursInSpace
{
	public delegate void ProcessPixelData(byte[] colourPixels);

	class ColoursProcessor
	{
		private byte[] pixelBgraData;

		public ushort amntTargetBoxes { set; get; }

		private SendOscMsg sendOscMsg;

		/// <summary>
		/// Initializes the ColoursProcessor class
		/// </summary>
        /// <param name="amntTargetBoxes">Amount of colours to be stored (the amount of targets set)</param>
		public ColoursProcessor(SendOscMsg sendOscMsg, ushort amntTargetBoxes = 3)
		{
			this.sendOscMsg = sendOscMsg;
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
		}

        /// <summary>
        /// Handles a new frame from the Kinect sensor
        /// </summary>
        /// <param name="colourBitmap">A cloned colourBitmap</param>
        public void ProcessPixelData(byte[] colourPixels)
        {
			Colours colours = new Colours();
			//Convert the BGRA bytes into colours
            colours.ProcessPixelBgraData(colourPixels);
            this.sendOscMsg();
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
