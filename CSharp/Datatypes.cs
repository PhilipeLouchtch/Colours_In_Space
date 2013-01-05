using System;
using System.Collections.Generic;
using System.Threading;

namespace ColoursInSpace
{

    public enum SonochromaticColourType
    {
        Red = 0,
        Orange = 1,
        Yellow = 2,
        Chartreuse = 3,
        Green,
        Spring,
        Cyan,
        Azure,
        Blue,
        Violet,
        Magenta,
        Rose,
		WHITE,
		GRAYS,
		BLACK
    };

	public enum ColourAveragingAlgorithms
	{
		Simple,
		Euclidian
	};

	public enum SCSynthType
	{
		Formant,
		Granular
	};

	class Colour
	{
		public byte red;
		public byte green;
		public byte blue;

		public Colour(byte blue, byte green, byte red)
		{
			this.red = red;
			this.green = green;
			this.blue = blue;
		}
	}

    class Colours
    {
        public Colour[,] pixels;

		public int dimension { get; private set; }

		public Colours(TargetBox targetBox)
        {
			this.dimension = targetBox.radius * 2;

			pixels = new Colour[dimension, dimension];

			for (int x = 0; x < dimension; x++)
			{
				for (int y = 0; y < dimension; y++)
				{
					pixels[x, y] = new Colour(0, 0, 0);
				}
			}
        }

		public static void ProcessPixelByteData(byte[] pixelData, ref TargetBox targetBox, ref Colour[,] boxColours)
        {
            // Going into performance critical section
            unsafe
            {
				int boxWidth = targetBox.radius * 2;
				int boxRadius = targetBox.radius;

				int boxXLeft = targetBox.x;
				int boxYTop = targetBox.middle.y - boxRadius;
				int boxXRight = boxXLeft + boxWidth;
				int boxYBottom = boxYTop + boxWidth;

				int pixelDataRowLength = 640 * 4;
				int iterations = boxWidth * boxWidth;

				// We start reading from this index
				int pixelDataIndex = (boxXLeft) * 4 + (boxYTop * pixelDataRowLength);
				int pixelDataIndexFrom = pixelDataIndex;

				int x = 0;
				int y = 0;

				for (int i = 0; i < iterations; i++)
				{
					byte blue = pixelData[pixelDataIndex];
					byte green = pixelData[pixelDataIndex + 1];
					byte red = pixelData[pixelDataIndex + 2];

					boxColours[x, y].blue = blue;
					boxColours[x, y].green = green;
					boxColours[x, y].red = red;

					#region Compute Index
					// Finished with this row, compute the index of the next one
					if (x == boxWidth - 1)
					{
						pixelDataIndex = pixelDataIndexFrom + (y + 1) * pixelDataRowLength;
						x = 0;
						y++;
					}
					// Continue with this row
					else
					{
						pixelDataIndex += 4;
						x++;
					}
					#endregion
                }
            }

			return;
        }
    }

	/// <summary>
	/// Colours container, stores colours in a list
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

    public delegate void SettingsChanged(Object sender);

    public sealed class RuntimeSettings
    {
        #region Concurrency-safe Singleton pattern
        private static volatile RuntimeSettings instance;
		private static object syncRoot = new Object();

