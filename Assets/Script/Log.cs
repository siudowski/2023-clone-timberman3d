using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : MonoBehaviour
{
    public int Position { get; set; }
    [SerializeField] Animator animator;
    public void Init(int pos)
    {
        Position = pos;
        animator = GetComponentInChildren<Animator>();
    }
    public void PlayAnimation(int anim)
    {
        if (anim == 0)
        {
            transform.Translate(0, -2, 0);
            animator.Play("log-fall");
        } else if (anim == 1)
        {
            StartCoroutine(CutAnimation());
        }
        
    }

    IEnumerator CutAnimation()
    {
        transform.Rotate(0, Random.Range(0f, 361f), 0);
        animator.Play("log-throw");
        yield return new WaitForSeconds(0.12f);
        Destroy(gameObject);
    }
}