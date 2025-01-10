﻿using linker.tunnel.connection;
using linker.tunnel.wanport;
using System.Net.Sockets;
using System.Net;
using System.Text;
using linker.libs.extends;
using System.Collections.Concurrent;
using linker.libs;
using System.Security.Cryptography.X509Certificates;

namespace linker.tunnel.transport
{
    /// <summary>
    /// 基于端口映射
    /// 这个没什么说的，就是设置了固定端口，就监听，对方来连这个固定的端口即可
    /// </summary>
    public sealed class TransportUdpPortMap : ITunnelTransport
    {
        public string Name => "UdpPortMap";

        public string Label => "UDP、端口映射";

        public TunnelProtocolType ProtocolType => TunnelProtocolType.Udp;

        public TunnelWanPortProtocolType AllowWanPortProtocolType => TunnelWanPortProtocolType.Tcp | TunnelWanPortProtocolType.Udp;

        public bool Reverse => true;

        public bool DisableReverse => false;

        public bool SSL => true;

        public bool DisableSSL => false;

        public byte Order => 4;

        public Action<ITunnelConnection> OnConnected { get; set; } = (state) => { };


        private const string flagTexts = $"{Helper.GlobalString}.udp.portmap.tunnel";
        private byte[] flagBytes = Encoding.UTF8.GetBytes(flagTexts);


        private readonly ConcurrentDictionary<string, TaskCompletionSource<State>> distDic = new ConcurrentDictionary<string, TaskCompletionSource<State>>();
        private readonly ConcurrentDictionary<IPEndPoint, ConnectionCacheInfo> connectionsDic = new ConcurrentDictionary<IPEndPoint, ConnectionCacheInfo>(new IPEndPointComparer());

        private readonly ITunnelMessengerAdapter tunnelMessengerAdapter;
        public TransportUdpPortMap(ITunnelMessengerAdapter tunnelMessengerAdapter)
        {
            this.tunnelMessengerAdapter = tunnelMessengerAdapter;
            CleanTask();
        }
        private X509Certificate certificate;
        public void SetSSL(X509Certificate certificate)
        {
            this.certificate = certificate;
        }

