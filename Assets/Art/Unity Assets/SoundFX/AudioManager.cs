using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip buttonCLickSound;
    public AudioClip sliderChangeSound;
    public AudioClip dropdownChangeSound;
    [Header("Game Sounds")]
    public AudioClip playerJumpSound;
    public AudioClip playerAttacksound;
    public AudioClip playerHurtSound;
    public AudioClip enemyDeathSound;
    public AudioClip enemyAttackSound;
    public AudioClip enemyHurtSound;
    public AudioClip itemPickupSound;

    [Header("UI Components")]
    public Slider slider;
    public Dropdown dropdown;

    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();

        if (slider != null)
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        if (dropdown != null)
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }
    public void PlaySFX(AudioClip clip)
    {
        if(clip !=null)
        sfxSource.PlayOneShot(clip);
    }

    public void OnSliderValueChanged(float value) 
    {
        PlaySFX(sliderChangeSound);
    }

    public void OnDropdownValueChanged(int index) 
    {
        PlaySFX(dropdownChangeSound);
    }
    public void PlayButtonClick()
    {
        PlaySFX(buttonCLickSound);
    }


}
