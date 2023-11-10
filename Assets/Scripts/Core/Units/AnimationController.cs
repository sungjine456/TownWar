using UnityEngine;

public enum AniMotion
{
    Idle,
    Attack,
    Run,
    Death
}

public class AnimationController : MonoBehaviour
{
    Animator _animator;

    public AniMotion Motion { get; private set; }

    void Awake()
    {
        _animator = GetComponent<Animator>();
        
        RuntimeAnimatorController rc = _animator.runtimeAnimatorController;

        for (int i = 0; i < rc.animationClips.Length; i++)
        {
            var clip = rc.animationClips[i];

            AnimationEvent animationEndEvent = new();
            animationEndEvent.time = clip.length;
            animationEndEvent.functionName = "AnimationCompleteHandler";

            clip.AddEvent(animationEndEvent);
        }
    }

    public void AnimationCompleteHandler()
    {
        if (Motion == AniMotion.Attack)
            Play();
    }

    public void SetMotion(AniMotion motion)
    {
        Motion = motion;
    }

    public void Play(bool isBlend = true)
    {
        if (isBlend)
            _animator.SetTrigger(Motion.ToString());
        else
            _animator.Play(Motion.ToString(), 0, 0f);
    }
}
