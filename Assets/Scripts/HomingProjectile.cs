using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class HomingProjectile : Projectile
{
	[SerializeField]
	private float turnSpeed = 3;
	
	private Transform target;

	private void Start() {
		//This is not the best way to do this, but it's quick and easy for a simple test
		target = GameObject.Find("HomingTarget").transform;
	}

	public override void ResetProjectile(Vector3 position, Quaternion rotation) {
		base.ResetProjectile(position, rotation);
		GetComponent<TrailRenderer>().Clear();
	}

	private void FixedUpdate() {
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position),
			turnSpeed * Time.fixedDeltaTime);
		rb.velocity = transform.forward * speed;
	}

	private void OnTriggerEnter(Collider other) {
		if (other.name == "HomingTarget")//Again, not the best way, but quick and easy for testing
			Manager.DespawnProjectile(this);
	}
}
