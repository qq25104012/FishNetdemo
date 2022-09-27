using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;

public interface IDamageable
{
    public void TakeDamage(float _damage, string _address);
}
