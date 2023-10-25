using UnityEngine;

public class SoundManager : SingletonDontDestroy<SoundManager>
{
    public enum AudioType
    {
        BGM,
        SFX,
        Max
    }
    public enum BgmClip
    {
        main,
        battle
    }
    public enum SfxClip
    {
        getGold,
        getElixir
    }

    AudioSource[] _audios;
    AudioClip[] _bgmClips;
    AudioClip[] _sfxClips;

    void Start()
    {
        _audios = new AudioSource[(int)AudioType.Max];

        var bgm = gameObject.AddComponent<AudioSource>();
        bgm.playOnAwake = true;
        bgm.loop = true;
        bgm.rolloffMode = AudioRolloffMode.Linear;

        var sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.loop = false;
        sfx.rolloffMode = AudioRolloffMode.Linear;

        _audios[(int)AudioType.BGM] = bgm;
        _audios[(int)AudioType.SFX] = sfx;

        //_bgmClips = Resources.LoadAll<AudioClip>("Sounds/BGM"); //TODO : BGM 추가
        _sfxClips = Resources.LoadAll<AudioClip>("Sounds/SFX");
    }

    public void PlayBGM(BgmClip bgm)
    {
        _audios[(int)AudioType.BGM].clip = _bgmClips[(int)bgm];
        _audios[(int)AudioType.BGM].Play();
    }

    public void PlaySFX(SfxClip sfx)
    {
        _audios[(int)AudioType.SFX].PlayOneShot(_sfxClips[(int)sfx]);
    }
}
