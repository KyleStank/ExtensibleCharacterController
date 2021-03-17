using UnityEngine;
using UnityEngine.InputSystem;

using ProjectSFPS.Cameras;
using ProjectSFPS.Core.Input;
using ProjectSFPS.Core.Variables;

namespace ProjectSFPS.Controllers
{
    [RequireComponent(typeof(SFPSUserInput))]
    public class SFPSCameraController : SFPSBehaviour
    {
        [Header("References")]
        [SerializeField]
        private SFPSBaseCamera m_ActiveCamera = null;
        [SerializeField]
        private SFPSCharacterController m_CharacterTarget = null;
        [SerializeField]
        private SFPSStringReference m_CharacterTag = "Player";

        [Header("Input Actions")]
        [SerializeField]
        private SFPSStringReference m_LookAction = "Look";

        private SFPSUserInput m_UserInput = null;
        private InputAction m_LookInputAction = null;

        private Vector2 m_CurrentInput = Vector2.zero;

        protected override void Initialize()
        {
            Log("Initialize Camera Controller");

            m_UserInput = GetComponent<SFPSUserInput>();

            // Find camera reference.
            if (m_ActiveCamera == null)
            {
                SFPSBaseCamera cam = GetComponentInChildren<SFPSBaseCamera>();
                if (cam != null)
                    m_ActiveCamera = cam;
                else
                    LogError("Could not find [SFPSBaseCamera] in children. [" + name + "] will not function properly.");
            }

            // Find character reference.
            if (m_CharacterTarget == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag(m_CharacterTag);
                if (go != null)
                {
                    m_CharacterTarget = go.GetComponent<SFPSCharacterController>();
                    if (m_CharacterTarget == null)
                        LogError("Could not find component [Character] on GameObject [" + go.name + "]");
                }
                else
                {
                    LogError("Could not find GameObject with tag [" + m_CharacterTag + "]");
                }
            }

            // Camera or character could still be null, so we check again.
            if (m_ActiveCamera != null && m_CharacterTarget != null)
            {
                m_ActiveCamera.SetTarget(m_CharacterTarget.transform);
                m_CharacterTarget.SetCamera(m_ActiveCamera);
            }
        }

        private void Start()
        {
            // Look input action.
            m_LookInputAction = m_UserInput.GetAction(m_LookAction);
            if (m_LookInputAction == null)
                LogError("InputAction [" + m_LookAction + "] not found. User input will be ignored on [" + name + "].");
        }

        private void Update()
        {
            if (m_LookInputAction == null) return;

            m_CurrentInput = m_LookInputAction.ReadValue<Vector2>();
        }

        private void LateUpdate()
        {
            if (m_ActiveCamera == null)
            {
                LogError("Unable to process SFPSCameraController because no camera is assigned");
                return;
            }

            // Update position and rotation.
            m_ActiveCamera.UpdatePosition(m_CurrentInput.x, m_CurrentInput.y, 0.0f);

            // m_ActiveCamera.Rotate(m_CharacterTarget.transform.eulerAngles.y, m_CurrentInput.y);
            m_ActiveCamera.UpdateRotation(m_CurrentInput.y, m_CurrentInput.x, 0.0f);
            m_ActiveCamera.ApplyRotation();
        }
    }
}
