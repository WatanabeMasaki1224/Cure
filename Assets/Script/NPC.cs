using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public  bool revived = false;
    public GameObject magic;
  
    public void Revaived()
    {
        if (revived)
        {
            return;
        }

        revived = true;
        Debug.Log("•œŠˆ");
    }

   
}
