using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ColoursInSpace
{
    //TODO: Clamp colours?
	/// <summary>
	/// Colours container, stores colours in a list
	/// TODO: Clamp colours on addition
	/// </summary>
    class Colours
    {
		private List<Color> colours;

		/// <summary>
		/// Initializes the container
		/// </summary>
		/// <param name="capacity">Amount of colours to be stored,
		/// should be equal to the amount of "Target Boxes"</param>
		public Colours(ushort capacity)
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
