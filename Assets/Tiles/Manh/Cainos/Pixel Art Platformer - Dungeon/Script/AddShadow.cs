#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Cainos.PixelArtPlatformer_Dungeon
{

    public class AddShadow : MonoBehaviour
    {
        public Material shadowMaterial;
        public Vector3 offset = new(-0.15f, -0.15f, 0.01f);

        public void Add()
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr == null) return;

            var shadowGO = new GameObject("Shadow");
            shadowGO.transform.parent = transform;

            var shadowSR = shadowGO.AddComponent<SpriteRenderer>();
            shadowSR.sprite = sr.sprite;
            shadowSR.material = shadowMaterial;

            shadowSR.transform.localPosition = offset;
            shadowSR.transform.localRotation = Quaternion.identity;
            shadowSR.transform.localScale = Vector3.one;

            Undo.RegisterCreatedObjectUndo(shadowGO, "Add Shadow Object");
            Undo.DestroyObjectImmediate(this);
        }
    }
}

#endif
