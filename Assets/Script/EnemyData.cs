using UnityEngine;

[CreateAssetMenu(menuName ="Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int moveSpeed;
    public int maxHP;
    public int attackPower;
    public EnemyType type;
}

public enum EnemyType
{
    TargetPlayer,
    TargetRepair
}

