using System;
using System.Collections.Generic;

namespace JKFrame
{
    public class EventModule
    {
        private static ObjectPoolModule objectPoolModule = new ObjectPoolModule();

        private Dictionary<string, IEventInfo> eventInfoDic = new Dictionary<string, IEventInfo>();
        #region �ڲ��ӿڡ��ڲ���

        private interface IEventInfo { void Destory(); }

        /// <summary>
        /// �޲�-�¼���Ϣ
        /// </summary>
        private class EventInfo : IEventInfo
        {
            public Action action;
            public void Init(Action action) { this.action = action; }
            public void Destory()
            {
                action =null;
                objectPoolModule.PushObject(this);
            }
        }

        /// <summary>
        /// ���Action�¼���Ϣ
        /// </summary>
        private class MultipleParameterEventInfo<TAction>:IEventInfo where TAction : MulticastDelegate
        {
            public TAction action;
            public void Init(TAction action) { this.action = action; }
            public void Destory()
            {
                action = null;
                objectPoolModule.PushObject(this);
            }
        };
        #endregion
        #region ����¼��ļ���������Ҫ����ĳ���¼���������¼���ʱ����ִ���㴫�ݹ�����Action
        /// <summary>
        /// ����޲��¼�
        /// </summary>
        public void AddEventListener(string eventName, Action action) 
        {
            // ��û�ж�Ӧ���¼����Լ���
            if (eventInfoDic.ContainsKey(eventName))
            {
                (eventInfoDic[eventName] as EventInfo).action += action;
            }
            // û�еĻ�����Ҫ���� ���ֵ��У�����Ӷ�Ӧ��Action
            else
            {
                EventInfo eventInfo = objectPoolModule.GetObject<EventInfo>();
                if (eventInfo == null) eventInfo = new EventInfo();
                eventInfo.Init(action);
                eventInfoDic.Add(eventName, eventInfo);
            }
        }


        // <summary>
        // ���1���¼�����
        // </summary>
        public void AddEventListener<TAction>(string eventName, TAction action) where TAction : MulticastDelegate
        {
            // ��û�ж�Ӧ���¼����Լ���
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo))
            {
                MultipleParameterEventInfo<TAction> info = (MultipleParameterEventInfo<TAction>)eventInfo;
                info.action = (TAction)Delegate.Combine(info.action, action);
            }
            else AddMultipleParameterEventInfo(eventName, action);
        }

        private void AddMultipleParameterEventInfo<TAction>(string eventName, TAction action) where TAction : MulticastDelegate
        {
            MultipleParameterEventInfo<TAction> newEventInfo = objectPoolModule.GetObject<MultipleParameterEventInfo<TAction>>();
            if (newEventInfo == null) newEventInfo = new MultipleParameterEventInfo<TAction>();
            newEventInfo.Init(action);
            eventInfoDic.Add(eventName, newEventInfo);
        }
        #endregion

        #region �����޷���ֵ�¼���֮������ô�ຯ�����Ǳ���ʹ��params��������GC��װ������
        /// <summary>
        /// �����޲ε��¼�
        /// </summary>
        public void EventTrigger(string eventName)
        {
            if (eventInfoDic.ContainsKey(eventName))
            {
                ((EventInfo)eventInfoDic[eventName]).action?.Invoke();
            }
        }
        /// <summary>
        /// ����1���������¼�
        /// </summary>
        public void EventTrigger<T>(string eventName, T arg)
        {
            if (eventInfoDic.TryGetValue(eventName,out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T>>)eventInfo).action?.Invoke(arg);
        }
        /// <summary>
        /// ����2���������¼�
        /// </summary>
        public void EventTrigger<T0,T1>(string eventName, T0 arg0,T1 arg1)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1>>)eventInfo).action?.Invoke(arg0, arg1);
        }
        /// <summary>
        /// ����3���������¼�
        /// </summary>
        public void EventTrigger<T0, T1,T2>(string eventName, T0 arg0, T1 arg1,T2 arg2)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1,T2>>)eventInfo).action?.Invoke(arg0, arg1,arg2);
        }
        /// <summary>
        /// ����4���������¼�
        /// </summary>
        public void EventTrigger<T0, T1, T2,T3>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2,T3>>)eventInfo).action?.Invoke(arg0, arg1, arg2,arg3);
        }
        /// <summary>
        /// ����5���������¼�
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3,T4>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3,T4 arg4)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3,T4>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3,arg4);
        }
        /// <summary>
        /// ����6���������¼�
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4,T5>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4,T5 arg5)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4,arg5);
        }
        /// <summary>
        /// ����7���������¼�
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5,T6>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,T6 arg6)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5,T6>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5,arg6);
        }
        /// <summary>
        /// ����8���������¼�
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6,T7>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6,arg7);
        }
        /// <summary>
        /// ����9���������¼�
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7,T8>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7,T8 arg8)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7,T8>>)eventInfo).action.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7,arg8);
        }
        /// <summary>
        /// ����10���������¼�
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8,T9>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8,T9 arg9)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8,T9>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8,arg9);
        }

        /// <summary>
        /// ����11���������¼�
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9,T10>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9,T10 arg10)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9,T10>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9,arg10);
        }

        /// <summary>
        /// ����12���������¼�
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10,T11 arg11)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10,arg11);
        }

        /// <summary>
        /// ����13���������¼�
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11,T12 arg12)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        /// <summary>
        /// ����14���������¼�
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12,T13>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12 ,T13 arg13)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12,T13>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12,arg13);
        }

        /// <summary>
        /// ����15���������¼�
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,T14>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13,T14 arg14)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,T14>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13,arg14);
        }

        /// <summary>
        /// ����16���������¼�
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14,T15>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14,T15 arg15)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14,T15>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14,arg15);
        }

        #endregion

        #region ȡ���¼��ļ���
        /// <summary>
        /// �Ƴ��޲ε��¼�����
        /// </summary>
        public void RemoveEventListener(string eventName, Action action)
        {
            if (eventInfoDic.TryGetValue(eventName,out IEventInfo eventInfo))
            {
                ((EventInfo)eventInfo).action -= action;
            }
        }
        /// <summary>
        /// �Ƴ��в������¼�����
        /// </summary>
        public void RemoveEventListener<TAction>(string eventName, TAction action) where TAction:MulticastDelegate
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo))
            {
                MultipleParameterEventInfo<TAction> info = (MultipleParameterEventInfo<TAction>)eventInfo;
                info.action = (TAction)Delegate.Remove(info.action,action);
            }
        }
        #endregion

        #region �Ƴ��¼�
        /// <summary>
        /// �Ƴ�/ɾ��һ���¼�
        /// </summary>
        public void RemoveEvent(string eventName)
        {
            if (eventInfoDic.Remove(eventName,out IEventInfo eventInfo))
            {
                eventInfo.Destory();
            }
        }

        /// <summary>
        /// ����¼�����
        /// </summary>
        public void Clear()
        {
            foreach (string eventName in eventInfoDic.Keys)
            {
                eventInfoDic[eventName].Destory();
            }
            eventInfoDic.Clear();
        }

        #endregion
    }
}
