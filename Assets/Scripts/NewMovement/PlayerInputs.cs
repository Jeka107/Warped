using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
	[Header("Character Input Values")]
	public Vector2 move;
	public Vector3 look;
	public Vector2 mousePosition;
	public bool jump;
	public bool sprint;
	public bool crouch;
	public bool action;
	public bool gravityCanceled;
	[Header("Movement Settings")]
	public bool analogMovement;

	[Header("Mouse Cursor Settings")]
	public bool cursorLocked = true;
	public bool cursorInputForLook = true;

	private PlayerActions playerActions;
	private GameManager gameManager;
	[HideInInspector] public double distance;

	private void Awake()
	{
		playerActions = GetComponent<PlayerActions>();
		gameManager = FindObjectOfType<GameManager>();
	}

	public void OnMove(InputValue value)
	{
		MoveInput(value.Get<Vector2>());
	}

	public void OnLook(InputValue value)
	{
		if (cursorInputForLook)
		{
			LookInput(value.Get<Vector2>());
		}
	}
	public void OnMousePosition(InputValue value)
	{
		MousePositionInput(value.Get<Vector2>());
	}
	public void OnMouseClick()
	{
		playerActions.MouseActions();
	}
	public void OnAction(InputValue value)
    {
		playerActions.ActionOnObject();
	}
	public void OnPickUp(InputValue value)
	{
		playerActions.Telekinesis();
	}
	public void OnGravityDirection(InputValue value)
	{
		playerActions.ChangeGravityDirection(value.isPressed);
	}
	public void OnGravityCancel(InputValue value)
	{
		playerActions.GravityCancel();
		GravityCanceledInput(!value.isPressed);
	}
	public void OnBlackHole()
	{
		playerActions.BlackHole();
	}

	public void OnJump(InputValue value)
	{ 
		JumpInput(value.isPressed);
	}

	public void OnSprint(InputValue value)
	{
		SprintInput(value.isPressed);
	}
	public void OnCrouch(InputValue value)
    {
		CrouchInput(value.isPressed);
	}
	public void OnRestart()//To throw object
	{
		gameManager.RestartScene();
	}
	public void ActionInput(bool newActionState)
    {
		action = newActionState;
	}
	public void MousePositionInput(Vector2 newMousePosition)
    {
		mousePosition = newMousePosition;
	}
	public void MoveInput(Vector2 newMoveDirection)
	{
		move = newMoveDirection;
	}
	public void LookInput(Vector2 newLookDirection)
	{
		look = newLookDirection;
	}
	public void JumpInput(bool newJumpState)
	{
		jump = newJumpState;
	}
	public void SprintInput(bool newSprintState)
	{
		sprint = newSprintState;
	}
	public void CrouchInput(bool newCrouchState)
    {
		crouch = newCrouchState;
    }
	public void GravityCanceledInput(bool newGravityCanceledState)
    {
		gravityCanceled = newGravityCanceledState;
	}
	private void OnApplicationFocus(bool hasFocus)
	{
		SetCursorState(cursorLocked);
	}
	private void SetCursorState(bool newState)
	{
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}
	public void OnItemUse(InputValue value)
	{
		float itemNum = value.Get<float>();
		GetComponent<PlayerActions>().ItemUse(itemNum);
	}
	
}
