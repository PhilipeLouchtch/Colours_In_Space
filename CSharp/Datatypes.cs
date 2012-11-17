using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ColoursInSpace
{
    class Colours
    {
        public Color[,] pixels;

        public Colours()
        {
            pixels = new Color[640, 480];            
        }
				
        public void ProcessPixelBgraData(byte[] pixelData)
        {
			ParallelOptions parallelOptions = new ParallelOptions();		
           
			//TODO: Tweak the #iterations when done
			//TODO: Test for correctness
			int iterations = 4;
			parallelOptions.MaxDegreeOfParallelism = iterations;
			int length = pixelData.Length / iterations;
			//Convert the pixelData to colours using # of iterations threads
			Parallel.For(0, iterations, parallelOptions, (iterationNo) =>
			{                
				for (int i = (iterationNo * length); i < (length * iterationNo + length); i += 4)
				{
					int x = ArrayIndexTranslation.TranslateToX(i);
					int y = ArrayIndexTranslation.TranslateToY(i);
					pixels[x, y] = Color.FromArgb(pixelData[i + 3], pixelData[i + 2], pixelData[i + 1], pixelData[i]);
				}
			});
        }
    }

    //TODO: Clamp colours?
	/// <summary>
	/// Colours container, stores colours in a list
	/// TODO: Clamp colours on addition
	/// </summary>
    class TargetColours
    {
		private List<Color> colours;

		/// <summary>
		/// Initializes the container
		/// </summary>
		/// <param name="capacity">Amount of colours to be stored,
		/// should be equal to the amount of "Target Boxes"</param>
		public TargetColours(ushort capacity)
		{
			colours = new List<Color>(capacity);
		}

		public Color this[ushort index]
		{
			get { return this.colours[index]; }
			set { this.colours[index] = value; }
		}
	}


    class RuntimeSettings
    {
        /// <summary>
        /// Enables the distance translation engine
        /// </summary>
        public bool distance    { get; set; }

        /// <summary>
        /// Enables the colour translation engine
        /// </summary>
        public bool colour      { get; set; }

        /// <summary>
        /// Makes the target boxes smaller effectively zooming in
        /// </summary>
        public bool zoom        { get; set; }


        //Placeholder for the filter type
        object filter;

        /// <summary>
        /// Volume, value range from 0 to 100. TODO: Define range of 0 to 100 for safety
        /// </summary>
        public ushort volume    { get; set; }


        /// <summary>
        /// Amout of TargetBoxes to be used, accepted values: 3, 5, 7. TODO: Define range.
        /// </summary>
        public ushort amntTargetBoxes { get; set; }

        public RuntimeSettings()
        {
            distance = true;
            colour = true;
            zoom = false;
            //filter;
            volume = 50;
            amntTargetBoxes = 3;
        }
    }
}
