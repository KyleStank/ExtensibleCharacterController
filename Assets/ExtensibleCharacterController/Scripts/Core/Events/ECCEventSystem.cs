using System;
using System.Collections.Generic;

namespace ExtensibleCharacterController.Core.Events
{
    /// <summary>
    /// Simple event system that subscribes, unsubscribes, and raises events based on a string name and a list of event actions.
    /// </summary>
    public static class ECCEventSystem
    {
        private static Dictionary<string, List<ECCBaseEventAction>> m_EventsDict = new Dictionary<string, List<ECCBaseEventAction>>();

        #region Subscribe Methods

        /// <summary>
        /// Subscribes a base action to an event.
        /// </summary>
        /// <param name="name">Name of event.</param>
        /// <param name="action">Action to add.</param>
        private static void Subscribe(string name, ECCBaseEventAction action)
        {
            // Get or create list of actions and add provided action to list.
            List<ECCBaseEventAction> actions;
            if (!m_EventsDict.TryGetValue(name, out actions))
            {
                actions = new List<ECCBaseEventAction>();
                actions.Add(action);
                m_EventsDict.Add(name, actions);
            }
            else
            {
                if (!actions.Contains(action))
                    actions.Add(action);
            }
        }

        /// <summary>
        /// Subscribes an action to an event.
        /// </summary>
        /// <param name="name">Name of event.</param>
        /// <param name="action">Action to add.</param>
        public static void Subscribe(string name, Action action)
        {
            ECCEventAction eventAction = new ECCEventAction(action);
            Subscribe(name, eventAction);
        }

        /// <summary>
        /// Subscribes a one parameter action to an event.
        /// </summary>
        /// <param name="name">Name of event.</param>
        /// <param name="action">Action to add.</param>
        public static void Subscribe<T1>(string name, Action<T1> action)
        {
            ECCEventAction<T1> eventAction = new ECCEventAction<T1>(action);
            Subscribe(name, eventAction);
        }

        /// <summary>
        /// Subscribes a two parameter action to an event.
        /// </summary>
        /// <param name="name">Name of event.</param>
        /// <param name="action">Action to add.</param>
        public static void Subscribe<T1, T2>(string name, Action<T1, T2> action)
        {
            ECCEventAction<T1, T2> eventAction = new ECCEventAction<T1, T2>(action);
            Subscribe(name, eventAction);
        }

        /// <summary>
        /// Subscribes a three parameter action to an event.
        /// </summary>
        /// <param name="name">Name of event.</param>
        /// <param name="action">Action to add.</param>
        public static void Subscribe<T1, T2, T3>(string name, Action<T1, T2, T3> action)
        {
            ECCEventAction<T1, T2, T3> eventAction = new ECCEventAction<T1, T2, T3>(action);
            Subscribe(name, eventAction);
        }

        #endregion

        #region Unsubscribe Methods

        /// <summary>
        /// Unsubscribes an action from an event.
        /// </summary>
        /// <param name="name">Name of event.</param>
        /// <param name="action">Action to remove.</param>
        public static void Unsubscribe(string name, Action action)
        {
            // Get list of actions.
            List<ECCBaseEventAction> actions;
            if (!m_EventsDict.TryGetValue(name, out actions)) return;

            // Search for action to remove.
            for (int i = 0; i < actions.Count; i++)
            {
                ECCEventAction eventAction = actions[i] as ECCEventAction;
                if (eventAction.EqualsAction(action))
                {
                    actions.RemoveAt(i);
                    break;
                }
            }

            if (actions.Count == 0)
                m_EventsDict.Remove(name);
        }

        /// <summary>
        /// Unsubscribes a one parameter action from an event.
        /// </summary>
        /// <param name="name">Name of event.</param>
        /// <param name="action">Action to remove.</param>
        public static void Unsubscribe<T1>(string name, Action<T1> action)
        {
            // Get list of actions.
            List<ECCBaseEventAction> actions;
            if (!m_EventsDict.TryGetValue(name, out actions)) return;

            // Search for action to remove.
            for (int i = 0; i < actions.Count; i++)
            {
                ECCEventAction<T1> eventAction = actions[i] as ECCEventAction<T1>;
                if (eventAction.EqualsAction(action))
                {
                    actions.RemoveAt(i);
                    break;
                }
            }

            if (actions.Count == 0)
                m_EventsDict.Remove(name);
        }

