using System.Collections.Generic;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using HUD;
using ProjectMER.Features.Extensions;
using UnityEngine;

namespace EgorPlugin.ReferenceDump;

[CustomItem(ItemType.Medkit)]
public class MedicineFeatures : CustomItem
{
    public override uint Id { get; set; } = 132;
    public override string Name { get; set; } = "Медицинский набор";
    public override string Description { get; set; } = "Многоразовый меднабор для качественных медицинских услуг.";
    public override float Weight { get; set; } = 15f;
    public override SpawnProperties? SpawnProperties { get; set; }
    public override Vector3 Scale { get; set; } = new(1.5f, 1, 1.5f);

    private const byte MedkitIntensity = 80;
    
    private const byte PainKillersIntensity = 20;
    
    private const byte AdrenalineIntensity = 50;

    private const byte MedicalkitIntensity = 95;


    private Dictionary<ushort, int> UsageOfKit { get; set; } = [];
    
    public PluginPriority Priority { get; set; } = PluginPriority.Default;
    
    // подписки
    
    protected override void SubscribeEvents()
    {
        base.SubscribeEvents();
        PlayerEvents.UsingItemCompleted += OnUsedItem;
        PlayerEvents.UsingItem += OnUsingItem;
        PlayerEvents.CancellingItemUse += OnCancel;
        PlayerEvents.UsedItem += OnUsedDItem;

    }

    protected override void UnsubscribeEvents()
    {   
        base.UnsubscribeEvents();
        PlayerEvents.UsingItemCompleted -= OnUsedItem;
        PlayerEvents.UsingItem -= OnUsingItem;
        PlayerEvents.CancellingItemUse -= OnCancel;
        PlayerEvents.UsedItem -= OnUsedDItem;
    }

    // чистка

    
    // перезапись методов customitem

    protected override void ShowSelectedMessage(Player player) { }
    protected override void ShowPickedUpMessage(Player player) { }

    protected override void OnPickingUp(PickingUpItemEventArgs ev)
    {
        base.OnPickingUp(ev);
        ev.Player.ShowHint("Вы подобрали <color=#ab4b69>\uf479 Медицинский Набор</color>", 3f, DrawUIComponent.HintPosition.CENTER, nameof(MedicineFeatures));

    }
    protected override void OnChanging(ChangingItemEventArgs ev)
    {
        base.OnChanging(ev);
        UsageOfKit.TryAdd(ev.Item.Serial, 3);
        switch (UsageOfKit[ev.Item.Serial])
        {
            case 3:
                ev.Player.ShowHint("Вы взяли <color=#ab4b69>\uf479 Медицинский Набор</color>\nМедикаментов в наборе осталось на <b>3</b> использования.", 3f, DrawUIComponent.HintPosition.CENTER, nameof(MedicineFeatures));
                break;
            case 2:
                ev.Player.ShowHint("Вы взяли <color=#ab4b69>\uf479 Медицинский Набор</color>\nМедикаментов в наборе осталось на <b>2</b> использования.", 3f, DrawUIComponent.HintPosition.CENTER, nameof(MedicineFeatures));
                break;
            case 1:
                ev.Player.ShowHint("Вы взяли <color=#ab4b69>\uf479 Медицинский Набор</color>\nМедикаментов в наборе осталось на <b>1</b> использование.", 3f, DrawUIComponent.HintPosition.CENTER, nameof(MedicineFeatures));
                break;
        }
        
    }
    
    protected override void OnDroppingItem(DroppingItemEventArgs ev)
    {
        base.OnDroppingItem(ev);
        ev.Player.ShowHint("Вы выбросили <color=#ab4b69>\uf479 Медицинский Набор</color>", 3f, DrawUIComponent.HintPosition.CENTER, nameof(MedicineFeatures));
    }

    // методы ивентов

