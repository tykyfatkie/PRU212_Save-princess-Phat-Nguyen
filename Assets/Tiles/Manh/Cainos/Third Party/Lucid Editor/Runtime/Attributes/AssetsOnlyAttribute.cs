using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.LucidEditor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class AssetsOnlyAttribute : Attribute
    {
        public AssetsOnlyAttribute()
        {
        }
    }
}
