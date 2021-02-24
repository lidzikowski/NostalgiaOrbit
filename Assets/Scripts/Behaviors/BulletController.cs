using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core;
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
    private bool IsAmmunition;

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

        if (Vector2.Distance(transform.position, tPos) > 1)
        {
            float angle = Mathf.Atan2(tPos.y - transform.position.y, tPos.x - transform.position.x);
            transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg + 180);

            lifeTime += Time.deltaTime * 2;

            transform.position = Vector3.MoveTowards(transform.position, tPos, (Time.deltaTime * (IsRocket ? 20 : IsAmmunition ? 200 : 50)) * lifeTime);
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

    public void Setup(Vector3 startPosition, GameObject targetGameObject, ResourceTypes resource)
    {
        transform.position = startPosition;
        TargetGameObject = targetGameObject;
        IsRocket = DLLHelpers.IsRocketType(resource);
        IsAmmunition = DLLHelpers.IsAmmunitionType(resource);

        setup = true;
    }
}