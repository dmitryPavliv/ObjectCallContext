
using System;
using System.Collections.Concurrent;
using System.Runtime.Remoting.Messaging;

namespace ObjectCallContext
{
    public static class ObjectCallContext
    {
        private static readonly ConcurrentDictionary<string, object> _dataTable =
            new ConcurrentDictionary<string, object>();

        public static bool TrySetData<TData>(string key, TData data)
            where TData : class
        {
            var realKey = Guid.NewGuid().ToString();
            CallContext.LogicalSetData(key, realKey);

            return _dataTable.TryAdd(realKey, data);
        }

        public static bool TryGetData<TData>(string key, out TData @out)
            where TData: class
        {
            @out = null;

            var realKey = CallContext.LogicalGetData(key) as string;

            if (realKey != null)
            {
                object data;
                if (_dataTable.TryGetValue(realKey, out data))
                {
                    @out = data as TData;
                }
            }
            return @out != null;
        }

        public static bool TryRemove<TData>(string key, out TData @out)
            where TData : class
        {
            @out = null;

            var realKey = CallContext.LogicalGetData(key) as string;
            CallContext.FreeNamedDataSlot(key);

            if (realKey != null)
            {
                object data;
                if (_dataTable.TryRemove(realKey, out data))
                {
                    @out = data as TData;
                }
            }
            return @out != null;
        }

        public static void ClearAll()
        {
            _dataTable.Clear();
        }
    }
}