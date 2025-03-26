using UnityEditor;

namespace Cainos.LucidEditor
{
    public static class SerializedPropertyUtility
    {
        public static System.Type GetUnderlyingType(this SerializedProperty property)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetFieldViaPath(property.propertyPath);
            return fi.FieldType;
        }

        public static System.Reflection.FieldInfo GetFieldViaPath(this System.Type type, string path)
        {
            System.Type parentType = type;
            System.Reflection.FieldInfo fi = type.GetField(path);
            string[] perDot = path.Split('.');
            foreach (string fieldName in perDot)
            {
                fi = parentType.GetField(fieldName);
                if (fi != null)
                    parentType = fi.FieldType;
                else
                    return null;
            }
            if (fi != null)
                return fi;
            else return null;
        }
    }
}