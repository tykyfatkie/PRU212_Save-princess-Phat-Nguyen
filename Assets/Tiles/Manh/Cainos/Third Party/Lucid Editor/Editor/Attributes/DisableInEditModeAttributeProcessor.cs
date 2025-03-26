using UnityEngine;
using Cainos.LucidEditor;

namespace Cainos.LucidEditor
{
    [CustomAttributeProcessor(typeof(DisableInEditModeAttribute))]
    public class DisableInEditModeAttributeProcessor : PropertyProcessor
    {
        public override void OnBeforeDrawProperty()
        {
            DisableInEditModeAttribute disableInEditMode = (DisableInEditModeAttribute)attribute;
            property.isEditable = Application.isPlaying;
        }
    }
}