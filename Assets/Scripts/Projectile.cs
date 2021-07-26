using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    protected float speed = 20;

	public ProjectileManager Manager { get; set; }

	protected Rigidbody rb;

	protected virtual void Awake() {
		rb = GetComponent<Rigidbody>();
	}

	//This would be OnEnable, but the pool refuses to actually call OnEnable consistently
	public virtual void ResetProjectile(Vector3 position, Quaternion rotation) {
		transform.position = position;
		transform.rotation = rotation;

		rb.velocity = transform.forward * speed;
	}
}
