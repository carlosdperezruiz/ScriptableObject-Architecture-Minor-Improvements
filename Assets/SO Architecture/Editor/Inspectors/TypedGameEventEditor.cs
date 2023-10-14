using System.Reflection;
using UnityEditor;
using UnityEngine;
using Type = System.Type;

namespace ScriptableObjectArchitecture.Editor
{
    [CustomEditor(typeof(GameEventBase<>), true)]
    public class TypedGameEventEditor : BaseGameEventEditor
    {
        private MethodInfo _raiseMethod;

        protected override void OnEnable()
        {
            base.OnEnable();

            _raiseMethod = target.GetType().BaseType.GetMethod("Raise", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
        }
        protected override void DrawRaiseButton()
        {
            SerializedProperty property = serializedObject.FindProperty("_debugValue");

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                /*
                 * Improvement:
                 *
                 * If the type is a generic (IEnumerator) or doesn't have a default value (struct) this function will
                 * cause an error. In these cases, this change is intended to remove the "Raise" button in the editor.
                 */
                if (property == null || property.serializedObject == null || property.serializedObject.targetObject == null)
                {
                    return;
                }

                Type debugValueType = GetDebugValueType(property);
                GenericPropertyDrawer.DrawPropertyDrawerLayout(property, debugValueType);

                if (scope.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }

            if (GUILayout.Button("Raise"))
            {
                CallMethod(GetDebugValue(property));
            }
        }
        private object GetDebugValue(SerializedProperty property)
        {
            Type targetType = property.serializedObject.targetObject.GetType();
            FieldInfo targetField = targetType.GetField("_debugValue", BindingFlags.Instance | BindingFlags.NonPublic);

            return targetField.GetValue(property.serializedObject.targetObject);
        }
        private Type GetDebugValueType(SerializedProperty property)
        {
            Type targetType = property.serializedObject.targetObject.GetType();
            FieldInfo targetField = targetType.GetField("_debugValue", BindingFlags.Instance | BindingFlags.NonPublic);

            return targetField.FieldType;
        }
        private void CallMethod(object value)
        {
            _raiseMethod.Invoke(target, new object[1] { value });
        }
    }
}