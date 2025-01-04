﻿using linker.libs.api;
using linker.libs.extends;
using linker.plugins.sforward.messenger;
using System.Collections.Concurrent;
using linker.messenger.signin;
using linker.libs;
using linker.messenger.api;

namespace linker.messenger.sforward.client
{
    public sealed class SForwardApiController : IApiController
    {
        private readonly SForwardClientTransfer forwardTransfer;
        private readonly IMessengerSender messengerSender;
        private readonly SignInClientState signInClientState;
        private readonly ISignInClientStore signInClientStore;
        private readonly SForwardDecenter sForwardDecenter;
        private readonly ISForwardClientStore sForwardClientStore;
        private readonly ISerializer serializer;
        private readonly IAccessStore accessStore;

        public SForwardApiController(SForwardClientTransfer forwardTransfer, IMessengerSender messengerSender, SignInClientState signInClientState, ISignInClientStore signInClientStore, SForwardDecenter sForwardDecenter, ISForwardClientStore sForwardClientStore, ISerializer serializer, IAccessStore accessStore)
        {
            this.forwardTransfer = forwardTransfer;
            this.messengerSender = messengerSender;
            this.signInClientState = signInClientState;
            this.signInClientStore = signInClientStore;
            this.sForwardDecenter = sForwardDecenter;
            this.sForwardClientStore = sForwardClientStore;
            this.serializer = serializer;
            this.accessStore = accessStore;
        }

        /// <summary>
        /// 获取密钥
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public string GetSecretKey(ApiControllerParamsInfo param)
        {
            return sForwardClientStore.SecretKey;
        }
        /// <summary>
        /// 设置密钥
        /// </summary>
        /// <param name="param"></param>
        [Access(AccessValue.Config)]
        public void SetSecretKey(ApiControllerParamsInfo param)
        {
            sForwardClientStore.SetSecretKey(param.Content);
        }


        public void Refresh(ApiControllerParamsInfo param)
        {
            sForwardDecenter.Refresh();
        }
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public SForwardListInfo GetCount(ApiControllerParamsInfo param)
        {
            ulong hashCode = ulong.Parse(param.Content);
            if (sForwardDecenter.DataVersion.Eq(hashCode, out ulong version) == false)
            {
                return new SForwardListInfo
                {
                    List = sForwardDecenter.CountDic,
                    HashCode = version
                };
            }
            return new SForwardListInfo { HashCode = version };
        }

        /// <summary>
        /// 获取穿透列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<List<SForwardInfo>> Get(ApiControllerParamsInfo param)
        {
            if (param.Content == signInClientStore.Id)
            {
                if (accessStore.HasAccess(AccessValue.ForwardShowSelf) == false) return new List<SForwardInfo>();
                return sForwardClientStore.Get();
            }

            if (accessStore.HasAccess(AccessValue.ForwardShowOther) == false) return new List<SForwardInfo>();
            var resp = await messengerSender.SendReply(new MessageRequestWrap
            {
                Connection = signInClientState.Connection,
                MessengerId = (ushort)SForwardMessengerIds.GetForward,
                Payload = serializer.Serialize(param.Content)
            });
            if (resp.Code == MessageResponeCodes.OK)
            {
                return serializer.Deserialize<List<SForwardInfo>>(resp.Data.Span);
            }
            return new List<SForwardInfo>();
        }

        /// <summary>
        /// 添加穿透
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<bool> Add(ApiControllerParamsInfo param)
        {
            SForwardAddForwardInfo info = param.Content.DeJson<SForwardAddForwardInfo>();
            if (info.MachineId == signInClientStore.Id)
            {
                if (accessStore.HasAccess(AccessValue.ForwardSelf) == false) return false;
                return forwardTransfer.Add(info.Data);
            }
            if (accessStore.HasAccess(AccessValue.ForwardOther) == false) return false;
            return await messengerSender.SendOnly(new MessageRequestWrap
            {
                Connection = signInClientState.Connection,
                MessengerId = (ushort)SForwardMessengerIds.AddClientForward,
                Payload = serializer.Serialize(info)
            });
        }

        /// <summary>
        /// 删除穿透
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<bool> Remove(ApiControllerParamsInfo param)
        {
            SForwardRemoveForwardInfo info = param.Content.DeJson<SForwardRemoveForwardInfo>();
            if (info.MachineId == signInClientStore.Id)
            {
                if (accessStore.HasAccess(AccessValue.ForwardSelf) == false) return false;
                return forwardTransfer.Remove(info.Id);
            }
            if (accessStore.HasAccess(AccessValue.ForwardOther) == false) return false;
            return await messengerSender.SendOnly(new MessageRequestWrap
            {
                Connection = signInClientState.Connection,
                MessengerId = (ushort)SForwardMessengerIds.RemoveClientForward,
                Payload = serializer.Serialize(info)
            });
        }

        /// <summary>
        /// 测试服务
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<bool> TestLocal(ApiControllerParamsInfo param)
        {
            if (param.Content == signInClientStore.Id)
            {
                forwardTransfer.SubscribeTest();
                return true;
            }
            await messengerSender.SendOnly(new MessageRequestWrap
            {
                Connection = signInClientState.Connection,
                MessengerId = (ushort)SForwardMessengerIds.TestClientForward,
                Payload = serializer.Serialize(param.Content)
            });
            return true;
        }

    }

    public sealed class SForwardListInfo
    {
        public ConcurrentDictionary<string, int> List { get; set; }
        public ulong HashCode { get; set; }
    }
}
