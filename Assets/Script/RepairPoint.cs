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
    [SerializeField] Color brokenColor = Color.red;
    [SerializeField] Color repairingColor = Color.yellow;
    [SerializeField] Color repairedColor = Color.green;

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
                sr.color = brokenColor;
                break;

            case RepairState.Repairing:
                sr.color = repairingColor;
                break;

            case RepairState.Repaired:
                sr.color = repairedColor;
                break;
        }
    }

    public void TakeDamage()
    {
        if (state == RepairState.Repaired)
        {
            state = RepairState.Broken;
            UpdateColor();

            Debug.Log("Repair Destroyed");

            // スコアマイナスここ
        }
    }
}
