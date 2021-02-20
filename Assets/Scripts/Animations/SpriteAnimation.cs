using NostalgiaOrbitDLL;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimation : MonoBehaviour
{
    [SerializeField]
    public bool BlockFrame = false;

    [Header("Delay between change sprite")]
    [SerializeField]
    public float FrameDelay = 0.02f;

    [Header("Rotating everytime")]
    [SerializeField]
    public bool Rotate_Always;
    [SerializeField]
    public bool Rotate_ClockDirection = true;
    [SerializeField]
    public float Rotate_FrameDelay = 0.02f;

    [Header("Prefab type")]
    public PrefabTypes SpriteResource;
    private Sprite[] spriteFrames;

    [Header("Sprite when use object")]
    [SerializeField]
    public Sprite Active_Sprite;
    [SerializeField]
    public float Active_FrameTime = 0;

    [Header("Destroy when after last sprite")]
    [SerializeField]
    public bool DestroyAfetrLastSprite;

    [Header("Prefab change type on event")]
    [SerializeField]
    public bool ChangePrefab;
    [SerializeField]
    public PrefabTypes ChangeSpriteResource;



    private SpriteRenderer spriteRenderer;
    private bool doRotating;
    private int currentFrame = 0;
    private int targetFrame = 0;
    private float framePerAngle;
    private bool calculateClockDirection;
    private bool canRotate;



    private void Start()
    {
        if (spriteRenderer == null)
        {
            ChangePrefabModel(SpriteResource);
        }
    }

    private void Update()
    {
        if (BlockFrame)
            return;

        if (!doRotating && canRotate)
        {
            if (Rotate_Always)
            {
                doRotating = true;

                StartCoroutine(Rotating(Rotate_FrameDelay, Rotate_ClockDirection));
            }
            else if (targetFrame != currentFrame)
            {
                doRotating = true;

                StartCoroutine(Rotating(FrameDelay, calculateClockDirection));
            }
        }
    }

    //void LateUpdate()
    //{
    //    spriteRenderer.sortingOrder = (int)Camera.main.WorldToScreenPoint(spriteRenderer.bounds.min).y;
    //}

    IEnumerator Rotating(float delay, bool inClockToward = true)
    {
        yield return new WaitForSeconds(delay);

        if (inClockToward)
            currentFrame++;
        else
            currentFrame--;

        if (currentFrame >= spriteFrames.Length)
        {
            currentFrame = 0;

            if (DestroyAfetrLastSprite)
            {
                Destroy(gameObject);
            }
        }
        else if (currentFrame < 0)
            currentFrame = spriteFrames.Length - 1;

        RenderFrame(currentFrame);

        doRotating = false;
    }

    public void RenderFrame(int frame)
    {
        if (spriteFrames.Length < frame)
            return;

        spriteRenderer.sprite = spriteFrames[frame];
    }

    private bool CalculateDirection()
    {
        var inLeft = targetFrame - currentFrame;
        if (inLeft < 0)
            inLeft += spriteFrames.Length + 1;

        var inRight = currentFrame - targetFrame;
        if (inRight < 0)
            inRight += spriteFrames.Length + 1;

        return inRight >= inLeft;
    }

    public int CalculateFrameToPosition(Transform startPosition, Vector3 position)
    {
        if (startPosition.position == position)
            return -1;

        Vector3 relative = startPosition.InverseTransformPoint(position);
        var angle = Mathf.Atan2(relative.y, -relative.x) * Mathf.Rad2Deg;
        if (angle < 0f)
            angle += 360f;

        var target = Mathf.FloorToInt(angle / framePerAngle);

        if (target >= spriteFrames.Length)
            target = spriteFrames.Length - 1;

        return target;
    }

    public void RotateToPosition(Vector3 position)
    {
        if ((Vector2)transform.position == (Vector2)position)
            return;

        var frame = CalculateFrameToPosition(transform, (Vector2)position);

        if (frame == -1)
            return;

        targetFrame = frame;
        calculateClockDirection = CalculateDirection();
    }

    public void RotateToObject(GameObject gameObject)
    {
        targetFrame = CalculateFrameToPosition(transform, gameObject.transform.position);
        calculateClockDirection = CalculateDirection();
    }

    public void ChangePrefabModel(PrefabTypes prefabType, bool mainSpriteType = true)
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (mainSpriteType)
        {
            SpriteResource = prefabType;
        }
        else
        {
            ChangeSpriteResource = prefabType;
        }

        spriteFrames = Helpers.LoadSpritesResource(prefabType);

        framePerAngle = 360 / spriteFrames.Length;
        canRotate = spriteFrames.Length > 0;

        RenderFrame(0);
    }

    public void ActiveSprite()
    {
        doRotating = true;

        StopCoroutine(nameof(Rotating));

        StartCoroutine(RenderActiveSprite());
    }

    private IEnumerator RenderActiveSprite()
    {
        spriteRenderer.sprite = Active_Sprite;

        yield return new WaitForSeconds(Active_FrameTime);

        doRotating = false;
    }
}