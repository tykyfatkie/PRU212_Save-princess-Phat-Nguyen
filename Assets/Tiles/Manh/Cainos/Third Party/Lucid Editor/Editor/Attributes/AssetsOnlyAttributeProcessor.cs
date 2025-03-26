using Cainos.LucidEditor;

namespace Cainos.LucidEditor
{
    [CustomAttributeProcessor(typeof(AssetsOnlyAttribute))]
    public class AssetsOnlyAttributeProcessor : PropertyProcessor
    {
        public override void OnBeforeDrawProperty()
        {
            property.allowSceneObject = false;
        }
    }
}