using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public Vector2 lookDirection;
		[Header("Movement Settings")]
		public bool analogMovement;
		public GameObject BlackHole;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		[SerializeField] private GameObject hitObject;
		[SerializeField] private GamePlayCanvas gamePlayCanvas;
		
		private MouseClickActions mouseClickActions;
		private bool buttonHeldDown;

		[HideInInspector] public bool activateSkill=false;

        private void Awake()
        {
			mouseClickActions = GetComponent<MouseClickActions>();
		}

#if ENABLE_INPUT_SYSTEM 
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}
		public void OnMousePosition(InputValue value)
		{
			Ray ray = Camera.main.ScreenPointToRay(value.Get<Vector2>());
			lookDirection=Camera.main.WorldToScreenPoint(value.Get<Vector2>()).normalized;
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit))
			{
				Debug.DrawLine(ray.origin, hit.point);

				if (hit.collider != null)
				{
					hitObject = hit.collider.gameObject;

					mouseClickActions.SetHitObject(hitObject);
					mouseClickActions.SetHitPoint(hit.point);

					if ((hit.collider.CompareTag("Draggable")))
					{
						//call function to show Press E
						gamePlayCanvas.ShowPressEText();
					}
					else
					{
						gamePlayCanvas.HidePressEText();
					}
				}
				
			}
			
		}
		public void OnMouseClick()//To throw object
		{
			mouseClickActions.MouseActionSkill();
		}
		public void OnPickUp()//To pick up object
		{
			if (gamePlayCanvas.checkE)
			{
				buttonHeldDown = false;
				mouseClickActions.SetButtonPressed(buttonHeldDown);
				//hitObject.GetComponent<MoveObject>().MoveToPlayer();
			}
		}
		public void OnGravityDirection(InputValue value)//To activate skill
		{
			if(value.isPressed)
			{
				buttonHeldDown = true;
				mouseClickActions.SetButtonPressed(buttonHeldDown);
			}
			else
            {
				buttonHeldDown = false;
				mouseClickActions.SetButtonPressed(buttonHeldDown);
			}
		}
		public void OnGravityCancel()//To activate skill
		{
			if (!activateSkill)
			{
				//gravityController.GravityCancelActive();
				activateSkill = true;
			}
			else
            {
				//gravityController.GravityCancelDeActive();
				activateSkill = false;
			}
		}

		public void OnBlackHole()
        {
			Instantiate(BlackHole);
        }

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
#endif


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
		
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}