using System;

namespace Cainos.LucidEditor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class HorizontalGroupAttribute : PropertyGroupAttribute
    {
        public HorizontalGroupAttribute(string groupName) : base(groupName) { }
    }
}