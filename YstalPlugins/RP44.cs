using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using HUD;
using LabApi.Events.Handlers;
using MEC;
using UnityEngine;

[CustomItem(ItemType.SCP500)]
public class ActoProtector : CustomItem
{
    public override uint Id { get; set; } = 441;
    public override string Name { get; set; } = "\uf486 Актопротектор";
    public override string Description { get; set; } = "Препарат, увеличивающий здоровье.";
    public override float Weight { get; set; } = 5f;
    public override SpawnProperties? SpawnProperties { get; set; }
    public override Vector3 Scale { get; set; } = new(1.5f, 1.5f, 1.5f);

    private Dictionary<int, bool> _wasused = [];
    
    protected override void SubscribeEvents()
    {
        base.SubscribeEvents();
        PlayerEvents.UsingItemCompleted += OnUse;
        PlayerEvents.ChangingRole += OnChanging;
    }

    protected override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        PlayerEvents.UsingItemCompleted -= OnUse;
        PlayerEvents.ChangingRole -= OnChanging;
    }

    protected override void ShowPickedUpMessage(Player player) { }
    protected override void ShowSelectedMessage(Player player) { }

    public override void Give(Player player, Item item, bool displayMessage = true)
    {
        base.Give(player, item, displayMessage);
        item.Scale = Scale;
    }
    protected override void OnPickingUp(PickingUpItemEventArgs ev)
    {
        base.OnPickingUp(ev);
        ev.Player.ShowHint($"<size=44>Вы подобрали\n<color=#a84397>\uf486{this.Name}</color></size>",
            3, DrawUIComponent.HintPosition.CENTER, nameof(ActoProtector));
    }
    protected override void OnChanging(ChangingItemEventArgs ev)
    {   
        base.OnChanging(ev);
        ev.Player.ShowHint($"<size=44>Вы взяли\n<color=#a84397>\uf486{this.Name}</color></size>", 3,
            DrawUIComponent.HintPosition.CENTER, nameof(ActoProtector));
    }
    protected override void OnDroppingItem(DroppingItemEventArgs ev)
    {
        base.OnDroppingItem(ev);
        ev.Player.ShowHint($"<size=44>Вы выбросили\n<color=#a84397>\uf486{this.Name}</color></size>", 3,
            DrawUIComponent.HintPosition.CENTER, nameof(ActoProtector));
    }

    private void OnUse(UsingItemCompletedEventArgs ev)
    {
        if (!Check(ev.Item)) return;
        ev.IsAllowed = false; 
        ev.Usable.Destroy();
        if (_wasused.ContainsKey(ev.Player.Id)) return;
        var lasthealth = ev.Player.Health;
        ev.Player.Heal(lasthealth * 1.25f);
        ev.Player.MaxHealth *= 1.25f;
        _wasused[ev.Player.Id] = true;
        Timing.RunCoroutine(CleaningCoroutine(ev.Player), nameof(ActoProtector));
    }

    private void OnChanging(ChangingRoleEventArgs ev)
    {
        Timing.KillCoroutines(nameof(ActoProtector));
        _wasused.Remove(ev.Player.Id);
    }

    private static IEnumerator<float> CleaningCoroutine(Player player)
    {
        for (var i = 0; i < 300; i++)
        {
            player.DisableEffect<Bleeding>();
            player.DisableEffect<Hemorrhage>();
            yield return Timing.WaitForSeconds(1f);
        }
    }
}


[CustomItem(ItemType.Coin)]
public class MoonWalkers : CustomItem
{
    public override uint Id { get; set; } = 442;
    public override string Name { get; set; } = "\uf0e7 Берцы \"Moonwalkers\"";

    public override string Description { get; set; } =
        "Удобные сапоги, дающие существенное преимущество при надевании.";

    public override float Weight { get; set; } = 25f;
    public override SpawnProperties? SpawnProperties { get; set; }
    public override Vector3 Scale { get; set; } = new(6f, 25f, 6f);
    
    private Dictionary<ushort, bool> _putons = new();
    protected override void SubscribeEvents()
    {
        base.SubscribeEvents();
        PlayerEvents.ChangingItem += OnUse;
        PlayerEvents.ItemRemoved += OnRemove;
    }
    
