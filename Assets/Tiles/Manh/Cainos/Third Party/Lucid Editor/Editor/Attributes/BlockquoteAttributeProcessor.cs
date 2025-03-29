using UnityEngine;
using UnityEditor;
using Cainos.LucidEditor;

namespace Cainos.LucidEditor
{
    [CustomAttributeProcessor(typeof(BlockquoteAttribute))]
    public class BlockquoteAttributeProcessor : PropertyProcessor
    {
        public override void OnBeforeDrawProperty()
        {
            BlockquoteAttribute blockquote = (BlockquoteAttribute)attribute;
            GUIStyle style = EditorStyles.label;
            style.wordWrap = true;
            
            LucidEditorGUILayout.Blockquote(blockquote.text);
        }
    }
}