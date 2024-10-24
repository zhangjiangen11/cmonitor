﻿using linker.client.config;
using linker.config;
using linker.libs;
using linker.plugins.client;
using MemoryPack;
using Microsoft.Extensions.DependencyInjection;

namespace linker.plugins.tunnel.excludeip
{
    public sealed partial class TunnelExcludeIPTransfer
    {
        private readonly List<ITunnelExcludeIP> excludeIPs;

        private readonly RunningConfig running;
        private readonly ClientSignInState clientSignInState;
        private readonly FileConfig fileConfig;

        private readonly ServiceProvider serviceProvider;
        public TunnelExcludeIPTransfer(RunningConfig running, ClientSignInState clientSignInState, FileConfig fileConfig, ServiceProvider serviceProvider)
        {
            this.running = running;
            this.clientSignInState = clientSignInState;
            this.fileConfig = fileConfig;
            this.serviceProvider = serviceProvider;

            IEnumerable<Type> types = GetSourceGeneratorTypes();
            excludeIPs = types.Select(c => (ITunnelExcludeIP)serviceProvider.GetService(c)).Where(c => c != null).ToList();
            LoggerHelper.Instance.Info($"load tunnel excludeips :{string.Join(",", types.Select(c => c.Name))}");
        }

        public List<ExcludeIPItem> Get()
        {
            List<ExcludeIPItem> result = new List<ExcludeIPItem>();
            foreach (var item in excludeIPs)
            {
                var ips = item.Get();
                if (ips != null && ips.Length > 0)
                {
                    result.AddRange(ips);
                }
            }
            if (running.Data.Tunnel.ExcludeIPs.Length > 0)
            {
                result.AddRange(running.Data.Tunnel.ExcludeIPs);
            }
            
            return result;
        }

        public ExcludeIPItem[] GetExcludeIPs()
        {
            return running.Data.Tunnel.ExcludeIPs;
        }
        public void SettExcludeIPs(ExcludeIPItem[] ips)
        {
            running.Data.Tunnel.ExcludeIPs = ips;
            running.Data.Update();
        }
        private void SettExcludeIPs(Memory<byte> data)
        {
            running.Data.Tunnel.ExcludeIPs = MemoryPackSerializer.Deserialize<ExcludeIPItem[]>(data.Span);
            running.Data.Update();
        }
    }
}
