using Cainos.LucidEditor;

namespace Cainos.LucidEditor
{
    [CustomAttributeProcessor(typeof(OnValueChangedAttribute))]
    public class OnValueChangedAttributeProcessor : PropertyProcessor
    {
        public override void OnAfterDrawProperty()
        {
            if (property.changed)
            {
                OnValueChangedAttribute onValueChanged = (OnValueChangedAttribute)attribute;
                ReflectionUtil.Invoke(property.parentObject, onValueChanged.methodName, property.serializedProperty.GetValue<object>());
            }
        }
    }
}