		public static RuntimeSettings Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new RuntimeSettings();
					}
				}

				return instance;
			}
		}
        #endregion

        /// <summary>
        /// Enables the distance translation engine
        /// </summary>
        public bool distance    { get; set; }

        /// <summary>
        /// Enables the colour translation engine
        /// </summary>
        public bool colour      { get; set; }

        /// <summary>
        /// Makes the target boxes smaller, effectively zooming in
        /// Fires a settingsChanged event when value is changed
        /// </summary>
		private bool _zoom;
		public bool zoom 
		{ 
			get { return this._zoom; } 
			set 
			{ 
				if (this._zoom != value)
				{
					// wait on the lock
					while (ColoursComputationRunningMutex)
						Thread.Sleep(1);

					amntTargetsChangingMutex = true;
					{
						_zoom = value;
						if (settingsChanged != null)
							settingsChanged(this);						
					}
					amntTargetsChangingMutex = false;
				}
			} 
		}

		public ColourAveragingAlgorithms algorithm { get; set; }

		private SCSynthType _synthType;
		public SCSynthType synthType
		{
			get
			{
				return _synthType;
			}
			set
			{
				if ((int)value != 1)
				{
					_synthType = SCSynthType.Formant;
				}
				else
				{
					_synthType = SCSynthType.Granular;
				}
				if (settingsChanged != null)
					settingsChanged(this);
			}
		}

        /// <summary>
        /// Volume, value range from 0 to 100
        /// </summary>
		private short _volume;
        public  short volume
		{
			get { return this._volume; }
			set
			{
				if (value < 0)
					this._volume = 0;
				else if (value > 100)
					this._volume = 100;
				else
					this._volume = value;

				if (settingsChanged != null)
					settingsChanged(this);	
			}
		}

        static public bool ColoursComputationRunningMutex;
        static public bool amntTargetsChangingMutex { get; private set; }

        /// <summary>
        /// Amout of TargetBoxes to be used, accepted values: 3, 5, 7. TODO: Define range.
        /// Fires a settingsChanged event when value is changed
        /// </summary>
		private ushort _amntTargetBoxes;
        public	ushort	amntTargetBoxes 
        {
			get { return this._amntTargetBoxes; }
			set 
			{
                if (this._amntTargetBoxes != value)
				{
					// wait on the lock
					while (ColoursComputationRunningMutex)
						Thread.Sleep(1);

					amntTargetsChangingMutex = true;
					{
						if (this._amntTargetBoxes != value && value >= 3 && value <= 7)
						{
							_amntTargetBoxes = value;
							if (settingsChanged != null)
								settingsChanged(this);
						}
					}
					amntTargetsChangingMutex = false;
				}
			} 
        }

        public event SettingsChanged settingsChanged;

        /// <summary>
        /// Constructor with default settings
        /// </summary>
        RuntimeSettings()
        {
            distance = false;
            colour = true;
			zoom = false;
			algorithm = ColourAveragingAlgorithms.Simple;
			synthType = SCSynthType.Formant;
            volume = 100;
            amntTargetBoxes = 5;
        }
    }

    struct Point
    {
        public int x { get; set; }
        public int y { get; set; }
    }

    /// <summary>
    /// Represents the positioning of a single TargetBox
    /// </summary>
    class TargetBox
    {
        /// <summary>
        /// The x coordinate of the left edge of the box
        /// </summary>
        public int x { get; set; }

		public int depth { get; set; }

		/// <summary>
		/// Dimension of the box
		/// </summary>
		public int radius { get; set; }

        public Point middle;

		public Colours boxColours;

		public TargetBox()
		{
			// unsed
			depth = 0;
		}
    }

    class TargetBoxes
    {
        public List<TargetBox> boxes;

		/// <summary>
		/// The padding (space) between the boxes
		/// </summary>

        public TargetBoxes(int size )
        {
            boxes = new List<TargetBox>(size);
        }
    }

	/// <summary>
	/// Contains information about the TargetBox, this will be sent to the OSC methods
	/// </summary>
	public class ShippingData
	{
		public SonochromaticColourType sonochromaticColour { get; private set; }

		public int distance { get; private set; }

		public ShippingData(SonochromaticColourType colourType, int depth = 0)
		{
			sonochromaticColour = colourType;
		}
	}

    public class ShippingDataSort : ShippingData
    {
        public static int compare(ShippingDataSort x, ShippingDataSort y)
        {
            if (x.boxNr < y.boxNr)
                return 1;
            if (x.boxNr > y.boxNr)
                return -1;
            else
                return 0;
        }

        public ShippingDataSort(SonochromaticColourType colourType, int boxNr) : base(colourType)
        {
            this.boxNr = boxNr;
        }

        public int boxNr;
    }
}
