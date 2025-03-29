using Cainos.LucidEditor;

namespace Cainos.LucidEditor
{
    [CustomAttributeProcessor(typeof(IndentAttribute))]
    public class IndentAttributeProcessor : PropertyProcessor
    {
        public override void OnBeforeDrawProperty()
        {
            IndentAttribute indent = (IndentAttribute)attribute;
            property.indent = indent.indent;
        }
    }
}