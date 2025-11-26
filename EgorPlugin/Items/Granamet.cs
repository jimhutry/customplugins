using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using UnityEngine;

namespace EgorPlugin.Items;

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


[CustomItem(ItemType.GunShotgun)]
public class GrenadeLauncherItem : CustomWeapon
{

    public override float Damage { get; set; } = 0;

    public override uint Id { get; set; } = 31;

    public override string Name { get; set; } = "\uf1e2 Миномёт";

    public override string Description { get; set; } = "Ручной револьверный миномёт. По 10 снарядов в барабане.";

    public override float Weight { get; set; }

    public override SpawnProperties? SpawnProperties { get; set; }

    public override ItemType Type { get; set; } = ItemType.GunShotgun;

    public override byte ClipSize { get; set; } = 1;

    public override Vector3 Scale { get; set; } = new(3, 1, 3);
    

    public static Dictionary<ushort, bool> OnReload { get; set; } = [];
    
    public override AttachmentName[] Attachments { get; set; } =
    [
        AttachmentName.HoloSight,
        AttachmentName.ShotgunDoubleShot,
        AttachmentName.AmmoCounter,
    ];
    

    protected override void SubscribeEvents()
    {
        base.SubscribeEvents();

        ItemEvents.ChangingAttachments += OnChangingAttachments;
        PlayerEvents.UnloadingWeapon += OnUnloadingWeapon;
        
    }

    protected override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        ItemEvents.ChangingAttachments -= OnChangingAttachments;
        PlayerEvents.UnloadingWeapon -= OnUnloadingWeapon;

    }

    protected override void ShowPickedUpMessage(Player player) { }

    protected override void ShowSelectedMessage(Player player) { }
    

    protected override void OnPickingUp(PickingUpItemEventArgs ev)
    {
        base.OnPickingUp(ev);

        ev.Player.ShowHint($"<size=44>Вы подобрали\n<color=#FFF416>{this.Name}</color></size>",
            3);
    }

    protected override void OnChanging(ChangingItemEventArgs ev)
    {
        base.OnChanging(ev);

        ev.Player.ShowHint($"<size=44>Вы взяли\n<color=#FFF416>{this.Name}</color></size>", 3);
    }

    protected override void OnDroppingItem(DroppingItemEventArgs ev)
    {
        base.OnDroppingItem(ev);

        ev.Player.ShowHint($"<size=44>Вы выбросили\n<color=#FFF416>{this.Name}</color></size>", 3);
    }

    private void OnChangingAttachments(ChangingAttachmentsEventArgs ev)
    {
        if (Check(ev.Firearm))
        {
            ev.IsAllowed = false;
        }
    }

    private void OnUnloadingWeapon(UnloadingWeaponEventArgs ev)
    {
        if (Check(ev.Item))
        {
            ev.IsAllowed = false;
        }
    }

    protected override void OnReloading(ReloadingWeaponEventArgs ev)
    {
        base.OnReloading(ev);

        ev.IsAllowed = false;
    }

    protected override void OnHurting(HurtingEventArgs ev)
    {
        base.OnHurting(ev);

        if (ev.DamageHandler.Type is not DamageType.Explosion)
        {
            ev.IsAllowed = false;
        }
    }

    protected override void OnShooting(ShootingEventArgs ev)
    {
        base.OnShooting(ev);

        if (OnReload.ContainsKey(ev.Item.Serial) && OnReload[ev.Item.Serial])
        {
            ev.IsAllowed = false;
            ev.Firearm.BarrelAmmo = 0;
            ev.Firearm.MagazineAmmo = 0;
            return;
        }
        
        if (!OnReload.ContainsKey(ev.Item.Serial))
        {
            if (Timing.CurrentCoroutine.Tag == nameof(GrenadeLauncherItem))
            {
                ev.IsAllowed = false;
                ev.Firearm.BarrelAmmo = 0;
                ev.Firearm.MagazineAmmo = 0;
                return;
            }
            OnReload[ev.Item.Serial] = true;
        }
        OnReload[ev.Item.Serial] = true;
        ev.Firearm.BarrelAmmo = 1;
        ev.Firearm.MagazineAmmo = 1;
        var player = ev.Player;
        var forward = player.CameraTransform.forward;
        var grenadeItem = Item.Create(ItemType.GrenadeHE) as ExplosiveGrenade;
        var pickup = grenadeItem.SpawnActive(ev.Player.Position);
        var component = pickup.GameObject.AddComponent<ExplosionComponent>();
        component.Grenade = pickup;
        pickup.GameObject.GetComponent<Rigidbody>().AddForce(forward * 2300, ForceMode.Acceleration);
        Timing.RunCoroutine(ReloadingCoroutine(ev.Item.Owner, ev.Firearm), nameof(GrenadeLauncherItem));
    }

    private static IEnumerator<float> ReloadingCoroutine(Player player, Firearm firearm)
    {
        for (var i = 0; i < 6; i++)
        {
            player.ShowHint($"Гранатомёт будет перезаряжаться еще {5-i} секунд.", 1f);
            yield return Timing.WaitForSeconds(1f);
        }
        firearm.BarrelAmmo = 1;
        firearm.MagazineAmmo = 1;
        OnReload[firearm.Serial] = false;
    }
}
