using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDanim : MonoBehaviour
{
    public Animator hudAnim;
    public bool notAnimated = false;
    [SerializeField] float timeBetweenAnim =12f;
    [SerializeField] float AnimtionTime= 4f;
    WaitForSeconds WFS;
    // Start is called before the first frame update
    void Start()
    {
        hudAnim.ResetTrigger("Circle");
       
    }

    // Update is called once per frame
    void Update()
    {
        if (notAnimated == false)
        {
            StartCoroutine(AnimDelay());
        }
        else
        {
            StartCoroutine(AnimDelayReset());
        }
       
    }
    IEnumerator AnimDelay()
    {
        //not animated time
      
        hudAnim.SetTrigger("Circle");
      
        hudAnim.ResetTrigger("falseTrigger");
       
        
        yield return new WaitForSeconds(timeBetweenAnim);
        notAnimated = true;
    }
    IEnumerator AnimDelayReset()
    {
        //animated time

        hudAnim.ResetTrigger("Circle");
        hudAnim.SetTrigger("falseTrigger");
        
        yield return new WaitForSeconds(AnimtionTime);
        notAnimated = false;

    }
}
