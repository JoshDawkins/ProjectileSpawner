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
	[SerializeField]
	[ColorUsage(showAlpha: false)]
	private Color editorColor = Color.red;

	#region OFFSET
	[Header("Offset")]
	[SerializeField]
	private Vector3 centerOffset = Vector3.zero;
	[SerializeField]
	[Min(0)]
	private float offsetRadius = 0;
	#endregion OFFSET

	#region MULTIDIRECTIONAL FIRE
	[Space]
	[SerializeField]
	[Range(-180, 180)]
	private float[] firingDirections = { 0 };
	#endregion MULTIDIRECTIONAL FIRE

	#region ROTATION
	[Header("Rotation")]
	[SerializeField]
	[Range(-720, 720)]
	private float rotationSpeed = 0;
	[SerializeField]
	[Range(-360, 360)]
	private float initialRotation = 0;
	[SerializeField]
	[Range(-360, 360)]
	private float rotationRangeMin = 0,
		rotationRangeMax = 360;
	[SerializeField]
	private bool pingPong = false;
	#endregion ROTATION

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
	private float initialAngle = 0;
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

	//Some variables that will be calculated and used by the coroutine, and may be read to draw gizmos
	private Quaternion rotation;
	private int shotsThisBurst = 0;
	private float currentRotation,
		currentSweepAngle;

	//Stores the currently running coroutine
	private Coroutine shootingCoroutine;

	#region SETUP
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

		//Initial values
		currentRotation = initialRotation;
		currentSweepAngle = initialAngle;
		if (burstFire)
			shotsThisBurst = shotsPerBurst;

		//Just keep shooting forever until OnDisable stops the coroutine
		while (true) {
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
				currentRotation = initialRotation;
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
				currentSweepAngle = initialAngle;
			} else {
				currentSweepAngle += sweepSpeed * fireRate;

				if (currentSweepAngle > sweepRangeMax) {
					//Went above the max range
					currentSweepAngle = sweepRangeMax - (currentSweepAngle - sweepRangeMax);
					sweepSpeed *= -1;
				} else if (currentSweepAngle < sweepRangeMin) {
					//Went below the min range
					currentSweepAngle = sweepRangeMin + (sweepRangeMin - currentSweepAngle);
					sweepSpeed *= -1;
				}
			}

			//Fire the projectiles
			foreach (float rot in firingDirections)
				FireOneDirection(rotation * Quaternion.Euler(0, rot, 0), currentSweepAngle);

			//Delay before the next shot
			if (burstFire && --shotsThisBurst < 1) {
				shotsThisBurst = shotsPerBurst;
				yield return new WaitForSeconds(timeBetweenBursts);
			} else
				yield return new WaitForSeconds(fireRate);
		}
	}

	private void FireOneDirection(Quaternion rotation, float currentSweepAngle) {
		Vector3 offset = transform.position + centerOffset + (rotation * (Vector3.forward * offsetRadius));
		Quaternion sweepAngle = transform.rotation * rotation * Quaternion.Euler(0, currentSweepAngle, 0);

		if (coneStreams == 1) {
			projectileSource.SpawnProjectile(offset, sweepAngle);
		} else {
			float per = coneTipWidth / (coneStreams - 1),
				halfWidth = coneTipWidth / 2,
				anglePer = coneAngle / (coneStreams - 1),
				halfAngle = coneAngle / 2;
			for (int i = 0; i < coneStreams; i++) {
				projectileSource.SpawnProjectile(
					offset + (rotation * Vector3.right * (per * i - halfWidth)),
					sweepAngle * Quaternion.Euler(0, anglePer * i - halfAngle, 0)
				);
			}
		}
	}

	private void OnDrawGizmosSelected() {
		//Ensure needed values are set when the coroutine isn't running, such as when the editor isn't playing
		if (shootingCoroutine == null) {
			currentRotation = initialRotation;
			currentSweepAngle = initialAngle;
			rotation = Quaternion.Euler(0, currentRotation, 0);
		}

		//Set drawing color for the gizmos
		Gizmos.color = editorColor;

		//Calculate a few things
		var center = transform.position + centerOffset;
		var radius = Vector3.forward * offsetRadius;
		var min = Quaternion.Euler(0, rotationRangeMin, 0);
		var max = Quaternion.Euler(0, rotationRangeMax, 0);
		Vector3 drawPos;
		Quaternion sweepAngle, finalRot;

		//Draw the offset position and radius
		Gizmos.DrawSphere(center, 0.1f);
		Gizmos.DrawWireSphere(center, offsetRadius);

		//Display the number of shots left in this burst if burst fire is enabled
#if UNITY_EDITOR
		if (burstFire) {
			if (shootingCoroutine == null)
				shotsThisBurst = shotsPerBurst;

			GUIStyle style = new GUIStyle();
			style.normal.textColor = editorColor;
			UnityEditor.Handles.Label(center + transform.right * 0.15f, shotsThisBurst.ToString(), style);
		}
#endif

		//Draw rotation limits
		Gizmos.DrawLine(center + (min * (radius + Vector3.forward * -0.2f)), center + (min * (radius + Vector3.forward * 0.2f)));
		Gizmos.DrawLine(center + (max * (radius + Vector3.forward * -0.2f)), center + (max * (radius + Vector3.forward * 0.2f)));

		//Draw each spawn position and its shooting direction
		foreach (float rot in firingDirections) {
			finalRot = rotation * Quaternion.Euler(0, rot, 0);
			drawPos = center + (finalRot * radius);
			sweepAngle = transform.rotation * finalRot * Quaternion.Euler(0, currentSweepAngle, 0);

			if (coneStreams == 1) {
				Gizmos.DrawSphere(drawPos, 0.05f);
				Gizmos.DrawLine(drawPos, drawPos + (sweepAngle * Vector3.forward * 0.3f));
			} else {
				float per = coneTipWidth / (coneStreams - 1),
					halfWidth = coneTipWidth / 2,
					anglePer = coneAngle / (coneStreams - 1),
					halfAngle = coneAngle / 2;
				Vector3 finalPos;
				for (int i = 0; i < coneStreams; i++) {
					finalPos = drawPos + (finalRot * Vector3.right * (per * i - halfWidth));
					Gizmos.DrawSphere(finalPos, 0.05f);
					Gizmos.DrawLine(finalPos, finalPos + (sweepAngle * Quaternion.Euler(0, anglePer * i - halfAngle, 0) * Vector3.forward * 0.3f));
				}
			}
		}
	}
}
