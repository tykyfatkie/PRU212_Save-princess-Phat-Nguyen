using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


namespace Cainos.PixelArtPlatformer_Dungeon
{
    using UnityEngine;

    public class Door2 : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite spriteOpened;
        [SerializeField] private Sprite spriteClosed;
        [SerializeField] private Animator animator;
        [SerializeField] private bool isOpened;

        public bool IsOpened { get; internal set; }

        private void Start()
        {
            animator.Play(isOpened ? "Opened" : "Closed");
            UpdateDoorState();
        }

        public void Open() => SetDoorState(true);
        public void Close() => SetDoorState(false);

        private void SetDoorState(bool state)
        {
            isOpened = state;
            animator.SetBool("IsOpened", isOpened);
            if (spriteRenderer) spriteRenderer.sprite = isOpened ? spriteOpened : spriteClosed;
        }

        private void UpdateDoorState() => SetDoorState(isOpened);
    }

}
