// Source: https://www.kinematicsoup.com/news/2016/8/9/rrypp5tkubynjwxhxjzd42s3o034o8

using UnityEngine;

namespace ExtensibleCharacterController.Core.Physics
{
    [DefaultExecutionOrder(100)]
    public class ECCInterpolatedTransformUpdater : MonoBehaviour
    {
        private ECCInterpolatedTransform m_InterpolatedTransform;

        private void Awake()
        {
            m_InterpolatedTransform = GetComponent<ECCInterpolatedTransform>();
        }

        private void FixedUpdate()
        {
            m_InterpolatedTransform.LateFixedUpdate();
        }
    }
}
