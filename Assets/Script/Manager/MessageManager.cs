using System;
using System.Collections.Generic;


    public class MessageManager
    {
        static MessageManager mInstance;
        public static MessageManager Instance { get { return mInstance ?? (mInstance = new MessageManager()); } }

        Dictionary<string, Action<object[]>> mMessageDict = new Dictionary<string, Action<object[]>>(32);
        //分发消息缓存字典，主要应对消息还没注册但Dispatch已经调用的情况.
        Dictionary<string, object[]> mDispatchCacheDict = new Dictionary<string, object[]>(16);


        private MessageManager() { }

        //订阅消息
        public void Subscribe(string message, Action<object[]> action)
        {
            Action<object[]> value = null;
            if (mMessageDict.TryGetValue(message, out value))
            {
                value += action;
                mMessageDict[message] = value;
            }
            else
            {
                mMessageDict.Add(message, action);
            }
        }

        //取消订阅消息
        public void Unsubscribe(string message)
        {
            mMessageDict.Remove(message);
        }

        //分发消息
        public void Dispatch(string message, object[] args = null, bool addToCache = false)
        {
            if (addToCache)
            {
                mDispatchCacheDict[message] = args;
            }
            else
            {
                Action<object[]> value = null;
                if (mMessageDict.TryGetValue(message, out value))
                    value(args);
            }
        }

        //处理分发消息缓存，isAutoRemove参数表示是否将消息从缓存中自动移除
        public void PullMessageCache(string message, bool isAutoRemove = true)
        {
            object[] value = null;
            if (mDispatchCacheDict.TryGetValue(message, out value))
            {
                Dispatch(message, value);

                if (isAutoRemove)
                    mDispatchCacheDict.Remove(message);
            }
        }

        //手动移除分发缓存里的消息
        public bool RemoveFromMessageCache(string message)
        {
            return mDispatchCacheDict.Remove(message);
        }
    }