        Socket socket;
        public async Task Listen(int localPort)
        {
            if (socket != null && (socket.LocalEndPoint as IPEndPoint).Port == localPort)
            {
                if (LoggerHelper.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                {
                    LoggerHelper.Instance.Warning($"{Name} {socket.LocalEndPoint} already exists");
                }
                return;
            }

            try
            {
                socket?.SafeClose();
                if (localPort == 0) return;

                IPAddress localIP = IPAddress.IPv6Any;

                socket = new Socket(localIP.AddressFamily, SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
                socket.WindowsUdpBug();
                socket.IPv6Only(localIP.AddressFamily, false);
                socket.Bind(new IPEndPoint(localIP, localPort));

                if (LoggerHelper.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                {
                    LoggerHelper.Instance.Debug($"{Name} listen {localPort}");
                }

                byte[] bytes = new byte[65 * 1024];
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                while (true)
                {
                    try
                    {
                        SocketReceiveFromResult result = await socket.ReceiveFromAsync(bytes.AsMemory(), ep);
                        if (result.ReceivedBytes == 0)
                        {
                            break;
                        }

                        IPEndPoint remoteEP = result.RemoteEndPoint as IPEndPoint;
                        Memory<byte> memory = bytes.AsMemory(0, result.ReceivedBytes);

                        if (connectionsDic.TryGetValue(remoteEP, out ConnectionCacheInfo cache) == false)
                        {
                            if (memory.Length > flagBytes.Length && memory.Span.Slice(0, flagBytes.Length).SequenceEqual(flagBytes))
                            {
                                connectionsDic.TryAdd(remoteEP, new ConnectionCacheInfo { });
                                string key = memory.GetString();
                                if (distDic.TryRemove(key, out TaskCompletionSource<State> tcs))
                                {
                                    await socket.SendToAsync(memory, result.RemoteEndPoint);
                                    try
                                    {
                                        State state = new State { Socket = socket, RemoteEndPoint = remoteEP };
                                        tcs.SetResult(state);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }
                        else if (cache.Connection != null)
                        {
                            bool success = await cache.Connection.ProcessWrite(memory);
                            if (success == false)
                            {
                                connectionsDic.TryRemove(remoteEP, out _);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        socket.SafeClose();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                if (LoggerHelper.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                {
                    LoggerHelper.Instance.Error(ex);
                }
            }
        }

        public async Task<ITunnelConnection> ConnectAsync(TunnelTransportInfo tunnelTransportInfo)
        {
            if (tunnelTransportInfo.Direction == TunnelDirection.Forward)
            {
                if (tunnelTransportInfo.Remote.PortMapWan == 0)
                {
                    if (LoggerHelper.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                        LoggerHelper.Instance.Error($"ConnectAsync Forward【{Name}】{tunnelTransportInfo.Remote.MachineName} port mapping not configured");
                    return null;
                }
                //正向连接
                if (await tunnelMessengerAdapter.SendConnectBegin(tunnelTransportInfo).ConfigureAwait(false) == false)
                {
                    return null;
                }
                await Task.Delay(100).ConfigureAwait(false);
                ITunnelConnection connection = await ConnectForward(tunnelTransportInfo).ConfigureAwait(false);
                if (connection != null)
                {
                    await tunnelMessengerAdapter.SendConnectSuccess(tunnelTransportInfo).ConfigureAwait(false);
                    return connection;
                }
            }
            else if (tunnelTransportInfo.Direction == TunnelDirection.Reverse)
            {
                if (tunnelTransportInfo.Local.PortMapWan == 0)
                {
                    if (LoggerHelper.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                        LoggerHelper.Instance.Error($"ConnectAsync Reverse【{Name}】{tunnelTransportInfo.Local.MachineName} port mapping not configured");
                    return null;
                }
                //反向连接
                TunnelTransportInfo tunnelTransportInfo1 = tunnelTransportInfo.ToJsonFormat().DeJson<TunnelTransportInfo>();
                //等待对方连接，如果连接成功，我会收到一个socket，并且创建一个连接对象，失败的话会超时，那就是null
                var task = WaitConnect(tunnelTransportInfo1);
                if (await tunnelMessengerAdapter.SendConnectBegin(tunnelTransportInfo1).ConfigureAwait(false) == false)
                {
                    return null;
                }
                ITunnelConnection connection = await task.ConfigureAwait(false);
                if (connection != null)
                {
                    await tunnelMessengerAdapter.SendConnectSuccess(tunnelTransportInfo).ConfigureAwait(false);
                    return connection;
                }
            }


            await tunnelMessengerAdapter.SendConnectFail(tunnelTransportInfo).ConfigureAwait(false);
            return null;
        }
        public async Task OnBegin(TunnelTransportInfo tunnelTransportInfo)
        {
            if (tunnelTransportInfo.SSL && certificate == null)
            {
                LoggerHelper.Instance.Error($"{Name}->ssl Certificate not found");
                await tunnelMessengerAdapter.SendConnectFail(tunnelTransportInfo).ConfigureAwait(false);
                return;
            }
            //正向连接，等他来连
            if (tunnelTransportInfo.Direction == TunnelDirection.Forward)
            {
                if (tunnelTransportInfo.Local.PortMapWan == 0)
                {
                    if (LoggerHelper.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                        LoggerHelper.Instance.Error($"OnBegin WaitConnect 【{Name}】{tunnelTransportInfo.Local.MachineName} port mapping not configured");
                    await tunnelMessengerAdapter.SendConnectFail(tunnelTransportInfo).ConfigureAwait(false);
                    return;
                }
                _ = WaitConnect(tunnelTransportInfo).ContinueWith((result) =>
                {
                    OnConnected(result.Result);
                });
            }
            //我要连它，那就连接
            else
            {
                if (tunnelTransportInfo.Remote.PortMapWan == 0)
                {
                    if (LoggerHelper.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                        LoggerHelper.Instance.Error($"OnBegin ConnectForward 【{Name}】{tunnelTransportInfo.Remote.MachineName} port mapping not configured");
                    await tunnelMessengerAdapter.SendConnectFail(tunnelTransportInfo).ConfigureAwait(false);
                    return;
                }

                ITunnelConnection connection = await ConnectForward(tunnelTransportInfo).ConfigureAwait(false);
                if (connection != null)
                {
                    OnConnected(connection);
                    await tunnelMessengerAdapter.SendConnectSuccess(tunnelTransportInfo).ConfigureAwait(false);
                }
                else
                {
                    await tunnelMessengerAdapter.SendConnectFail(tunnelTransportInfo).ConfigureAwait(false);
                }
            }
        }

        public void OnFail(TunnelTransportInfo tunnelTransportInfo)
        {
        }
        public void OnSuccess(TunnelTransportInfo tunnelTransportInfo)
        {
        }

        private async Task<ITunnelConnection> WaitConnect(TunnelTransportInfo tunnelTransportInfo)
        {
            TaskCompletionSource<State> tcs = new TaskCompletionSource<State>(TaskCreationOptions.RunContinuationsAsynchronously);
            string key = $"{flagTexts}-{tunnelTransportInfo.Remote.MachineId}-{tunnelTransportInfo.FlowId}";
            distDic.TryAdd(key, tcs);
            try
            {
                State state = await tcs.Task.WaitAsync(TimeSpan.FromMilliseconds(5000)).ConfigureAwait(false);

                TunnelConnectionUdp result = new TunnelConnectionUdp
                {
                    RemoteMachineId = tunnelTransportInfo.Remote.MachineId,
                    RemoteMachineName = tunnelTransportInfo.Remote.MachineName,
                    Direction = tunnelTransportInfo.Direction,
                    ProtocolType = TunnelProtocolType.Udp,
                    Type = TunnelType.P2P,
                    Mode = TunnelMode.Server,
                    TransactionId = tunnelTransportInfo.TransactionId,
                    TransactionTag = tunnelTransportInfo.TransactionTag,
                    TransportName = tunnelTransportInfo.TransportName,
                    IPEndPoint = state.RemoteEndPoint,
                    Label = string.Empty,
                    BufferSize = tunnelTransportInfo.BufferSize,
                    Receive = false,
                    UdpClient = state.Socket,
                    SSL = tunnelTransportInfo.SSL,
                    Crypto = CryptoFactory.CreateSymmetric(tunnelTransportInfo.Local.MachineId)
                };
                if (connectionsDic.TryGetValue(state.RemoteEndPoint, out ConnectionCacheInfo cache))
                {
                    cache.Connection = result;
                    return result;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                distDic.TryRemove(key, out _);
            }
            return null;
        }
        private async Task<ITunnelConnection> ConnectForward(TunnelTransportInfo tunnelTransportInfo)
        {
            if (LoggerHelper.Instance.LoggerLevel <= LoggerTypes.DEBUG)
            {
                LoggerHelper.Instance.Warning($"{Name} connect to {tunnelTransportInfo.Remote.MachineId}->{tunnelTransportInfo.Remote.MachineName} {string.Join("\r\n", tunnelTransportInfo.RemoteEndPoints.Select(c => c.ToString()))}");
            }

            List<IPEndPoint> eps = new List<IPEndPoint>();
            if (tunnelTransportInfo.Remote.LocalIps.Any(c => c.AddressFamily == AddressFamily.InterNetworkV6)
              && tunnelTransportInfo.Local.LocalIps.Any(c => c.AddressFamily == AddressFamily.InterNetworkV6))
            {
                eps.Add(new IPEndPoint(tunnelTransportInfo.Remote.LocalIps.FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetworkV6), tunnelTransportInfo.Remote.PortMapWan));
            }
            eps.Add(new IPEndPoint(tunnelTransportInfo.Remote.Remote.Address, tunnelTransportInfo.Remote.PortMapWan));

            foreach (var ep in eps)
            {
                Socket targetSocket = new(ep.AddressFamily, SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
                try
                {
                    targetSocket.WindowsUdpBug();
                    targetSocket.ReuseBind(new IPEndPoint(ep.AddressFamily == AddressFamily.InterNetwork ? tunnelTransportInfo.Local.Local.Address : IPAddress.IPv6Any, tunnelTransportInfo.Local.Local.Port));

                    if (LoggerHelper.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    {
                        LoggerHelper.Instance.Warning($"{Name} connect to {tunnelTransportInfo.Remote.MachineId}->{tunnelTransportInfo.Remote.MachineName} {ep}");
                    }

                    await targetSocket.SendToAsync($"{flagTexts}-{tunnelTransportInfo.Local.MachineId}-{tunnelTransportInfo.FlowId}".ToBytes(), ep).ConfigureAwait(false);
                    await targetSocket.ReceiveFromAsync(new byte[1024], new IPEndPoint(ep.AddressFamily == AddressFamily.InterNetwork ? IPAddress.Any: IPAddress.IPv6Any, 0)).WaitAsync(TimeSpan.FromMilliseconds(500)).ConfigureAwait(false);

                    if (LoggerHelper.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                        LoggerHelper.Instance.Debug($"{Name} connect to {tunnelTransportInfo.Remote.MachineId}->{tunnelTransportInfo.Remote.MachineName} {ep} success");

                    TunnelConnectionUdp result = new TunnelConnectionUdp
                    {
                        IPEndPoint = ep,
                        TransactionId = tunnelTransportInfo.TransactionId,
                        TransactionTag = tunnelTransportInfo.TransactionTag,
                        RemoteMachineId = tunnelTransportInfo.Remote.MachineId,
                        RemoteMachineName = tunnelTransportInfo.Remote.MachineName,
                        TransportName = Name,
                        Direction = tunnelTransportInfo.Direction,
                        ProtocolType = TunnelProtocolType.Udp,
                        Type = TunnelType.P2P,
                        Mode = TunnelMode.Client,
                        Label = string.Empty,
                        BufferSize = tunnelTransportInfo.BufferSize,
                        Receive = true,
                        UdpClient = targetSocket,
                        SSL = tunnelTransportInfo.SSL,
                        Crypto = CryptoFactory.CreateSymmetric(tunnelTransportInfo.Remote.MachineId)
                    };
                    ConnectionCacheInfo cache = new ConnectionCacheInfo { Connection = result };
                    connectionsDic.AddOrUpdate(ep, cache, (a, b) => cache);
                    return result;
                }
                catch (Exception ex)
                {
                    if (LoggerHelper.Instance.LoggerLevel <= LoggerTypes.DEBUG)
                    {
                        LoggerHelper.Instance.Error($"{Name} connect {ep} fail {ex}");
                    }
                    targetSocket.SafeClose();
                }
            }
            return null;
        }


        private void CleanTask()
        {
            TimerHelper.SetInterval(() =>
            {
                var keys = connectionsDic.Where(c => (c.Value.Connection == null && c.Value.LastTicks.DiffGreater(5000)) || (c.Value.Connection != null && c.Value.Connection.Connected == false)).Select(c => c.Key).ToList();
                foreach (var item in keys)
                {
                    connectionsDic.TryRemove(item, out _);
                }
                return true;
            }, 30000);
        }
    }

    public sealed class State
    {
        public IPEndPoint RemoteEndPoint { get; set; }
        public Socket Socket { get; set; }
    }

    public sealed class ConnectionCacheInfo
    {
        public LastTicksManager LastTicks { get; set; } = new LastTicksManager();
        public TunnelConnectionUdp Connection { get; set; }
    }

    public sealed class IPEndPointComparer : IEqualityComparer<IPEndPoint>
    {
        public bool Equals(IPEndPoint x, IPEndPoint y)
        {
            return x.Equals(y);
        }
        public int GetHashCode(IPEndPoint obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }
}
