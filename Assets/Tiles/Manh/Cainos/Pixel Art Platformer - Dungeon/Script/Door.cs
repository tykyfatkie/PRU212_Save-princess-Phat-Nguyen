using UnityEngine;
using Cainos.LucidEditor;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


namespace Cainos.PixelArtPlatformer_Dungeon
{
    public class Door : MonoBehaviour
    {
        [FoldoutGroup("Reference")] public SpriteRenderer spriteRenderer;
        [FoldoutGroup("Reference")] public Sprite spriteOpened;
        [FoldoutGroup("Reference")] public Sprite spriteClosed;


        private Animator Animator
        {
            get
            {
                if (animator == null ) animator = GetComponent<Animator>();
                return animator;
            }
        }
        private Animator animator;


        [FoldoutGroup("Runtime"), ShowInInspector]
        public bool IsOpened
        {
            get { return isOpened; }
            set
            {
                isOpened = value;

                #if UNITY_EDITOR
                if (Application.isPlaying == false)
                {
                    EditorUtility.SetDirty(this);
                    EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
                #endif


                if (Application.isPlaying)
                {
                    Animator.SetBool("IsOpened", isOpened);
                }
                else
                {
                    if(spriteRenderer) spriteRenderer.sprite = isOpened ? spriteOpened : spriteClosed;
                }
            }
        }
        [SerializeField,HideInInspector]
        private bool isOpened;

        private void Start()
        {
            Animator.Play(isOpened ? "Opened" : "Closed");
            IsOpened = isOpened;
        }


        [FoldoutGroup("Runtime"), HorizontalGroup("Runtime/Button"), Button("Open")]
        public void Open()
        {
            IsOpened = true;
        }

        [FoldoutGroup("Runtime"), HorizontalGroup("Runtime/Button"), Button("Close")]
        public void Close()
        {
            IsOpened = false;
        }
    }
}
