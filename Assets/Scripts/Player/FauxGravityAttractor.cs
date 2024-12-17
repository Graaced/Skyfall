using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravityAttractor : MonoBehaviour 
{

	public static FauxGravityAttractor instance;
    public float gravity = -10f;

    private SphereCollider SphereCollider;

    void Awake()
    {
        instance = this;
        SphereCollider = GetComponent<SphereCollider>();
    }


    public void Attract(Rigidbody body)
    {
        Vector3 gravityUp = (body.position - transform.position).normalized;
        body.AddForce(gravityUp * gravity);

        RotateBody(body);
    }

    public void PlaceOnSurface(Rigidbody body)
    {
        body.MovePosition((body.position - transform.position).normalized * (transform.localScale.x * SphereCollider.radius));

        RotateBody(body);
    }

    void RotateBody(Rigidbody body)
    {
        Vector3 gravityUp = (body.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.FromToRotation(body.transform.up, gravityUp) * body.rotation;
        body.MoveRotation(Quaternion.Slerp(body.rotation, targetRotation, 50f * Time.deltaTime));
    }

}
