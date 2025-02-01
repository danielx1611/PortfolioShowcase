using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameTextPopup : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            animator.SetTrigger("FadeIn");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            animator.SetTrigger("FadeOut");
        }       
    }
}
