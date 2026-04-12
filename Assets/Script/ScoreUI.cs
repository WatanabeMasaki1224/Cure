using UnityEngine;
using UnityEngine.UI;
public class ScoreUI : MonoBehaviour
{
    [SerializeField] Text scoreText;

    void Update()
    {
        scoreText.text = "Score : " + ScoreManager.Instance._totalScore;
    }
}
