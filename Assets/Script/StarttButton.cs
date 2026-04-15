using UnityEngine;
using UnityEngine.SceneManagement;

public class StarttButton : MonoBehaviour
{
    public void Started()
    {
        SceneManager.LoadScene("GameScean");
    }
}
