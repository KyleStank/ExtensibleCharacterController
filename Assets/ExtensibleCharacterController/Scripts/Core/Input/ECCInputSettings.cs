using UnityEngine;
using UnityEngine.InputSystem;

namespace ExtensibleCharacterController.Core.Input
{
    [CreateAssetMenu(fileName = "Input Settings", menuName = "Extensible Character Controller/Input Settings")]
    public class ECCInputSettings : ScriptableObject
    {
        [SerializeField]
        private InputActionAsset m_InputActionAsset = null;
        public InputActionAsset InputActionAsset
        {
            get { return m_InputActionAsset; }
        }
    }
}
