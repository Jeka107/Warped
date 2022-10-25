using Enums;
using UnityEngine;
using System;

public class PlayerActions : MonoBehaviour
{
	public delegate void OnPress(KeyCode button,GameObject hitObject);
	public event OnPress onPress;

	public delegate void OnQuest();
	public event OnQuest onQuest;

	[Header("Canvas")]
	[SerializeField] private GamePlayCanvas gamePlayCanvas;
	[SerializeField] private GameObject spaceHelmet;

	[Space(10)]
	[SerializeField] public GravityController gravityController;
	[SerializeField] private GameObject blackHole;

	[Header("Skill Mp Cost")]
	[SerializeField] public int BlackholeMpCost;
	[SerializeField] public int TelekinesisMpCost;
	[SerializeField] public int GravityCancelMpCost;

	[Header("Hands")]
	[SerializeField] private GameObject lHand;
	[SerializeField] private GameObject lHandWithWeapon;

	[Header("Inventory")]
	private InventorySystem inventorySystem;
	[SerializeField] private InventoryUI inventoryUI;
	[SerializeField] public InventoryItem holdingItem;

	[Header("Quest")]
	[SerializeField] public QuestManager questManager;
	[SerializeField] private int distanceToQuestObject;

	[Header("Animation")]
	[SerializeField] private HandsAnimation handsAnimation;

	[Space]
	[SerializeField] private LayerMask layerMask;

	[Header("For Me")]
	public GameObject hitObject;

	[Header("Effects")]
	[SerializeField] private GameObject telekanesiesVisualEffect;
	[SerializeField] private GameObject gravityCancleVisualEffect;
	[SerializeField] private GameObject gravityChangeVisualEffect;

	[HideInInspector] public bool activateSkill = false;

	private PlayerInputs playerInputs;
	private MouseClickActions mouseClickActions;
	private PlayerMovement playerMovement;
	private bool buttonHeldDown;
	private Quest currentQuest;
	private double distance;
	private int enemyKill;
	

	[HideInInspector] public KeyCode keyCode;



	private void Awake()
	{
		mouseClickActions = GetComponent<MouseClickActions>();
		playerInputs = GetComponent<PlayerInputs>();
		playerMovement = GetComponent<PlayerMovement>();
		inventorySystem = FindObjectOfType<InventorySystem>();
		Cursor.lockState = CursorLockMode.Locked;

		EnemyAI.onKill += QuestAction;
	}
    private void OnDestroy()
    {
		EnemyAI.onKill -= QuestAction;
	}
    private void Update()
    {
		ActionTextAppearance();
	}

	#region TextAppearance
	public void ActionTextAppearance()
    {
		Ray ray = Camera.main.ScreenPointToRay(playerInputs.mousePosition);
		var lookDirection = Camera.main.WorldToScreenPoint(playerInputs.mousePosition).normalized;
		
		RaycastHit hit;

		//show text UI depends on which object player looking at.
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
		{
			Debug.DrawLine(ray.origin, hit.point);
			distance = Vector3.Distance(hit.point, ray.origin);

			currentQuest = questManager?.quest;
			hitObject = hit.collider.gameObject;

			mouseClickActions.SetHitObject(hitObject);
			mouseClickActions.SetHitPoint(hit.point);
			if (currentQuest != null)
			{
				gamePlayCanvas.HideQuestPressText();
				gamePlayCanvas.HidePressQText();

				if (currentQuest == hitObject.GetComponent<QuestObject>()?.quest && currentQuest.button == KeyCode.E)
				{
					//gamePlayCanvas.HidePressEText();
					gamePlayCanvas.ShowQuestPressText();
				}
				else if (currentQuest == hitObject.GetComponent<QuestObject>()?.quest && distance <= distanceToQuestObject)
				{
					//gamePlayCanvas.HidePressEText();
					gamePlayCanvas.ShowPressQText();
					gamePlayCanvas.ShowQuestPressText();
				}
				else if (currentQuest.goalType == GOALTYPE.InputAction|| currentQuest.goalType == GOALTYPE.Kill)
				{
					gamePlayCanvas.ShowQuestPressText();
				}

				else if (((hit.collider.CompareTag(ITEM_COLLECT_TAGS.Collectable.ToString())) ||
					(hit.collider.CompareTag(ITEM_COLLECT_TAGS.AttackCollectable.ToString())) ||
					(hit.collider.CompareTag(HIT_OBJECT_TAGS.Useable.ToString()))) && distance <= distanceToQuestObject)
				{
					gamePlayCanvas.ShowPressQText();
				}
			}
		}
	}
	#endregion

