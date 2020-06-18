using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform target;
    public LayerMask layerMask;
    public float aimbotDistance = 10f;
    public float seekingDistance = 1f;
    public EnemyConfig config;

    bool foundPlayer = false;
    float timeToFind;
    Vector3 lastSeenPosition;
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = config.CSGoPlayer ? aimbotDistance : seekingDistance;
    }

    void OnEnable()
    {
        if (config.attackPlayerAtStart)
        {
            lastSeenPosition = target.position;
            foundPlayer = true;
        }
        GetComponent<Health>().maxHealth = config.health;
    }

    void Update()
    {
        if (config.CSGoPlayer)
        {
            AimbotMode();
        }
        else
        {
            SeekMode();
        }

        agent.SetDestination(lastSeenPosition);
        Quaternion lookRotation = Quaternion.LookRotation(lastSeenPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, config.turnSpeed);
    }

    /* TODO: 9. If you get bored: watching Thijs and Jotaro, the game seems to "stall" 
     * in that both players aim at each other and keep shooting. On the higher levels, 
     * rather that making the computer player shoot better, let the computer enemy run better, 
     * move sideways while shooting (strafing: https://doom.fandom.com/wiki/Strafing). 
     * If you get carried away, let the enemy hide behind the obstacles. 
     * This will make the game more interesting.
     */
    void SeekMode()
    {
        Vector3 playerDirection = target.position - transform.position;
        float rotationDifference = Vector3.Angle(transform.forward, playerDirection);

        if (Mathf.Abs(rotationDifference) <= config.fieldOfView)
        {
            if (Physics.Raycast(transform.position, playerDirection, out RaycastHit hit, playerDirection.magnitude, layerMask))
            {
                if (foundPlayer = hit.transform.Equals(target))
                {
                    lastSeenPosition = hit.transform.position;
                    transform.Rotate(0, Random.Range(-config.inaccuracy, config.inaccuracy), 0);
                }
            }
        } else
        {
            foundPlayer = false;
        }

        if (!foundPlayer && timeToFind >= config.timeTillSeek)
        {
            Vector3 randomDirection = Random.insideUnitSphere * config.randomStepSpeed;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, config.randomStepSpeed, 1);
            lastSeenPosition = hit.position;

            timeToFind = 0;
        }
        else if (!foundPlayer)
        {
            timeToFind += Time.deltaTime;
        }
    }

    void AimbotMode()
    {
        lastSeenPosition = target.position;
    }

    public void GotShot(GameObject from)
    {
        if (!config.returnsFireOnAttack) return;
        foundPlayer = true;
        lastSeenPosition = from.transform.position;
    }
}
