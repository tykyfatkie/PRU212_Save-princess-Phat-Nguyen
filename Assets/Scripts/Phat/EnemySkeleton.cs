using UnityEngine;

public class EnemySkeleton : MonoBehaviour
{
    private Animator anim;
    private bool hasRisen = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasRisen) 
        {
            anim.SetTrigger("Rise"); 
            hasRisen = true; 
        }
    }
}

