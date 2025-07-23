using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialMagicController : MonoBehaviour
{
    private string currentMagic;
    public void SetSpecialMagic(string magicName)
    {
        currentMagic = magicName;
    }

    public void UseSpecialMagic(string magicName)
    {
        currentMagic = magicName;
    }

    public void UseSpecialMagi()
    {
        if (!string.IsNullOrEmpty(currentMagic))
        {
            Debug.Log($"{currentMagic} の魔法を使用！");
            // ここに魔法の発動処理を書く
        }
        else
        {
            Debug.Log("魔法を持っていません");
        }
    }

    public string GetCurrentMagic()
    {
        return currentMagic;
    }


}
