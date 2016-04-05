using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace GameJamSpring2016

{
    class ClientContainer
    {
        public Socket socket;

        public const int bufferSize = 4096;

        public byte[] buffer = new byte[bufferSize];

        public StringBuilder sb = null;
    }
}
