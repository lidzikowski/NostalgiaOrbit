using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FooterButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public GameObject ItemsGameobject;

    [SerializeField]
    public GameObject[] OtherItems;

    public bool goUp;
    public bool goDown;
    public bool enter;

    private Vector2 minVectorY;
    private Vector2 maxVectorY;

    private static float speedUp = 150;
    private static float speedDown = 20;

    private RectTransform Rect => transform.GetComponent<RectTransform>();

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Click);

        var rect = Rect;

        minVectorY = new Vector2(rect.anchoredPosition.x, -37);
        maxVectorY = new Vector2(rect.anchoredPosition.x, -10);
    }

    private void Update()
    {
        if (goUp)
        {
            Up();
        }
        else if (goDown)
        {
            Down();
        }
    }

    public void StartAnimation()
    {
        goUp = true;
        goDown = false;
    }

    private void Up()
    {
        var rect = Rect;

        if (rect.anchoredPosition != maxVectorY)
        {
            rect.anchoredPosition = Vector3.MoveTowards(rect.anchoredPosition, maxVectorY, Time.deltaTime * speedUp);
        }
        else if (!enter)
        {
            goUp = false;
            goDown = true;
        }
    }

    private void Down()
    {
        var rect = Rect;

        if (rect.anchoredPosition != minVectorY)
        {
            rect.anchoredPosition = Vector3.MoveTowards(rect.anchoredPosition, minVectorY, Time.deltaTime * speedDown);
        }
        else
        {
            goUp = false;
            goDown = false;
        }
    }

    public void Click()
    {
        if (OtherItems?.Any() ?? false)
        {
            foreach (var item in OtherItems)
            {
                item.SetDisable();
            }
        }

        ItemsGameobject?.SetEnable();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        enter = true;
        goUp = true;
        goDown = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        enter = false;
        goUp = false;
        goDown = true;
    }
}