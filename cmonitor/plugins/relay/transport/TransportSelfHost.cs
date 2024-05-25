﻿using cmonitor.client.tunnel;
using cmonitor.config;
using cmonitor.plugins.relay.messenger;
using cmonitor.server;
using common.libs;
using common.libs.extends;
using MemoryPack;
using System.Net;
using System.Net.Sockets;

namespace cmonitor.plugins.relay.transport
{
    public sealed class TransportSelfHost : ITransport
    {
        public string Name => "默认";
        public RelayCompactType Type => RelayCompactType.Self;
        public TunnelProtocolType ProtocolType => TunnelProtocolType.Tcp;

        private readonly TcpServer tcpServer;
        private readonly MessengerSender messengerSender;

        public TransportSelfHost(TcpServer tcpServer, MessengerSender messengerSender)
        {
            this.tcpServer = tcpServer;
            this.messengerSender = messengerSender;
        }

        public async Task<ITunnelConnection> RelayAsync(RelayInfo relayInfo)
        {
            Socket socket = new Socket(relayInfo.Server.AddressFamily, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            socket.Reuse(true);
            socket.IPv6Only(relayInfo.Server.AddressFamily, false);
            await socket.ConnectAsync(relayInfo.Server).WaitAsync(TimeSpan.FromMilliseconds(500));

            IConnection connection = await tcpServer.BeginReceive(socket);
            MessageResponeInfo resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = connection,
                MessengerId = (ushort)RelayMessengerIds.RelayForward,
                Payload = MemoryPackSerializer.Serialize(relayInfo),
                Timeout = 2000
            });
            if (resp.Code != MessageResponeCodes.OK || resp.Data.Span.SequenceEqual(Helper.TrueArray) == false)
            {
                connection.Disponse();
                return null;
            }
            connection.Cancel();
            await Task.Delay(10);
            return new TunnelConnectionTcp
            {
                Direction = TunnelDirection.Forward,
                ProtocolType = TunnelProtocolType.Tcp,
                RemoteMachineName = relayInfo.RemoteMachineName,
                Socket = connection.TcpSourceSocket,
                Mode = TunnelMode.Client,
                IPEndPoint = socket.RemoteEndPoint as IPEndPoint,
                TransactionId = relayInfo.TransactionId,
                TransportName = Name,
                Type = TunnelType.Relay
            };
        }

        public async Task<ITunnelConnection> OnBeginAsync(RelayInfo relayInfo)
        {
            Socket socket = new Socket(relayInfo.Server.AddressFamily, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            socket.Reuse(true);
            socket.IPv6Only(relayInfo.Server.AddressFamily, false);
            await socket.ConnectAsync(relayInfo.Server).WaitAsync(TimeSpan.FromMilliseconds(500));

            IConnection connection = await tcpServer.BeginReceive(socket);
            await messengerSender.SendOnly(new MessageRequestWrap
            {
                Connection = connection,
                MessengerId = (ushort)RelayMessengerIds.RelayForward,
                Payload = MemoryPackSerializer.Serialize(relayInfo)
            });
            connection.Cancel();
            await Task.Delay(10);
            return new TunnelConnectionTcp
            {
                Direction = TunnelDirection.Reverse,
                ProtocolType = TunnelProtocolType.Tcp,
                RemoteMachineName = relayInfo.RemoteMachineName,
                Socket = connection.TcpSourceSocket,
                Mode = TunnelMode.Server,
                IPEndPoint = socket.RemoteEndPoint as IPEndPoint,
                TransactionId = relayInfo.TransactionId,
                TransportName = Name,
                Type = TunnelType.Relay
            };
        }
    }
}
