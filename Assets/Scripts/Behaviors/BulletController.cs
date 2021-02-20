using UnityEngine;

public class BulletController : MonoBehaviour
{
    private GameObject TargetGameObject;
    private Vector3 targetPosition;
    private Vector3 TargetPosition
    {
        get
        {
            try
            {
                targetPosition = TargetGameObject.transform.position;
            }
            catch { }
            return targetPosition;
        }
    }
    private bool setup;
    private AudioSource audioSource;
    private bool destroy;
    private bool hidden;
    private bool IsRocket;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private float lifeTime = 0;
    private void Update()
    {
        if (!setup)
            return;

        var tPos = TargetPosition;

        if (transform.position != tPos)
        {
            float angle = Mathf.Atan2(tPos.y - transform.position.y, tPos.x - transform.position.x);
            transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg + 180);

            if (IsRocket)
                lifeTime += Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, tPos, Time.deltaTime * (IsRocket ? lifeTime * 25 : 50));
        }
        else if (!hidden)
        {
            hidden = true;
            if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                spriteRenderer.enabled = false;

                if (IsRocket)
                    transform.GetChild(0).gameObject.SetDisable();
            }
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

    public void Setup(Vector3 startPosition, GameObject targetGameObject, bool isAmmunition)
    {
        transform.position = startPosition;
        TargetGameObject = targetGameObject;
        IsRocket = isAmmunition;

        setup = true;
    }
}