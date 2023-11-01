using System.Collections;
using System.Collections.Generic;

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
        battle,
        town,
        waitBattle
    }
    public enum SfxClip
    {
        bow,
        getElixir,
        getGold,
        sword
    }

    const int _maxCount = 5;

    AudioSource[] _audios;
    AudioClip[] _bgmClips;
    AudioClip[] _sfxClips;

    Dictionary<SfxClip, int> _sfxPlayCountList;

    protected override void OnAwake()
    {
        base.OnAwake();

        _sfxPlayCountList = new();
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

        _bgmClips = Resources.LoadAll<AudioClip>("Sounds/BGM");
        _sfxClips = Resources.LoadAll<AudioClip>("Sounds/SFX");
    }

    IEnumerator Coroutine_CheckPlayCount(SfxClip sfx, float length)
    {
        yield return new WaitForSeconds(length); //TODO : YieldInstructionCache 수정 후 변경 필요

        _sfxPlayCountList[sfx]--;

        if (_sfxPlayCountList[sfx] < 0)
            _sfxPlayCountList[sfx] = 0;
    }

    public void PlayBGM(BgmClip bgm)
    {
        _audios[(int)AudioType.BGM].clip = _bgmClips[(int)bgm];
        _audios[(int)AudioType.BGM].Play();
    }

    public void PlaySFX(SfxClip sfx)
    {
        if (_sfxPlayCountList.TryGetValue(sfx, out int count))
        {
            if (count >= _maxCount)
                return;

            _sfxPlayCountList[sfx]++;
        }
        else
            _sfxPlayCountList.Add(sfx, 1);

        _audios[(int)AudioType.SFX].PlayOneShot(_sfxClips[(int)sfx]);
        StartCoroutine(Coroutine_CheckPlayCount(sfx, _sfxClips[(int)sfx].length));
    }

    public void SetVolumeBGM(float volume)
    {
        _audios[(int)AudioType.BGM].volume = volume / 3;
        GameManager.Instance.MyPlayer.SetBGMVolume(volume);
    }

    public void SetVolumeSFX(float volume)
    {
        _audios[(int)AudioType.SFX].volume = volume / 3;
        GameManager.Instance.MyPlayer.SetSFXVolume(volume);
    }

    public void SetMute(bool isOn)
    {
        for (int i = 0; i < _audios.Length; i++)
            _audios[i].mute = isOn;

        GameManager.Instance.MyPlayer.SetMute(isOn);
    }
}
