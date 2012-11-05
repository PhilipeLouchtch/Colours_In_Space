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

        public OSC(string ipAdress, ushort port)
        {
            writer = new UdpWriter(ipAdress, port);
        }

        public void SendMsg()
        {
            //OscElement msg = new OscElement("/test", arr);
            //OscBundle bundle = new OscBundle();

            OscElement msg = new OscElement("/chat", "Hello From Visual Studio 2012");

            writer.Send(msg);
        }
    }
}
