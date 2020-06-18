using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/EnemyConfig", order = 1)]
public class EnemyConfig : ScriptableObject
{
    public int health = 100;
    public float speed = 3;
    public bool CSGoPlayer = false;
    public float turnSpeed = 6;
    public float randomStepSpeed = 8;
    public float fieldOfView = 30;
    public float timeTillSeek = 2f;
    public float inaccuracy = 0.2f;
    public bool returnsFireOnAttack = true;
    public bool attackPlayerAtStart = true;
}
