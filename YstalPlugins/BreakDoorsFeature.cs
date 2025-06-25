using System;
using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;


namespace EgorPlugin;

public class BreakDoorsFeature
{
    private static List<Player> Players { get; } = [];

    public void RegisterEvents()
    {
        PlayerEvents.ChangingRole += OnChangingRole;
        PlayerEvents.Destroying += OnDestroying;
        PlayerEvents.InteractingDoor += OnInteractingDoor;
    }
    
    public void UnRegisterEvents()
    {
        PlayerEvents.ChangingRole -= OnChangingRole;
        PlayerEvents.Destroying -= OnDestroying;
        PlayerEvents.InteractingDoor -= OnInteractingDoor;
    }

    public static void GiveStatus(Player player)
    {
        if (!player.IsAlive)
        {
            return;
        }    
        Players.Add(player);
    }

    private void OnInteractingDoor(InteractingDoorEventArgs ev)
    {
        if (!Players.Contains(ev.Player)) return;
        if (ev.Door is BreakableDoor breakableDoor)
        {
            breakableDoor.Break();
        }
    }

    private void OnChangingRole(ChangingRoleEventArgs ev)
    {
        Players.Remove(ev.Player);
    }

    private void OnDestroying(DestroyingEventArgs ev)
    {
        Players.Remove(ev.Player);
    }
}