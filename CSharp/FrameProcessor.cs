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
using System.ComponentModel;

namespace ColoursInSpace
{
	public delegate void ProcessFrameData(byte[] colourPixels, short[] depthPixels);

	class FrameProcessor
	{
		private byte[] pixelBGRAData;
		private short[] grayDepthData;

		public ushort amntTargetBoxes { set; get; }

		private Colours colours;

		private SendOscMsg sendOscMsg;

		private BackgroundWorker backgroundProcessColour;
		private BackgroundWorker backgroundProcessDepth;

		/// <summary>
		/// Initializes the FrameProcessor class
		/// </summary>
        /// <param name="amntTargetBoxes">Amount of colours to be stored (the amount of targets set)</param>
		public FrameProcessor(SendOscMsg sendOscMsg, ushort amntTargetBoxes = 3)
		{
			colours = new Colours();
			this.sendOscMsg = sendOscMsg;
			this.amntTargetBoxes = amntTargetBoxes;
            pixelBGRAData = new byte[(640 * 480 * 4)];  //Hardcoded size? Not very nice, yes

			backgroundProcessColour = new BackgroundWorker();
			backgroundProcessDepth = new BackgroundWorker();
			backgroundProcessColour.DoWork += new DoWorkEventHandler(this.ProcessColourData);
			backgroundProcessDepth.DoWork += new DoWorkEventHandler(this.ProcessDepthData);
		}

		private void ProcessColourData(object sender, DoWorkEventArgs e)
		{
			colours.ProcessPixelBgraData((byte[])e.Argument, sender);
		}

		private void ProcessDepthData(object depthPixels, DoWorkEventArgs e)
		{
			//Nothing yet
		}

        /// <summary>
        /// Handles a new frame from the Kinect sensor
        /// </summary>
		public void ProcessPixelData(byte[] colourPixels, short[] depthPixels)
        {
			//Convert the BGRA bytes into colours
			if (!backgroundProcessColour.IsBusy)
			{
				backgroundProcessColour.RunWorkerAsync(colourPixels);
				this.sendOSCMessage("Processing frame!");
			}
			else
				this.sendOSCMessage("Worker is busy, skipping frame...");

			//TODO: process depth data

			
        }

		private void sendOSCMessage(string message)
		{
			this.sendOscMsg(message);
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