    private void OnUsingItem(UsingItemEventArgs ev)
    {
        // выдача эффектов для медицинских предметов
        if (!Check(ev.Item))
        {
            switch (ev.Item.Type)
            {
                case ItemType.Medkit:
                {
                    if (ev.Player.TryGetEffect<Slowness>(out var statusEffect))
                    {
                        if (statusEffect.Intensity > MedkitIntensity && statusEffect.TimeLeft > 4.2f)
                        {
                            StoreAndApplySlowness(ev.Player, statusEffect.Intensity, statusEffect.TimeLeft);
                            return;
                        }
                    }
                    ev.Player.EnableEffect<Slowness>(MedkitIntensity, 4.2f);
                    return;
                }
                

                case ItemType.Painkillers:
                    
                    if (ev.Player.TryGetEffect<Slowness>(out var statusEffect2))
                    {
                        if (statusEffect2.Intensity > PainKillersIntensity && statusEffect2.TimeLeft > 1.2f)
                        {
                            StoreAndApplySlowness(ev.Player, statusEffect2.Intensity, statusEffect2.TimeLeft);
                            return;

                        }
                    }
                    ev.Player.EnableEffect<Slowness>(PainKillersIntensity, 1.2f);
                    return;

                case ItemType.Adrenaline:
                    
                    if (ev.Player.TryGetEffect<Slowness>(out var statusEffect3))
                    {
                        if (statusEffect3.Intensity > AdrenalineIntensity && statusEffect3.TimeLeft > 2.1f)
                        {
                            StoreAndApplySlowness(ev.Player, statusEffect3.Intensity, statusEffect3.TimeLeft);
                            return;
                        }
                    }
                    ev.Player.EnableEffect<Slowness>(AdrenalineIntensity, 2.1f);
                    return;
                default:
                    return;
            }
            
        }
        // выдача эффекта для меднабора
        if (ev.Player.TryGetEffect<Slowness>(out var statusEffect4))
        {
            if (statusEffect4.Intensity > MedicalkitIntensity && statusEffect4.TimeLeft > 4.2f)
            {
                StoreAndApplySlowness(ev.Player, statusEffect4.Intensity, statusEffect4.TimeLeft);
                return;
            }
        }
        ev.Player.EnableEffect<Slowness>(MedicalkitIntensity, 4.2f);
    }

    
    private void OnCancel(CancellingItemUseEventArgs ev)
    {
        if (ev.Item.Type is not (ItemType.Adrenaline or ItemType.Painkillers or ItemType.Medkit) && !Check(ev.Item)) return;
        if (!ev.Player.SessionVariables.ContainsKey("medicine-features"))
        {
            ev.Player.DisableEffect<Slowness>();
            return;
        } 
        var success = ev.Player.TryGetSessionVariable<List<float>>("medicine-features", out var effinfo);
        ev.Player.DisableEffect<Slowness>();
        ev.Player.EnableEffect<Slowness>(intensity: (byte)effinfo[0], duration: effinfo[1]);
        ev.Player.SessionVariables.Remove("medicine-features");
    }
    private void StoreAndApplySlowness(Player player, byte intensity, float duration)
    {
        player.SessionVariables["medicine-features"] = new List<float> { intensity, duration };
    }

    private void OnUsedDItem(UsedItemEventArgs ev)
    {
        if (ev.Item.Type is not (ItemType.Medkit or ItemType.Painkillers or ItemType.Adrenaline)) return;
        if (!ev.Player.SessionVariables.ContainsKey("medicine-features")) return;
        var success = ev.Player.TryGetSessionVariable<List<float>>("medicine-features", out var effinfo);        
        ev.Player.EnableEffect<Slowness>(intensity: (byte) effinfo[0], duration: effinfo[1]);
        ev.Player.SessionVariables.Remove("medicine-features");
    }
    
    private void OnUsedItem(UsingItemCompletedEventArgs ev)
    {
        
        if (!Check(ev.Item)) return;
        UsageOfKit.TryAdd(ev.Item.Serial, 3);
        UsageOfKit[ev.Item.Serial]--;
        ev.IsAllowed = false;
        ev.Player.Heal(75);
        ev.Player.SessionVariables.Remove("medicine-features");
        foreach (var effect in ev.Player.ActiveEffects)
        {
            if (effect.Classification == StatusEffectBase.EffectClassification.Negative)
            {
                effect.DisableEffect();
            }
        }
        if (UsageOfKit[ev.Item.Serial] != 0) return;
        ev.Player.ShowHint("Вы израсходовали все медикаменты.", 3f, DrawUIComponent.HintPosition.CENTER, nameof(MedicineFeatures));
        Log.Info("Удаление предмета");
        ev.Item.Destroy();
        UsageOfKit.Remove(ev.Item.Serial);        
    }
}