using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoursInSpace
{
	class ArrayIndexTranslation
	{
		
		public static int TranslateToX(int i)
		{
			return (i >> 2) % 640;			
		}

		public static int TranslateToY(int i)
		{
			return (i >> 2) / 640;
		}
	}	
	
}
