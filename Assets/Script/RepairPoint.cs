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
    
    public void StartRepair()
    {
        if (state == RepairState.Broken)
        {
            state = RepairState.Repairing;
            timer = 0f;
        }
    }

    public void StopRepair()
    {
        if(state == RepairState.Repairing)
        {
            state = RepairState.Broken;
            timer = 0f;
        }
    }

    public bool UpdateRepair(float deltaTime)
    {
        if (state != RepairState.Repairing) return false;
        
        timer += deltaTime;

        if(timer > repairTime)
        {
            state =RepairState.Repaired;
            return true;
        }
        return false;
    }

    void OnEnable()
    {
        RepairManager.Instance.Register(this);
    }

    void OnDisable()
    {
        RepairManager.Instance.Unregister(this);
    }
}
