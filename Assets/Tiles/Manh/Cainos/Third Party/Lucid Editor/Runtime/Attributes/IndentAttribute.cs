using System;
using UnityEngine;

namespace Cainos.LucidEditor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class IndentAttribute : Attribute
    {
        public readonly int indent;

        public IndentAttribute()
        {
            this.indent = 1;
        }
        
        public IndentAttribute(int indent)
        {
            this.indent = indent;
        }
    }
}