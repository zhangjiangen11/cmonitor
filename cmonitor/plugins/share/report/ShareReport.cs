﻿using cmonitor.client.report;
using cmonitor.config;
using cmonitor.libs;
using common.libs;
using Microsoft.Win32;

namespace cmonitor.plugins.share.report
{
    public sealed class ShareReport : IClientReport
    {
        public string Name => "Share";

        private readonly ShareMemory shareMemory;
        private long version = 0;

        Dictionary<string, ShareItemInfo> dic = new Dictionary<string, ShareItemInfo>();

        public ShareReport(Config config, ShareMemory shareMemory)
        {
#if DEBUG
#else
            //if (config.Data.Common.BlueProtect && OperatingSystem.IsWindows())
            {
                //ProcessBlueProtection.Protect();
                //SystemEvents.SessionEnding += SystemEvents_SessionEnding;
                //SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            }
#endif

            this.shareMemory = shareMemory;
            InitShare();
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (OperatingSystem.IsWindows())
            {
                if (e.Reason == SessionSwitchReason.SessionLogoff)
                {
                    ProcessBlueProtection.Unprotect();
                }
            }
        }

        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            if (OperatingSystem.IsWindows())
            {
                ProcessBlueProtection.Unprotect();
            }
        }

        public object GetReports(ReportType reportType)
        {
            bool updated = GetShare();
            if ((dic.Count > 0 && updated) || reportType == ReportType.Full)
            {
                return dic;
            }
            return null;
        }

        private void InitShare()
        {
            shareMemory.AddAttribute(0, ShareMemoryAttribute.Running);
            shareMemory.RemoveAttribute(0, ShareMemoryAttribute.Closed);
            shareMemory.AddAttributeAction(0, ShareMemoryStateChanged);
        }
        private bool GetShare()
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    if (shareMemory != null)
                    {
                        dic = shareMemory.ReadItems(out long _version);
                        bool res = _version > version;
                        version = _version;
                        return res;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        private void ShareMemoryStateChanged(ShareMemoryAttribute state)
        {
            if ((state & ShareMemoryAttribute.Closed) == ShareMemoryAttribute.Closed)
            {
                shareMemory.RemoveAttribute(0, ShareMemoryAttribute.Closed);
                shareMemory.RemoveAttribute(0, ShareMemoryAttribute.Running);
                if (OperatingSystem.IsWindows())
                {
                    //ProcessBlueProtection.Unprotect();
                }
                Environment.Exit(0);
            }
        }

    }


}
