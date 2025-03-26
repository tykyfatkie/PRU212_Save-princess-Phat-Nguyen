using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtPlatformer_Dungeon
{
    public class SwingingBladeTrap : MonoBehaviour
    {
        [Header("Params")]
        public AnimationCurve bladeRotationCurve;
        public float bladeRotationMaxAngle = 50.0f;
        public float bladeRotationTime = 3.0f;

        [Header("Objects")]
        public Transform blade;

        private float timer;
        private float v;

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer > bladeRotationTime) timer = 0.0f;

            v = bladeRotationCurve.Evaluate(timer / bladeRotationTime);

            blade.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, v * bladeRotationMaxAngle);
        }
    }
}
