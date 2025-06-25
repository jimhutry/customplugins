using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using HUD;

namespace EgorPlugin;

[CustomItem(ItemType.KeycardChaosInsurgency)]
public class Pulsometr : CustomItem
{
    public override uint Id { get; set; } = 446;
    public override string Name { get; set; } = "\ue025Пульсометр";
    public override string Description { get; set; } = "Сканирует пространство впереди себя на формы жизни.";
    public override float Weight { get; set; } = 4f;
    public override SpawnProperties? SpawnProperties { get; set; }
    private Dictionary<ushort, bool> _enableds = [];

    protected override void SubscribeEvents()
    {
        base.SubscribeEvents();
    }

    protected override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
    }

    protected override void ShowPickedUpMessage(Player player)
    {
    }

    protected override void ShowSelectedMessage(Player player)
    {
    }

    protected override void OnAcquired(Player player, Item item, bool displayMessage)
    {
        base.OnAcquired(player, item, displayMessage);
        player.ShowHint($"<size=44>Вы подобрали\n<color=#a84397>{this.Name}</color></size>",
            3, DrawUIComponent.HintPosition.CENTER, nameof(Pulsometr));
        _enableds[item.Serial] = false;
    }

    protected override void OnChanging(ChangingItemEventArgs ev)
    {   
        base.OnChanging(ev);
        ev.Player.ShowHint($"<size=44>Вы взяли\n<color=#a84397>{this.Name}</color></size>", 3,
            DrawUIComponent.HintPosition.CENTER, nameof(Pulsometr));
    }
    
    protected override void OnDroppingItem(DroppingItemEventArgs ev)
    {
        base.OnDroppingItem(ev);
        if (!ev.IsThrown)
        {
            _enableds[ev.Item.Serial] = false;
            return;
        }
        ev.IsAllowed = false;
        if (_enableds[ev.Item.Serial])
        {
            
        }
        ev.Player.ShowHint("Вы включили сканирование пульсометра.", 3, DrawUIComponent.HintPosition.CENTER, nameof(Pulsometr));
    }
}