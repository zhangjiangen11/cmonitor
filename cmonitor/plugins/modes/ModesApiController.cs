﻿using cmonitor.plugins.sapi;
using cmonitor.server.ruleConfig;
using common.libs.api;
using common.libs.extends;

namespace cmonitor.plugins.modes
{
    public sealed class ModesApiController : IApiServerController
    {

        private readonly IRuleConfig ruleConfig;
        public ModesApiController(IRuleConfig ruleConfig)
        {
            this.ruleConfig = ruleConfig;
        }

        public string Update(ApiControllerParamsInfo param)
        {
            UpdateModesInfo info = param.Content.DeJson<UpdateModesInfo>();
            ruleConfig.Set(info.UserName, "Modes", info.Data);
            return string.Empty;
        }

        public sealed class ModesInfo
        {
            public string Name { get; set; }
            public string Data { get; set; }
        }

        public sealed class UpdateModesInfo
        {
            public string UserName { get; set; }
            public List<ModesInfo> Data { get; set; } = new List<ModesInfo>();
        }
    }
}
