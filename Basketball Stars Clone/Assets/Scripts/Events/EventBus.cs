using System;
using System.Collections.Generic;

namespace Events
{
    public static class EventBus
    {
        private static Dictionary<Type, Delegate> actionsWithArgs = new();
        private static Dictionary<Type, Action> actionsNoArgs = new();
    
        public static void Subscribe<T>(Action<T> action) where T: EventData
        {

            Type type = typeof(T);
            
            if (actionsWithArgs.ContainsKey(type))
            {
                actionsWithArgs[type] = Delegate.Combine(actionsWithArgs[type], action);
            }
            else
            {
                actionsWithArgs[type] = action;
            }
        }
        
        public static void Subscribe<T>(Action action) where T : EventData
        {
            var type = typeof(T);
            if (actionsNoArgs.ContainsKey(type))
                actionsNoArgs[type] += action;
            else
                actionsNoArgs[type] = action;
        }
    
        public static void Unsubscribe<T>(Action<T> action) where T : EventData
        {
            Type type = typeof(T);
        
            if (actionsWithArgs.ContainsKey(type))
            {
                actionsWithArgs[type] = Delegate.Remove(actionsWithArgs[type], action);
            }
        }
        
        public static void Unsubscribe<T>(Action action) where T : EventData
        {
            var type = typeof(T);
            if (actionsNoArgs.ContainsKey(type))
            {
                actionsNoArgs[type] -= action;
                if (actionsNoArgs[type] == null)
                    actionsNoArgs.Remove(type);
            }
        }
    
        public static void Raise(EventData data)
        {
            var type = data.GetType();

            if (actionsWithArgs.TryGetValue(type, out var del))
                del?.DynamicInvoke(data);

            if (actionsNoArgs.TryGetValue(type, out var action))
                action?.Invoke();
        }

        public static void Clear()
        {
            actionsWithArgs.Clear();
            actionsNoArgs.Clear();
        }
    }
}