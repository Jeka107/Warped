using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    private GameMaster gm;

     void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {

            gm.lastCheckpointPos = transform.position;
            Debug.Log("OK");
            Debug.Log(gm.lastCheckpointPos);
        }
    }
}
