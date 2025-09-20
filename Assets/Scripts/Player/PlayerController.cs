using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float runSpeed = 8f, groundAccel = 40f, groundDecel = 60f;
    public Rigidbody2D rb { get; private set; }
    IPlayerState _current;
    public Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        SwitchState(new IdleState());
    }

    void Update()
    {
        _current?.Tick();
    }

    public void SwitchState(IPlayerState next)
    {
        _current?.Exit();
        _current = next;
        _current.Enter(this);
    }
    public void OnMove(InputAction.CallbackContext c)
    {
        moveInput = c.ReadValue<Vector2>();
    }
}
