using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation
{
    private Animator _animator;
    public PlayerAnimation(Animator animator)
    {
        _animator = animator;
    }
    public void EnterMovingAnim()
    {
        _animator.SetBool("IsFishing", false);
        _animator.SetBool("IsMoving", true);
    }
    public void EnterIdleStateAnim()
    {
        _animator.SetBool("IsFishing", false);
        _animator.SetBool("IsMoving", false);
    }
    public void EnterFishingStateAnim()
    {
        _animator.SetBool("IsFishing", true);
    }
}
