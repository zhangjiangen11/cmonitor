﻿using linker.client.config;
using linker.config;
using linker.libs;
using linker.messenger;
using linker.plugins.client;
using linker.tunnel;
using linker.tunnel.transport;
using LiteDB;
using System.Net;
using System.Net.Quic;
using System.Security.Cryptography.X509Certificates;
using MemoryPack;
using System.Text.Json.Serialization;



namespace linker.plugins.tunnel
{
    public sealed class TunnelConfigTransfer
    {
        public int RouteLevel => config.Data.Client.Tunnel.RouteLevel + running.Data.Tunnel.RouteLevelPlus;
        public IPAddress[] LocalIPs => config.Data.Client.Tunnel.LocalIPs;
        public IPAddress[] RouteIPs => config.Data.Client.Tunnel.RouteIPs;
        public int PortMapLan => running.Data.Tunnel.PortMapLan;
        public int PortMapWan => running.Data.Tunnel.PortMapWan;
        public List<TunnelTransportItemInfo> Transports => config.Data.Client.Tunnel.Transports;
        public X509Certificate2 Certificate { get; private set; }

        private readonly FileConfig config;
        private readonly RunningConfig running;
        private readonly ClientSignInState clientSignInState;
        private readonly IMessengerSender messengerSender;
        private readonly TunnelUpnpTransfer upnpTransfer;
        private readonly ClientConfigTransfer clientConfigTransfer;

        public Action OnChanged { get; set; } = () => { };

        public TunnelConfigTransfer(FileConfig config, RunningConfig running, ClientSignInState clientSignInState, IMessengerSender messengerSender, TunnelUpnpTransfer upnpTransfer, ClientConfigTransfer clientConfigTransfer)
        {
            this.config = config;
            this.running = running;
            this.clientSignInState = clientSignInState;
            this.messengerSender = messengerSender;
            this.upnpTransfer = upnpTransfer;
            this.clientConfigTransfer = clientConfigTransfer;

            string path = Path.GetFullPath(clientConfigTransfer.SSL.File);
            if (File.Exists(path))
            {
                Certificate = new X509Certificate2(path, clientConfigTransfer.SSL.Password, X509KeyStorageFlags.Exportable);
            }
            clientSignInState.NetworkEnabledHandle += (times) =>
            {
                TimerHelper.Async(RefreshRouteLevel);
            };
            TestQuic();
        }

        public void SetTransports(List<TunnelTransportItemInfo> transports)
        {
            config.Data.Client.Tunnel.Transports = transports;
            config.Data.Update();
        }
     
        /// <summary>
        /// 刷新网关等级数据
        /// </summary>
        public void RefreshRouteLevel()
        {
            config.Data.Client.Tunnel.RouteLevel = NetworkHelper.GetRouteLevel(clientConfigTransfer.Server.Host, out List<IPAddress> ips);
            config.Data.Client.Tunnel.RouteIPs = ips.ToArray();
            config.Data.Client.Tunnel.LocalIPs = NetworkHelper.GetIPV6().Concat(NetworkHelper.GetIPV4()).ToArray();
            OnChanged();
        }

        /// <summary>
        /// 修改自己的网关层级信息
        /// </summary>
        /// <param name="tunnelTransportFileConfigInfo"></param>
        public void OnLocalRouteLevel(TunnelTransportRouteLevelInfo tunnelTransportFileConfigInfo)
        {
            running.Data.Tunnel.RouteLevelPlus = tunnelTransportFileConfigInfo.RouteLevelPlus;
            running.Data.Tunnel.PortMapWan = tunnelTransportFileConfigInfo.PortMapWan;
            running.Data.Tunnel.PortMapLan = tunnelTransportFileConfigInfo.PortMapLan;
            running.Data.Update();
            OnChanged();
        }
        public TunnelTransportRouteLevelInfo GetLocalRouteLevel()
        {
            return new TunnelTransportRouteLevelInfo
            {
                MachineId = clientConfigTransfer.Id,
                RouteLevel = config.Data.Client.Tunnel.RouteLevel,
                RouteLevelPlus = running.Data.Tunnel.RouteLevelPlus,
                PortMapWan = running.Data.Tunnel.PortMapWan,
                PortMapLan = running.Data.Tunnel.PortMapLan,
                NeedReboot = false
            };
        }

        private void TestQuic()
        {
            if (OperatingSystem.IsWindows())
            {
                if (QuicListener.IsSupported == false)
                {
                    try
                    {
                        if (File.Exists("msquic-openssl.dll"))
                        {
                            LoggerHelper.Instance.Warning($"copy msquic-openssl.dll -> msquic.dll，please restart linker");
                            File.Move("msquic.dll", "msquic.dll.temp", true);
                            File.Move("msquic-openssl.dll", "msquic.dll", true);

                            if (Environment.UserInteractive == false && OperatingSystem.IsWindows())
                            {
                                Environment.Exit(1);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                try
                {
                    if (File.Exists("msquic.dll.temp"))
                    {
                        File.Delete("msquic.dll.temp");
                    }
                    if (File.Exists("msquic-openssl.dll"))
                    {
                        File.Delete("msquic-openssl.dll");
                    }
                }
                catch (Exception)
                {
                }
            }
        }
      
    }
}


namespace linker.client.config
{
    public sealed partial class RunningConfigInfo
    {
        /// <summary>
        /// 打洞配置
        /// </summary>
        public TunnelRunningInfo Tunnel { get; set; } = new TunnelRunningInfo();
    }

    public sealed class TunnelRunningInfo
    {
        public TunnelRunningInfo() { }
        public ObjectId Id { get; set; }
        /// <summary>
        /// 附加的网关层级
        /// </summary>
        public int RouteLevelPlus { get; set; }

        public int PortMapWan { get; set; }
        public int PortMapLan { get; set; }

    }
}

namespace linker.config
{
    public partial class ConfigClientInfo
    {
        public TunnelConfigClientInfo Tunnel { get; set; } = new TunnelConfigClientInfo();
    }
    public sealed class TunnelConfigClientInfo
    {
        [JsonIgnore]
        public int RouteLevel { get; set; }

        [JsonIgnore]
        public IPAddress[] LocalIPs { get; set; }

        [JsonIgnore]
        public IPAddress[] RouteIPs { get; set; }

        /// <summary>
        /// 打洞协议列表
        /// </summary>
        public List<TunnelTransportItemInfo> Transports { get; set; } = new List<TunnelTransportItemInfo>();
    }


    [MemoryPackable]
    public sealed partial class TunnelTransportRouteLevelInfo
    {
        public string MachineId { get; set; }
        public int RouteLevel { get; set; }
        public int RouteLevelPlus { get; set; }

        public bool NeedReboot { get; set; }

        public int PortMapWan { get; set; }
        public int PortMapLan { get; set; }
    }

}
