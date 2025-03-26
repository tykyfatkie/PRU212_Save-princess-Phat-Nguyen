using System;
using UnityEngine;

namespace Cainos.LucidEditor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class ShowIfAttribute : Attribute
    {
        public readonly string condition;

        public ShowIfAttribute(string condition)
        {
            this.condition = condition;
        }
    }
}