using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; 
    public int _totalScore;

    private void Awake()
    {
        Instance = this;
    }
    
    public void Add(int score)
    {
        _totalScore += score;
    }

    public void Sub(int score)
    {
        _totalScore -= score;
    }
}
