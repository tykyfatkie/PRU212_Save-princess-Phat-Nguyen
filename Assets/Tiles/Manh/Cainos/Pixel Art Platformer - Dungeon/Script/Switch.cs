using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cainos.LucidEditor;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

namespace Cainos.PixelArtPlatformer_Dungeon
{
    public class Switch : MonoBehaviour
    {
        [FoldoutGroup("Reference")] public Door target;
        [Space]
        [FoldoutGroup("Reference")] public SpriteRenderer spriteRenderer;
        [FoldoutGroup("Reference")] public Sprite spriteOn;
        [FoldoutGroup("Reference")] public Sprite spriteOff;

        private Animator Animator
        {
            get
            {
                if (animator == null) animator = GetComponent<Animator>();
                return animator;
            }
        }
        private Animator animator;

        private void Start()
        {
            Animator.SetBool("IsOn", isOn);
            IsOn = isOn;
        }

        [FoldoutGroup("Runtime"), ShowInInspector]
        public bool IsOn
        {
            get { return isOn; }
            set
            {
                isOn = value;

                #if UNITY_EDITOR
                if (Application.isPlaying == false)
                {
                    EditorUtility.SetDirty(this);
                    EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
                #endif

                if (target) target.IsOpened = isOn;

                if (Application.isPlaying )
                {
                    Animator.SetBool("IsOn", isOn);
                }
                else
                {
                    if (spriteRenderer) spriteRenderer.sprite = isOn ? spriteOn: spriteOff;
                }
            }
        }
        [SerializeField, HideInInspector]
        private bool isOn;

        [FoldoutGroup("Runtime"), HorizontalGroup("Runtime/Button"), Button("Turn On")]
        public void TurnOn()
        {
            IsOn = true;
        }

        [FoldoutGroup("Runtime"), HorizontalGroup("Runtime/Button"), Button("Turn Off")]
        public void TurnOff()
        {
            IsOn = false;
        }
    }
}
