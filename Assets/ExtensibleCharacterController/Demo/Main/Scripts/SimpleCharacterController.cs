using UnityEngine;

using ExtensibleCharacterController.Characters;

namespace ExtensibleCharacterController.Demo
{
    public class SimpleCharacterController : MonoBehaviour, IECCCharacterController
    {
        [SerializeField]
        private float m_Speed = 5.0f;

        private Vector2 m_Input = Vector2.zero;

        public Vector2 GetInput()
        {
            m_Input.x = Input.GetAxisRaw("Horizontal");
            m_Input.y = Input.GetAxisRaw("Vertical");
            return m_Input * m_Speed;
        }
    }
}
