using System;
using UnityEngine;

namespace Cainos.LucidEditor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = true)]
    public class TitleHeaderAttribute : Attribute
    {
        public readonly string title;

        public TitleHeaderAttribute(string title)
        {
            this.title = title;
        }
    }
}