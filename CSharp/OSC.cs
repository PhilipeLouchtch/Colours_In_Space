using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Ventuz.OSC;

namespace ColoursInSpace
{
    class OSC
    {
        private UdpWriter writer;
		private bool disposed;

		public OSC(string ipAdress = "127.0.0.1", ushort port = 57120)
        {
			disposed = false;
            writer = new UdpWriter(ipAdress, port);
        }

		#region Destructors and Dispose
		~OSC()
		{
			dispose(true);
			GC.SuppressFinalize(this);
		}

		//C# Disposing pattern
		public void dispose(bool disposing)
		{
			if (!disposed)
				if (disposing)
					dispose();
			disposed = true;
		}

		private bool dispose()
		{
			this.writer.Dispose();
			return true;
		}
		#endregion

		public void SendMsg()
        {
            //OscElement msg = new OscElement("/test", arr);
            //OscBundle bundle = new OscBundle();

            OscElement msg = new OscElement("/chat", "Hello From Visual Studio 2012");

            writer.Send(msg);
        }
    }
}
