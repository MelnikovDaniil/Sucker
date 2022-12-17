using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimation : MonoBehaviour
{
    public Animator animator;
    public List<string> animations;

    void Start()
    {
        animator.Play(animations.GetRandom(), 0);
    }
}