    protected override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        PlayerEvents.ChangingItem -= OnUse;
        PlayerEvents.ItemRemoved -= OnRemove;
    }

    protected override void ShowPickedUpMessage(Player player) { }
    protected override void ShowSelectedMessage(Player player) { }

    public override void Give(Player player, Item item, bool displayMessage = true)
    {
        base.Give(player, item, displayMessage);
        item.Scale = Scale;
    }
    protected override void OnPickingUp(PickingUpItemEventArgs ev)
    {
        base.OnPickingUp(ev);
        ev.Player.ShowHint($"<size=44>Вы подобрали\n<color=#7fa9db>{this.Name}</color></size>",
            3, DrawUIComponent.HintPosition.CENTER, nameof(MoonWalkers));
    }
    protected override void OnDroppingItem(DroppingItemEventArgs ev)
    {
        base.OnDroppingItem(ev);
        ev.Player.ShowHint($"<size=44>Вы выбросили\n<color=#7fa9db>{this.Name}</color></size>", 3,
            DrawUIComponent.HintPosition.CENTER, nameof(MoonWalkers));
        if (_putons.ContainsKey(ev.Item.Serial) && _putons[ev.Item.Serial])
        {
            ev.Player.DisableEffect<SilentWalk>();
            ev.Player.DisableEffect<MovementBoost>();
            _putons[ev.Item.Serial] = false;
        };
    }

    private void OnRemove(ItemRemovedEventArgs ev)
    {
        if (!Check(ev.Item)) return;
        _putons.Remove(ev.Item.Serial);
        ev.Player.DisableEffect<SilentWalk>();
        ev.Player.DisableEffect<MovementBoost>();
    }
    private void OnUse(ChangingItemEventArgs ev)
    {
        if (!Check(ev.Item)) return;
        if (_putons.ContainsKey(ev.Item.Serial) && _putons[ev.Item.Serial])
        {
            _putons[ev.Item.Serial] = false;
            ev.Player.ShowHint($"<size=44>Вы сняли\n<color=#7fa9db>{this.Name}</color></size>", 3,
                DrawUIComponent.HintPosition.CENTER, nameof(MoonWalkers));
            ev.Player.DisableEffect<SilentWalk>();
            ev.Player.DisableEffect<MovementBoost>();
            ev.IsAllowed = false;
            return;
        }

        _putons[ev.Item.Serial] = true;
        ev.Player.ShowHint($"<size=44>Вы надели\n<color=#7fa9db>{this.Name}</color></size>", 3,
            DrawUIComponent.HintPosition.CENTER, nameof(MoonWalkers));
        ev.Player.EnableEffect<SilentWalk>(5);
        ev.Player.EnableEffect<MovementBoost>(8);
        ev.IsAllowed = false;
    }
}

[CustomItem(ItemType.Painkillers)]
public class TempV : CustomItem
{
    public override uint Id { get; set; } = 443;
    public override string Name { get; set; } = "\uf492 Временный препарат Синего типа";

    public override string Description { get; set; } =
        "Воздействует на тело человека, заставляя его отращивать поврежденные ткани.";

    public override float Weight { get; set; } = 3f;
    public override SpawnProperties? SpawnProperties { get; set; }
    public override Vector3 Scale { get; set; } = new(2f, 2f, 2f);

    protected override void SubscribeEvents()
    {
        base.SubscribeEvents();
        PlayerEvents.UsingItemCompleted += OnUse;
        PlayerEvents.ChangingRole += OnCh;
    }

    protected override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        PlayerEvents.UsingItemCompleted -= OnUse;
        PlayerEvents.ChangingRole -= OnCh;

    }

    protected override void ShowPickedUpMessage(Player player) { }
    protected override void ShowSelectedMessage(Player player) { }

    public override void Give(Player player, Item item, bool displayMessage = true)
    {
        base.Give(player, item, displayMessage);
        item.Scale = Scale;
    }
    protected override void OnPickingUp(PickingUpItemEventArgs ev)
    {
        base.OnPickingUp(ev);
        ev.Player.ShowHint($"<size=44>Вы подобрали\n<color=#1000ba>\uf492{this.Name}</color></size>",
            3, DrawUIComponent.HintPosition.CENTER, nameof(TempV));
    }
    protected override void OnChanging(ChangingItemEventArgs ev)
    {   
        base.OnChanging(ev);
        ev.Player.ShowHint($"<size=44>Вы взяли\n<color=#1000ba>\uf492{this.Name}</color></size>", 3,
            DrawUIComponent.HintPosition.CENTER, nameof(TempV));
    }
    protected override void OnDroppingItem(DroppingItemEventArgs ev)
    {
        base.OnDroppingItem(ev);
        ev.Player.ShowHint($"<size=44>Вы выбросили\n<color=#1000ba>\uf492{this.Name}</color></size>", 3,
            DrawUIComponent.HintPosition.CENTER, nameof(TempV));
    }

    private void OnCh(ChangingRoleEventArgs ev)
    {
        Timing.KillCoroutines(nameof(TempV));
    }
    
    private void OnUse(UsingItemCompletedEventArgs ev)
    {
        if (!Check(ev.Item)) return;
        ev.IsAllowed = false;
        ev.Usable.Destroy();
        Timing.RunCoroutine(RegenerationCorountine(ev.Player), Segment.RealtimeUpdate, nameof(TempV));
    }

    private IEnumerator<float> RegenerationCorountine(Player plr)
    {
        for (var j = 0; j <= 20; j++)
        {
            for (int i = 0; i <= 5; i++)
            {
                plr.Heal(5f);
                yield return Timing.WaitForSeconds(0.2f);
            }
        }
    }
}

