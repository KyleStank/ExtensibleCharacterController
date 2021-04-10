// Source: https://www.kinematicsoup.com/news/2016/8/9/rrypp5tkubynjwxhxjzd42s3o034o8

using UnityEngine;

namespace ExtensibleCharacterController.Core.Physics
{
    [DefaultExecutionOrder(-50)]
    [RequireComponent(typeof(ECCInterpolatedTransformUpdater))]
    public class ECCInterpolatedTransform : MonoBehaviour
    {
        private TransformData[] m_LastTransforms;
        private int m_NewTransformIndex;

        private void OnEnable()
        {
            ForgetPreviousTransforms();
        }

        public void ForgetPreviousTransforms()
        {
            m_LastTransforms = new TransformData[2];
            TransformData t = new TransformData(
                transform.localPosition,
                transform.localRotation,
                transform.localScale
            );

            m_LastTransforms[0] = t;
            m_LastTransforms[1] = t;
            m_NewTransformIndex = 0;
        }

        private void FixedUpdate()
        {
            TransformData newestTransform = m_LastTransforms[m_NewTransformIndex];
            transform.localPosition = newestTransform.position;
            transform.localRotation = newestTransform.rotation;
            transform.localScale = newestTransform.scale;
        }

        public void LateFixedUpdate()
        {
            m_NewTransformIndex = OldTransformIndex();
            m_LastTransforms[m_NewTransformIndex] = new TransformData(
                                                        transform.localPosition,
                                                        transform.localRotation,
                                                        transform.localScale);
        }

        private void Update()
        {
            TransformData newestTransform = m_LastTransforms[m_NewTransformIndex];
            TransformData olderTransform = m_LastTransforms[OldTransformIndex()];

            transform.localPosition = Vector3.Lerp(
                olderTransform.position,
                newestTransform.position,
                ECCInterpolationController.InterpolationFactor
            );
            transform.localRotation = Quaternion.Slerp(
                olderTransform.rotation,
                newestTransform.rotation,
                ECCInterpolationController.InterpolationFactor
            );
            transform.localScale = Vector3.Lerp(
                olderTransform.scale,
                newestTransform.scale,
                ECCInterpolationController.InterpolationFactor
            );
        }

        private int OldTransformIndex()
        {
            return (m_NewTransformIndex == 0 ? 1 : 0);
        }

        private struct TransformData
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;

            public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
            {
                this.position = position;
                this.rotation = rotation;
                this.scale = scale;
            }
        }
    }
}
