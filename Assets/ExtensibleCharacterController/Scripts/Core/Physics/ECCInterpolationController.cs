// Source: https://www.kinematicsoup.com/news/2016/8/9/rrypp5tkubynjwxhxjzd42s3o034o8

using UnityEngine;

namespace ExtensibleCharacterController.Core.Physics
{
    [DefaultExecutionOrder(-100)]
    public class ECCInterpolationController : MonoBehaviour
    {
        private float[] m_LastFixedUpdateTimes;
        private int m_NewTimeIndex;

        private static float m_InterpolationFactor;
        public static float InterpolationFactor
        {
            get => m_InterpolationFactor;
        }

        public void Start()
        {
            m_LastFixedUpdateTimes = new float[2];
            m_NewTimeIndex = 0;
        }

        public void FixedUpdate()
        {
            m_NewTimeIndex = OldTimeIndex();
            m_LastFixedUpdateTimes[m_NewTimeIndex] = Time.fixedTime;
        }

        public void Update()
        {
            float newerTime = m_LastFixedUpdateTimes[m_NewTimeIndex];
            float olderTime = m_LastFixedUpdateTimes[OldTimeIndex()];

            if (newerTime != olderTime)
            {
                m_InterpolationFactor = (Time.time - newerTime) / (newerTime - olderTime);
            }
            else
            {
                m_InterpolationFactor = 1;
            }
        }

        private int OldTimeIndex()
        {
            return (m_NewTimeIndex == 0 ? 1 : 0);
        }
    }
}
