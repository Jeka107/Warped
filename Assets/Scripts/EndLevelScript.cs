using UnityEngine;

public class EndLevelScript : MonoBehaviour
{
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))//on collide with player,loading next scene.
        {
            InventoryRemember.Remember(inventorySystem); //remember inventory from prev scene.
            gameManager.LoadNexTScene();
            gameManager.SetLevel();
        }
    }
}
