﻿using linker.libs.extends;
using System.Buffers;
using System.Net;
using System.Net.Sockets;

namespace linker.tunnel.wanport
{
    public sealed class TunnelWanPortProtocolLinkerUdp : ITunnelWanPortProtocol
    {
        public string Name => "Linker Udp";
        public TunnelWanPortType Type => TunnelWanPortType.Linker;

        public TunnelWanPortProtocolType ProtocolType => TunnelWanPortProtocolType.Udp;

        public TunnelWanPortProtocolLinkerUdp()
        {

        }

        public async Task<TunnelWanPortEndPoint> GetAsync(IPEndPoint server)
        {
            UdpClient udpClient = new UdpClient(AddressFamily.InterNetwork);
            udpClient.Client.Reuse();
            udpClient.Client.WindowsUdpBug();

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    await udpClient.SendAsync(new byte[1] { 0 }, server).ConfigureAwait(false);
                    UdpReceiveResult result = await udpClient.ReceiveAsync().WaitAsync(TimeSpan.FromMilliseconds(500)).ConfigureAwait(false);
                    if (result.Buffer.Length == 0)
                    {
                        return null;
                    }

                    for (int j = 0; j < result.Buffer.Length; j++)
                    {
                        result.Buffer[j] = (byte)(result.Buffer[j] ^ byte.MaxValue);
                    }
                    AddressFamily addressFamily = (AddressFamily)result.Buffer[0];
                    int length = addressFamily == AddressFamily.InterNetwork ? 4 : 16;
                    IPAddress ip = new IPAddress(result.Buffer.AsSpan(1, length));
                    ushort port = result.Buffer.AsMemory(1 + length).ToUInt16();

                    IPEndPoint remoteEP = new IPEndPoint(ip, port);

                    return new TunnelWanPortEndPoint { Local = udpClient.Client.LocalEndPoint as IPEndPoint, Remote = remoteEP };
                }
                catch (Exception)
                {
                }
            }

            return null;
        }
    }


    public sealed class TunnelWanPortProtocolLinkerTcp : ITunnelWanPortProtocol
    {
        public string Name => "Linker Tcp";
        public TunnelWanPortType Type => TunnelWanPortType.Linker;

        public TunnelWanPortProtocolType ProtocolType => TunnelWanPortProtocolType.Tcp;

        public TunnelWanPortProtocolLinkerTcp()
        {

        }

        public async Task<TunnelWanPortEndPoint> GetAsync(IPEndPoint server)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(20);
            try
            {
                Socket socket = new Socket(server.AddressFamily, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                socket.Reuse(true);
                await socket.ConnectAsync(server).ConfigureAwait(false);
                await socket.SendAsync(new byte[] { 0 });
                int length = await socket.ReceiveAsync(buffer.AsMemory(), SocketFlags.None).ConfigureAwait(false);

                for (int j = 0; j < length; j++)
                {
                    buffer[j] = (byte)(buffer[j] ^ byte.MaxValue);
                }
                AddressFamily addressFamily = (AddressFamily)buffer[0];
                int iplength = addressFamily == AddressFamily.InterNetwork ? 4 : 16;
                IPAddress ip = new IPAddress(buffer.AsSpan(1, iplength));
                ushort port = buffer.AsMemory(1 + iplength).ToUInt16();

                IPEndPoint remoteEP = new IPEndPoint(ip, port);
                IPEndPoint localEP = socket.LocalEndPoint as IPEndPoint;
                socket.Close();

                return new TunnelWanPortEndPoint { Local = localEP, Remote = remoteEP };
            }
            catch (Exception)
            {
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            return null;
        }
    }
}
