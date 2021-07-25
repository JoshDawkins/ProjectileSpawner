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

	protected virtual void OnEnable() {
		rb.velocity = transform.forward * speed;
	}
}
