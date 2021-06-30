using UnityEngine;

[RequireComponent(typeof(HingeJoint2D))]
public class HingeJointMotor : MonoBehaviour
{
    [SerializeField] private HingeJoint2D hingeJoint2D;

    [SerializeField] private float maxMotorTorque = 100f;

    [SerializeField] private float motorSpeedMultiplier = 10f;

    [SerializeField] private float _startAngle;

    
    private void Start()
    {
        hingeJoint2D = GetComponent<HingeJoint2D>();
        _startAngle = hingeJoint2D.jointAngle;
    }
    
    private void FixedUpdate()
    {
        var jointAngleDelta = hingeJoint2D.jointAngle;// - _startAngle;

        hingeJoint2D.motor = new JointMotor2D()
        {
            maxMotorTorque = maxMotorTorque,
            motorSpeed = -(jointAngleDelta * motorSpeedMultiplier)
        };
    }
}
