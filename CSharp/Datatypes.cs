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
                ArrayIndexTranslation translator = new ArrayIndexTranslation();
				for (int i = (iterationNo * length); i < (length * iterationNo); i += 4)
				{
                    translator.TranslateTo2D(i);
                    int x = translator.x;
					int y = translator.y;
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

    class ArrayIndexTranslation
    {
        public int x 
        { 
            get {
                if (x == -1 || x >= 640)
                    throw new System.IndexOutOfRangeException("X coordinate is out of range");
                return x;
            } 
            private set { x = value; }
        }

        public int y
        {   get { 
                if (x == -1 || x >= 480)
                    throw new System.IndexOutOfRangeException("Y coordinate is out of range");
                return x;
            }
            private set { y = value; }
        }

        public ArrayIndexTranslation()
        {
            x = -1;
            y = -1;
        }

        /// <summary>
        /// Translate the index of the 1D array into 2D array indexes
        /// </summary>
        /// <param name="i">1D Array index to be translated to 2D</param>
        public void TranslateTo2D(int i)
        {
            int x = (i >> 2) % 640;
			int y = (i >> 2) / 640;
        }
    }
}
