using UnityEngine;
using UnityEngine.InputSystem;

public enum ExitDirection
{
    Right,
    Down
}

public class GridCar : MonoBehaviour
{
    public bool isHorizontal = true;
    public int length = 2;

    public int gridX;
    public int gridY;

    public bool isMainCar = false;
    public int winX = 4;
    public int winY = 6;

    [Header("Exit")]
    public ExitDirection exitDirection = ExitDirection.Right;
    public float exitDistance = 4f;

    public float moveSpeed = 55f;
    public float moveCooldown = 0f;
    public float exitSpeed = 12f;

    [Header("Mobile Control")]
    public float swipeThreshold = 45f;

    [Header("Sound")]
    public AudioSource moveAudio;

    private static bool anyCarMoving = false;

    private float nextMoveTime = 0f;
    private Vector2 startInput;
    private Vector2 currentInput;
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

    private void OnDisable()
    {
        if (isMoving || isExiting || dragging)
        {
            anyCarMoving = false;
            isMoving = false;
            isExiting = false;
            dragging = false;
        }
    }

    private void Update()
    {
        ReadInput();

        if (isMoving || isExiting)
        {
            MoveToTarget();
            return;
        }

        HandleDragMovement();
    }

    private void ReadInput()
    {
        bool pressed = false;
        bool released = false;
        bool hasInput = false;

        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            currentInput = touch.position.ReadValue();
            pressed = touch.press.wasPressedThisFrame;
            released = touch.press.wasReleasedThisFrame;

            if (touch.press.isPressed || pressed || released)
                hasInput = true;
        }

        if (!hasInput && Mouse.current != null)
        {
            currentInput = Mouse.current.position.ReadValue();
            pressed = Mouse.current.leftButton.wasPressedThisFrame;
            released = Mouse.current.leftButton.wasReleasedThisFrame;
            hasInput = true;
        }

        if (!hasInput)
            return;

        if (pressed)
            TryStartDrag(currentInput);

        if (released)
            dragging = false;
    }

    private void HandleDragMovement()
    {
        if (!dragging)
            return;

        if (anyCarMoving && !isMoving && !isExiting)
            return;

        Vector2 delta = currentInput - startInput;

        if (delta.magnitude < swipeThreshold)
            return;

        if (isHorizontal)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                TryMove(delta.x > 0 ? 1 : -1, 0);
                startInput = currentInput;
            }
        }
        else
        {
            if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
            {
                TryMove(0, delta.y > 0 ? 1 : -1);
                startInput = currentInput;
            }
        }
    }

    private void MoveToTarget()
    {
        float speed = isExiting ? exitSpeed : moveSpeed;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.unscaledDeltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            return;

        transform.position = targetPosition;
        StopMoveSound();

        if (isExiting)
        {
            Debug.Log("LEVEL COMPLETE");

            if (WinManager.Instance != null)
                WinManager.Instance.ShowWinPanel();

            isMoving = false;
            isExiting = false;
            dragging = false;
            anyCarMoving = false;
            return;
        }

        isMoving = false;
        isExiting = false;
        dragging = false;
        anyCarMoving = false;
    }

    private void TryStartDrag(Vector2 screenPos)
    {
        if (anyCarMoving)
            return;

        if (MovesManager.Instance != null && MovesManager.Instance.gameOver)
            return;

        if (cam == null)
            cam = Camera.main;

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
            startInput = screenPos;
        }
    }

    private void TryMove(int dx, int dy)
    {
        if (Time.unscaledTime < nextMoveTime)
            return;

        if (anyCarMoving && !isMoving && !isExiting)
            return;

        if (MovesManager.Instance != null && MovesManager.Instance.gameOver)
            return;

        int oldX = gridX;
        int oldY = gridY;

        int newX = gridX + dx;
        int newY = gridY + dy;

        if (!GridManager.Instance.CanPlace(this, newX, newY))
            return;

        if (UndoManager.Instance != null)
            UndoManager.Instance.SaveMove(this, oldX, oldY);

        GridManager.Instance.UnregisterCar(this);

        gridX = newX;
        gridY = newY;

        targetPosition = GridManager.Instance.GridToWorld(gridX, gridY, isHorizontal, length);
        isMoving = true;
        anyCarMoving = true;

        PlayMoveSound();

        GridManager.Instance.RegisterCar(this);

        nextMoveTime = Time.unscaledTime + moveCooldown;

        if (MovesManager.Instance != null)
            MovesManager.Instance.UseMove();

        CheckWin();
    }

    private void CheckWin()
    {
        if (!isMainCar)
            return;

        if (gridX == winX && gridY == winY)
        {
            GridManager.Instance.UnregisterCar(this);

            Vector3 exitVector = Vector3.right;

            if (exitDirection == ExitDirection.Down)
                exitVector = Vector3.down;

            targetPosition = transform.position + exitVector * exitDistance;

            isExiting = true;
            dragging = false;
            anyCarMoving = true;

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
        // звук не зупиняємо
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