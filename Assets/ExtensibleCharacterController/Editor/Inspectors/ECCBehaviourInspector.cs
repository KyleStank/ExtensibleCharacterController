using UnityEngine;

using UnityEditor;
using UnityEditor.AnimatedValues;

namespace ExtensibleCharacterController.Editor.Inspectors
{
    [CustomEditor(typeof(ECCBehaviour), true)]
    public sealed class ECCBehaviourInspector : ECCBaseInspector<ECCBehaviour>
    {
        private const string PROPNAME_LOGGINGENABLED = "m_LoggingEnabled";
        private const string PROPNAME_SHOWBASEPROPS = "m_ShowBaseProps";
        private const string PROPNAME_SHOWDERIVEDPROPS = "m_ShowDerivedProps";

        private SerializedProperty m_PropLoggingEnabled;
        private SerializedProperty m_PropShowBaseProps;
        private SerializedProperty m_PropShowDerivedProps;

        private bool m_HasDerivedProperties = false;

        private AnimBool m_BasePropsAnim;
        private AnimBool m_DerivedPropsAnim;

        private string m_TypeName = "Component";

        protected override void Initialize()
        {
            // Add to excluded properties.
            ExcludedDefaultProps.Add(PROPNAME_LOGGINGENABLED);
            ExcludedDefaultProps.Add(PROPNAME_SHOWBASEPROPS);
            ExcludedDefaultProps.Add(PROPNAME_SHOWDERIVEDPROPS);

            // Grab serialized properties.
            m_PropLoggingEnabled = serializedObject.FindProperty(PROPNAME_LOGGINGENABLED);
            m_PropShowBaseProps = serializedObject.FindProperty(PROPNAME_SHOWBASEPROPS);
            m_PropShowDerivedProps = serializedObject.FindProperty(PROPNAME_SHOWDERIVEDPROPS);

            // Check if there are any derived propertfdies.
            // NOTE: Always use the last serialized property in the order they are declared.
            m_HasDerivedProperties = m_PropShowDerivedProps.Copy().CountRemaining() > 0;

            // Setup animation values.
            m_BasePropsAnim = new AnimBool(m_PropShowBaseProps.isExpanded, Repaint);
            m_DerivedPropsAnim = new AnimBool(m_PropShowDerivedProps.isExpanded, Repaint);

            m_TypeName = Target.GetType().Name;
        }

        private void OnDestroy()
        {
            // Clean up event listeners.
            m_BasePropsAnim.valueChanged.RemoveAllListeners();
            m_DerivedPropsAnim.valueChanged.RemoveAllListeners();
        }

        protected override void DrawInspector()
        {
            DrawBaseProperties();
            if (m_HasDerivedProperties)
            {
                EditorGUILayout.Space();
                DrawDerivedProperties();
            }
        }

        /// <summary>
        /// Draws the base properties.
        /// </summary>
        private void DrawBaseProperties()
        {
            if (GUILayout.Button("Base Settings"))
            {
                m_PropShowBaseProps.isExpanded = !m_PropShowBaseProps.isExpanded;
                m_BasePropsAnim.target = m_PropShowBaseProps.isExpanded;
            }

            // Fade foldout content in/out.
            if (EditorGUILayout.BeginFadeGroup(m_BasePropsAnim.faded))
            {
                // Logging property.
                EditorGUILayout.PropertyField(m_PropLoggingEnabled);
            }

            EditorGUILayout.EndFadeGroup();
        }

        /// <summary>
        /// Draws the derived properties.
        /// </summary>
        private void DrawDerivedProperties()
        {
            if (GUILayout.Button(m_TypeName + " Settings"))
            {
                m_PropShowDerivedProps.isExpanded = !m_PropShowDerivedProps.isExpanded;
                m_DerivedPropsAnim.target = m_PropShowDerivedProps.isExpanded;
            }

            // Fade foldout content in/out.
            if (EditorGUILayout.BeginFadeGroup(m_DerivedPropsAnim.faded))
            {
                DrawDefaultProperties();
            }

            EditorGUILayout.EndFadeGroup();
        }
    }
}
