using UnityEngine;

public class Shooting : MonoBehaviour
{
    public float maxRayDistance = 20f;
    public LayerMask hitLayers;
    // TODO: 6. A clever way of keeping track of hits might be to make the damage/second dependent on how precisely you hit the opponent, rather than having a step function hit/no hit.
    public int damage = 2;
    public bool isUpper = true;
    public AudioClip defaultClip;
    public AudioClip wallClip;
    public AudioClip hitClip;

    public float fireSpreadAngle = 2f;
    public Transform enemyTransform;

    AudioSource audioSource;
    AudioClip _currentClip;
    LineRenderer lineRenderer;
    PantoHandle handle;

    AudioClip currentClip
    {
        get => _currentClip;
        set
        {
            if (_currentClip == null) _currentClip = value;
            else if (!currentClip.Equals(value))
            {
                _currentClip = value;
                audioSource.Stop();
                audioSource.clip = value;
                audioSource.Play();
            }
        }
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = defaultClip;

        GameObject panto = GameObject.Find("Panto");
        if (isUpper)
        {
            handle = panto.GetComponent<UpperHandle>();
        } else
        {
            handle = panto.GetComponent<LowerHandle>();
        }
    }

    void Update()
    {
        //Fire();
        FireCone();
    }

    /// <summary>
    /// Fire gun with aiming assistance.
    /// </summary>
    void FireCone()
    {
        RaycastHit hit;

        // Getting upper rotation only for player interesting
        if (isUpper)
            transform.rotation = Quaternion.Euler(0, handle.GetRotation(), 0);

        Vector3 enemyDirection = enemyTransform.position - transform.position;
        float rotationDifference = Vector3.Angle(transform.forward, enemyDirection);

        if (Mathf.Abs(rotationDifference) <= fireSpreadAngle)
        {
            if (Physics.Raycast(transform.position, enemyDirection, out hit, maxRayDistance, hitLayers))
            {
                lineRenderer.SetPositions(new Vector3[] { transform.position, hit.point });

                Health enemy = hit.transform.GetComponent<Health>();

                if (enemy)
                {
                    enemy.TakeDamage(damage, gameObject);

                    currentClip = hitClip;
                }
                else
                {
                    currentClip = wallClip;
                }
            }
        }
        else
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxRayDistance, hitLayers))
            {
                lineRenderer.SetPositions(new Vector3[] { transform.position,
                    hit.point });

                Health enemy = hit.transform.GetComponent<Health>();

                if (enemy)
                {
                    enemy.TakeDamage(damage, gameObject);

                    currentClip = hitClip;
                }
                else
                {
                    currentClip = wallClip;
                }
            }
            else
            {
                lineRenderer.SetPositions(new Vector3[] { transform.position,
                    transform.position + transform.forward * maxRayDistance });
                currentClip = defaultClip;
            }
            
        }
    }

    /// <summary>
    /// Simple firing in forward direction. Doesn't require a target.
    /// </summary>
    void Fire()
    {
        RaycastHit hit;

        if (isUpper)
            transform.rotation = Quaternion.Euler(0, handle.GetRotation(), 0);

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxRayDistance, hitLayers))
        {
            lineRenderer.SetPositions(new Vector3[] { transform.position, hit.point });

            Health enemy = hit.transform.GetComponent<Health>();

            if (enemy) {
                enemy.TakeDamage(damage, gameObject);

                currentClip = hitClip;
            } else
            {
                currentClip = wallClip;
            }
        } else
        {
            lineRenderer.SetPositions(new Vector3[] { transform.position,
                transform.position + transform.forward * maxRayDistance });
            currentClip = defaultClip;
        }
    }
}
