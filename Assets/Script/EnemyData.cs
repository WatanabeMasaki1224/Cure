using UnityEngine;

[CreateAssetMenu(menuName ="Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int moveSpeed;
    public int maxHP;
    public int attackPower;
    public EnemyType type;
    public GameObject prefab;
    public int maxCount;     // ‚±‚Ì“G‚ÌÅ‘å”
    public float spawnRate;  // oŒ»—¦id‚İ
    public int score;
}

public enum EnemyType
{
    TargetPlayer,
    TargetRepair
}

