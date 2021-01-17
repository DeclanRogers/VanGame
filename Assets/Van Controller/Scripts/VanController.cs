using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VanController : MonoBehaviour
{
    [Header("MOVEMENT")]
    [Tooltip("Maximum vehicle speed without the use of boost")]
    public float m_MaxSpeed = 30.0f;
    
    [Tooltip("Vehicle acceleration speed")]
    public float m_AccelerationSpeed = 30.0f;
    
    [Tooltip("Desired turn speed of the vehicle")]
    public float m_TurnSpeed = 20.0f;
    
    [Tooltip("Maximum turn speed of the vehicle (clamping ensures no excess of this speed)")]
    public float m_MaxTurnSpeed = 15.0f;
    
    [Tooltip("How much acceleration and max speed is altered while boost is in use")]
    public float m_BoostPower = 20.0f;
    
    [Tooltip("Alters the fallspeed and weight on the supsension of the vehicle\nNOTE: The same effect can be applied by directly altering the gravity in the project settings which will also keep world consistency.\nSet this to 1 to not use the modifier.")]
    public float m_GravityModifier = 1.5f;
    
    [Space(10.0f)]
    [Tooltip("W/S input axis")]
    public float m_AccelerationInput;
    
    [Tooltip("A/D input axis")]
    public float m_TurnInput;

    private bool m_Grounded = true;
    private bool m_Boost = false;
    private bool m_Drift = false;
    private float m_DriftSmoothing;
    private int m_WheelsOnGround = 0;

    [Header("SUSPENSION")]
    [Min(0.0f)]
    [Tooltip("How far off of the ground the vehicle sits")]
    public float m_SuspensionHeight = 0.6f;

    [Min(0.0f)]
    [Tooltip("Dampens the springy/bouncy behaviour of the vehicle")]
    public float m_Dampening = 1.0f;

    [Min(1.0f)]
    [Tooltip("Affects the rigidity or \"tightness\" of the suspension\nLower = softer\nHigher = harder")]
    public float m_SuspensionStiffness = 15.0f;

    [Tooltip("The offset of the \"wheels\" on the vehicle")]
    public float m_VerticalOffset = 0.5f;
    [Space(10.0f)]

    [Tooltip("Front Left Wheel Position")]
    public Transform m_FrontLeftWheel;
    private float m_FLWheelCompRatio;

    [Tooltip("Front Right Wheel Position")]
    public Transform m_FrontRightWheel;
    private float m_FRWheelCompRatio;

    [Tooltip("Rear Left Wheel Position")]
    public Transform m_RearLeftWheel;
    private float m_RLWheelCompRatio;

    [Tooltip("Rear Right Wheel Position")]
    public Transform m_RearRightWheel;
    private float m_RRWheelCompRatio;

    [Header("DEBUG")]
    public bool m_DebugView = false;
    public float m_FlipForce = 10.0f;
    private Rigidbody m_Rigidbody;

    private void Awake() => m_Rigidbody = GetComponent<Rigidbody>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            ForceFlip();

        m_AccelerationInput = Input.GetAxisRaw("Vertical");
        m_TurnInput = Input.GetAxisRaw("Horizontal");
        m_Boost = Input.GetKey(KeyCode.LeftShift) ? true : false;
        m_Drift = Input.GetKey(KeyCode.Space) ? true : false;
    }

    private void FixedUpdate()
    {
        if (m_Rigidbody.velocity.y != 0.0f)
            m_Rigidbody.velocity += Vector3.up * Physics.gravity.y * 1.5f * Time.fixedDeltaTime;

        CalculateSuspension();

        if (!m_Grounded)
            return;

        Move();
        Rotate();
    }

    private void CalculateSuspension()
    {
        m_WheelsOnGround = 0;
        WheelThing(m_FrontLeftWheel.position, ref m_FLWheelCompRatio, m_SuspensionHeight, m_SuspensionStiffness);
        WheelThing(m_FrontRightWheel.position, ref m_FRWheelCompRatio, m_SuspensionHeight, m_SuspensionStiffness);
        WheelThing(m_RearLeftWheel.position, ref m_RLWheelCompRatio, m_SuspensionHeight, m_SuspensionStiffness);
        WheelThing(m_RearRightWheel.position, ref m_RRWheelCompRatio, m_SuspensionHeight, m_SuspensionStiffness);
        m_Grounded = m_WheelsOnGround >= 2 ? true : false;
    }

    private void WheelThing(Vector3 wheelPos, ref float wheelCompressionRatio, float suspensionHeight, float suspensionStiffness)
    {
        RaycastHit hitInfo;

        bool flGrounded = Physics.Raycast(wheelPos, -transform.up, out hitInfo, suspensionHeight);
        wheelCompressionRatio = 1.0f - hitInfo.distance / suspensionHeight;
        if (flGrounded)
        {
            float springForce = (wheelCompressionRatio * suspensionStiffness) - (m_Dampening * m_Rigidbody.GetPointVelocity(wheelPos).y);
            m_Rigidbody.AddForceAtPosition(transform.up * springForce * m_Rigidbody.mass, wheelPos);
            m_WheelsOnGround++;
            
            if (m_DebugView)
                Debug.DrawRay(wheelPos, -transform.up * hitInfo.distance, Color.green);
        }
        else
        {
            wheelCompressionRatio = 0.0f;

            if (m_DebugView)
                Debug.DrawRay(wheelPos, -transform.up * suspensionHeight, Color.red);
        }
    }

    private void Move()
    {
        Vector3 direction = transform.forward;
        Vector3 accelerationPointOffset = transform.position + transform.forward * 0.5f - transform.up * 0.25f;

        float boost = m_Boost ? m_BoostPower : 0.0f;
        float speed = m_AccelerationInput * m_AccelerationSpeed + boost;

        if (m_AccelerationInput != 0.0f)
            m_Rigidbody.AddForceAtPosition(direction * m_Rigidbody.mass * speed, accelerationPointOffset);
        else
        {
            Vector3 localVel = transform.InverseTransformDirection(m_Rigidbody.velocity);
            m_Rigidbody.AddForceAtPosition(direction * -localVel.z, accelerationPointOffset);
        }

        if (m_Rigidbody.velocity.magnitude > m_MaxSpeed + boost)
            m_Rigidbody.velocity = Vector3.ClampMagnitude(m_Rigidbody.velocity, m_MaxSpeed);
    }

    private void Rotate()
    {
        Vector3 localVel = transform.InverseTransformDirection(m_Rigidbody.velocity);
        float turnMod = m_TurnSpeed * 2.0f - Mathf.Abs(localVel.z);
        float torque = localVel.z * turnMod * m_Rigidbody.mass * m_TurnInput * Time.fixedDeltaTime;

        if (m_Rigidbody.angularVelocity.y > 0.0f && torque < 0.0f || m_Rigidbody.angularVelocity.y < 0.0f && torque > 0.0f)
            torque *= 7.5f;

        m_Rigidbody.AddTorque(transform.up * torque);
        m_Rigidbody.angularVelocity = Vector3.ClampMagnitude(m_Rigidbody.angularVelocity, m_MaxTurnSpeed);

        if (m_Drift)
            m_DriftSmoothing += 1.5f * Time.fixedDeltaTime;
        else
            m_DriftSmoothing -= 1.5f * Time.fixedDeltaTime;

        m_DriftSmoothing = Mathf.Clamp(m_DriftSmoothing, 0.0f, 1.0f);
        localVel.x *= m_DriftSmoothing;
        m_Rigidbody.velocity = transform.TransformDirection(localVel);
    }

    private void ForceFlip()
    {
        Vector3 torque;
        torque.x = Random.Range(-1.0f, 1.0f);
        torque.y = Random.Range(-1.0f, 1.0f);
        torque.z = Random.Range(-1.0f, 1.0f);

        m_Rigidbody.AddTorque(torque * m_FlipForce * m_Rigidbody.mass, ForceMode.Impulse);
        m_Rigidbody.AddForce(Vector3.up * m_FlipForce * m_Rigidbody.mass, ForceMode.Impulse);
    }
}