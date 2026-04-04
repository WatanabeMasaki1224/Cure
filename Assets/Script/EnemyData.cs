using UnityEngine;

[CreateAssetMenu(menuName ="Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float moveSpeed;
    public int maxHP;
    public float attaclPower;
    public EnemyType type;
}

public enum EnemyType
{
    TargetPlayer,
    TargetRepair
}

