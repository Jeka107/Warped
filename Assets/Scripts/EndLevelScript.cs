using UnityEngine;

public class EndLevelScript : MonoBehaviour
{
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryRemember.Remember(inventorySystem);
            gameManager.LoadNexTScene();
            gameManager.SetLevel();
        }
    }
}
