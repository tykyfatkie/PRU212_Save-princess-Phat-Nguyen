using Cainos.LucidEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Cainos.LucidEditor
{
    public sealed class EditableInspectorProperty : InspectorProperty
    {
        private MethodInfo getter;
        private MethodInfo setter;
        private PropertyInfo info;

        private List<PropertyProcessor> processors = new List<PropertyProcessor>();
        internal EditableInspectorProperty(SerializedObject serializedObject, object parentObject, string name, Attribute[] attributes) : base(serializedObject, null, parentObject, name, attributes) { }

        internal override void Initialize()
        {
            processors.Clear();
            foreach (Attribute attribute in attributes)
            {
                PropertyProcessor processor = ProcessorUtil.CreateAttributeProcessor(this, attribute);

                if (processor != null)
                {
                    processor.Initialize();
                    processors.Add(processor);
                }
            }

            info = parentObject.GetType().GetProperty(name);
            getter = this.info.GetGetMethod();
            setter = this.info.GetSetMethod();
        }

        private string Name
        {
            get
            {
                return ObjectNames.NicifyVariableName(name);
            }
        }

        internal override void Draw()
        {
            foreach (PropertyProcessor processor in processors) processor.OnBeforeDrawProperty();

            if (isHidden) return;

            LucidEditorGUILayout.BeginLayoutIndent(EditorGUI.indentLevel + indent);
            if (!isEditable) EditorGUI.BeginDisabledGroup(true);
            {
                //object value = ReflectionUtil.GetValue(parentObject, name);
                //LucidEditorGUILayout.ReadOnlyField(Name, value, value.GetType());
                if ( GetPropertyType(info, out SerializedPropertyType serialzedProertyType))
                {
                    Draw_Internal( serialzedProertyType);
                }
            }
            if (!isEditable) EditorGUI.EndDisabledGroup();
            LucidEditorGUILayout.EndLayoutIndent();

            foreach (PropertyProcessor processor in processors) processor.OnAfterDrawProperty();
        }

        internal void Draw_Internal(SerializedPropertyType serialzedProertyType)
        {
            var emptyOptions = new GUILayoutOption[0];
            EditorGUILayout.BeginHorizontal(emptyOptions);

            if (serialzedProertyType == SerializedPropertyType.Integer)
            {
                var oldValue = (int)GetValue();
                var newValue = EditorGUILayout.IntField(Name, oldValue, emptyOptions);
                if (oldValue != newValue)
                    SetValue(newValue);
            }
            else if (serialzedProertyType == SerializedPropertyType.Float)
            {
                var oldValue = (float)GetValue();
                var newValue = EditorGUILayout.FloatField(Name, oldValue, emptyOptions);
                if (oldValue != newValue)
                   SetValue(newValue);
            }
            else if (serialzedProertyType == SerializedPropertyType.Boolean)
            {
                var oldValue = (bool)GetValue();
                var newValue = EditorGUILayout.Toggle(Name, oldValue, emptyOptions);
                if (oldValue != newValue)
                    SetValue(newValue);
            }
            else if (serialzedProertyType == SerializedPropertyType.String)
            {
                var oldValue = (string)GetValue();
                var newValue = EditorGUILayout.TextField(Name, oldValue, emptyOptions);
                if (oldValue != newValue)
                    SetValue(newValue);
            }
            else if (serialzedProertyType == SerializedPropertyType.Vector2)
            {
                var oldValue = (Vector2)GetValue();
                var newValue = EditorGUILayout.Vector2Field(Name, oldValue, emptyOptions);
                if (oldValue != newValue)
                    SetValue(newValue);
            }
            else if (serialzedProertyType == SerializedPropertyType.Vector3)
            {
                var oldValue = (Vector3)GetValue();
                var newValue = EditorGUILayout.Vector3Field(Name, oldValue, emptyOptions);
                if (oldValue != newValue)
                    SetValue(newValue);
            }
            else if (serialzedProertyType == SerializedPropertyType.Enum)
            {
                var oldValue = (Enum)GetValue();
                var newValue = EditorGUILayout.EnumPopup(Name, oldValue, emptyOptions);
                if (oldValue != newValue)
                    SetValue(newValue);
            }
            else if (serialzedProertyType == SerializedPropertyType.Color)
            {
                var oldValue = (Color)GetValue();
                var newValue = EditorGUILayout.ColorField(Name, oldValue, emptyOptions);
                if (oldValue != newValue)
                    SetValue(newValue);
            }

            else if (serialzedProertyType == SerializedPropertyType.ObjectReference)
            {
                var oldValue = (UnityEngine.Object)GetValue();
                var newValue = LucidEditorGUILayout.ObjectField(Name, oldValue, info.PropertyType, !TryGetAttribute<AssetsOnlyAttribute>(out _), emptyOptions);
                if (oldValue != newValue)
                    SetValue(newValue);
            }

            EditorGUILayout.EndHorizontal();
        }

        private object GetValue() { return getter.Invoke(parentObject, null); }
        private void SetValue(object value)
        {
            if (setter == null) return;
            setter.Invoke(parentObject, new[] { value });
        }

        private bool GetPropertyType(PropertyInfo info, out SerializedPropertyType propertyType)
        {
            Type type = info.PropertyType;
            propertyType = SerializedPropertyType.Generic;
            if (type == typeof(int))
                propertyType = SerializedPropertyType.Integer;
            else if (type == typeof(float))
                propertyType = SerializedPropertyType.Float;
            else if (type == typeof(bool))
                propertyType = SerializedPropertyType.Boolean;
            else if (type == typeof(string))
                propertyType = SerializedPropertyType.String;
            else if (type == typeof(Vector2))
                propertyType = SerializedPropertyType.Vector2;
            else if (type == typeof(Vector3))
                propertyType = SerializedPropertyType.Vector3;
            else if (type == typeof(Color))
                propertyType = SerializedPropertyType.Color;
            else if (type.IsEnum)
                propertyType = SerializedPropertyType.Enum;
            else if (type.IsSubclassOf(typeof(UnityEngine.Object)))
                propertyType = SerializedPropertyType.ObjectReference;
            return propertyType != SerializedPropertyType.Generic;
        }

        internal override void OnBeforeInspectorGUI()
        {
            foreach (PropertyProcessor processor in processors) processor.OnBeforeInspectorGUI();
        }

        internal override void OnAfterInspectorGUI()
        {
            foreach (PropertyProcessor processor in processors) processor.OnAfterInspectorGUI();
        }
    }
}