using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

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

            ServerFire(firePoint.position, firePoint.forward);

            Invoke(nameof(ResetShot), cooldown);
        }
    }

    [ServerRpc]
    private void ServerFire(Vector3 _firePoint, Vector3 _fireDirection)
    {
        if (Physics.Raycast(_firePoint, _fireDirection, out RaycastHit hit))
        {
            if (hit.transform.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }
        }
    }

    private void ResetShot()
    {
        canShoot = true;
    }
}
