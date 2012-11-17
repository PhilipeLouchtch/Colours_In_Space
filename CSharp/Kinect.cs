using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace ColoursInSpace
{
    class Kinect
    {
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;
		
        /// <summary>
        /// Delegate to the ColoursProcessor class, processes the new frame of pixels
        /// </summary>
        private ProcessFrameData ProcessFrameData;

		///// <summary>
		///// Intermediate storage for the colour data received from the camera
		///// </summary>
		//private byte[] colourPixels;

		///// <summary>
		///// Intermediate storage for the depth data from the infrared camera
		///// </summary>
		private short[] grayDepthData;

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        public Kinect(ProcessFrameData ProcessPixelData)
        {
			//Initialize the delegate
			//this.ProcessColourBitmap = ProcessColourBitmap;
            this.ProcessFrameData = ProcessPixelData;           
        }

		public void ConnectToSensor()
		{
			 // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;					
                }
            }

            if (null != this.sensor)
            {
                // Turn on the color stream to receive color frames
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
				this.sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

                // Allocate space to put the pixels we'll receive
                //this.colourPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                // Add an event handler to be called whenever there is new color frame data
				this.sensor.AllFramesReady += this.SensorFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }
		}

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        ~Kinect()
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

		/// <summary>
		/// Event handler for Kinect sensor's AllFramesReady event. 
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">event arguments</param>
		private void SensorFrameReady(object sender, AllFramesReadyEventArgs e)
		{
			byte [] colourFrame = CopyColorPixels(e);
			short[]  depthFrame = CopyDepthPixels(e);
			ProcessFrameData(colourFrame, depthFrame);
		}

		private byte[] CopyColorPixels(AllFramesReadyEventArgs e)
		{
			using (ColorImageFrame colourFrame = e.OpenColorImageFrame())
			{
				if (colourFrame != null)
				{
					byte[] colourPixels = new byte[(640 * 480 * 4)];
					colourFrame.CopyPixelDataTo(colourPixels);
					return colourPixels;
				}
				else
					return null;
			}
		}

		private short[] CopyDepthPixels(AllFramesReadyEventArgs e)
		{
			using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
			{
				if (depthFrame != null)
				{
					short[] depthPixels = new short[(640 * 480)];
					depthFrame.CopyPixelDataTo(depthPixels);
					return depthPixels;
				}
				else
					return null;
			}
		}
    }
}
