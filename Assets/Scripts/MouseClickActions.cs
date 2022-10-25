using UnityEngine;

public class MouseClickActions:MonoBehaviour
{
    [SerializeField] private HandsAnimation handsAnimation;
    [SerializeField] private WeaponAnimation weaponAnimation;
    [SerializeField] public int GravityChangeMpCost;
    [SerializeField] private AudioSource playerAttackingSound;

    private PlayerActions playerActions;
    private QuestManager questManager;
    private PlayerStats playerStats;
    private bool buttonHeld;
    private GameObject hitObject;
    private Vector3 hitPoint;

    public AudioSource throwingSoundEffect;
    public GameObject telekanesiesVisualEffect;
    private void Awake()
    {
        playerActions = GetComponent<PlayerActions>();
        questManager = FindObjectOfType<QuestManager>();
        playerStats = GetComponent<PlayerStats>();
    }
    public void SetButtonPressed(bool buttonHeld)
    {
        this.buttonHeld = buttonHeld;
    }
    public void SetHitObject(GameObject hitObject)
    {
        this.hitObject = hitObject;
    }
    public void SetHitPoint(Vector3 hitPoint)
    {
        this.hitPoint = hitPoint;
    }
    public void MouseActionAttack()
    {
        //simple attack
        weaponAnimation?.AttackAnimation(true);
        playerAttackingSound?.Play();
    }

    public void MouseActionSkill()
    {
        switch (hitObject?.tag)
        {
            case "Shoot":
                hitObject.GetComponent<MoveObject>().ThrowObject();
                handsAnimation.TelePullReleaseAnimation();
                telekanesiesVisualEffect.SetActive(false);
                //throwingSoundEffect.GetComponent<AudioSource>().Play();
                playerActions.QuestAction(KeyCode.Mouse0);
                break;
            case "Wall":
                if (buttonHeld&&buttonHeld&&playerStats.playerMP >= GravityChangeMpCost)
                {
                    playerActions.QuestAction(KeyCode.Mouse0);
                    if (!playerActions.questManager.GetIfKeyIsActiveFirstTime(KeyCode.Mouse0)) { return; }

                    GetComponent<PlayerActions>().gravityController.GravityDirection(hitObject.transform.position, hitPoint);
                    GetComponent<PlayerStats>().ReduceMp(GravityChangeMpCost);
                }
                break;
        }
    }
}