[CustomItem(ItemType.ArmorHeavy)]
public class PolymericArmor : CustomArmor
{
    public override uint Id { get; set; } = 444;
    public override string Name { get; set; } = "\uf1cd Полимерная броня";
    public override string Description { get; set; } = "Дополнительная защита в виде полимерных пластин.";
    public override float Weight { get; set; } = 11f;
    public override SpawnProperties? SpawnProperties { get; set; }
    private Dictionary<ushort, bool> _putons = [];
    private readonly string _anothername = "Полимерную броню";

    protected override void SubscribeEvents()
    {
        base.SubscribeEvents();
        PlayerEvents.ItemRemoved += OnRemove;
        PlayerEvents.Hurt += OnHurt;
    }

    protected override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        PlayerEvents.ItemRemoved -= OnRemove;
        PlayerEvents.Hurt -= OnHurt;
    }


    protected override void OnAcquired(Player player, Item item, bool displayMessage)
    {
        base.OnAcquired(player, item, displayMessage);
        if (!player.SessionVariables.ContainsKey("polymeric"))
        {
            player.SessionVariables.Add("polymeric", item.Serial);
        }

        if (!_putons.ContainsKey(item.Serial))
        {
            _putons.Add(item.Serial, false);
        }
        if (!_putons[item.Serial])
        {
            player.MaxHumeShield = 30;
            Timing.KillCoroutines(nameof(PolymericArmor) + "2");
            Timing.RunCoroutine(RegenerationCoroutine(player), Segment.RealtimeUpdate, nameof(PolymericArmor));
            player.ShowHint($"<size=44>Вы подобрали\n<color=#757148>\uf1cd{_anothername}</color></size>",
                3, DrawUIComponent.HintPosition.CENTER, nameof(PolymericArmor));
        }
    }

    protected override void ShowPickedUpMessage(Player player) { }
    protected override void ShowSelectedMessage(Player player) { }
    
    protected override void OnDroppingItem(DroppingItemEventArgs ev)
    {
        base.OnDroppingItem(ev);
        ev.Player.ShowHint($"<size=44>Вы выбросили\n<color=#757148>\uf1cd{_anothername}</color></size>", 3,
            DrawUIComponent.HintPosition.CENTER, nameof(PolymericArmor));
        if (_putons[ev.Item.Serial] == false)
        {
            Timing.KillCoroutines(nameof(PolymericArmor));
            Timing.RunCoroutine(DestroyHumeShield(ev.Player), Segment.RealtimeUpdate, nameof(PolymericArmor) + "2");
        }
        ev.Player.SessionVariables.Remove("polymeric");
    }

    private void OnRemove(ItemRemovedEventArgs ev)
    {
        if (!Check(ev.Item)) return;
        if (_putons[ev.Item.Serial] == false)
        {
            Timing.KillCoroutines(nameof(PolymericArmor));
            Timing.RunCoroutine(DestroyHumeShield(ev.Player), Segment.RealtimeUpdate, nameof(PolymericArmor) + "2");
        }
        ev.Player.SessionVariables.Remove("polymeric");
    }

    private void OnHurt(HurtEventArgs ev)
    {
        if (ev.Player.HumeShield == 0 && ev.Player.SessionVariables.ContainsKey("polymeric"))
        {
            _putons[(ushort) ev.Player.SessionVariables["polymeric"]] = true;
            Timing.KillCoroutines(nameof(PolymericArmor));
            Timing.RunCoroutine(DestroyHumeShield(ev.Player), Segment.RealtimeUpdate, nameof(PolymericArmor) + "2");
            Timing.RunCoroutine(UntilRegenCoroutine(ev.Player, (ushort) ev.Player.SessionVariables["polymeric"]), Segment.RealtimeUpdate, nameof(PolymericArmor) + "3");
        } 
    }
    
    private IEnumerator<float> RegenerationCoroutine(Player plr)
    {
        while (plr.HumeShield < 30)
        {
            plr.HumeShield += 1;
            yield return Timing.WaitForSeconds(0.3f);
        }
    }


    private IEnumerator<float> DestroyHumeShield(Player player)
    {
        while (player.HumeShield > 0)
        {
            player.HumeShield -= 1;
            yield return Timing.WaitForSeconds(0.15f);
        }
    }

    private IEnumerator<float> UntilRegenCoroutine(Player plr, ushort ser)
    {
        for (var i = 0; i < 5; i++)
        {
            yield return Timing.WaitForSeconds(1f);
        }
        _putons[ser] = false;
        if (plr.SessionVariables.ContainsKey("polymeric"))
        {
            Timing.RunCoroutine(RegenerationCoroutine(plr));
        }
    }
}

