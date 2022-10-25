using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManagerReference : MonoBehaviour
{
    [SerializeField] GameObject gameManager;

    public void RestartLevelReference()
    {
        gameManager.GetComponent<GameManager>().RestartScene();
        Debug.Log("pressed");
    }

    public void RestartGameReference()
    {
        gameManager.GetComponent<GameManager>().RestartGame();
        Debug.Log("pressed");
    }
}
