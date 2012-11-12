using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Ventuz.OSC;

namespace ColoursInSpace
{
	public delegate void SendOscMsg();

    class OSC : IDisposable
    {
        private UdpWriter writer;
		private bool disposed;
		private int count;

		public OSC(string ipAdress = "127.0.0.1", ushort port = 57120)
        {
			disposed = false;
            writer = new UdpWriter(ipAdress, (int)port);
			count = 0;
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

		public void SendMsg()
        {
            //count++;
            //if (count == 29)
            //{
                OscElement msg = new OscElement("/chat", "I've got a frame!");
                writer.Send(msg);
            //    count = 0;
            //}
        }
    }
}
