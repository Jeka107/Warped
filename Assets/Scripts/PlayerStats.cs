using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    enum potions {Hp,Mp}

    [Header("HP")]
    [SerializeField] public float playerHP=100;
    [SerializeField] public float hpIncreaseOnRest = 5;
    [SerializeField] public float timeOfResetHP = 10f;

    [Header("MP")]
    [SerializeField] public float playerMP = 100;
    [SerializeField] public float mpIncreaseOnRest = 5; 
    [SerializeField] public float timeOfResetMP= 10f;

    [Header("Potions")]
    [SerializeField] public Bandages_Script bandages;
    [SerializeField] public Adrenalin_Script adrenalin;

    [Space]
    [SerializeField] private float waitTimeBetweenAdd;

    [HideInInspector] public float maxHp;
    [HideInInspector] public float maxMp;
    private float time;
    private bool needHp;
    private bool needMp;

    [Header("UI")]
    [SerializeField] public GameObject deathScreen;
    [SerializeField] public GameObject bloodFeedback;
    private float BloodOpacity;
    private Image bloodImage;
    private PlayerMovement playerMovement;
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        maxHp = playerHP;
        maxMp = playerMP;
    }

    private void Update()
    {
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            playerHP -= 10;
            playerMP -= 20;
        }

        bloodImage = bloodFeedback.GetComponent<Image>();
        var tempColor = bloodImage.color;
        tempColor.a = BloodOpacity;
        bloodImage.color = tempColor;
        BloodOpacity = 1 - (playerHP * 0.01f);
        
        time += Time.deltaTime;
        var seconds = time % 60;//count seconds

        //reset hp and mp after time.
        if(seconds >= timeOfResetHP&& playerHP<=maxHp&& seconds >= timeOfResetMP && playerMP <= maxMp)
        {
            StartCoroutine(IncreasStatsOnRest());
        }
    }
    IEnumerator IncreasStatsOnRest() //when player rest increase his stats.
    {
        if (playerHP < maxHp)
        {
            playerHP += hpIncreaseOnRest;
        }
        if (playerMP < maxMp)
        {
            playerMP += mpIncreaseOnRest;
        }
        yield return new WaitForSeconds(waitTimeBetweenAdd);//time between adding.
    }
    public bool IncreaseHp() //used when collectin bandges.
    {
        var healthBoost = bandages.healthBoost;
        CheckStats(potions.Hp);

        if (needHp)
        {
            playerHP += healthBoost;
            needHp = false;
            time = 0; //on action reset timer.

            return true;
        }
        return false;
    }
    public void ReduceHp(int healthReduce) //when player get hit reduce HP.
    {
        playerHP -= healthReduce;
        time = 0;

        //Hit feedback

        if (playerHP<=0)
        {
            deathScreen.SetActive(true);
            playerMovement.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Destroy(gameObject);
        }
    }
    public bool IncreaseMp()//used when collectin adrenalin.
    {
        var adrenalinBoost = adrenalin.manaBoost;
        CheckStats(potions.Mp);

        if (needMp)
        {
            playerMP += adrenalinBoost;
            needMp = false;
            time = 0;

            return true;
        }
        return false;
    }
    public void ReduceMp(int adrenalinReduce)//when player use power reduce MP.
    {
        time = 0;
        if (!(playerMP<=0))
        {
            playerMP -= adrenalinReduce;
        }
    }

    private void CheckStats(potions potion)
    {
        switch(potion)
        {
            case potions.Hp:
                if (playerHP <= maxHp - bandages.healthBoost) //for not passing maximum
                {
                    needHp = true;
                }
                else if (playerHP < maxHp)
                {
                    playerHP = maxHp;
                    return;
                }
                else
                {
                    needHp = false;
                }
                break;
            case potions.Mp:
                if (playerMP < maxMp - adrenalin.manaBoost) //for not passing maximum
                {
                    needMp = true;
                }
                else if (playerMP < maxMp)
                {
                    playerMP = maxMp;
                    return;
                }
                else
                {
                    needMp = false;
                }
                break;
        }
    }

}
