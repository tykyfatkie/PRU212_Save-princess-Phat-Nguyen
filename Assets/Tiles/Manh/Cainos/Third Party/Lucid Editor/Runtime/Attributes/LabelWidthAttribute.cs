using System;

namespace Cainos.LucidEditor
{
    [AttributeUsage(AttributeTargets.Field)]
    public class LabelWidthAttribute : Attribute
    {
        public readonly float width;

        public LabelWidthAttribute(float width)
        {
            this.width = width;
        }
    }
}