	#region MouseActions
	public void MouseActions()
	{
		if (Enum.TryParse(typeof(KEY_CODES),keyCode.ToString(),out object key))//if activate telekanisis or gravity change skill.
		{
			keyCode = KeyCode.None;
			mouseClickActions.MouseActionSkill();	
		}
		else if(holdingItem?.data.itemPrefab.tag==ITEM_COLLECT_TAGS.AttackCollectable.ToString()) //else do simple attack.
        {
			mouseClickActions.MouseActionAttack();
		}
	}
    #endregion

    #region Action- E Button
    public void Telekinesis()
    {
		QuestAction(KeyCode.E);
		if (!questManager.GetIfKeyIsActiveFirstTime(KeyCode.E)) { return; }

		if ((GetComponent<PlayerStats>().playerMP >= TelekinesisMpCost))//if player got enough MP.
		{
			buttonHeldDown = false;
			mouseClickActions.SetButtonPressed(buttonHeldDown);
			hitObject.GetComponent<MoveObject>()?.MoveToPlayer(hitObject);

			//activate object to move to player and play animation and effects..
			if (hitObject.GetComponent<MoveObject>())
			{
				handsAnimation.TelePullAnimation();
				GetComponent<PlayerStats>().ReduceMp(TelekinesisMpCost);
				telekanesiesVisualEffect.SetActive(true);
			}
		}
	}
	#endregion

	#region Action-Hold F button
	public void ChangeGravityDirection(bool input)
    {
		keyCode = KeyCode.Mouse0;
		if (input)
		{
			buttonHeldDown = true;
			mouseClickActions.SetButtonPressed(buttonHeldDown);
			gravityChangeVisualEffect.SetActive(true);
		}
		else
		{
			buttonHeldDown = false;
			mouseClickActions.SetButtonPressed(buttonHeldDown);
			keyCode = KeyCode.None;
			gravityChangeVisualEffect.SetActive(false);
		}
	}
	#endregion

	#region Action-Tap F buttom
	public void GravityCancel()
    {
		keyCode = KeyCode.F;
		QuestAction(KeyCode.F);
		if (!questManager.GetIfKeyIsActiveFirstTime(KeyCode.F) &&!buttonHeldDown) { return; } //checkin if button can press down.

		if (!activateSkill) //if skill not activated.
		{
			gravityController.GravityCancelActive(); //activate skill.

			if (!gravityController.CheckIfDraggableObjectsEmpty())//checking if there is draggable object in range.
			{

				//activate skill and reduce mp.
				GetComponent<PlayerStats>().ReduceMp(GravityCancelMpCost);
				activateSkill = true;
				gravityCancleVisualEffect.SetActive(true);
			}
		}
		else
		{
			//deactivate skill but don't reduce MP.
			gravityController.GravityCancelDeActive();
			activateSkill = false;
			gravityCancleVisualEffect.SetActive(false);
		}
	}
	#endregion

	#region Action-R button
	public void BlackHole()
    {
		if (GetComponent<PlayerStats>().playerMP >= BlackholeMpCost)//if player got enough mp.
		{
			//activate skill and animation.
			handsAnimation.BlackHoleAnimation();
			Instantiate(blackHole);
			GetComponent<PlayerStats>().ReduceMp(BlackholeMpCost);
		}
		QuestAction(KeyCode.R);
	}
	#endregion

	#region Action-Q button
	public void ActionOnObject()
	{
		QuestAction(KeyCode.Q);

		//do action depends on object the player looking at.
		if (hitObject.tag == ITEM_COLLECT_TAGS.Collectable.ToString()|| hitObject.tag == ITEM_COLLECT_TAGS.AttackCollectable.ToString())
		{
			hitObject.GetComponent<ItemObject>()?.OnPickUp();
		}
		if(hitObject.tag=="Useable")
        {
			hitObject.GetComponent<CardReader>()?.UseObject(holdingItem.data);
		}
		handsAnimation.InteractAnimation();
	}
	#endregion

