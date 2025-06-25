using System;
using Exiled.API.Features.Pickups.Projectiles;
using MEC;
using UnityEngine;

namespace EgorPlugin;

public class ExplosionComponent : MonoBehaviour
{
    public ExplosionGrenadeProjectile Grenade {get; set;}
    public bool IsActive { get; set; }

    public void Start()
    {
        Timing.CallDelayed(0.15f, () => IsActive = true);
    }

    public void OnCollisionEnter(Collision other)
    {
        if (!IsActive) return;
        Grenade.Explode();
        Destroy(this);
    }
}