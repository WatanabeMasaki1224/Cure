using UnityEngine;
using UnityEngine.SceneManagement;

public class StarttButton : MonoBehaviour
{
    [SerializeField]  FadeManager fadeManager;

    public void Started()
    {
        fadeManager.FadeOut();
    }
}
