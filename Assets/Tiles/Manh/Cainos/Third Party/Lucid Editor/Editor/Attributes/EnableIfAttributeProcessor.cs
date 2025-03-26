using Cainos.LucidEditor;

namespace Cainos.LucidEditor
{
    [CustomAttributeProcessor(typeof(EnableIfAttribute))]
    public class EnableIfAttributeProcessor : PropertyProcessor
    {
        public override void OnBeforeDrawProperty()
        {
            EnableIfAttribute enableIf = (EnableIfAttribute)attribute;
            property.isEditable = ReflectionUtil.GetValueBool(property.parentObject, enableIf.condition);
        }
    }
}