using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


public static class ExposeProperties {

    public static void Expose(PropertyField[] properties) {
        var emptyOptions = new GUILayoutOption[0];
        EditorGUILayout.BeginVertical(emptyOptions);

        foreach (var field in properties) {
            EditorGUILayout.BeginHorizontal(emptyOptions);

            switch (field.Type) {
                case SerializedPropertyType.Integer:
                    field.SetValue(EditorGUILayout.IntField(field.Name, (int)field.GetValue(), emptyOptions));
                    break;

                case SerializedPropertyType.Float:
                    field.SetValue(EditorGUILayout.FloatField(field.Name, (float)field.GetValue(), emptyOptions));
                    break;

                case SerializedPropertyType.Boolean:
                    field.SetValue(EditorGUILayout.Toggle(field.Name, (bool)field.GetValue(), emptyOptions));
                    break;

                case SerializedPropertyType.String:
                    field.SetValue(EditorGUILayout.TextField(field.Name, (string)field.GetValue(), emptyOptions));
                    break;

                case SerializedPropertyType.Vector2:
                    field.SetValue(EditorGUILayout.Vector2Field(field.Name, (Vector2)field.GetValue(), emptyOptions));
                    break;

                case SerializedPropertyType.Vector3:
                    field.SetValue(EditorGUILayout.Vector3Field(field.Name, (Vector3)field.GetValue(), emptyOptions));
                    break;

                case SerializedPropertyType.Enum:
                    field.SetValue(EditorGUILayout.EnumPopup(field.Name, (Enum)field.GetValue(), emptyOptions));
                    break;

                case SerializedPropertyType.ObjectReference:
                    field.SetValue(EditorGUILayout.ObjectField(field.Name, (UnityEngine.Object)field.GetValue(), field.GetPropertyType(), true, emptyOptions));
                    break;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    public static PropertyField[] GetProperties(object obj) {
        var fields = new List<PropertyField>();

        PropertyInfo[] infos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var info in infos) {
            if (!(info.CanRead && info.CanWrite))
                continue;

            var attributes = info.GetCustomAttributes(true);
            if (attributes.All(o => o.GetType() != typeof(ExposePropertyAttribute)))
                continue;

            SerializedPropertyType type = SerializedPropertyType.Integer;
            if (PropertyField.GetPropertyType(info, out type)) {
                var field = new PropertyField(obj, info, type);
                fields.Add(field);
            }
        }

        return fields.ToArray();
    }
}


public class PropertyField {
    object _instance;
    PropertyInfo _info;
    SerializedPropertyType _type;

    MethodInfo _getter;
    MethodInfo _setter;

    public SerializedPropertyType Type {
        get { return _type; }
    }

    public string Name {
        get { return ObjectNames.NicifyVariableName(_info.Name); }
    }

    public PropertyField(object instance, PropertyInfo info, SerializedPropertyType type) {
        _instance = instance;
        _info = info;
        _type = type;

        _getter = _info.GetGetMethod();
        _setter = _info.GetSetMethod();
    }

    public object GetValue() {
        return _getter.Invoke(_instance, null);
    }

    public void SetValue(object value) {
        _setter.Invoke(_instance, new[] { value });
    }

    public Type GetPropertyType() {
        return _info.PropertyType;
    }

    public static bool GetPropertyType(PropertyInfo info, out SerializedPropertyType propertyType) {
        propertyType = SerializedPropertyType.Generic;
        Type type = info.PropertyType;

        if (type == typeof(int)) {
            propertyType = SerializedPropertyType.Integer;
            return true;
        }
        if (type == typeof(float)) {
            propertyType = SerializedPropertyType.Float;
            return true;
        }
        if (type == typeof(bool)) {
            propertyType = SerializedPropertyType.Boolean;
            return true;
        }
        if (type == typeof(string)) {
            propertyType = SerializedPropertyType.String;
            return true;
        }
        if (type == typeof(Vector2)) {
            propertyType = SerializedPropertyType.Vector2;
            return true;
        }
        if (type == typeof(Vector3)) {
            propertyType = SerializedPropertyType.Vector3;
            return true;
        }
        if (type.IsEnum) {
            propertyType = SerializedPropertyType.Enum;
            return true;
        }
        // COMMENT OUT to NOT expose custom objects/types
        propertyType = SerializedPropertyType.ObjectReference;
        return true;

        //return false;
    }
}