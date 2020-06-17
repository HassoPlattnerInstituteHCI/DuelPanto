using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform target;
    public float turnSpeed = 7f;
    public float fieldOfView = 30;
    public LayerMask layerMask;
    public float aimbotDistance = 10f;
    public float seekingDistance = 1f;
    public float timeTillSeek = 2f;
    public float inaccuracy = 0.2f;
    public bool CSGoPlayer = false;
    public bool returnsFireOnAttack = true;

    bool foundPlayer = false;
    float timeToFind;
    Vector3 lastSeenPosition;
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastSeenPosition = target.position;
        foundPlayer = true;
        agent.stoppingDistance = CSGoPlayer ? aimbotDistance : seekingDistance;
    }

    void Update()
    {
        if (CSGoPlayer)
        {
            AimbotMode();
        }
        else
        {
            SeekMode();
        }

        agent.SetDestination(lastSeenPosition);
        Quaternion lookRotation = Quaternion.LookRotation(lastSeenPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, turnSpeed);
    }

    void SeekMode()
    {
        Vector3 playerDirection = target.position - transform.position;
        Quaternion rotationToPlayer = Quaternion.LookRotation(playerDirection, Vector3.up);
        float rotationDifference = rotationToPlayer.eulerAngles.y - transform.rotation.eulerAngles.y;

        if (Mathf.Abs(rotationDifference) <= fieldOfView)
        {
            if (Physics.Raycast(transform.position, playerDirection, out RaycastHit hit, playerDirection.magnitude, layerMask))
            {
                if (foundPlayer = hit.transform.Equals(target))
                {
                    lastSeenPosition = hit.transform.position;
                    transform.Rotate(0, Random.Range(-inaccuracy, inaccuracy), 0);
                }
            }
        } else
        {
            foundPlayer = false;
        }

        if (!foundPlayer && timeToFind >= timeTillSeek)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 10f, 1);
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
        if (!returnsFireOnAttack) return;
        foundPlayer = true;
        lastSeenPosition = from.transform.position;
    }
}
