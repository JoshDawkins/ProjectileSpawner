using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField]
    private Projectile projectilePrefab = null;

    private ObjectPool<Projectile> pool;

	private void OnEnable() {
		//Create a pool of these projectiles
		pool = new ObjectPool<Projectile>(
			() => Instantiate(projectilePrefab),
			(p) => p.gameObject.SetActive(true),
			(p) => p.gameObject.SetActive(false),
			(p) => Destroy(gameObject),
			true, 100, 500
		);
	}

	private void OnDisable() {
		pool.Dispose();
	}

	private void OnTriggerEnter(Collider other) {
		//How can I check if the other object belongs to this pool?
	}
}
