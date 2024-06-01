﻿using cmonitor.client;
using cmonitor.config;
using common.libs;
using common.libs.extends;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace cmonitor.plugins.tunnel.compact
{
    public sealed class TunnelCompactTransfer
    {
        private List<ITunnelCompact> compacts;

        private readonly Config config;
        private readonly ServiceProvider serviceProvider;

        public TunnelCompactTransfer(Config config, ServiceProvider serviceProvider)
        {
            this.config = config;
            this.serviceProvider = serviceProvider;
        }

        public void Load(Assembly[] assembs)
        {
            IEnumerable<Type> types = ReflectionHelper.GetInterfaceSchieves(assembs, typeof(ITunnelCompact));
            compacts = types.Select(c => (ITunnelCompact)serviceProvider.GetService(c)).Where(c => c != null).Where(c => string.IsNullOrWhiteSpace(c.Name) == false).ToList();

            Logger.Instance.Warning($"load tunnel compacts:{string.Join(",", compacts.Select(c => c.Name))}");
        }

        public List<TunnelCompactTypeInfo> GetTypes()
        {
            return compacts.Select(c => new TunnelCompactTypeInfo { Value = c.Type, Name = c.Type.ToString() }).Distinct(new TunnelCompactTypeInfoEqualityComparer()).ToList();
        }

        public void OnServers(TunnelCompactInfo[] servers)
        {
            config.Data.Client.Tunnel.Servers = servers;
            config.Save();
        }

        public async Task<TunnelCompactIPEndPoint> GetExternalIPAsync(IPAddress localIP)
        {
            for (int i = 0; i < config.Data.Client.Tunnel.Servers.Length; i++)
            {
                TunnelCompactInfo item = config.Data.Client.Tunnel.Servers[i];
                if (item.Disabled || string.IsNullOrWhiteSpace(item.Host)) continue;
                ITunnelCompact compact = compacts.FirstOrDefault(c => c.Type == item.Type);
                if (compact == null) continue;

                try
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    IPEndPoint server = NetworkHelper.GetEndPoint(item.Host, 3478);
                    sw.Stop();
                    if (sw.ElapsedMilliseconds > 1000)
                    {
                        Logger.Instance.Warning($"get domain ip time:{sw.ElapsedMilliseconds}ms");
                    }
                    TunnelCompactIPEndPoint externalIP = await compact.GetExternalIPAsync(server);
                    if (externalIP != null)
                    {
                        externalIP.Local.Address = localIP;
                        return externalIP;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error(ex);
                }
            }
            return null;
        }
    }
}
