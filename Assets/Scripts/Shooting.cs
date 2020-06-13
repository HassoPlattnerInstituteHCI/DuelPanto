using UnityEngine;

public class Shooting : MonoBehaviour
{
    public float maxRayDistance = 20f;
    public LayerMask hitLayers;
    public int damage = 2;
    public bool isUpper = true;

    public AudioClip defaultClip;
    public AudioClip wallClip;
    public AudioClip hitClip;

    private AudioSource audioSource;
    private AudioClip _currentClip;

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

    private LineRenderer lineRenderer;
    private PantoHandle handle;

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
        Fire();
    }

    void Fire()
    {
        lineRenderer.enabled = true;

        RaycastHit hit;

        if (isUpper)
            transform.rotation = Quaternion.Euler(0, handle.getRotation(), 0);

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxRayDistance, hitLayers))
        {
            lineRenderer.SetPositions(new Vector3[] { transform.position, hit.point });

            Health enemy = hit.transform.GetComponent<Health>();

            if (enemy) {
                enemy.TakeDamage(damage);
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
