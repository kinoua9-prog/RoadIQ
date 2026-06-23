using UnityEngine;
using UnityEngine.InputSystem;

public class CarMover : MonoBehaviour
{
    public bool isHorizontal = true;

    private bool isDragging = false;
    private Vector3 offset;
    private Camera cam;
    private Collider2D col;

    void Start()
    {
        cam = Camera.main;
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
            worldPos.z = 0;

            if (col.OverlapPoint(worldPos))
            {
                isDragging = true;
                offset = transform.position - worldPos;
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
            worldPos.z = 0;

            Vector3 target = worldPos + offset;

            if (isHorizontal)
                transform.position = new Vector3(target.x, transform.position.y, transform.position.z);
            else
                transform.position = new Vector3(transform.position.x, target.y, transform.position.z);
        }
    }
}