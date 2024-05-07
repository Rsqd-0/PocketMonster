using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        // Load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Rsqd");
    }
    
    public void QuitGame()
    {
        // Quit the game
        Application.Quit();
    }
    
    [SerializeField] private AudioMixer audioMixer = null;

    [SerializeField] private Slider audioMainSlider = null;
    [SerializeField] private Slider audioMusicSlider = null;
    [SerializeField] private Slider audioSfxSlider = null;

    private void Awake()
    {
        audioMixer.GetFloat("MainVolume", out float mainVolume);
        audioMainSlider.value = Mathf.Exp(mainVolume / 20f);

        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        audioMusicSlider.value = Mathf.Exp(musicVolume / 20);

        audioMixer.GetFloat("SFXVolume", out float sfxVolume);
        audioSfxSlider.value = Mathf.Exp(sfxVolume / 20);
    }

    private void Start()
    {
        audioMainSlider.onValueChanged.AddListener(OnMainVolumeChanged);
        audioMusicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        audioSfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        
    }

    private void OnMainVolumeChanged(float value)
    {
        audioMixer.SetFloat("MainVolume", Mathf.Log(value) * 20f);
    }

    private void OnMusicVolumeChanged(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log(value) * 20f);
    }

    private void OnSFXVolumeChanged(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log(value) * 20f);
    }

    public void SetFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
    
}
