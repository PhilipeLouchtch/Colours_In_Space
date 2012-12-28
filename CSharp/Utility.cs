using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ColoursInSpace
{
	class Utility
	{
		/// <summary>
		/// Converts a RBG colour to the HSL colour space
		/// Return the result via the Out by-reference keyword
		/// Code copied from http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm
		/// </summary>
		/// <param name="red">Integer [0-255] RGB Red value</param>
		/// <param name="green">Integer [0-255] RGB Green value</param>
		/// <param name="blue">Integer [0-255] RGB Blue value</param>
		/// <param name="h">Out-keyword double hue HSL value</param>
		/// <param name="s">Out-keyword double saturation HSL value</param>
		/// <param name="l">Out-keyword double light HSL value</param>
		public static void RGB2HSL(int red, int green, int blue, out double h, out double s, out double l)
		{
			#region RGB2HSL function body
			double r = red / 255.0;
			double g = green / 255.0;
			double b = blue / 255.0;

			double v;
			double m;
			double vm;

			double r2, g2, b2;

			h = 0; // default to black
			s = 0;
			l = 0;

			v = Math.Max(r, g);
			v = Math.Max(v, b);
			m = Math.Min(r, g);
			m = Math.Min(m, b);

			l = (m + v) / 2.0;

			if (l <= 0.0)
			{
				return;
			}

			vm = v - m;
			s = vm;

			if (s > 0.0)
			{
				s /= (l <= 0.5) ? (v + m) : (2.0 - v - m);
			}
			else
			{
				return;
			}

			r2 = (v - r) / vm;
			g2 = (v - g) / vm;
			b2 = (v - b) / vm;

			if (r == v)
			{
				h = (g == m ? 5.0 + b2 : 1.0 - g2);
			}
			else if (g == v)
			{
				h = (b == m ? 1.0 + r2 : 3.0 - b2);
			}
			else
			{
				h = (r == m ? 3.0 + g2 : 5.0 - r2);
			}

			h /= 6.0;
			#endregion
		}

		public static SonochromaticColourType HueToSonochromatic(int hue)
		{
			//TODO: Black and white check
			SonochromaticColourType colour;
			if (hue < 20)
				colour = SonochromaticColourType.Red;
			else if (hue < 45)
				colour = SonochromaticColourType.Orange;
			else if (hue < 73)
				colour = SonochromaticColourType.Yellow;
			else if (hue < 97)
				colour = SonochromaticColourType.Chartreuse;
			else if (hue < 124)
				colour = SonochromaticColourType.Green;
			else if (hue < 162)
				colour = SonochromaticColourType.Spring;
			else if (hue < 195)
				colour = SonochromaticColourType.Cyan;
			else if (hue < 217)
				colour = SonochromaticColourType.Azure;
			else if (hue < 235)
				colour = SonochromaticColourType.Blue;
			else if (hue < 279)
				colour = SonochromaticColourType.Violet;
			else if (hue < 315)
				colour = SonochromaticColourType.Magenta;
			else
				colour = SonochromaticColourType.Orange;
			return colour;
		}
	}

    
    static class DominantColourAlgorithms
    {
        // Heavily modified from http://chironexsoftware.com/blog/?p=60
        unsafe public static double CalculateDominantColorByEuclidianDistance(Colours colours)
        {
            List<Tuple<Colour, double>> colourDist = new List<Tuple<Colour, double>>();

			int dimension = colours.dimension;

			double hue = 0;
			double saturation = 0;
			double light = 0;
			double sumDist = 0;

			for (int x = 0; x < dimension; x += 6)
            {
                for (int y = 0; y < dimension; y += 6)
                {
                    Colour c1 = colours.pixels[x, y];
                    double dist = 0;

					for (int x2 = 0; x2 < dimension; x2 += 6)
                    {
						for (int y2 = 0; y2 < dimension; y2 += 6)
                        {
                            Colour c2 = colours.pixels[x2, y2];

                            dist += Math.Sqrt(Math.Pow(c2.red   - c1.red, 2) +
                                              Math.Pow(c2.green - c1.green, 2) +
                                              Math.Pow(c2.blue  - c1.blue, 2));
                        }
                    }

                    colourDist.Add(new Tuple<Colour, double>(c1, dist));
                }
            }
                     
            var clrs = (from entry in colourDist
                        orderby entry.Item2 ascending
                        select new { colour = entry.Item1, Dist = 1.0 / Math.Max(1, entry.Item2) }).ToList();

			sumDist = clrs.Sum(x => x.Dist);

            Utility.RGB2HSL((int)(clrs.Sum(x => x.colour.red   * x.Dist) / sumDist),
                            (int)(clrs.Sum(x => x.colour.green * x.Dist) / sumDist),
                            (int)(clrs.Sum(x => x.colour.blue  * x.Dist) / sumDist),
                            out hue,
                            out saturation,
                            out light);

            // Convert hue from normalized to HSL 360 degree hue
            hue = hue * 360;
            return hue;
        }

        unsafe public static double CalculateAverageColourByAveraging(Colours colours)
        {
			int dimension = colours.dimension;
			ulong pixelCount = (ulong) (dimension * dimension);

			ulong red = 0;
			ulong green = 0;
			ulong blue = 0;

            double hue = 0;
            double saturation = 0;
            double light = 0;

            // Calculate the average (literally) colour in the target box
			// Steps of two for a speedup and hopefuly less junk
            for (int x = 0; x < dimension; x += 2)
            {
                for (int y = 0; y < dimension; y += 2)
                {
                    red	  += colours.pixels[x, y].red;
                    green += colours.pixels[x, y].green;
                    blue  += colours.pixels[x, y].blue;
                }
            }

			Utility.RGB2HSL((int)(red   / pixelCount),
							(int)(green / pixelCount),
							(int)(blue  / pixelCount),
							out hue,
							out saturation,
							out light);

            // Convert hue from normalized to HSL 360 degree hue
            hue = hue * 360;
            return hue;
        }

    }


}
