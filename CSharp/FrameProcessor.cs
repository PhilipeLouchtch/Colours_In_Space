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
		/// Storing the information about the target boxes here
		/// </summary>
		TargetBoxes targetBoxes;

        /// <summary>
        /// Storing all the kinect settings here
        /// </summary>
        public RuntimeSettings settings;

        /// <summary>
        /// Intermediary storage for the processed ABGR frame
        /// </summary>
		private Colours colours;

        /// <summary>
        /// Delegates to the OSC send message methods
        /// </summary>
		private SendOscMsg          sendOSCMsg;
        private SendOscColourBoxes5 sendOSCBoxes5;

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
		public FrameProcessor(SendOscMsg sendOscMsg, SendOscColourBoxes5 sendOSCColourBoxes5, RuntimeSettings settings)
		{
			colours = new Colours();
			this.sendOSCMsg = sendOscMsg;
            this.sendOSCBoxes5 = sendOSCColourBoxes5;
            pixelBGRAData = new byte[(640 * 480 * 4)];  //Hardcoded size? Not very nice, yes

			this.settings = settings;
			processTargetBoxChanges(settings); //Run it the first time
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

			int dimension	= targetBoxes.boxes[0].dimension;
			int upperY		= 240 - (dimension / 2);
			int pixels		= (int)Math.Pow(dimension, 2);

			for (int i = 0; i < targetBoxes.boxes.Count; i++)
			{
				long red = 0;
				long green = 0;
				long blue = 0;

				double hue = 0;
				double saturation = 0;
				double light = 0;

				int leftX = targetBoxes.boxes[i].x;

				for (int x = leftX; x < dimension + leftX; x++)
				{
					for (int y = upperY; y < upperY + dimension; y++)
					{
						red += colours.pixels[x, y].red;
						green += colours.pixels[x, y].green;
						blue += colours.pixels[x, y].blue;
					}
				}
				Utility.RGB2HSL((int)(red / pixels), (int)(green / pixels), (int)blue / pixels, out hue, out saturation, out light);

                hue = hue * 360;

                //TODO: Black and white check
                //TODO: Place in utility function
                ColourTypes colour;
                if (hue < 20)
                    colour = ColourTypes.Red;
                else if (hue < 45)
                    colour = ColourTypes.Orange;
                else if (hue < 73)
                    colour = ColourTypes.Yellow;
                else if (hue < 97)
                    colour = ColourTypes.Chartreuse;
                else if (hue < 124)
                    colour = ColourTypes.Green;
                else if (hue < 162)
                    colour = ColourTypes.Spring;
                else if (hue < 195)
                    colour = ColourTypes.Cyan;
                else if (hue < 217)
                    colour = ColourTypes.Azure;
                else if (hue < 235)
                    colour = ColourTypes.Blue;
                else if (hue < 279)
                    colour = ColourTypes.Violet;
                else if (hue < 315)
                    colour = ColourTypes.Magenta;
                else
                    colour = ColourTypes.Orange;

                targetBoxes.boxes[i].colour = colour;
			}
            //hardcoded 5 right now
            sendOSCBoxes5((int)targetBoxes.boxes[0].colour, (int)targetBoxes.boxes[1].colour, (int)targetBoxes.boxes[2].colour, (int)targetBoxes.boxes[3].colour, (int)targetBoxes.boxes[4].colour);
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
				//this.sendOSCMessage("Processing frame!");
			}
			else
				this.sendOSCMessage("Worker is busy, skipping frame...");

			//TODO: process depth data

			
        }

		private void sendOSCMessage(string message)
		{
			this.sendOSCMsg(message);
		}


		private void processTargetBoxChanges(Object sender)
        {
			settings = (RuntimeSettings) sender;
            bool   zoom   = settings.zoom;
            ushort boxes  = settings.amntTargetBoxes;
            TargetBox	targetBox = new TargetBox();

            // Padding between the targetBoxes, there are n - 1 paddings needed
            //int padding = 100 * (1 / (boxes - 1));

			// Initialize the targetBoxes collection
			targetBoxes = new TargetBoxes(boxes);

			// The vertical middle
			
			
			int boxStep = 640 / (boxes + 1);
			int boxDimension = (zoom ? 440 : 540) / (boxes + 1) / 2;

			for (int i = 0; i < boxes; i++)
			{
				targetBox = new TargetBox();
				targetBox.middle.y = 480 / 2;
				targetBox.middle.x =  boxStep * (i + 1);
				targetBox.x = targetBox.middle.x - boxDimension;
				targetBox.dimension = boxDimension * 2;
				targetBoxes.boxes.Add(targetBox);
			}
        }
		
	}
}
