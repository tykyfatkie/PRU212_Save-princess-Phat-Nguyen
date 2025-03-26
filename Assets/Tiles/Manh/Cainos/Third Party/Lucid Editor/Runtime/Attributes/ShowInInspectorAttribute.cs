using System;

namespace Cainos.LucidEditor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public class ShowInInspectorAttribute : Attribute { }
}