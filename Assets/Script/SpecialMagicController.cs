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
            Debug.Log($"{currentMagic} �̖��@���g�p�I");
            // �����ɖ��@�̔�������������
        }
        else
        {
            Debug.Log("���@�������Ă��܂���");
        }
    }

    public string GetCurrentMagic()
    {
        return currentMagic;
    }


}
