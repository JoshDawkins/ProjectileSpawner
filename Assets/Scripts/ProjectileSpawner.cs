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
	[SerializeField]
	[Min(0)]
	private float startDelay = 0;

	#region OFFSET
	[Header("Offset")]
	[SerializeField]
	private Vector3 centerOffset = Vector3.zero;
	[SerializeField]
	[Min(0)]
	private float offsetRadius = 0;
	[SerializeField]
	[Range(-180, 180)]
	private float offsetStartAngle = 0,
		rotationFromOffset = 0;
	[SerializeField]
	private Color editorColor = Color.red;
	#endregion OFFSET

	#region ROTATION
	[Header("Rotation")]
	[SerializeField]
	[Range(-720, 720)]
	private float rotationSpeed = 0;
	[SerializeField]
	[Range(-360, 360)]
	private float rotationRangeMin = 0,
		rotationRangeMax = 360;
	[SerializeField]
	private bool pingPong = false;
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
	[Range(-720, 720)]
	private float sweepSpeed = 0;
	[SerializeField]
	[Range(-360, 360)]
	private float sweepRangeMin = -30,
		sweepRangeMax = 30;
	#endregion SWEEP FIRE

	#region BURST FIRE
	[Header("Burst Fire")]
	[SerializeField]
	private bool burstFire = false;
	[SerializeField]
	[Min(1)]
	private int shotsPerBurst = 1;
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
		//Initial delay
		if (startDelay > 0)
			yield return new WaitForSeconds(startDelay);

		//Instantiate some variables to use
		Quaternion rotation, sweepAngle;
		Vector3 offset;
		int shotsThisBurst = 0;
		float currentRotation = offsetStartAngle,
			currentSweepAngle = rotationFromOffset;

		while (true) {
			offset.x = offset.y = 0;
			offset.z = offsetRadius;
			if (rotationRangeMax < rotationRangeMin) {
				//Make sure the min is actually less than the max
				var temp = rotationRangeMin;
				rotationRangeMin = rotationRangeMax;
				rotationRangeMax = temp;
			}
			if (sweepRangeMax < sweepRangeMin) {
				//Make sure the min is actualy less than the max
				var temp = sweepRangeMin;
				sweepRangeMin = sweepRangeMax;
				sweepRangeMax = temp;
			}

			//Rotation
			if (rotationSpeed == 0) {
				currentRotation = offsetStartAngle;
			} else {
				currentRotation += rotationSpeed * fireRate;

				if (currentRotation > rotationRangeMax) {
					//Went above the max range
					if (pingPong) {
						currentRotation = rotationRangeMax - (currentRotation - rotationRangeMax);
						rotationSpeed *= -1;
					} else {
						currentRotation = rotationRangeMin + (currentRotation - rotationRangeMax);
					}
				} else if (currentRotation < rotationRangeMin) {
					//Went below the min range
					if (pingPong) {
						currentRotation = rotationRangeMin + (rotationRangeMin - currentRotation);
						rotationSpeed *= -1;
					} else {
						currentRotation = rotationRangeMax - (rotationRangeMin - currentRotation);
					}
				}
			}
			rotation = Quaternion.Euler(0, currentRotation, 0);

			//Sweep
			if (sweepSpeed == 0) {
				currentSweepAngle = rotationFromOffset;
			} else {
				currentSweepAngle += sweepSpeed * fireRate;

				if (currentSweepAngle > sweepRangeMax) {
					currentSweepAngle = sweepRangeMax - (currentSweepAngle - sweepRangeMax);
					sweepSpeed *= -1;
				} else if (currentSweepAngle < sweepRangeMin) {
					currentSweepAngle = sweepRangeMin + (sweepRangeMin - currentSweepAngle);
					sweepSpeed *= -1;
				}
			}
			sweepAngle = Quaternion.Euler(0, currentSweepAngle, 0);

			//Fire a projectile
			projectileSource.SpawnProjectile(transform.position + centerOffset + (rotation * offset), transform.rotation * rotation * sweepAngle);

			//Delay before the next shot
			if (burstFire && ++shotsThisBurst >= shotsPerBurst) {
				yield return new WaitForSeconds(timeBetweenBursts);
				shotsThisBurst = 0;
			} else
				yield return new WaitForSeconds(fireRate);
		}
	}

	private void OnDrawGizmosSelected() {
		Gizmos.color = editorColor;

		var center = transform.position + centerOffset;
		var radVec = Vector3.forward * offsetRadius;
		var min = Quaternion.Euler(0, rotationRangeMin, 0);
		var max = Quaternion.Euler(0, rotationRangeMax, 0);

		Gizmos.DrawSphere(center, 0.1f);
		Gizmos.DrawWireSphere(center, offsetRadius);
		Gizmos.DrawSphere(center + (Quaternion.Euler(0, offsetStartAngle, 0) * radVec), 0.05f);

		Gizmos.DrawLine(center + (min * (radVec + Vector3.forward * -0.2f)), center + (min * (radVec + Vector3.forward * 0.2f)));
		Gizmos.DrawLine(center + (max * (radVec + Vector3.forward * -0.2f)), center + (max * (radVec + Vector3.forward * 0.2f)));
	}
}
