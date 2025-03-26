using UnityEditor;
using Cainos.LucidEditor;

namespace Cainos.LucidEditor
{
    [CustomAttributeProcessor(typeof(TitleHeaderAttribute))]
    public class TitleHeaderAttributeProcessor : PropertyProcessor
    {
        public override void OnBeforeDrawProperty()
        {
            EditorGUILayout.Space(7);
            LucidEditorGUILayout.TitleHeader(((TitleHeaderAttribute)attribute).title);
        }
    }
}