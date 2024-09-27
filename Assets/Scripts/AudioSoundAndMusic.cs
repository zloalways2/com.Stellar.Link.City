using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSoundAndMusic : MonoBehaviour
{
    [SerializeField] private AudioMixer _soundEngine;
    [SerializeField] private Image soundButtonImage;
    [SerializeField] private Image musicButtonImage;
    [SerializeField] private Sprite soundOnSprite;  
    [SerializeField] private Sprite soundOffSprite; 

    private bool _audioEnabled;
    private bool _musicEnabled;

    void Start()
    {
        _audioEnabled = PlayerPrefs.GetInt("audioEnabled", 1) == 1;
        _musicEnabled = PlayerPrefs.GetInt("musicEnabled", 1) == 1;

        UpdateSoundState();
        UpdateMusicState();
    }

    public void SwitchAudioState()
    {
        _audioEnabled = !_audioEnabled;
        PlayerPrefs.SetInt("audioEnabled", _audioEnabled ? 1 : 0);
        UpdateSoundState();
    }

    public void AlterMusicStatus()
    {
        _musicEnabled = !_musicEnabled;
        PlayerPrefs.SetInt("musicEnabled", _musicEnabled ? 1 : 0);
        UpdateMusicState();
    }

    private void UpdateSoundState()
    {
        _soundEngine.SetFloat(ChronicleCache.TITLE_TRACK, _audioEnabled ? 0f : -80f);
        soundButtonImage.sprite = _audioEnabled ? soundOnSprite : soundOffSprite;
    }

    private void UpdateMusicState()
    {
        _soundEngine.SetFloat(ChronicleCache.IDENTIFIER_SOUND, _musicEnabled ? 0f : -80f);
        musicButtonImage.sprite = _musicEnabled ? soundOnSprite : soundOffSprite;
    }
}