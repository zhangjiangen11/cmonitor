﻿namespace linker.messenger.signin
{
    public interface ISignInClientStore
    {
        /// <summary>
        /// 信标服务器
        /// </summary>
        public SignInClientServerInfo Server { get; }
        /// <summary>
        /// 分组
        /// </summary>
        public SignInClientGroupInfo Group { get; }
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 设置名称
        /// </summary>
        /// <param name="newName"></param>
        public void SetName(string newName);
        /// <summary>
        /// 设置分组，第一个生效
        /// </summary>
        /// <param name="groups"></param>
        public void SetGroups(SignInClientGroupInfo[] groups);
        /// <summary>
        /// 设置生效分组的密码
        /// </summary>
        /// <param name="password"></param>
        public void SetGroupPassword(string password);
        /// <summary>
        /// 设置信标服务器
        /// </summary>
        /// <param name="servers"></param>
        public void SetServer(SignInClientServerInfo servers);
        /// <summary>
        /// 设置信标密钥
        /// </summary>
        /// <param name="secretKey"></param>
        public void SetSecretKey(string secretKey);
        /// <summary>
        /// 设置id
        /// </summary>
        /// <param name="id"></param>
        public void SetId(string id);

        public bool Confirm();
    }

}