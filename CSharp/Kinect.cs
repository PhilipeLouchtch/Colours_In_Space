using System;
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
	public delegate void ProcessColourBitmapDelegate(WriteableBitmap colourBitmap);

    class Kinect
    {
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

		/// <summary>
		/// Bitmap that will hold a single frame
		/// </summary>
		private WriteableBitmap colourBitmap;
		
		private ProcessColourBitmapDelegate ProcessColourBitmap;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;

        /// <summary>
        /// Execute startup tasks
        /// </summary>
		public Kinect(ProcessColourBitmapDelegate ProcessColourBitmap)
        {
			//Initialize the delegate
			this.ProcessColourBitmap = ProcessColourBitmap;

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

                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

				// Initialize the colourBitmap
				this.colourBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth,
														this.sensor.ColorStream.FrameHeight,
														96.0, 96.0, PixelFormats.Bgr32, null);

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.ColorFrameReady += this.SensorColorFrameReady;

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
        /// Event handler for Kinect sensor's ColorFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    // Write the pixel data into the bitmap
                    this.colourBitmap.WritePixels(new Int32Rect(0, 0, this.colourBitmap.PixelWidth, this.colourBitmap.PixelHeight),
                                                 this.colorPixels,
                                                 this.colourBitmap.PixelWidth * sizeof(int),
                                                 0);
                }
            }
			ProcessColourBitmap(colourBitmap.Clone());
        }
    }
}
