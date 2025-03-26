using System;

namespace Cainos.LucidEditor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class BoxGroupAttribute : PropertyGroupAttribute
    {
        public BoxGroupAttribute(string groupName) : base(groupName) { }
    }
}