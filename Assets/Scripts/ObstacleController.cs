using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [Header("Tipo de Obstáculo")]
    public ObstacleType type;

    [Header("Movimiento")]
    public Vector3 moveDirection = Vector3.right;
    public float moveSpeed = 2f;
    public float moveDistance = 5f;
    private Vector3 startPosition;

    [Header("Rotación")]
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 50f;

    [Header("Daño")]
    public bool causesRestart = true;
    public int timePenalty = 5; // Segundos de penalización

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        switch (type)
        {
            case ObstacleType.Static:
                // No hace nada
                break;

            case ObstacleType.Moving:
                MoveObstacle();
                break;

            case ObstacleType.Rotating:
                RotateObstacle();
                break;

            case ObstacleType.MovingAndRotating:
                MoveObstacle();
                RotateObstacle();
                break;
        }
    }

    void MoveObstacle()
    {
        float movement = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        transform.position = startPosition + moveDirection.normalized * movement;
    }

    void RotateObstacle()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (causesRestart)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.RestartLevel();
                }
            }
            else
            {
                // Aplicar penalización de tiempo
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.timeRemaining -= timePenalty;
                }
            }
        }
    }
}

public enum ObstacleType
{
    Static,
    Moving,
    Rotating,
    MovingAndRotating
}