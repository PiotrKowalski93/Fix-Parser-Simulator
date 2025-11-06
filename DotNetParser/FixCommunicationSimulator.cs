using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DotNetParser
{
    public class FixCommunicationSimulator
    {
        private int _port;
        private TcpListener _listener;

        public FixCommunicationSimulator(int port = 98781)
        {
                _port = port;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {

        }

    }
}
