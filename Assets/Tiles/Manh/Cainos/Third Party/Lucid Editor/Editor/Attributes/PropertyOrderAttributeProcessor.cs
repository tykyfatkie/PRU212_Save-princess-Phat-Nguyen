using Cainos.LucidEditor;

namespace Cainos.LucidEditor
{
    [CustomAttributeProcessor(typeof(PropertyOrderAttribute))]
    public class PropertyOrderAttributeProcessor : PropertyProcessor
    {
        public override void OnBeforeInspectorGUI()
        {
            PropertyOrderAttribute propertyOrder = (PropertyOrderAttribute)attribute;
            property.order = propertyOrder.propertyOrder;
        }
    }
}