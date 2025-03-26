using Cainos.LucidEditor;

namespace Cainos.LucidEditor
{
    [CustomAttributeProcessor(typeof(LabelTextAttribute))]
    public class LabelTextAttributeProcessor : PropertyProcessor
    {
        public override void OnBeforeDrawProperty()
        {
            property.displayName = ((LabelTextAttribute)attribute).label;
        }
    }
}