using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField]
    private Projectile projectilePrefab = null;
    [SerializeField]
    [Min(0)]
    private float fireRate = 0.5f;

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

    [Header("Multidirectional Fire")]
    [SerializeField]
    [Min(1)]
    private int spawnDirections = 1;
    [SerializeField]
    [Range(0, 360)]
    private float multiDirAngle = 0;

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

    [Header("Sweep Fire")]
    [SerializeField]
    [Range(0, 360)]
    private float sweepSpeed = 0;
    [SerializeField]
    [Range(0, 360)]
    private float sweepRange = 0;
    [SerializeField]
    private bool sweepClockwise = true;
}
