using UnityEditor;
using Cainos.LucidEditor;

namespace Cainos.LucidEditor
{
    [CustomAttributeProcessor(typeof(SectionHeaderAttribute))]
    public class SectionHeaderAttributeProcessor : PropertyProcessor
    {
        public override void OnBeforeDrawProperty()
        {
            EditorGUILayout.Space(7);
            LucidEditorGUILayout.SectionHeader(((SectionHeaderAttribute)attribute).title);
        }
    }
}