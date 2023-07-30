using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodySpatialTemporal : SpatialTemporal
{
    private Rigidbody _rigidbody;

    protected override void Start()
    {
        base.Start();
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected override SpatialTemporalState GetState()
    {
        _rigidbody.isKinematic = false;
        
        return base.GetState();
    }

    protected override void SetState(SpatialTemporalState state)
    {
        _rigidbody.isKinematic = true;
        base.SetState(state);
    }

    private void OnCollisionEnter(Collision other)
    {
        InteractWithOtherRigidBodyTemporal(other.gameObject.GetComponent<RigidbodySpatialTemporal>());
    }

    private void OnCollisionStay(Collision other)
    {
        InteractWithOtherRigidBodyTemporal(other.gameObject.GetComponent<RigidbodySpatialTemporal>());
    }

    private void InteractWithOtherRigidBodyTemporal(RigidbodySpatialTemporal other)
    {
        if (other == null || Reversing)
            return;

        var thisRigidbodyStationary = _rigidbody.velocity.magnitude < float.Epsilon;
        var otherRigidbodyStationary = other._rigidbody.velocity.magnitude < float.Epsilon;

        if (thisRigidbodyStationary && otherRigidbodyStationary)
            return;

        LockedEnd = CurrentFrame;
    }
}