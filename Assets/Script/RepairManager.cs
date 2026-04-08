using System.Collections.Generic;
using UnityEngine;

public class RepairManager : MonoBehaviour
{
    public static RepairManager Instance;

    public List<RepairPoint> points = new List<RepairPoint>();

    void Awake()
    {
        Instance = this;
    }

    public void Register(RepairPoint point)
    {
        points.Add(point);
    }

    public void Unregister(RepairPoint point)
    {
        points.Remove(point);
    }

    public RepairPoint GetClosestTarget(Vector2 pos)
    {
        float minDist = Mathf.Infinity;
        RepairPoint closest = null;

        foreach (var p in points)
        {
            if (p.state == RepairPoint.RepairState.Repairing || p.state == RepairPoint.RepairState.Repaired)
            {
                float dist = Vector2.Distance(pos, p.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = p;
                }
            }
        }

        return closest;
    }
}
