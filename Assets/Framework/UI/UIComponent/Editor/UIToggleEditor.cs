using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CanEditMultipleObjects, CustomEditor(typeof(UIToggle), true)]
	public class UIToggleEditor : Editor
	{
		private SerializedProperty m_TargetContentProperty;
        private SerializedProperty m_TargetPrefabProperty;
        private SerializedProperty m_ContentParentProperty;
		private SerializedProperty m_ImageTargetProperty;
		private SerializedProperty m_ImageTransitionProperty;
		private SerializedProperty m_ImageColorsProperty;
		private SerializedProperty m_ImageSpriteStateProperty;
		private SerializedProperty m_TextTargetProperty;
		private SerializedProperty m_TextTransitionProperty;
		private SerializedProperty m_TextColorsProperty;
		private SerializedProperty m_OnValueChangedProperty;
		private SerializedProperty m_TransitionProperty;
		private SerializedProperty m_GraphicProperty;
		private SerializedProperty m_GroupProperty;
		private SerializedProperty m_IsOnProperty;
		private SerializedProperty m_NavigationProperty;
        private SerializedProperty m_TargetContent;
        private SerializedProperty m_ActivateProperty;
        private SerializedProperty m_DeactivateProperty;
        private SerializedProperty m_IsTabSpriteNativeSize;


        protected virtual void OnEnable()
		{
			this.m_TargetContentProperty = this.serializedObject.FindProperty("m_TargetContent");
            this.m_TargetPrefabProperty = this.serializedObject.FindProperty("m_targetPrefab");
            this.m_ContentParentProperty = this.serializedObject.FindProperty("m_contentParent");
			this.m_ImageTargetProperty = this.serializedObject.FindProperty("m_ImageTarget");
			this.m_ImageTransitionProperty = this.serializedObject.FindProperty("m_ImageTransition");
			this.m_ImageColorsProperty = this.serializedObject.FindProperty("m_ImageColors");
			this.m_ImageSpriteStateProperty = this.serializedObject.FindProperty("m_ImageSpriteState");
			this.m_TextTargetProperty = this.serializedObject.FindProperty("m_TextTarget");
			this.m_TextTransitionProperty = this.serializedObject.FindProperty("m_TextTransition");
			this.m_TextColorsProperty = this.serializedObject.FindProperty("m_TextColors");
			this.m_TransitionProperty = base.serializedObject.FindProperty("toggleTransition");
			this.m_GraphicProperty = base.serializedObject.FindProperty("graphic");
			this.m_GroupProperty = base.serializedObject.FindProperty("m_Group");
			this.m_IsOnProperty = base.serializedObject.FindProperty("m_IsOn");
			this.m_OnValueChangedProperty = base.serializedObject.FindProperty("onValueChanged");
			this.m_NavigationProperty = base.serializedObject.FindProperty("m_Navigation");
            this.m_ActivateProperty = base.serializedObject.FindProperty("m_Activate");
            this.m_DeactivateProperty = base.serializedObject.FindProperty("m_Deactivate");
            this.m_IsTabSpriteNativeSize = base.serializedObject.FindProperty("m_IsTabSpriteNativeSize");
        }
		
		public override void OnInspectorGUI()
		{

			this.serializedObject.Update();
			EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.m_IsTabSpriteNativeSize, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_TargetContentProperty, new GUIContent("Target Content"));
            EditorGUILayout.PropertyField(this.m_TargetPrefabProperty, new GUIContent("Target Prefab"));
            EditorGUILayout.PropertyField(this.m_ContentParentProperty, new GUIContent("Content Parent"));
			EditorGUILayout.Space();
			this.DrawTargetImageProperties();
			EditorGUILayout.Space();
			this.DrawTargetTextProperties();
            EditorGUILayout.Space();
            this.DrawObjectProperties();
            this.serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space();

            EditorGUILayout.Space();
			this.DrawToggleProperties();

            //base.OnInspectorGUI();
            //DrawDefaultInspector();
		}
		
		private void DrawTargetImageProperties()
		{
			EditorGUILayout.LabelField("Image Target Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			
			EditorGUILayout.PropertyField(this.m_ImageTargetProperty);
			
			// Check if image is set
			if (this.m_ImageTargetProperty.objectReferenceValue != null)
			{
				Image image = (this.m_ImageTargetProperty.objectReferenceValue as Image);
				
				EditorGUILayout.PropertyField(this.m_ImageTransitionProperty);
				
				// Get the selected transition
				Selectable.Transition transition = (Selectable.Transition)this.m_ImageTransitionProperty.enumValueIndex;
				
				if (transition != Selectable.Transition.None)
				{
					EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
					if (transition == Selectable.Transition.ColorTint)
					{
						EditorGUILayout.PropertyField(this.m_ImageColorsProperty);
					}
					else if (transition == Selectable.Transition.SpriteSwap)
					{
						EditorGUILayout.PropertyField(this.m_ImageSpriteStateProperty);
					}
					else if (transition == Selectable.Transition.Animation)
					{
						//
					}
					EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
				}
			}
			
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
		}
		
		private void DrawTargetTextProperties()
		{
			EditorGUILayout.LabelField("Text Target Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			
			EditorGUILayout.PropertyField(this.m_TextTargetProperty);
			
			// Check if image is set
			if (this.m_TextTargetProperty.objectReferenceValue != null)
			{
				EditorGUILayout.PropertyField(this.m_TextTransitionProperty);
				
				// Get the selected transition
                UIToggle.TextTransition transition = (UIToggle.TextTransition)this.m_TextTransitionProperty.enumValueIndex;

                if (transition != UIToggle.TextTransition.None)
				{
					EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                    if (transition == UIToggle.TextTransition.ColorTint)
					{
						EditorGUILayout.PropertyField(this.m_TextColorsProperty);
					}
					EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
				}
			}
			
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
		}

        private void DrawObjectProperties()
        {
            EditorGUILayout.LabelField("GameObject Target Properties", EditorStyles.boldLabel);
            EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
            EditorGUILayout.PropertyField(this.m_ActivateProperty, new GUIContent("Active Objects"), true);
            EditorGUILayout.PropertyField(this.m_DeactivateProperty, new GUIContent("Deactive Objects"), true);
            EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
        }

        private void DrawToggleProperties()
		{
			EditorGUILayout.LabelField("Toggle Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_IsOnProperty, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_TransitionProperty, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_GraphicProperty, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_GroupProperty, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_NavigationProperty);
			EditorGUILayout.Space();
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			EditorGUILayout.PropertyField(this.m_OnValueChangedProperty, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}

    }


    [CustomPropertyDrawer(typeof(ToggleColorBlock), true)]
    public class ToggleColorBlockDrawer : PropertyDrawer
    {
        protected static ToggleColorBlock m_Copy;
        protected static bool m_HasCopy = false;

        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            Rect position = rect;
            position.height = EditorGUIUtility.singleLineHeight;
            SerializedProperty property = prop.FindPropertyRelative("m_OnColor");
            SerializedProperty property2 = prop.FindPropertyRelative("m_OffColor");
            SerializedProperty property3 = prop.FindPropertyRelative("m_DisabledColor");
            SerializedProperty property4 = prop.FindPropertyRelative("m_FadeDuration");

            EditorGUI.PropertyField(position, property);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property2);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property3);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property4);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            Rect controlRect = EditorGUILayout.GetControlRect();
            controlRect.xMin = (controlRect.xMin + EditorGUIUtility.labelWidth);

            // Copy button
            if (GUI.Button(new Rect(controlRect.x, controlRect.y, ((controlRect.width / 2f) - 2f), controlRect.height), "Copy", EditorStyles.miniButton))
            {
                // Save the current values
                Copy(prop);
            }

            // Disable the paste button if we dont have a copied property
            if (!m_HasCopy)
                GUI.enabled = false;

            if (GUI.Button(new Rect((controlRect.x + ((controlRect.width / 2f) + 4f)), controlRect.y, ((controlRect.width / 2f) - 2f), controlRect.height), "Paste", EditorStyles.miniButton))
            {
                // Apply the copied values
                Paste(ref prop);
            }
            GUI.enabled = true;
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return 4f * EditorGUIUtility.singleLineHeight + 3f * EditorGUIUtility.standardVerticalSpacing;
        }

        protected static void Copy(SerializedProperty prop)
        {
            m_Copy = new ToggleColorBlock();
            m_Copy.onColor = prop.FindPropertyRelative("m_OnColor").colorValue;
            m_Copy.offColor = prop.FindPropertyRelative("m_OffColor").colorValue;
            m_Copy.disabledColor = prop.FindPropertyRelative("m_DisabledColor").colorValue;
            m_Copy.fadeDuration = prop.FindPropertyRelative("m_FadeDuration").floatValue;

            m_HasCopy = true;
        }

        protected static void Paste(ref SerializedProperty prop)
        {
            if (!m_HasCopy)
                return;

            prop.FindPropertyRelative("m_OnColor").colorValue = m_Copy.onColor;
            prop.FindPropertyRelative("m_OffColor").colorValue = m_Copy.offColor;
            prop.FindPropertyRelative("m_DisabledColor").colorValue = m_Copy.disabledColor;
            prop.FindPropertyRelative("m_FadeDuration").floatValue = m_Copy.fadeDuration;
        }
    }


    [CustomPropertyDrawer(typeof(ToggleSpriteState), true)]
    public class ToggleSpriteStateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            Rect position = rect;
            position.height = EditorGUIUtility.singleLineHeight;
            SerializedProperty property = prop.FindPropertyRelative("m_OnSprite");
            SerializedProperty property2 = prop.FindPropertyRelative("m_OffSprite");
            SerializedProperty property3 = prop.FindPropertyRelative("m_DisabledSprite");

            EditorGUI.PropertyField(position, property);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property2);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property3);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return 4f * EditorGUIUtility.singleLineHeight + 3f * EditorGUIUtility.standardVerticalSpacing;
        }
    }

}
