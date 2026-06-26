using UnityEngine;
using UnityEngine.InputSystem;

public class GridCar : MonoBehaviour
{
    public bool isHorizontal = true;
    public int length = 2;

    public int gridX;
    public int gridY;

    public bool isMainCar = false;
    public int winX = 4;
    public int winY = 6;
    public Vector3 exitOffset = new Vector3(4f, 0f, 0f);

    public float moveSpeed = 30f;
    public float moveCooldown = 0.15f;
    public float exitSpeed = 10f;

    [Header("Sound")]
    public AudioSource moveAudio;

    private float nextMoveTime = 0f;

    private Vector2 startMouse;
    private bool dragging;

    private Camera cam;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isExiting = false;

    private void Start()
    {
        cam = Camera.main;

        if (moveAudio == null)
            moveAudio = GetComponent<AudioSource>();

        transform.position = GridManager.Instance.GridToWorld(gridX, gridY, isHorizontal, length);
        targetPosition = transform.position;

        GridManager.Instance.RegisterCar(this);
    }

    private void Update()
    {
        if (isMoving || isExiting)
        {
            float speed = isExiting ? exitSpeed : moveSpeed;

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;

                StopMoveSound();

                if (isExiting)
                {
                    Debug.Log("LEVEL COMPLETE");

                    if (WinManager.Instance != null)
                        WinManager.Instance.ShowWinPanel();
                }

                isMoving = false;
                isExiting = false;
            }

            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryStartDrag();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            dragging = false;
        }

        if (!dragging) return;

        Vector2 currentMouse = Mouse.current.position.ReadValue();
        Vector2 delta = currentMouse - startMouse;

        if (delta.magnitude < 80) return;

        if (isHorizontal)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x > 0)
                    TryMove(1, 0);
                else
                    TryMove(-1, 0);

                startMouse = currentMouse;
            }
        }
        else
        {
            if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
            {
                if (delta.y > 0)
                    TryMove(0, 1);
                else
                    TryMove(0, -1);

                startMouse = currentMouse;
            }
        }
    }

    private void TryStartDrag()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();

        Vector3 worldPos3 = cam.ScreenToWorldPoint(screenPos);
        Vector2 worldPos = new Vector2(worldPos3.x, worldPos3.y);

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);

        GridCar bestCar = null;
        int bestOrder = -999;

        foreach (Collider2D hit in hits)
        {
            GridCar car = hit.GetComponent<GridCar>();

            if (car == null)
                continue;

            SpriteRenderer sr = car.GetComponent<SpriteRenderer>();
            int order = sr != null ? sr.sortingOrder : 0;

            if (order > bestOrder)
            {
                bestOrder = order;
                bestCar = car;
            }
        }

        if (bestCar == this)
        {
            dragging = true;
            startMouse = Mouse.current.position.ReadValue();
        }
    }

    private void TryMove(int dx, int dy)
    {
        if (Time.unscaledTime < nextMoveTime)
            return;

        if (MovesManager.Instance.gameOver)
            return;

        int newX = gridX + dx;
        int newY = gridY + dy;

        if (!GridManager.Instance.CanPlace(this, newX, newY))
            return;

        GridManager.Instance.UnregisterCar(this);

        gridX = newX;
        gridY = newY;

        targetPosition = GridManager.Instance.GridToWorld(gridX, gridY, isHorizontal, length);
        isMoving = true;

        PlayMoveSound();

        GridManager.Instance.RegisterCar(this);

        nextMoveTime = Time.unscaledTime + moveCooldown;

        MovesManager.Instance.UseMove();

        CheckWin();

        dragging = false;
    }

    private void CheckWin()
    {
        if (!isMainCar)
            return;

        if (gridX == winX && gridY == winY)
        {
            GridManager.Instance.UnregisterCar(this);

            targetPosition = transform.position + exitOffset;
            isExiting = true;
            dragging = false;

            PlayMoveSound();

            Debug.Log("WIN START");
        }
    }

    private void PlayMoveSound()
    {
        if (moveAudio == null)
            return;

        moveAudio.pitch = Random.Range(0.95f, 1.05f);
        moveAudio.Play();
    }

   private void StopMoveSound()
{
    // нічого не робимо
}

    public void InitCar(int x, int y, bool horizontal, int carLength, bool mainCar)
    {
        gridX = x;
        gridY = y;
        isHorizontal = horizontal;
        length = carLength;
        isMainCar = mainCar;

        transform.position = GridManager.Instance.GridToWorld(
            gridX,
            gridY,
            isHorizontal,
            length
        );

        targetPosition = transform.position;

        GridManager.Instance.RegisterCar(this);
    }
}