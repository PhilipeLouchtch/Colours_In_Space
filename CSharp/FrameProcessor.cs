using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.Threading;
using System.ComponentModel;
using System.Collections.Concurrent;

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
        /// Delegates to the OSC send message methods
        /// Msg:	Simple string message
        /// Boxes:	List with data for sonification
        /// </summary>
        private SendOSCMsg sendOSCMsg;
        private SendOSCBoxes sendOSCBoxes;

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
            this.sendOSCMsg = sendOscMsg;
            this.sendOSCBoxes = sendOSCBoxes;
            pixelBGRAData = new byte[(640 * 480 * 4)];

            this.settings = settings;
            processTargetBoxChanges(settings); // Process the first-time settings
            settings.settingsChanged += this.processTargetBoxChanges;

            // Setting up the backgroundProcesses
            backgroundProcessColour = new BackgroundWorker();
            backgroundProcessDepth = new BackgroundWorker();
            backgroundProcessColour.DoWork += new DoWorkEventHandler(this.ProcessColourData);
            backgroundProcessDepth.DoWork += new DoWorkEventHandler(this.ProcessDepthData);
        }

        private void ProcessColourData(object sender, DoWorkEventArgs e)
		{
			byte[] pixelData = (byte[])e.Argument;

			// Concurrency safe datastructure
			ConcurrentBag<ShippingDataSort> bag = new ConcurrentBag<ShippingDataSort>();
			List<ShippingDataSort> shippingData = new List<ShippingDataSort>(targetBoxes.boxes.Count);

			while (RuntimeSettings.amntTargetsChangingMutex) Thread.Sleep(1); // wait for changes to be processed there.

			RuntimeSettings.ColoursComputationRunningMutex = true;
			{
				ParallelOptions options = new ParallelOptions();
				options.MaxDegreeOfParallelism = 4;
				Parallel.For(0, targetBoxes.boxes.Count, options, (i) =>
				//for (int i = 0; i < targetBoxes.boxes.Count; i++)
				{
					TargetBox targetBox = targetBoxes.boxes[i];

					targetBox.boxColours.ProcessPixelByteData(pixelData, ref targetBox);

					double hue = DominantColourAlgorithms.CalculateAverageColourByAveraging(targetBox.boxColours);

					//double hue = DominantColourAlgorithms.CalculateDominantColorByEuclidianDistance(targetBox.boxColours);

					// Get the associated sonochromatic colour from the hue
					SonochromaticColourType colour = Utility.HueToSonochromatic((int)hue);
					bag.Add(new ShippingDataSort(colour, i));
				});
			}
			RuntimeSettings.ColoursComputationRunningMutex = false;

			int bagSize = bag.Count;
			for (int i = 0; i < bagSize; i++)
			{
				ShippingDataSort element;
				while (!bag.TryTake(out element)) ;
				shippingData.Add(element);
			}

			// Needs to be sorted,
			// boxes have been inserted in a non-linear order due to concurrent loop
			shippingData.Sort(ShippingDataSort.compare);

			sendOSCBoxes(shippingData);
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
            bool zoom = this.settings.zoom;
            ushort boxes = this.settings.amntTargetBoxes;
            TargetBox targetBox = new TargetBox();

            // Initialize the targetBoxes collection
            targetBoxes = new TargetBoxes(boxes);

            int boxStep = 640 / (boxes + 1);
            int boxWidth = (zoom ? 380 : 540) / boxes;
            int boxRadius = boxWidth / 2;

            // Prepare the boxes first to make things easier
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

            if (boxes == 3)
            {
                //nop
            }
            // I've given up on calculating this dynamically, so hardcoded will do
            else if (boxes == 5)
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
                int widthRemaining = targetBoxes.boxes[mid].x - boxWidth;	// Space remaining between the first and the middle box
                int padding = (widthRemaining - boxWidth * 2) / 3;			// Empty space between the boxes

                // Left of the middle box
                targetBoxes.boxes[1].x = boxWidth + padding;
                targetBoxes.boxes[1].middle.x = targetBoxes.boxes[1].x + boxRadius;
                targetBoxes.boxes[2].x = targetBoxes.boxes[1].middle.x + boxRadius + padding;
                targetBoxes.boxes[2].middle.x = targetBoxes.boxes[2].x + boxRadius;

                // Right of the middle box
                targetBoxes.boxes[4].x = targetBoxes.boxes[mid].middle.x + boxRadius + padding;
                targetBoxes.boxes[4].middle.x = targetBoxes.boxes[4].x + boxRadius;
                targetBoxes.boxes[5].x = targetBoxes.boxes[4].middle.x + boxRadius + padding;
                targetBoxes.boxes[5].middle.x = targetBoxes.boxes[5].x + boxRadius;
            }
            else
                throw new NotImplementedException("More target boxes than 7 is not implemented");

			for (int i = 0; i < boxes; i++ )
			{
				TargetBox tempBox = targetBoxes.boxes[i];
				tempBox.boxColours = new Colours(targetBox);
			}
        }


    }
}
