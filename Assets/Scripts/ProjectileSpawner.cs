using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
	#region SERIALIZED FIELDS
	[SerializeField]
	private ProjectileManager projectileSource = null;
	[SerializeField]
	[Min(.01f)]
	private float fireRate = 0.2f;

	#region OFFSET
	[Header("Offset")]
	[SerializeField]
	private Vector3 centerOffset = Vector3.zero;
	[SerializeField]
	[Min(0)]
	private float offsetRadius = 0;
	[SerializeField]
	[Range(0, 360)]
	private float offsetRotation = 0;
	[SerializeField]
	private Color editorColor = Color.red;
	#endregion OFFSET

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
		shootingCoroutine = StartCoroutine(Shoot());
	}

	private void OnDisable() {
		StopCoroutine(shootingCoroutine);
	}
	#endregion SETUP

	private IEnumerator Shoot() {
		Quaternion rot;
		Vector3 offset = Vector3.zero;

		while (true) {
			rot = Quaternion.AngleAxis(offsetRotation, Vector3.up);
			offset.x = offset.y = 0;
			offset.z = offsetRadius;

			//Fire a projectile
			projectileSource.SpawnProjectile(transform.position + centerOffset + (rot * offset), transform.rotation * rot);

			//Delay before the next shot
			yield return new WaitForSeconds(fireRate);
		}
	}

	private void OnDrawGizmosSelected() {
		Gizmos.color = editorColor;

		Gizmos.DrawSphere(transform.position + centerOffset, 0.1f);
		Gizmos.DrawWireSphere(transform.position + centerOffset, offsetRadius);
		Gizmos.DrawSphere(transform.position + centerOffset + (Quaternion.AngleAxis(offsetRotation, Vector3.up) * new Vector3(0, 0, offsetRadius)), 0.1f);
	}
}
