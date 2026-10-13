using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LighterPlayerController : MonoBehaviour
{
    private Rigidbody body;
    private Animator animator;
    private Vector3 desired;
    private bool jump;
    private bool grounded;
    private float nextJump;
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (GameManager.instance == null || !GameManager.instance.IsPlaying) { desired = Vector3.zero; jump = false; return; }
        var axes = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) + LighterInput.Direction, 1);
        var cam = Camera.main.transform;
        desired = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized * axes.y + Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized * axes.x;
        if (Input.GetKeyDown(KeyCode.Space) || LighterInput.Jump) jump = true;
        LighterInput.Jump = false;
        if (transform.position.y < -3 && GameManager.instance.IsPlaying) GameManager.instance.EndGame("You stepped off the arena.");
    }
    private void FixedUpdate()
    {
        grounded = Physics.Raycast(transform.position + Vector3.up * .1f, Vector3.down, .22f, ~0, QueryTriggerInteraction.Ignore);
        var velocity = body.linearVelocity;
        velocity.x = desired.x * 3.5f;
        velocity.z = desired.z * 3.5f;
        if (jump && grounded && Time.time >= nextJump)
        {
            velocity.y = 4;
            nextJump = Time.time + .7f;
            if (animator != null) animator.SetTrigger("Jump");
            LighterAudio.Play(420, .075f, .12f);
        }
        jump = false;
        body.linearVelocity = velocity;
        if (desired.sqrMagnitude > .01f) body.MoveRotation(Quaternion.Slerp(body.rotation, Quaternion.LookRotation(desired), Time.fixedDeltaTime * 15));
        if (animator != null) { animator.SetFloat("MoveSpeed", desired.magnitude); animator.SetBool("Grounded", grounded); }
    }
}
