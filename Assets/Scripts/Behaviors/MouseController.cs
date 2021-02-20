using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerController))]
public class MouseController : MonoBehaviour
{
    private PlayerController PlayerController;


    [SerializeField]
    public RectTransform MiniMapTransform;



    private void Start()
    {
        PlayerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                    MiniMapDetect();

                return;
            }

            if (Input.GetMouseButtonDown(0))
                Controller();
            else if (Input.GetMouseButton(0))
                Controller(false);
        }
    }

    private void Controller(bool pressed = true)
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
        bool can = hit.collider != null;

        if (pressed && can)
        {
            switch (hit.transform.tag)
            {
                case "Ship":
                    var ship = hit.transform.gameObject.GetComponent<ShipController>();

                    if (ship.MapObject.Hitpoints <= 0)
                    {
                        PlayerController.DisposeMapObject(ship.MapObject.Id);
                        break;
                    }

                    PlayerController.LocalShipController.SelectedMapObject = ship;

                    break;
                case "Ore":

                    ChangeTargetPosition(OreController.GetOrePosition(hit.transform));

                    break;
            }
        }
        else if (!can)
        {
            if (Camera.main == null)
            {
                return;
            }

            ChangeTargetPosition(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z * -1)));
        }
    }

    public int map_x = 1600;
    public int map_y = 1000;

    public float x = 154;
    public float y = 96;

    private void MiniMapDetect()
    {
        Vector2 localMousePosition = MiniMapTransform.InverseTransformPoint(Input.mousePosition);
        if (MiniMapTransform.rect.Contains(localMousePosition))
        {
            var position = new Vector2(((localMousePosition.x + MiniMapTransform.rect.width / 2) / x) * map_x, ((localMousePosition.y - MiniMapTransform.rect.height / 2) / y) * map_y);
            //Debug.LogError(position);
            ChangeTargetPosition(position);
        }
    }

    public void ChangeTargetPosition(Vector3 position)
    {
        PlayerController.LocalShipController?.OnNewTargetPosition(position);
    }
}