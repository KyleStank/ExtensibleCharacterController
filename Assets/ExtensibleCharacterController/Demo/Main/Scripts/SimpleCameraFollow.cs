using UnityEngine;

namespace ExtensibleCharacterController.Demo
{
    public class SimpleCameraFollow : MonoBehaviour
    {
        [SerializeField]
        private Transform m_Target;

        private Vector3 m_Offset;

        private void Awake()
        {
            // m_Offset = m_Target.position - transform.position;
            m_Offset = transform.position - m_Target.position;
        }

        private void FixedUpdate()
        {
            transform.position = m_Offset + m_Target.position;
        }
    }
}
