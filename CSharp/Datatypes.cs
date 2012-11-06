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

        public void ProcessPixelBgraData(ref byte[] pixelData)
        {

            int x;
            int y;
            int length = pixelData.Length;

            //Convert the pixelData to colours
            for (int i = 0; i < length; i += 4)
            {
                x = (i / 4) % 640;
                y = (i / 4) / 640;
                pixels[x, y] = Color.FromArgb(pixelData[i + 3], pixelData[i + 2], pixelData[i + 1], pixelData[i]);
            }
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
}
