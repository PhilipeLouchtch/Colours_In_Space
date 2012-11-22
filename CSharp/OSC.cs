using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Ventuz.OSC;

namespace ColoursInSpace
{
	public delegate void SendOscMsg(string message);
    public delegate void SendOscColourBoxes5(int colour1, int colour2, int colour3, int colour4, int colour5);

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

        public void SendColourBoxes5(int colour1, int colour2, int colour3, int colour4, int colour5)
        {
            OscElement msg = new OscElement("/boxes5", colour1, colour2, colour3, colour4, colour5);
            writer.Send(msg);
        }
    }

}
