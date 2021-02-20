using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Vector3 TargetPosition;
    private bool setup;
    private AudioSource audioSource;
    private bool destroy;
    private bool hidden;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!setup)
            return;

        if (transform.position != TargetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, Time.deltaTime * 50);
        }
        else if (!hidden)
        {
            hidden = true;
            if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                spriteRenderer.enabled = false;
        }

        if (destroy)
        {
            Destroy(gameObject);
        }
        else
        {
            if (!audioSource.isPlaying)
                destroy = true;
        }
    }

    public void Setup(Vector3 startPosition, Vector3 targetPosition)
    {
        transform.position = startPosition;
        TargetPosition = targetPosition;

        float angle = Mathf.Atan2(TargetPosition.y - transform.position.y, TargetPosition.x - transform.position.x);
        transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg + 180);

        setup = true;
    }
}