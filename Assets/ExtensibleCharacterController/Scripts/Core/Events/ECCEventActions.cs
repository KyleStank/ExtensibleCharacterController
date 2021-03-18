using System;

namespace ExtensibleCharacterController.Core.Events
{
    /// <summary>
    /// Event action with no parameters.
    /// </summary>
    public sealed class ECCEventAction : ECCBaseEventAction<Action>
    {
        public ECCEventAction(Action action) : base(action) {}

        public void InvokeAction()
        {
            m_Action?.Invoke();
        }
    }

    /// <summary>
    /// Event action with one parameter.
    /// </summary>
    /// <typeparam name="T1">Type of first parameter.</typeparam>
    public sealed class ECCEventAction<T1> : ECCBaseEventAction<Action<T1>>
    {
        public ECCEventAction(Action<T1> action) : base (action) {}

        public void InvokeAction(T1 param1)
        {
            m_Action?.Invoke(param1);
        }
    }

    /// <summary>
    /// Event action with two parameters.
    /// </summary>
    /// <typeparam name="T1">Type of first parameter.</typeparam>
    /// <typeparam name="T2">Type of second parameter.</typeparam>
    public sealed class ECCEventAction<T1, T2> : ECCBaseEventAction<Action<T1, T2>>
    {
        public ECCEventAction(Action<T1, T2> action) : base (action) {}

        public void InvokeAction(T1 param1, T2 param2)
        {
            m_Action?.Invoke(param1, param2);
        }
    }

    /// <summary>
    /// Event action with three parameters.
    /// </summary>
    /// <typeparam name="T1">Type of first parameter.</typeparam>
    /// <typeparam name="T2">Type of second parameter.</typeparam>
    /// <typeparam name="T3">Type of third parameter.</typeparam>
    public sealed class ECCEventAction<T1, T2, T3> : ECCBaseEventAction<Action<T1, T2, T3>>
    {
        public ECCEventAction(Action<T1, T2, T3> action) : base (action) {}

        public void InvokeAction(T1 param1, T2 param2, T3 param3)
        {
            m_Action.Invoke(param1, param2, param3);
        }
    }
}
