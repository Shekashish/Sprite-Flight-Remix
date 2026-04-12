using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float thrustForce = 1f;
    public float maxSpeed = 5f;
    public GameObject boosterFlame;


    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            // Calculate mouse direction
            Vector3  mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            Vector2 direction = (mousePos - transform.position).normalized; 

            // Move player in the direction of the mouse
            transform.up = direction;
            rb.AddForce(direction * thrustForce);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                boosterFlame.SetActive(true);
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                boosterFlame.SetActive(false);
            }
        }
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
