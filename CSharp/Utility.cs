using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;namespace ColoursInSpace{	class Utility	{		/// <summary>
		/// Converts a RBG colour to the HSL colour space
		/// Return the result via the Out by-reference keyword
		/// Code copied from http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm
		/// </summary>
		/// <param name="red">Integer [0-255] RGB Red value</param>
		/// <param name="green">Integer [0-255] RGB Green value</param>
		/// <param name="blue">Integer [0-255] RGB Blue value</param>
		/// <param name="h">Out-keyword double hue HSL value</param>
		/// <param name="s">Out-keyword double saturation HSL value</param>
		/// <param name="l">Out-keyword double light HSL value</param>		public static void RGB2HSL(int red, int green, int blue, out double h, out double s, out double l)
		{
			#region RGB2HSL function body
			double r = red / 255.0;			double g = green / 255.0;			double b = blue / 255.0;			double v;			double m;			double vm;			double r2, g2, b2;			h = 0; // default to black			s = 0;			l = 0;			v = Math.Max(r, g);			v = Math.Max(v, b);			m = Math.Min(r, g);			m = Math.Min(m, b);			l = (m + v) / 2.0;			if (l <= 0.0)			{				return;			}			vm = v - m;			s = vm;			if (s > 0.0)			{				s /= (l <= 0.5) ? (v + m) : (2.0 - v - m);			}			else			{				return;			}			r2 = (v - r) / vm;			g2 = (v - g) / vm;			b2 = (v - b) / vm;			if (r == v)			{				h = (g == m ? 5.0 + b2 : 1.0 - g2);			}			else if (g == v)			{				h = (b == m ? 1.0 + r2 : 3.0 - b2);			}			else			{				h = (r == m ? 3.0 + g2 : 5.0 - r2);			}			h /= 6.0;
			#endregion
		}	}}