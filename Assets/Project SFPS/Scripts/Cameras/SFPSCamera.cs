using UnityEngine;

using ProjectSFPS.Core.Variables;

namespace ProjectSFPS.Cameras
{
    public class SFPSCamera : SFPSBaseCamera
    {
        [Header("Camera Settings")]
        [SerializeField]
        private SFPSVector3Reference m_Offset = Vector3.zero;
        [SerializeField]
        private SFPSVector2Reference m_Sensitivity = new Vector2(5.0f, 5.0f);

        [SerializeField]
        private SFPSFloatReference m_TopClamp = -65.0f;
        [SerializeField]
        private SFPSFloatReference m_BottomClamp = 65.0f;
        [SerializeField]
        private SFPSBoolReference m_EnableSmoothing = false;
        [SerializeField]
        private SFPSFloatReference m_RotationSpeed = 10.0f;

        public override void UpdatePosition(float xAxis, float yAxis, float zAxis)
        {
            if (m_Target == null)
            {
                LogError("Cannot move SFPSCamera because the target is null");
                return;
            }

            transform.position = m_Target.position + m_Target.TransformDirection(m_Offset);
        }

        public override void UpdateRotation(float xEuler, float yEuler, float zEuler)
        {
            Quaternion rot = m_NextRotation;
            Vector3 eulerAngles = rot.eulerAngles;

            float xRot = eulerAngles.x - xEuler * m_Sensitivity.Value.y;
            float yRot = eulerAngles.y + yEuler * m_Sensitivity.Value.x;
            rot = Quaternion.Euler(
                new Vector3(
                    Mathf.Clamp(
                        xRot > 180.0f ? xRot - 360.0f : xRot,
                        m_TopClamp,
                        m_BottomClamp
                    ),
                    yRot,
                    m_NextRotation.z
                )
            );

            m_NextRotation = rot;
        }

        public override void ApplyRotation()
        {
            transform.rotation = m_EnableSmoothing ?
                Quaternion.Lerp(transform.rotation, m_NextRotation, Time.deltaTime * m_RotationSpeed) :
                m_NextRotation;
        }
    }
}
