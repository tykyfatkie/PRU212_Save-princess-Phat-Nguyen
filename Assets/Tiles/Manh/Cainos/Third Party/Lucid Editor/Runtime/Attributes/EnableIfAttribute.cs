using System;
using UnityEngine;

namespace Cainos.LucidEditor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class EnableIfAttribute : Attribute
    {
        public readonly string condition;

        public EnableIfAttribute(string condition)
        {
            this.condition = condition;
        }
    }
}