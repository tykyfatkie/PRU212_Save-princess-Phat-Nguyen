using System;

namespace Cainos.LucidEditor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public class FoldoutGroupAttribute : PropertyGroupAttribute
    {
        public FoldoutGroupAttribute(string groupName) : base(groupName) { }
    }
}