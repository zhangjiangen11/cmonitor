﻿namespace linker.messenger.tuntap.lease
{
    public interface ILeaseServerStore
    {
        public List<LeaseCacheInfo> Get();
        public bool Add(LeaseCacheInfo info);
        public bool Update(LeaseCacheInfo info);
        public bool Remove(string id);
        public bool Confirm();
    }
}