        /// <summary>
        /// Unsubscribes a two parameter action from an event.
        /// </summary>
        /// <param name="name">Name of event.</param>
        /// <param name="action">Action to remove.</param>
        public static void Unsubscribe<T1, T2>(string name, Action<T1, T2> action)
        {
            // Get list of actions.
            List<ECCBaseEventAction> actions;
            if (!m_EventsDict.TryGetValue(name, out actions)) return;

            // Search for action to remove.
            for (int i = 0; i < actions.Count; i++)
            {
                ECCEventAction<T1, T2> eventAction = actions[i] as ECCEventAction<T1, T2>;
                if (eventAction.EqualsAction(action))
                {
                    actions.RemoveAt(i);
                    break;
                }
            }

            if (actions.Count == 0)
                m_EventsDict.Remove(name);
        }

        /// <summary>
        /// Unsubscribes a three parameter action from an event.
        /// </summary>
        /// <param name="name">Name of event.</param>
        /// <param name="action">Action to remove.</param>
        public static void Unsubscribe<T1, T2, T3>(string name, Action<T1, T2, T3> action)
        {
            // Get list of actions.
            List<ECCBaseEventAction> actions;
            if (!m_EventsDict.TryGetValue(name, out actions)) return;

            // Search for action to remove.
            for (int i = 0; i < actions.Count; i++)
            {
                ECCEventAction<T1, T2, T3> eventAction = actions[i] as ECCEventAction<T1, T2, T3>;
                if (eventAction.EqualsAction(action))
                {
                    actions.RemoveAt(i);
                    break;
                }
            }

            if (actions.Count == 0)
                m_EventsDict.Remove(name);
        }

        #endregion

        #region Raise Methods

        /// <summary>
        /// Invokes all actions associated with an event.
        /// </summary>
        /// <param name="name">Name of event.</param>
        public static void Raise(string name)
        {
            // Get list of actions.
            List<ECCBaseEventAction> actions;
            if (!m_EventsDict.TryGetValue(name, out actions)) return;

            for (int i = 0; i < actions.Count; i++)
                (actions[i] as ECCEventAction)?.InvokeAction();
        }

        /// <summary>
        /// Invokes all actions associated with an event and passes parameters to each action.
        /// </summary>
        /// <param name="name">Name of event.</param>
        /// <param name="param1">First parameter to pass.</param>
        public static void Raise<T1>(string name, T1 param1)
        {
            // Get list of actions.
            List<ECCBaseEventAction> actions;
            if (!m_EventsDict.TryGetValue(name, out actions)) return;

            for (int i = 0; i < actions.Count; i++)
                (actions[i] as ECCEventAction<T1>)?.InvokeAction(param1);
        }

        /// <summary>
        /// Invokes all actions associated with an event and passes parameters to each action.
        /// </summary>
        /// <param name="name">Name of event.</param>
        /// <param name="param1">First parameter to pass.</param>
        /// <param name="param2">Second parameter to pass.</param>
        public static void Raise<T1, T2>(string name, T1 param1, T2 param2)
        {
            // Get list of actions.
            List<ECCBaseEventAction> actions;
            if (!m_EventsDict.TryGetValue(name, out actions)) return;

            for (int i = 0; i < actions.Count; i++)
                (actions[i] as ECCEventAction<T1, T2>)?.InvokeAction(param1, param2);
        }

        /// <summary>
        /// Invokes all actions associated with an event and passes parameters to each action.
        /// </summary>
        /// <param name="name">Name of event.</param>
        /// <param name="param1">First parameter to pass.</param>
        /// <param name="param2">Second parameter to pass.</param>
        /// <param name="param3">Third parameter to pass.</param>
        public static void Raise<T1, T2, T3>(string name, T1 param1, T2 param2, T3 param3)
        {
            // Get list of actions.
            List<ECCBaseEventAction> actions;
            if (!m_EventsDict.TryGetValue(name, out actions)) return;

            for (int i = 0; i < actions.Count; i++)
                (actions[i] as ECCEventAction<T1, T2, T3>)?.InvokeAction(param1, param2, param3);
        }

        #endregion
    }
}
