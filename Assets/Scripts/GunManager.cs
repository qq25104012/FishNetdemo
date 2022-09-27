using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using Steamworks;
using FishNet.Connection;

public class GunManager : NetworkBehaviour
{
    [SerializeField] private GameObject bullet;

    [SerializeField] private float cooldown = 1f;

    [SerializeField] private Transform firePoint;

    [SerializeField] private float damage = 10f;

    private bool canShoot = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (canShoot)
        {
            canShoot = false;

            ServerFire(firePoint.position, firePoint.forward, LocalConnection);

            Invoke(nameof(ResetShot), cooldown);
        }
    }

    [ServerRpc]
    private void ServerFire(Vector3 _firePoint, Vector3 _fireDirection, NetworkConnection _connection)
    {
        if (Physics.Raycast(_firePoint, _fireDirection, out RaycastHit hit))
        {
            if (hit.transform.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage, _connection);
            }
            
            if (hit.transform.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce((hit.point - transform.position) * 10f);
            }
        }
    }

    private void ResetShot()
    {
        canShoot = true;
    }
}
