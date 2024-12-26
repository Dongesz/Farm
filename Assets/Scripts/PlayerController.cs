using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private Animator animator;

    // Sebesség változók
    public float moveSpeed = 5f;  // Alap mozgás sebesség
    public float runSpeed = 10f;  // Futás sebesség
    public float smoothness = 0.1f;  // Finomhangolás mértéke
    public float acceleration = 10f;  // Gyorsulás
    public float deceleration = 15f; // Lassulás

    private Vector2 currentVelocity;

    private float moveX;
    private float moveY;

    private bool isRunning = false;

    void Start()
    {

    }

    void Update()
    {
        HandleActionsInput();  // Futás input
    }

    void FixedUpdate()
    {
        HandleMovementInput();
        UpdateAnimator();
    }

    // Mozgás kezelése (séta, futás)
    void HandleMovementInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");  // Bal-jobb nyilak vagy A/D
        moveY = Input.GetAxisRaw("Vertical");    // Fel-le nyilak vagy W/S

        // Ha futunk, nagyobb sebesség, ha csak sétálunk, akkor lassabb
        float targetSpeed = isRunning ? runSpeed : moveSpeed;

        // A karakter mozgása, átlós mozgás esetén sebesség normalizálás
        Vector2 targetVelocity = new Vector2(moveX, moveY).normalized * targetSpeed;

        // A gyorsulás és lassulás, hogy simább mozgást érjünk el
        if (moveX != 0 || moveY != 0)  // Ha van mozgás
        {
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, acceleration * Time.deltaTime);
        }
        else  // Ha nincs mozgás
        {
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, Vector2.zero, deceleration * Time.deltaTime);
        }
    }

    // Különböző akciók, mint ugrás, guggolás, stb.
    void HandleActionsInput()
    {
        // Futás ellenőrzése
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }
    void UpdateAnimator()
    {
        if (animator != null)
        {
            // Debugging kiírás
            // Debug.Log($"Horizontal: {moveX}, Vertical: {moveY}");

            if (moveX != 0 || moveY != 0)
            {
                animator.SetBool("IsMoving", true);  // Ha mozog, IsMoving true
            }
            else
            {
                animator.SetBool("IsMoving", false); // Ha nem mozog, IsMoving false
            }

            // Beállítjuk a vízszintes és függőleges mozgást
            animator.SetFloat("Horizontal", moveX);
            animator.SetFloat("Vertical", moveY);
        }
    }


}
