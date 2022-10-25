using System.Collections.Generic;
using UnityEngine;

public class GravityController:MonoBehaviour
{
    public delegate void OnGravityChange();
    public static event OnGravityChange onGravityChange;

    public delegate void OnGravityStatus(bool status);
    public static event OnGravityStatus onGravityStatus;

    [SerializeField] private GravitationalRadius gravitationalRadius;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private List<GameObject> draggableObjects;
    [SerializeField] private GameObject rooms;

    [Header("TutorialTirgger")]
    [SerializeField] private QuestData questData;
    [HideInInspector] public bool cockPitRoom=false;
    [HideInInspector] public bool gravityCanceled;

    private float gravity = 9.81f;
    private Rigidbody rb;
    private GameObject currentObject;
    private readonly float CAMERA_OFFSET = 0.5f;
    private Vector3 direction=new Vector3(0f,-9.81f,0f);

    [HideInInspector] public DirectionProps directionProps;
    private void Start()
    {
        questData = FindObjectOfType<QuestManager>().ReturnIndexQuestData(11);//after quest number 10 activate stuff.
    }
    public void GravityCancelActive()//deactive gravitation ,depents on radius from the player
    {
        if (questData.isComplete)
        {
            //DestroyTriggers
            Destroy(FindObjectOfType<CockPointTriggerOn>());
            Destroy(FindObjectOfType<CockPointTriggerOff>());
            cockPitRoom = false;
        }

        if (cockPitRoom) { return; }

        else
        {
            foreach (GameObject gameObject in draggableObjects)
            {
                int x = Random.Range(0, 3);
                int y = Random.Range(0, 3);
                int z = Random.Range(0, 3);


                rb = gameObject.GetComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = false;
                rb.velocity = -direction / 9.81f;
                rb.angularVelocity = new Vector3(x, y, z);
            }
        }
        
        onGravityStatus?.Invoke(true);
    }
    public void GravityCancelDeActive()//Activate gravitation no depends on radius
    {
        foreach (MoveObject moveObject in FindObjectsOfType<MoveObject>())
        {
            rb = moveObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
        }
        onGravityStatus?.Invoke(false);
    }
    public void GravityDirection(Vector3 hitObjectPoint,Vector3 hitPoint)
    {
        directionProps = DirectionCalc(hitObjectPoint,hitPoint);
        var _gravity = directionProps._sign == SIGN.POSITIVE ? gravity : -gravity;

        switch (directionProps._axis)
        {
            case AXIS.X:
                direction = new Vector3(_gravity, 0, 0);
                break;
            case AXIS.Y:
                direction = new Vector3(0, _gravity, 0);
                break;
            case AXIS.Z:
                direction = new Vector3(0, 0, -_gravity);
                break;
        }

        player.SetGravityDirection(direction);
        onGravityChange?.Invoke();

        if (questData.isComplete)
        {
            foreach (MoveObject moveObject in UnityEngine.Object.FindObjectsOfType<MoveObject>())
            {
                currentObject = moveObject.gameObject;
                currentObject.GetComponent<Rigidbody>().isKinematic = false;
                GravityDirectionAction(direction);
            }
        }
        else if(rooms!=null) // the problem in the floating lockers might be here!!!
        {
            foreach (MoveObject moveObject in rooms.GetComponentsInChildren<MoveObject>())
            {
                currentObject = moveObject.gameObject;
                currentObject.GetComponent<Rigidbody>().isKinematic = false;
                GravityDirectionAction(direction);
            }
        }
    }

    public DirectionProps DirectionCalc(Vector3 hitObjectPoint, Vector3 hitPoint)
    {
        DirectionProps directionProps = null;

        if (Mathf.Abs(hitObjectPoint.x) == Mathf.Abs(hitPoint.x) + CAMERA_OFFSET)
        {
            directionProps = new DirectionProps(DirectionCalcHelper(hitObjectPoint.x), AXIS.X);
        }
        else if (Mathf.Abs(hitObjectPoint.y) == Mathf.Abs(hitPoint.y) + CAMERA_OFFSET)
        {
            directionProps = new DirectionProps(DirectionCalcHelper(hitObjectPoint.y), AXIS.Y);
        }
        else if (Mathf.Abs(hitObjectPoint.z) == Mathf.Abs(hitPoint.z) + CAMERA_OFFSET)
        {
            directionProps = new DirectionProps(DirectionCalcHelper(-hitObjectPoint.z), AXIS.Z);
        }

        return directionProps;
    }

    public SIGN DirectionCalcHelper(float value)
    {
        return value >= 0 ? SIGN.POSITIVE : SIGN.NEGATIVE;
    }
    public void GravityDirectionAction(Vector3 direction)
    {
        Physics.gravity = direction;
    }

    //list gameobject adding and removing
    public void AddToList(GameObject draggable)
    {
        draggableObjects.Add(draggable);
    }
    public void RemoveFromList(GameObject draggable)
    {
        draggableObjects.Remove(draggable);
    }
    public bool CheckIfDraggableObjectsEmpty()
    {
        if (draggableObjects.Count == 0)
            return true;
        else
            return false;
    }
}