	#region QuestAction
	public void QuestAction(KeyCode keyCode)
	{
		currentQuest = questManager?.quest;

		if (currentQuest != null)
		{
			if (currentQuest.button == keyCode)
			{
				switch (currentQuest.goalType)//check quest depends on quest type.
				{
					case GOALTYPE.ActionOnObject:
						if (currentQuest == hitObject.GetComponent<QuestObject>()?.quest && currentQuest.button == KeyCode.E)
						{
							onPress.Invoke(keyCode, hitObject);
						}
						else if (currentQuest == hitObject.GetComponent<QuestObject>()?.quest && distance <= distanceToQuestObject)
						{
							if(hitObject.tag==HIT_OBJECT_TAGS.Wearable.ToString())//if hitobject is wearable.
                            {
								spaceHelmet.SetActive(true);
								//Put on The Wearable object.
                            }
							onPress.Invoke(keyCode, hitObject);
						}
						break;
					case GOALTYPE.ActionOnObjectWithItem:
						if (currentQuest == hitObject.GetComponent<QuestObject>()?.quest && distance <= distanceToQuestObject)
                        {
							if(holdingItem.data== hitObject.GetComponent<QuestObject>()?.quest.itemToUse)
                            {
								onPress.Invoke(keyCode, hitObject);
							}
                        }
						break;
					case GOALTYPE.InputAction:
						onPress.Invoke(keyCode, hitObject);
						break;
				}
			}
			
		}
	}
	public void QuestAction(Quest quest)//used to check kill enemy type quest.
	{
		currentQuest = questManager?.quest;
		if (currentQuest != null)
		{
			if (currentQuest.goalType == quest.goalType)
			{
				enemyKill++;
				if (enemyKill == currentQuest.killRequiredAmount)
				{
					onQuest.Invoke();
					enemyKill = 0;
				}
			}
		}
	}

	#endregion

	#region InventoryUse
	public void ItemUse(float itemNum)
	{
		holdingItem = inventorySystem.ItemUse((int)itemNum);

		if (holdingItem != null)
		{
			SelectedItem(holdingItem);

			//bandages and adrenalin are Consumeble items.
			if (holdingItem.data.itemPrefab.tag== ITEM_COLLECT_TAGS.ConsumeCollectable.ToString())
			{
				var itemValue = inventorySystem.Get(holdingItem.data);

				itemValue.data.itemPrefab.TryGetComponent<Adrenalin_Script>(out Adrenalin_Script adrenalin);
				itemValue.data.itemPrefab.TryGetComponent<Bandages_Script>(out Bandages_Script bandages);

				if (bandages != null)
				{
					lHand.SetActive(true);
					lHandWithWeapon.SetActive(false);

					var bandageUsed = GetComponent<PlayerStats>().IncreaseHp();
					if (bandageUsed)
					{
						inventorySystem.Remove(holdingItem.data);
					}
				}

				if (adrenalin != null)
				{
					lHand.SetActive(true);
					lHandWithWeapon.SetActive(false);

					var adrnelinUsed = GetComponent<PlayerStats>().IncreaseMp();
					if (adrnelinUsed)
					{
						inventorySystem.Remove(holdingItem.data);
					}
				}
			}
			//weapon
			else if (holdingItem.data.itemPrefab.tag == ITEM_COLLECT_TAGS.AttackCollectable.ToString())
            {
				playerMovement.SetLHandWeapon();
				lHand.SetActive(false);
				lHandWithWeapon.SetActive(true);
			}
			//item that not consumed by the player.
			else if (holdingItem.data.itemPrefab.tag == ITEM_COLLECT_TAGS.Collectable.ToString())
            {
				playerMovement.SetLeftHand();
				lHand.SetActive(true);
				lHandWithWeapon.SetActive(false);
			}
		}
        else
        {
			playerMovement.SetLeftHand();
			lHand.SetActive(true);
			lHandWithWeapon.SetActive(false);
		}
	}
	private void SelectedItem(InventoryItem holdingItem) //interface for selected item.
	{
		inventoryUI.SelectedItem(holdingItem);
	}

	#endregion

	public void SetKeyCode(KeyCode _keyCode)
    {
		keyCode = _keyCode;
    }
}
