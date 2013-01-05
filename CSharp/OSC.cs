using System;
using System.Collections.Generic;
using Ventuz.OSC;

namespace ColoursInSpace
{
	public delegate void SendOSCMsg(string message);
    public delegate void SendOSCBoxes(List<ShippingDataSort> shippingData);

    class OSC : IDisposable
    {
        private UdpWriter writer;
		private bool disposed;

		public OSC(string ipAdress = "127.0.0.1", ushort port = 57120)
        {
			disposed = false;
            writer = new UdpWriter(ipAdress, (int)port);
        }

		#region Destructors and Dispose
		~OSC()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		//C# Disposing pattern
		private void Dispose(bool disposing)
		{
			if (!disposed)
				if (disposing)
					Dispose();
			disposed = true;
		}

		public void Dispose()
		{
			this.writer.Dispose();
		}
		#endregion

		public void SendMsg(string message)
        {
				OscElement msg = new OscElement("/chat", message);
                writer.Send(msg);
        }

        public void SendBoxes(List<ShippingDataSort> shippingData)
        {
			int capacity = shippingData.Capacity;
			OscElement boxesMSG;

			// Will return the instance of the settings, not new settings
			RuntimeSettings settings = RuntimeSettings.Instance;
			int synthType = (int)settings.synthType;
			double volume = settings.volume / 100.0;

			if (capacity == 3)
				boxesMSG = new OscElement("/boxes3", synthType,
													 volume,
													 (int)shippingData[0].sonochromaticColour,
													 (int)shippingData[1].sonochromaticColour,
													 (int)shippingData[2].sonochromaticColour,
													 ColourTools.GetColourVolume(shippingData[0].sonochromaticColour),
													 ColourTools.GetColourVolume(shippingData[1].sonochromaticColour),
													 ColourTools.GetColourVolume(shippingData[2].sonochromaticColour));
			else if (capacity == 5)
				boxesMSG = new OscElement("/boxes5", synthType,
													 volume,
													 (int)shippingData[0].sonochromaticColour,
													 (int)shippingData[1].sonochromaticColour,
													 (int)shippingData[2].sonochromaticColour,
													 (int)shippingData[3].sonochromaticColour,
													 (int)shippingData[4].sonochromaticColour,
													 ColourTools.GetColourVolume(shippingData[0].sonochromaticColour),
													 ColourTools.GetColourVolume(shippingData[1].sonochromaticColour),
													 ColourTools.GetColourVolume(shippingData[2].sonochromaticColour),
													 ColourTools.GetColourVolume(shippingData[3].sonochromaticColour),
													 ColourTools.GetColourVolume(shippingData[4].sonochromaticColour));
			else
				boxesMSG = new OscElement("/boxes7", synthType,
													 volume,
													 (int)shippingData[0].sonochromaticColour,
													 (int)shippingData[1].sonochromaticColour,
													 (int)shippingData[2].sonochromaticColour,
													 (int)shippingData[3].sonochromaticColour,
													 (int)shippingData[4].sonochromaticColour,
													 (int)shippingData[5].sonochromaticColour,
													 (int)shippingData[6].sonochromaticColour,
													 ColourTools.GetColourVolume(shippingData[0].sonochromaticColour),
													 ColourTools.GetColourVolume(shippingData[1].sonochromaticColour),
													 ColourTools.GetColourVolume(shippingData[2].sonochromaticColour),
													 ColourTools.GetColourVolume(shippingData[3].sonochromaticColour),
													 ColourTools.GetColourVolume(shippingData[4].sonochromaticColour),
													 ColourTools.GetColourVolume(shippingData[5].sonochromaticColour),
													 ColourTools.GetColourVolume(shippingData[6].sonochromaticColour));
			writer.Send(boxesMSG);
        }
    }

}
