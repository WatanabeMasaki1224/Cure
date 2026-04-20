using UnityEngine;

public class RepairPoint : MonoBehaviour
{
    public enum RepairState
    {
        Broken,
        Repairing,
        Repaired
    }

    public RepairState state = RepairState.Broken;
    [SerializeField] float repairTime = 3f;
    float timer = 0f;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Sprite brokenSprite;
    [SerializeField] Sprite repairingSprite;
    [SerializeField] Sprite repairedSprite;
    [SerializeField] int repairScore = 50;
    [SerializeField] int breakRepairPontScore = 30;
    [SerializeField] int sustainableScore = 2;

    private void Start()
    {
        UpdateColor();
    }

    public void StartRepair()
    {
        if (state == RepairState.Broken)
        {
            state = RepairState.Repairing;
            timer = 0f;
            UpdateColor();
        }
    }

    public void StopRepair()
    {
        if(state == RepairState.Repairing)
        {
            state = RepairState.Broken;
            timer = 0f;
            UpdateColor();
        }
    }

    public bool UpdateRepair(float deltaTime)
    {
        if (state != RepairState.Repairing) return false;
        
        timer += deltaTime;

        if(timer > repairTime)
        {
            state =RepairState.Repaired;
            UpdateColor();
            ScoreManager.Instance.Add(repairScore);
            return true;
        }
        return false;
    }

    void OnEnable()
    {
        if(RepairManager.Instance != null)
        {
            RepairManager.Instance.Register(this);
        }
    }

    void OnDisable()
    {
        if(RepairManager.Instance != null)
        {
            RepairManager.Instance.Unregister(this);
        }
    }

    void UpdateColor()
    {
        switch (state)
        {
            case RepairState.Broken:
                sr.sprite = brokenSprite;
                break;

            case RepairState.Repairing:
                sr.sprite = repairingSprite;
                break;

            case RepairState.Repaired:
                sr.sprite = repairedSprite;
                break;
        }
    }

    public void TakeDamage()
    {
        if (state == RepairState.Repaired)
        {
            state = RepairState.Broken;
            UpdateColor();
            ScoreManager.Instance.Sub(breakRepairPontScore);
            Debug.Log("Repair Destroyed");
        }
    }
}
