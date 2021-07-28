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
			createFunc: () => {
				Projectile p = Instantiate(projectilePrefab);
				p.Manager = this;
				return p;
			},
			actionOnGet: (p) => p.gameObject.SetActive(true),
			actionOnRelease: (p) => p.gameObject.SetActive(false),
			actionOnDestroy: (p) => Destroy(gameObject),
			collectionCheck: true,
			defaultCapacity: initialCapacity,
			maxSize: maxCapacity
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

		DespawnProjectile(p);
	}

	public Projectile SpawnProjectile(Vector3 position, Quaternion rotation) {
		Projectile p = pool.Get();
		p.ResetProjectile(position, rotation);

		return p;
	}

	public void DespawnProjectile(Projectile p) {
		pool.Release(p);
	}
}
