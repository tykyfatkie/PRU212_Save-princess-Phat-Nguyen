using UnityEngine;

public class SpringBounce : MonoBehaviour
{
    Animator myAnimator;
    void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            myAnimator.SetTrigger("isActivated");
        }
    }
}
