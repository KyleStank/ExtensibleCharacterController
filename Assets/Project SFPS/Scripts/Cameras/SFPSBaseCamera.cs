using UnityEngine;

namespace ProjectSFPS.Cameras
{
    [RequireComponent(typeof(Camera))]
    public abstract class SFPSBaseCamera : SFPSBehaviour
    {
        protected Camera m_Camera = null;
        public Camera Camera
        {
            get => m_Camera;
        }

        protected Transform m_Target = null;
        public Transform Target
        {
            get => m_Target;
        }

        protected Quaternion m_OriginalRotation = Quaternion.identity;
        protected Quaternion m_NextRotation = Quaternion.identity;

        protected override void Initialize()
        {
            Log("Initialize Camera");

            m_Camera = GetComponent<Camera>();

            m_OriginalRotation = transform.rotation;
            m_NextRotation = m_OriginalRotation;
        }

        public void SetTarget(Transform target)
        {
            m_Target = target;
        }

        public abstract void UpdatePosition(float xAxis, float yAxis, float zAxis);
        public abstract void ApplyRotation();

        public abstract void UpdateRotation(float xEuler, float yEuler, float zEuler);
    }
}