[CustomItem(ItemType.SCP1344)]
public class Refractor : CustomItem
{
    public override uint Id { get; set; } = 445;
    public override string Name { get; set; } = "\uf530 Рефрактор дальнего действия";
    public override string Description { get; set; } = "Увеличивает обзор";
    public override float Weight { get; set; } = 10f;
    public override SpawnProperties? SpawnProperties { get; set; }
    private Dictionary<ushort, bool> _putons = [];
    
    protected override void SubscribeEvents()
    {
        base.SubscribeEvents();
        PlayerEvents.ChangingItem += OnUse;
        PlayerEvents.ItemRemoved += OnRemove;
    }

    protected override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        PlayerEvents.ChangingItem -= OnUse;
        PlayerEvents.ItemRemoved -= OnRemove;
    }

    protected override void ShowPickedUpMessage(Player player) { }
    protected override void ShowSelectedMessage(Player player) { }
    
    protected override void OnPickingUp(PickingUpItemEventArgs ev)
    {
        base.OnPickingUp(ev);
        ev.Player.ShowHint($"<size=44>Вы подобрали\n<color=#df65f7>{this.Name}</color></size>",
            3, DrawUIComponent.HintPosition.CENTER, nameof(Refractor));
    }
    protected override void OnChanging(ChangingItemEventArgs ev)
    {   
        base.OnChanging(ev);
        ev.Player.ShowHint($"<size=44>Вы взяли\n<color=#df65f7>{this.Name}</color></size>", 3,
            DrawUIComponent.HintPosition.CENTER, nameof(Refractor));
    }
    protected override void OnDroppingItem(DroppingItemEventArgs ev)
    {
        base.OnDroppingItem(ev);
        ev.Player.ShowHint($"<size=44>Вы выбросили\n<color=#df65f7>{this.Name}</color></size>", 3,
            DrawUIComponent.HintPosition.CENTER, nameof(Refractor));
        if (_putons.ContainsKey(ev.Item.Serial) && _putons[ev.Item.Serial])
        {
            ev.Player.DisableEffect<FogControl>();
            ev.Player.DisableEffect<Concussed>();
            _putons[ev.Item.Serial] = false;
        };
    }

    private void OnRemove(ItemRemovedEventArgs ev)
    {
        if (!Check(ev.Item)) return;
        _putons.Remove(ev.Item.Serial);
        ev.Player.DisableEffect<FogControl>();
        ev.Player.DisableEffect<Concussed>();
    }
    private void OnUse(ChangingItemEventArgs ev)
    {
        if (!Check(ev.Item)) return;
        if (_putons.ContainsKey(ev.Item.Serial) && _putons[ev.Item.Serial])
        {
            _putons[ev.Item.Serial] = false;
            ev.Player.ShowHint($"<size=44>Вы сняли\n<color=#7fa9db>{this.Name}</color></size>", 3,
                DrawUIComponent.HintPosition.CENTER, nameof(Refractor));
            ev.Player.DisableEffect<FogControl>();
            ev.Player.DisableEffect<Concussed>();
            ev.IsAllowed = false;
            return;
        }
        _putons[ev.Item.Serial] = true;
        ev.Player.ShowHint($"<size=44>Вы надели\n<color=#7fa9db>{this.Name}</color></size>", 3,
            DrawUIComponent.HintPosition.CENTER, nameof(Refractor));
        ev.Player.SessionVariables["refractor"] = true;
        ev.Player.EnableEffect<FogControl>(3);
        ev.Player.EnableEffect<Concussed>();
        ev.IsAllowed = false;
    }
}

[CustomItem(ItemType.GrenadeHE)]
public class MilitaryGrenade : CustomGrenade
{
    public override uint Id { get; set; } = 909;
    public override string Name { get; set; } = "Ударная граната";
    public override string Description { get; set; } = "Взрывается при касании поверхности";
    public override float Weight { get; set; } = 10f;
    public override SpawnProperties? SpawnProperties { get; set; }
    public override bool ExplodeOnCollision { get; set; } = true;
    public override float FuseTime { get; set; } = 25;
    protected override void ShowPickedUpMessage(Player player) { }

    protected override void ShowSelectedMessage(Player player) { }

    protected override void OnPickingUp(PickingUpItemEventArgs ev)
    {
        base.OnPickingUp(ev);
        ev.Player.ShowHint("Вы подобрали 🔥<color=#fc7200>Ударную гранату</color>", 3);
    }

    protected override void OnChanging(ChangingItemEventArgs ev)
    {
        base.OnChanging(ev);
        ev.Player.ShowHint("У вас в руках 🔥<color=#fc7200>Ударная граната</color>", 3);
    }
}