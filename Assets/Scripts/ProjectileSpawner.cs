using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
	#region SERIALIZED FIELDS
	[SerializeField]
	private Projectile projectilePrefab = null;
	[SerializeField]
	[Min(0)]
	private float fireRate = 0.5f;
	[SerializeField]
	private Vector3 centerOffset = Vector3.zero;
	[SerializeField]
	private float radius = 0;

	#region ROTATION
	[Header("Rotation")]
	[SerializeField]
	[Range(0, 360)]
	private float rotationSpeed = 0;
	[SerializeField]
	[Range(0, 360)]
	private float rotationRange = 0;
	[SerializeField]
	private bool rotateClockwise = true,
		pingPong = true;
	#endregion ROTATION

	#region MULTIDIRECTIONAL FIRE
	[Header("Multidirectional Fire")]
	[SerializeField]
	[Min(1)]
	private int spawnDirections = 1;
	[SerializeField]
	[Range(0, 360)]
	private float multiDirAngle = 0;
	#endregion MULTIDIRECTIONAL FIRE

	#region CONE FIRE
	[Header("Cone Fire")]
	[SerializeField]
	[Min(1)]
	private int coneStreams = 1;
	[SerializeField]
	[Range(0, 360)]
	private float coneAngle = 0;
	[SerializeField]
	[Min(0)]
	private float coneTipWidth = 0;
	#endregion CONE FIRE

	#region SWEEP FIRE
	[Header("Sweep Fire")]
	[SerializeField]
	[Range(0, 360)]
	private float sweepSpeed = 0;
	[SerializeField]
	[Range(0, 360)]
	private float sweepRange = 0;
	[SerializeField]
	private bool sweepClockwise = true;
	#endregion SWEEP FIRE

	#region BURST FIRE
	[Header("Burst Fire")]
	[SerializeField]
	private bool burstFire = false;
	[SerializeField]
	[Min(1)]
	private int shotsPerBursts = 1;
	[SerializeField]
	[Min(0)]
	private float timeBetweenBursts = 0;
	#endregion BURST FIRE
	#endregion SERIALIZED FIELDS

	#region SETUP
	private Coroutine shootingCoroutine;

	private void OnEnable() {
		//shootingCoroutine = StartCoroutine(Shoot());
	}

	private void OnDisable() {
		StopCoroutine(shootingCoroutine);
	}
	#endregion SETUP

	//private IEnumerator Shoot() {}
}
