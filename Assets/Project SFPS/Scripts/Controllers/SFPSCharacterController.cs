using UnityEngine;
using UnityEngine.InputSystem;

using ProjectSFPS.Cameras;
using ProjectSFPS.Characters;
using ProjectSFPS.Core.Input;
using ProjectSFPS.Core.Variables;

namespace ProjectSFPS.Controllers
{
    [RequireComponent(typeof(SFPSUserInput))]
    public class SFPSCharacterController : SFPSBehaviour
    {
        [Header("Input Actions")]
        [SerializeField]
        private SFPSStringReference m_MoveAction = "Move";

        private SFPSBaseCamera m_Camera = null;
        private SFPSUserInput m_UserInput = null;
        private SFPSCharacterMotor m_CharacterMotor = null;

        private InputAction m_MoveInputAction = null;

        private Vector2 m_CurrentMoveInput = Vector2.zero;

        protected override void Initialize()
        {
            Log("Initialize Character");

            m_UserInput = GetComponent<SFPSUserInput>();
            m_CharacterMotor = GetComponent<SFPSCharacterMotor>();

            if (m_CharacterMotor == null)
                LogWarning("Character movement and rotation will not be processed because no CharacterMotor is attached");
        }

        private void Start()
        {
            // Move input action.
            m_MoveInputAction = m_UserInput.GetAction(m_MoveAction);
            if (m_MoveInputAction == null)
                LogError("InputAction [" + m_MoveAction + "] not found. Movement input will be ignored on [" + name + "].");
        }

        private void ReadInput()
        {
            if (m_MoveInputAction != null)
                m_CurrentMoveInput = m_MoveInputAction.ReadValue<Vector2>();
        }

        private void Update()
        {
            ReadInput();

            if (m_CharacterMotor != null)
                m_CharacterMotor.Rotate(m_Camera.transform.rotation.eulerAngles.y, 0.0f);
        }

        private void FixedUpdate()
        {
            if (m_CharacterMotor != null)
                m_CharacterMotor.Move(m_CurrentMoveInput.x, m_CurrentMoveInput.y);
        }

        public void SetCamera(SFPSBaseCamera camera)
        {
            m_Camera = camera;
        }
    }
}
