﻿using cmonitor.config;
using common.libs.extends;
using System.Net;
using System.Net.Sockets;

namespace cmonitor.plugins.tunnel.compact
{
    public sealed class CompactSelfHost : ICompact
    {
        public string Name => "self";

        public TunnelCompactType Type => TunnelCompactType.Self;

        public CompactSelfHost()
        {
        }

        public async Task<TunnelCompactIPEndPoint> GetExternalIPAsync(IPEndPoint server)
        {
            using UdpClient udpClient = new UdpClient();
            udpClient.Client.Reuse(true);

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    await udpClient.SendAsync(new byte[1] { 0 }, server);
                    UdpReceiveResult result = await udpClient.ReceiveAsync().WaitAsync(TimeSpan.FromMilliseconds(500));
                    if (result.Buffer.Length == 0)
                    {
                        return null;
                    }
                    IPEndPoint remoteEP = IPEndPoint.Parse(result.Buffer.AsSpan().GetString());

                    return new TunnelCompactIPEndPoint { Local = udpClient.Client.LocalEndPoint as IPEndPoint, Remote = remoteEP };
                }
                catch (Exception)
                {
                }
            }
            return null;
        }
    }
}
