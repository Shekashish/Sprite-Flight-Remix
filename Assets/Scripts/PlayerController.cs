using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // Score
    private float elapsedTime = 0f;
    private float score = 0f;
    public float scoreMultiplier = 10f;

    // Movement
    public float thrustForce = 5f;
    public float maxSpeed = 5f;
    public float rotationSpeed = 300f;
    public float minMouseDistance = 0.5f;

    public GameObject boosterFlame;

    // UI Toolkit
    public UIDocument uiDocument;
    private Label scoreText;
    private Button restartButton;

    // Explosion
    public GameObject explosionEffect;

    // Internal state
    private Rigidbody2D rb;
    private Vector2 thrustDirection;
    private bool isThrusting = false;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;     // start completely still

        // Get UI elements
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        restartButton.style.display = DisplayStyle.None;
        restartButton.clicked += ReloadScene;

        if (boosterFlame != null)
            boosterFlame.SetActive(false);
    }

    void Update()
    {
        if (isDead)
            return;

        // Score update
        elapsedTime += Time.deltaTime;
        score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);
        scoreText.text = "Score: " + score;

        // Only read mouse & rotate while button is held
        isThrusting = Mouse.current.leftButton.isPressed;

        if (isThrusting)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0f;

            Vector2 toMouse = (Vector2)mousePos - rb.position;

            // Avoid crazy spinning when mouse is very close
            if (toMouse.magnitude > minMouseDistance)
            {
                thrustDirection = toMouse.normalized;

                float targetAngle = Mathf.Atan2(thrustDirection.y, thrustDirection.x) * Mathf.Rad2Deg - 90f;
                float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.deltaTime);
                rb.MoveRotation(newAngle);
            }
        }

        if (boosterFlame != null)
            boosterFlame.SetActive(isThrusting);
    }

    void FixedUpdate()
    {
        if (isDead)
            return;

        if (isThrusting)
        {
            // Thrust in the direction the ship is currently facing
            rb.AddForce((Vector2)transform.up * thrustForce);

            // Clamp max speed
#if UNITY_6000_0_OR_NEWER
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
#else
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
#endif
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead)
            return;

        isDead = true;

        if (boosterFlame != null)
            boosterFlame.SetActive(false);

        // Spawn explosion
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // Hide player and show restart
        gameObject.SetActive(false);
        restartButton.style.display = DisplayStyle.Flex;
    }

    void ReloadScene()
    {
        Scene active = SceneManager.GetActiveScene();
        SceneManager.LoadScene(active.name);
    }
}