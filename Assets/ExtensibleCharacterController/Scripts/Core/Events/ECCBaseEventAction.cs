namespace ExtensibleCharacterController.Core.Events
{
    /// <summary>
    /// Abstract class used by the event system to invoke an event action with any number of generic types.
    /// </summary>
    public abstract class ECCBaseEventAction {}

    /// <summary>
    /// Abstract class used to define a custom System.Action.
    /// </summary>
    /// <typeparam name="T">Type that should be a System.Action with any number of parameters.</typeparam>
    public abstract class ECCBaseEventAction<T> : ECCBaseEventAction
    {
        protected T m_Action;

        public ECCBaseEventAction(T action)
        {
            m_Action = action;
        }

        /// <summary>
        /// Checks if the event action's System.Action matches the provided System.Action.
        /// </summary>
        /// <param name="action">System.Action to check.</param>
        public bool EqualsAction(T action)
        {
            return m_Action.Equals(action);
        }
    }
}
