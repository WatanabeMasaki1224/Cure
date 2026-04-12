using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [SerializeField] float limitTime = 60f;
    float currentTime;

    [SerializeField] Text timeText;
    [SerializeField] GameObject clearPanel;
    [SerializeField] ClearUI clearUI;

    bool isFinished = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentTime = limitTime;
        clearPanel.SetActive(false);
    }

    void Update()
    {
        if (isFinished) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            Finish();
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        timeText.text = "Time : " + currentTime.ToString("F1");
    }

    void Finish()
    {
        isFinished = true;
        clearUI.Show();
        Time.timeScale = 0f; // ƒQ[ƒ€’âŽ~
    }
}
