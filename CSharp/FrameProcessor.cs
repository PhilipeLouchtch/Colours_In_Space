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

        /// <summary>
        /// Storing all the kinect settings here
        /// </summary>
        public RuntimeSettings settings;


        /// <summary>
        /// Intermediary storage for the processed ABGR frame
        /// </summary>
		private Colours colours;


        /// <summary>
        /// Delegate to the OSC.SendMsg method
        /// </summary>
		private SendOscMsg sendOscMsg;


        /// <summary>
        /// BackgroundWorker for handling the processing of a colour frame ready event from the KinectLogic
        /// </summary>
		private BackgroundWorker backgroundProcessColour;

        /// <summary>
        /// BackgroundWorker for handling the processing of a depth frame ready event from the KinectLogic
        /// </summary>
		private BackgroundWorker backgroundProcessDepth;

		/// <summary>
		/// Initializes the FrameProcessor class
		/// </summary>
        /// <param name="amntTargetBoxes">Amount of colours to be stored (the amount of targets set)</param>
		public FrameProcessor(SendOscMsg sendOscMsg, RuntimeSettings settings)
		{
			colours = new Colours();
			this.sendOscMsg = sendOscMsg;
            pixelBGRAData = new byte[(640 * 480 * 4)];  //Hardcoded size? Not very nice, yes

			this.settings = settings;
			processTargetBoxChanges();
			settings.settingsChanged += this.processTargetBoxChanges;

            //Setting up the backgroundProcesses
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
            ushort        targets       = settings.amntTargetBoxes;
			TargetColours targetColours = new TargetColours(targets);

            switch (targets)
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

		private void processTargetBoxChanges()
        {
            bool   zoom   = settings.zoom;
            ushort boxes  = settings.amntTargetBoxes;
            TargetBox	targetBox = new TargetBox();
			TargetBoxes targetBoxes;

            // Padding between the targetBoxes, there are n - 1 paddings needed
            //int padding = 100 * (1 / (boxes - 1));

			// Initialize the targetBoxes collection
			targetBoxes = new TargetBoxes(boxes);

			// The vertical middle
			targetBox.middle.y = 480 / 2;
			targetBox.y = targetBox.middle.y;
			int boxStep = 640 / (boxes + 1);
			int boxDimension = 540 / (boxes + 1) / 2;

			for (int i = 0; i < boxes; i++)
			{
				targetBox.middle.x =  boxStep * (i + 1);
				targetBox.x = targetBox.middle.x - boxDimension;
				targetBox.dimension = boxDimension * 2;
				targetBoxes.boxes.Add(targetBox);
			}
        }
		
	}
}
