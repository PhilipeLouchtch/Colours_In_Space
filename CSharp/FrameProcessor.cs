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
		/// Msg:	Simple string message
		/// Boxes:	List with data for sonification
        /// </summary>
		private SendOSCMsg          sendOSCMsg;
		private SendOSCBoxes		sendOSCBoxes;

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
		public FrameProcessor(SendOSCMsg sendOscMsg, SendOSCBoxes sendOSCBoxes, RuntimeSettings settings)
		{
			colours = new Colours();
			this.sendOSCMsg = sendOscMsg;
			this.sendOSCBoxes = sendOSCBoxes;
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

			int dimension	= targetBoxes.boxes[0].radius;
			int upperY		= 240 - (dimension / 2);
			// Amount of pixels in the targetbox
			int pixels		= (int)Math.Pow(dimension, 2);

			List<ShippingData> dataToTransmit = new List<ShippingData>(targetBoxes.boxes.Capacity);

			for (int i = 0; i < targetBoxes.boxes.Count; i++)
			{
				long red = 0;
				long green = 0;
				long blue = 0;

				double hue = 0;
				double saturation = 0;
				double light = 0;

				int leftX = targetBoxes.boxes[i].x;

				// Calculate the average (literally) colour in the target box
				for (int x = leftX; x < dimension + leftX; x++)
				{
					for (int y = upperY; y < upperY + dimension; y++)
					{
						red += colours.pixels[x, y].red;
						green += colours.pixels[x, y].green;
						blue += colours.pixels[x, y].blue;
					}
				}

				// Get the Hue, Saturation and Light from RGB
				Utility.RGB2HSL((int)(red / pixels), (int)(green / pixels), (int)blue / pixels, out hue, out saturation, out light);

				// Convert hue from normalized to HSL 360 degree hue
                hue = hue * 360;

				// Get the associated sonochromatic colour from the hue
				SonochromaticColourType colour = Utility.HueToSonochromatic((int)hue);

				dataToTransmit.Add(new ShippingData(colour));
			}
            //hardcoded to 5 right now
			sendOSCBoxes(dataToTransmit);
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

			// Initialize the targetBoxes collection
			targetBoxes = new TargetBoxes(boxes);				
			
			int boxStep = 640 / (boxes + 1);
			int boxWidth = (zoom ? 380 : 540) / boxes;
			int boxRadius = boxWidth / 2;

			// Prepate the boxes first to make things easier
			for (int i = 0; i < boxes; i++)
			{
				targetBox = new TargetBox();
				targetBox.middle.y = 480 / 2;
				targetBox.radius = boxRadius;
				targetBoxes.boxes.Add(targetBox);
			}		

			// First the outer left/right boxes are placed at their respective edge
			targetBoxes.boxes[0].x = 0;
			targetBoxes.boxes[0].middle.x = boxRadius;
			targetBoxes.boxes[boxes - 1].x = 640 - boxWidth;
			targetBoxes.boxes[boxes - 1].middle.x = 640 - boxRadius;

			// The middle box
			int mid = boxes / 2; // will get rounded down, which is what we want due to 0 based
			targetBoxes.boxes[mid].middle.x = 320; // middle box is always @ 320px
			targetBoxes.boxes[mid].x = 320 - boxRadius;

			// I've given up on calculating this dynamically, so hardcoded will do
			if (boxes == 5)
			{
				// Left of the middle box
				targetBoxes.boxes[1].middle.x = (320 - targetBoxes.boxes[0].middle.x) / 2 + boxRadius;
				targetBoxes.boxes[1].x = targetBoxes.boxes[1].middle.x - boxRadius;

				// Right of the middle box
				targetBoxes.boxes[3].middle.x = 320 + (targetBoxes.boxes[4].middle.x - 320) / 2;
				targetBoxes.boxes[3].x = targetBoxes.boxes[3].middle.x - boxRadius;
			}
			else if (boxes == 7)
			{
				//TODO
				throw new NotImplementedException("7 TargetBoxes is yet to be implemented");
			}
			else
				throw new NotImplementedException("More target boxes than 7 is not implemented");

        }
		
	}
}
