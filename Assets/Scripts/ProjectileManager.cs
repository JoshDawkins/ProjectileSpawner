using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField]
    private Projectile projectilePrefab = null;
	[SerializeField]
	private int initialCapacity = 100,
		maxCapacity = 500;

    private ObjectPool<Projectile> pool;

	private void OnEnable() {
		//Create a pool of these projectiles
		pool = new ObjectPool<Projectile>(
			() => {
				Projectile p = Instantiate(projectilePrefab);
				p.Manager = this;
				return p;
			},
			(p) => p.gameObject.SetActive(true),
			(p) => p.gameObject.SetActive(false),
			(p) => Destroy(gameObject),
			true, initialCapacity, maxCapacity
		);
	}

	private void OnDisable() {
		pool.Dispose();
	}

	private void OnTriggerExit(Collider other) {
		//Confirm that the exiting object is a projectile that belongs to this manager
		Projectile p = other.GetComponent<Projectile>();
		if (p == null || p.Manager != this)
			return;

		pool.Release(p);
	}

	public Projectile SpawnProjectile(Vector3 position, Quaternion rotation) {
		Projectile p = pool.Get();
		p.transform.position = position;
		p.transform.rotation = rotation;

		return p;
	}
}
