using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoursInSpace
{
	class Colour
	{
		public byte red;
		public byte green;
		public byte blue;

		public Colour(byte red, byte green, byte blue)
		{
			this.red = red;
			this.green = green;
			this.blue = blue;
		}
	}

    class Colours
    {
        public Colour[,] pixels;

        public Colours()
        {
            pixels = new Colour[640, 480];
			for (int i = 0; i < 480; i++)
			{
				for (int j = 0; j < 640; j++)
				{
					pixels[j, i] = new Colour(0, 0, 0);
				}
			}
        }
				
        public void ProcessPixelBgraData(byte[] pixelData, object sender)
        {
			ParallelOptions parallelOptions = new ParallelOptions();

			//TODO: Tweak the #iterations when done
			//TODO: Test for correctness
			int iterations = 2;
			parallelOptions.MaxDegreeOfParallelism = iterations;
			int length = pixelData.Length / iterations;
			//Convert the pixelData to colours using # of iterations threads
			Parallel.For(0, iterations, parallelOptions, (iterationNo) =>
			{
				int from = iterationNo * length;
				int to	 = length * (iterationNo + 1);
				int x	 = (from >> 2) % 640;
				int y	 = (from >> 2) / 640;

				for (int i = from; i < to; i += 4)
				{			
					pixels[x, y].blue = pixelData[i];
					pixels[x, y].green = pixelData[i + 1]; 
					pixels[x, y].red = pixelData[i + 2];
				
					if (x < 639) x++; else { x = 0; y++; }  //more efficient than a modulo and a div operation
				}
			});

			return;
        }
    }

    //TODO: Clamp colours?
	/// <summary>
	/// Colours container, stores colours in a list
	/// TODO: Clamp colours on addition
	/// </summary>
    class TargetColours
    {
		private List<Colour> colours;

		/// <summary>
		/// Initializes the container
		/// </summary>
		/// <param name="capacity">Amount of colours to be stored,
		/// should be equal to the amount of "Target Boxes"</param>
		public TargetColours(ushort capacity)
		{
			colours = new List<Colour>(capacity);
		}

		public Colour this[ushort index]
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
