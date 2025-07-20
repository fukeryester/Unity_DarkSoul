using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventControl : MonoBehaviour
{
    private Animator anim;
    private KeyboardInput PI;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        PI = GetComponentInParent<KeyboardInput>();
    }

    public void ResetTrigger(string TriggerName)
    {
        anim.ResetTrigger(TriggerName);
    }

    public void AnimationNearEnd_UnlockInput()
    {
        PI.inputEnabled = true;
    }
}
