﻿using cmonitor.api;
using cmonitor.client.ruleConfig;
using cmonitor.plugins.signIn.messenger;
using cmonitor.plugins.snatch.messenger;
using cmonitor.plugins.snatch.report;
using cmonitor.server;
using common.libs.extends;

namespace cmonitor.plugins.snatch
{
    public sealed class SnatchApiController : IApiController
    {
        private readonly MessengerSender messengerSender;
        private readonly RuleConfig ruleConfig;
        private readonly SignCaching signCaching;
        private readonly ISnatachCaching snatachCaching;

        public SnatchApiController(RuleConfig ruleConfig, MessengerSender messengerSender, SignCaching signCaching, ISnatachCaching snatachCaching)
        {
            this.ruleConfig = ruleConfig;
            this.messengerSender = messengerSender;
            this.signCaching = signCaching;
            this.snatachCaching = snatachCaching;
        }

        public string AddGroup(ApiControllerParamsInfo param)
        {
            return ruleConfig.AddSnatchGroup(param.Content.DeJson<UpdateSnatchGroupInfo>());
        }
        public string DeleteGroup(ApiControllerParamsInfo param)
        {
            return ruleConfig.DeleteSnatchGroup(param.Content.DeJson<DeleteSnatchGroupInfo>());
        }
        public string Add(ApiControllerParamsInfo param)
        {
            return ruleConfig.AddSnatch(param.Content.DeJson<AddSnatchItemInfo>());
        }
        public string Del(ApiControllerParamsInfo param)
        {
            return ruleConfig.DelSnatch(param.Content.DeJson<DeletedSnatchItemInfo>());
        }


        public AnswerGroupInfo[] GetQuestion(ApiControllerParamsInfo param)
        {
            if (snatachCaching.Get(param.Content, out SnatchQuestionCacheInfo info))
            {
                return info.Answers.GroupBy(c => c.Question).Select(c => new AnswerGroupInfo
                {
                    Question = c.Key,
                    Answers = c.ToArray()
                }).ToArray();
            }

            return Array.Empty<AnswerGroupInfo>();
        }
        public async Task<bool> AddQuestion(ApiControllerParamsInfo param)
        {
            SnatchQuestionCacheParamInfo info = param.Content.DeJson<SnatchQuestionCacheParamInfo>();
            if (snatachCaching.Add(info.Cache, info.Question) && snatachCaching.Get(info.Cache.UserName, out SnatchQuestionCacheInfo cache))
            {
                if (info.Question != null)
                {
                    byte[] bytes = info.Question.ToBytes();
                    for (int i = 0; i < info.Cache.MachineNames.Length; i++)
                    {
                        if (signCaching.Get(info.Cache.MachineNames[i], out SignCacheInfo signCache))
                        {
                            await messengerSender.SendOnly(new MessageRequestWrap
                            {
                                Connection = signCache.Connection,
                                MessengerId = (ushort)SnatchMessengerIds.AddQuestion,
                                Payload = bytes
                            });
                        }
                    }
                }
            }
            return true;
        }
        public SnatchItemInfo[] RandomQuestion(ApiControllerParamsInfo param)
        {
            return ruleConfig.SnatchRandom(int.Parse(param.Content));
        }
        public async Task<bool> UpdateQuestion(ApiControllerParamsInfo param)
        {
            UpdateQuestionCacheParamInfo info = param.Content.DeJson<UpdateQuestionCacheParamInfo>();
            foreach (UpdateQuestionCacheParamItemInfo item in info.Items)
            {
                SnatchAnswerInfo answer = null;
                bool conti = snatachCaching.Update(info.UserName, item.MachineName, item.Question) == false
                    || snatachCaching.Get(info.UserName, item.MachineName, out answer) == false
                    || answer == null || answer.Question == null;
                if (conti)
                {
                    continue;
                }
                byte[] bytes = answer.Question.ToBytes();
                if (signCaching.Get(answer.MachineName, out SignCacheInfo signCache))
                {
                    await messengerSender.SendOnly(new MessageRequestWrap
                    {
                        Connection = signCache.Connection,
                        MessengerId = (ushort)SnatchMessengerIds.AddQuestion,
                        Payload = bytes
                    });
                }
            }
            return true;
        }
        public async Task<bool> RemoveQuestion(ApiControllerParamsInfo param)
        {
            if (snatachCaching.Remove(param.Content, out SnatchQuestionCacheInfo info))
            {
                for (int i = 0; i < info.MachineNames.Length; i++)
                {
                    if (signCaching.Get(info.MachineNames[i], out SignCacheInfo cache))
                    {
                        await messengerSender.SendOnly(new MessageRequestWrap
                        {
                            Connection = cache.Connection,
                            MessengerId = (ushort)SnatchMessengerIds.RemoveQuestion
                        });
                    }
                }
            }
            return true;
        }


        public sealed class UpdateQuestionCacheParamInfo
        {
            public string UserName { get; set; }
            public UpdateQuestionCacheParamItemInfo[] Items { get; set; }
        }

        public sealed class UpdateQuestionCacheParamItemInfo
        {
            public string MachineName { get; set; }
            public SnatchQuestionInfo Question { get; set; }
        }

        public sealed class SnatchQuestionCacheParamInfo
        {
            public SnatchQuestionCacheInfo Cache { get; set; }
            public SnatchQuestionInfo Question { get; set; }
        }

        public sealed class AnswerGroupInfo
        {
            public SnatchQuestionInfo Question { get; set; }
            public SnatchAnswerInfo[] Answers { get; set; }
        }
    }

}