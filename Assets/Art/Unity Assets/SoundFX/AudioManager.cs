using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
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
    public AudioClip playerMovementSound;
    public AudioClip playerDeathSound;
    public AudioClip playerDashSound;
    public AudioClip waterSound;
    public AudioClip enemyDeathSound;
    public AudioClip enemyAttackSound;
    public AudioClip enemyBlockSound;
    public AudioClip enemyHurtSound;
    public AudioClip enemyMovementSound;
    public AudioClip flyingEnemySound;
    public AudioClip flyingEnemyAttackSound;
    public AudioClip flyingEnemyHurtSound;
    public AudioClip flyingEnemyDeathSound;
    public AudioClip itemPickupSound;
    public AudioClip bossmoveSound;
    public AudioClip bossHurtSound;
    public AudioClip bossAttackSound;
    public AudioClip bossDeathSound;
    public AudioClip bossMagicSound;
    public AudioClip levelCompleteSound;
    public AudioClip magicSound;

    [Header("UI Components")]
    public Slider slider;
    public Dropdown dropdown;

    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    private void Start()
    { 
        musicSource.loop = true;
        musicSource.clip = backgroundMusic;
        musicSource.Play();

        if (slider != null)
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        if (dropdown != null)
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }
    public void PlaySFX(AudioClip clip)
    {
        if(clip !=null && sfxSource !=null)
        sfxSource.PlayOneShot(clip);
    }
    // --- UI Methods ---
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
    // --- Player Methods for Animation Events ---
    public void PlayPlayerJump()
    {
        PlaySFX(playerJumpSound);
    }
    public void PlayPlayerAttack()
    {
        PlaySFX(playerAttacksound);
    }
    public void PlayPlayerHurt(){
        PlaySFX(playerHurtSound);
    }

    public void PlayPlayerMovement(){
        PlaySFX(playerMovementSound);
    }
    public void PlayPlayerDeath()
    {
        PlaySFX(playerDeathSound);
    }
    public void PlayPlayerDash()
    {
        PlaySFX(playerDashSound);
    }
    public void PlayWaterSound(){
        PlaySFX(waterSound);
    }

    // --- Enemy Methods for Animation Events ---
    public void PlayEnemyDeath()
    {
        PlaySFX(enemyDeathSound);
    }
    public void PlayEnemyAttack() 
    {
        PlaySFX(enemyAttackSound);
    }
    public void PlayEnemyHurt() 
    {
        PlaySFX(enemyHurtSound);
    }
    public void PlayEnemyMovement() 
    {
        PlaySFX(enemyMovementSound);
    }
    public void PlayEnemyBlock() 
    {
        PlaySFX(enemyBlockSound);
    }
    public void PlayFlyingEnemySound() 
    {
        PlaySFX(flyingEnemySound);
    }
    public void PlayFlyingEnemyAttack() 
    {
        PlaySFX(flyingEnemyAttackSound);
    }
    public void PlayFlyingEnemyHurt() 
    {
        PlaySFX(flyingEnemyHurtSound);
    }
    public void PlayFlyingEnemyDeath() 
    {
        PlaySFX(flyingEnemyDeathSound);
    }


    // --- Item Methods ---
    public void PlayItemPickup()
    {
        PlaySFX(itemPickupSound);
    }
    // --- Boss Methods for Animation Events ---
    public void PlayBossMove()
    {
        PlaySFX(bossmoveSound);
    }
    public void PlayBossHurt()
    {
        PlaySFX(bossHurtSound);
    }
    public void PlayBossAttack()
    {
        PlaySFX(bossAttackSound);
    }
    public void PlayBossDeath()
    {
        PlaySFX(bossDeathSound);
    }
    public void PlayBossMagic()
    {
        PlaySFX(bossMagicSound);
    }
    // music for magic
    public void PlayMagicSound()
    {
                PlaySFX(magicSound);
    }
    // --- Level Methods ---
    public void PlayLevelComplete()
    {
        PlaySFX(levelCompleteSound);
    }



}
