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
		private byte[] colourPixels;

		///// <summary>
		///// Intermediate storage for the depth data from the infrared camera
		///// </summary>
		private short[] depthPixels;

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
                this.colourPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];
				this.depthPixels = new short[this.sensor.DepthStream.FramePixelDataLength];

                // Add event handlers to be called whenever there is new color or depth frame data
				this.sensor.DepthFrameReady += this.DepthFrameReady;
				this.sensor.ColorFrameReady += this.ColorFrameReady;

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
		/// Event handler for Kinect sensor's ImageFrame event. 
		/// Once triggered lets the depth and colour frames be processed by the processing class
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">event arguments</param>
		private void ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
		{
			using (ColorImageFrame colourFrame = e.OpenColorImageFrame())
			{
				if (colourFrame != null)
				{
					colourFrame.CopyPixelDataTo(colourPixels);
				}
			}
		}

		/// <summary>
		/// Event handler for Kinect sensor's DepthFrameReady event. 
		/// Once triggered lets the depth and colour frames be processed by the processing class
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">event arguments</param>
		private void DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
		{
			using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
			{
				if (depthFrame != null)
				{
					depthFrame.CopyPixelDataTo(depthPixels);
				}
			}
			ProcessFrameData(colourPixels, depthPixels);
		}

    }
}
