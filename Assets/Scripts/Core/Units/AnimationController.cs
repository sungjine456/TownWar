using UnityEngine;
using System.Text;

public class AnimationController : MonoBehaviour
{
    public enum AniMotion
    {
        Idle,
        Attack,
        Run,
        Death
    }

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
            Play(AniMotion.Attack);
    }

    public void SetMotion(AniMotion motion)
    {
        Motion = motion;
    }

    public void Play(AniMotion motion, bool isBlend = true)
    {
        Motion = motion;
        Play(motion.ToString(), isBlend);
    }

    public void Play(string aniName, bool isBlend = true)
    {
        if (isBlend)
            _animator.SetTrigger(aniName);
        else
            _animator.Play(aniName, 0, 0f);
    }
}
