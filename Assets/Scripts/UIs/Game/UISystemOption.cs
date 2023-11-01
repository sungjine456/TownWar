using UnityEngine;
using UnityEngine.UI;

public class UISystemOption : SingletonMonoBehaviour<UISystemOption>
{
    [SerializeField] GameObject _elements;
    [SerializeField] Button _closeBtn;
    [SerializeField] Slider _bgmVolume;
    [SerializeField] Slider _sfxVolume;
    [SerializeField] Toggle _mute;

    protected override void OnAwake()
    {
        base.OnAwake();

        _closeBtn.onClick.AddListener(Close);
        SetStatus(false);
    }

    void Close()
    {
        SetStatus(false);
        UIMain.Instance.SetStatus(true);
    }

    public void Initialized(float bgmVolume, float sfxVolume, bool mute)
    {
        _bgmVolume.value = bgmVolume;
        _sfxVolume.value = sfxVolume;
        _mute.isOn = mute;

        SetMute(mute);
    }

    public void SetStatus(bool status)
    {
        _elements.SetActive(status);
    }

    public void SetBGMVolume(float volume)
    {
        SoundManager.Instance.SetVolumeBGM(volume);
    }

    public void SetSFXVolume(float volume)
    {
        SoundManager.Instance.SetVolumeSFX(volume);
    }

    public void SetMute(bool isOn)
    {
        SoundManager.Instance.SetMute(isOn);
    }
}
