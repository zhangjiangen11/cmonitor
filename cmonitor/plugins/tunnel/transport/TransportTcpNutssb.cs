﻿using cmonitor.client.tunnel;
using cmonitor.plugins.tunnel.server;
using common.libs;
using common.libs.extends;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace cmonitor.plugins.tunnel.transport
{
    public sealed class TransportTcpNutssb : ITransport
    {
        public string Name => "TcpNutssb";
        public TunnelProtocolType ProtocolType => TunnelProtocolType.Tcp;

        public Func<TunnelTransportInfo, Task<bool>> OnSendConnectBegin { get; set; } = async (info) => { return await Task.FromResult<bool>(false); };
        public Func<TunnelTransportInfo, Task> OnSendConnectFail { get; set; } = async (info) => { await Task.CompletedTask; };
        public Action<TunnelTransportInfo> OnConnectBegin { get; set; } = (info) => { };
        public Action<TunnelTransportInfo> OnConnecting { get; set; }
        public Action<ITunnelConnection> OnConnected { get; set; } = (state) => { };
        public Action<string> OnConnectFail { get; set; } = (machineName) => { };


        private readonly TunnelBindServer tunnelBindServer;
        public TransportTcpNutssb(TunnelBindServer tunnelBindServer)
        {
            this.tunnelBindServer = tunnelBindServer;
            tunnelBindServer.OnTcpConnected += OnTcpConnected;
        }

        public async Task<ITunnelConnection> ConnectAsync(TunnelTransportInfo tunnelTransportInfo)
        {
            OnConnecting(tunnelTransportInfo);

            //正向连接
            tunnelTransportInfo.Direction = TunnelDirection.Forward;
            if (await OnSendConnectBegin(tunnelTransportInfo) == false)
            {
                OnConnectFail(tunnelTransportInfo.Remote.MachineName);
                return null;
            }
            ITunnelConnection connection = await ConnectForward(tunnelTransportInfo);
            if (connection != null) return connection;

            //反向连接
            TunnelTransportInfo tunnelTransportInfo1 = tunnelTransportInfo.ToJsonFormat().DeJson<TunnelTransportInfo>();
            tunnelTransportInfo1.Direction = TunnelDirection.Reverse;
            tunnelBindServer.Bind(tunnelTransportInfo1.Local.Local, tunnelTransportInfo1);
            BindAndTTL(tunnelTransportInfo1);
            if (await OnSendConnectBegin(tunnelTransportInfo1) == false)
            {
                OnConnectFail(tunnelTransportInfo.Remote.MachineName);
                return null;
            }
            connection = await WaitReverse(tunnelTransportInfo1);
            if (connection != null) return connection;

            //正向反向都失败
            await OnSendConnectFail(tunnelTransportInfo);
            OnConnectFail(tunnelTransportInfo.Remote.MachineName);
            return null;
        }

        public void OnBegin(TunnelTransportInfo tunnelTransportInfo)
        {
            OnConnectBegin(tunnelTransportInfo);
            if (tunnelTransportInfo.Direction == TunnelDirection.Forward)
            {
                tunnelBindServer.Bind(tunnelTransportInfo.Local.Local, tunnelTransportInfo);
            }
            Task.Run(async () =>
            {
                if (tunnelTransportInfo.Direction == TunnelDirection.Forward)
                {
                    BindAndTTL(tunnelTransportInfo);
                }
                else
                {
                    ITunnelConnection connection = await ConnectForward(tunnelTransportInfo);
                    if (connection != null)
                    {
                        OnConnected(connection);
                    }
                    else
                    {
                        await OnSendConnectFail(tunnelTransportInfo);
                        OnConnectFail(tunnelTransportInfo.Local.MachineName);
                    }
                }
            });
        }

        public void OnFail(TunnelTransportInfo tunnelTransportInfo)
        {
            tunnelBindServer.RemoveBind(tunnelTransportInfo.Local.Local.Port);
        }

        private async Task<ITunnelConnection> ConnectForward(TunnelTransportInfo tunnelTransportInfo)
        {
            await Task.Delay(20);
            //要连接哪些IP
            IPEndPoint[] eps = new IPEndPoint[] {
                new IPEndPoint(tunnelTransportInfo.Remote.Local.Address,tunnelTransportInfo.Remote.Local.Port),
                new IPEndPoint(tunnelTransportInfo.Remote.Local.Address,tunnelTransportInfo.Remote.Remote.Port),
                new IPEndPoint(tunnelTransportInfo.Remote.Local.Address,tunnelTransportInfo.Remote.Remote.Port+1),
                new IPEndPoint(tunnelTransportInfo.Remote.Remote.Address,tunnelTransportInfo.Remote.Remote.Port),
                new IPEndPoint(tunnelTransportInfo.Remote.Remote.Address,tunnelTransportInfo.Remote.Remote.Port+1),
            };
            foreach (IPEndPoint ep in eps)
            {
                Socket targetSocket = new(ep.AddressFamily, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                targetSocket.IPv6Only(ep.Address.AddressFamily, false);
                targetSocket.ReuseBind(new IPEndPoint(ep.AddressFamily == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any, tunnelTransportInfo.Local.Local.Port));
                IAsyncResult result = targetSocket.BeginConnect(ep, null, null);

                int times = ep.Address.Equals(tunnelTransportInfo.Remote.Remote.Address) ? 25 : 5;
                for (int i = 0; i < times; i++)
                {
                    if (result.IsCompleted)
                    {
                        break;
                    }
                    await Task.Delay(20);
                }

                try
                {
                    if (result.IsCompleted == false)
                    {
                        targetSocket.SafeClose();
                        continue;
                    }

                    targetSocket.EndConnect(result);
                    return new TunnelConnectionTcp
                    {
                        Socket = targetSocket,
                        TransactionId = tunnelTransportInfo.TransactionId,
                        RemoteMachineName = tunnelTransportInfo.Remote.MachineName,
                        TransportName = Name,
                        Direction = tunnelTransportInfo.Direction,
                        ProtocolType = ProtocolType,
                        Type = TunnelType.P2P,
                        Label = string.Empty
                    };
                }
                catch (Exception ex)
                {
                    if (Logger.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    {
                        Logger.Instance.Error(targetSocket.RemoteEndPoint.ToString());
                        Logger.Instance.Error(ex);
                    }
                    targetSocket.SafeClose();
                }
            }
            return null;
        }
        private void BindAndTTL(TunnelTransportInfo tunnelTransportInfo)
        {
            //给对方发送TTL消息
            IPEndPoint[] eps = new IPEndPoint[] {
                new IPEndPoint(tunnelTransportInfo.Remote.Local.Address,tunnelTransportInfo.Local.Local.Port),
                new IPEndPoint(tunnelTransportInfo.Remote.Local.Address,tunnelTransportInfo.Remote.Remote.Port),
                new IPEndPoint(tunnelTransportInfo.Remote.Local.Address,tunnelTransportInfo.Remote.Remote.Port+1),
                new IPEndPoint(tunnelTransportInfo.Remote.Remote.Address,tunnelTransportInfo.Remote.Remote.Port),
                new IPEndPoint(tunnelTransportInfo.Remote.Remote.Address,tunnelTransportInfo.Remote.Remote.Port+1),
            };
            //过滤掉不支持IPV6的情况
            IEnumerable<Socket> sockets = eps.Where(c => NotIPv6Support(c.Address) == false).Select(ip =>
            {
                Socket targetSocket = new(ip.AddressFamily, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                try
                {
                    targetSocket.IPv6Only(ip.Address.AddressFamily, false);
                    targetSocket.Ttl = (short)(tunnelTransportInfo.Local.RouteLevel + 1);
                    targetSocket.ReuseBind(new IPEndPoint(ip.AddressFamily == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any, tunnelTransportInfo.Local.Local.Port));
                    _ = targetSocket.ConnectAsync(ip);
                    return targetSocket;
                }
                catch (Exception)
                {
                }
                return null;
            });
            foreach (Socket item in sockets.Where(c => c != null && c.Connected == false))
            {
                item.SafeClose();
            }
        }


        private ConcurrentDictionary<string, TaskCompletionSource<ITunnelConnection>> reverseDic = new ConcurrentDictionary<string, TaskCompletionSource<ITunnelConnection>>();
        private async Task<ITunnelConnection> WaitReverse(TunnelTransportInfo tunnelTransportInfo)
        {
            TaskCompletionSource<ITunnelConnection> tcs = new TaskCompletionSource<ITunnelConnection>();
            reverseDic.TryAdd(tunnelTransportInfo.Remote.MachineName, tcs);

            try
            {
                ITunnelConnection connection = await tcs.Task.WaitAsync(TimeSpan.FromMilliseconds(3000));
                return connection;
            }
            catch (Exception)
            {
            }
            finally
            {
                reverseDic.TryRemove(tunnelTransportInfo.Remote.MachineName, out _);
            }
            return null;
        }


        private void OnTcpConnected(object state, Socket socket)
        {
            if (state is TunnelTransportInfo _state && _state.TransportName == Name)
            {
                TunnelConnectionTcp result = new TunnelConnectionTcp
                {
                    RemoteMachineName = _state.Remote.MachineName,
                    Direction = _state.Direction,
                    ProtocolType = TunnelProtocolType.Tcp,
                    Socket = socket,
                    Type = TunnelType.P2P,
                    TransactionId = _state.TransactionId,
                    TransportName = _state.TransportName,
                    Label = string.Empty,
                };
                if (reverseDic.TryRemove(_state.Remote.MachineName, out TaskCompletionSource<ITunnelConnection> tcs))
                {
                    tcs.SetResult(result);
                    return;
                }

                OnConnected(result);
            }
        }

        private bool NotIPv6Support(IPAddress ip)
        {
            return ip.AddressFamily == AddressFamily.InterNetworkV6 && (NetworkHelper.IPv6Support == false);
        }

    }
}