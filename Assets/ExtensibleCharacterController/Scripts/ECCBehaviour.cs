using UnityEngine;

using ExtensibleCharacterController.Core.Variables;

namespace ExtensibleCharacterController
{
    /// <summary>
    /// Abstract MonoBehaviour that all components derive from.
    /// </summary>
    public abstract class ECCBehaviour : MonoBehaviour
    {
        #if UNITY_EDITOR
        [SerializeField]
        private ECCBoolReference m_ShowBaseProps = false;
        [SerializeField]
        private ECCBoolReference m_ShowDerivedProps = false;
        #endif

        [SerializeField]
        private ECCBoolReference m_LoggingEnabled = true;
        public bool LoggingEnabled
        {
            get { return m_LoggingEnabled; }
            set { m_LoggingEnabled = value; }
        }

        protected virtual void Awake() => Initialize();

        protected abstract void Initialize();

        #region Logging Methods

        protected string FormatLogMessage(object message)
        {
            return "[ECC]: " + message;
        }

        public void Log(object message, Object context = null)
        {
            if (!m_LoggingEnabled) return;

            Debug.Log(FormatLogMessage(message), context == null ? this : context);
        }

        public void LogWarning(object message, Object context = null)
        {
            if (!m_LoggingEnabled) return;

            Debug.LogWarning(FormatLogMessage(message), context == null ? this : context);
        }

        public void LogError(object message, Object context = null)
        {
            if (!m_LoggingEnabled) return;

            Debug.LogError(FormatLogMessage(message), context == null ? this : context);
        }

        #endregion
    }
}
