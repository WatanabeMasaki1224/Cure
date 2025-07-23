using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public  bool revived = false;
   
    public string magicName;
  
    public void Revive()
    {
        if (revived)
        {
            return;
        }

        revived = true;
        Debug.Log("•œŠˆ");
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public bool IsRevived()
    {
        return revived;
    }

    public string GetMagicName()
    {
        return magicName;
    }

}
