using System;
using System.Collections.Generic;
using AdminToys;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Components;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using InventorySystem.Items.Usables;
using MEC;
using UnityEngine;
using HUD;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.Events.EventArgs;

namespace EgorPlugin;

public class Scp6593
{
    private const string SchemName = "NF_Scp6593_EGOR";
    private const string MoneyCatcher = "CoinReceiver6593";
    private static readonly List<string> Products =
    [
        "MedKit", "PainKillers", "Adrenaline", "Epinephrine", 
        "Scp500", "Scp207", "Scp207914", "Scp1853",
        "ExplosiveGrenade", "FlashGrenade", "SmokeGrenade", "FireGrenade"
    ];
    private static readonly Dictionary<string, int> Prices = new Dictionary<string, int>()
    {
        ["MedKit"] = 2,
        ["PainKillers"] = 1,
        ["Adrenaline"] = 2,
        ["Epinephrine"] = 3,
        
        ["Scp500"] = 3,
        ["Scp207"] = 2,
        ["Scp207914"] = 2,
        ["Scp1853"] = 3,
        
        ["ExplosiveGrenade"] = 2,
        ["FlashGrenade"] = 1,
        ["SmokeGrenade"] = 1,
        ["FireGrenade"] = 3
    };

    private string _currentproduct;
    private int _amountofmoney = 0;
    private bool _isActivated = false;
    private SchematicObject? _schematic;
    
    public void SubscribeEvents()
    {
        PlayerEvents.DroppingItem += OnDroppingItem;
        SchematicEvents.SchematicSpawned += OnSchematicSpawned;
        SchematicEvents.SchematicDestroyed += OnSchematicDestroyed;
    }
    public void UnsubscribeEvents()
    {
        PlayerEvents.DroppingItem -= OnDroppingItem;
        SchematicEvents.SchematicSpawned -= OnSchematicSpawned;
        SchematicEvents.SchematicDestroyed -= OnSchematicDestroyed;

    }
    
    private void OnSchematicSpawned(SchematicSpawnedEventArgs ev)
    {
        _schematic = ev.Schematic;
    }

    private void OnSchematicDestroyed(SchematicDestroyedEventArgs ev)
    {
        _schematic = null;
    }
    private void OnDroppingItem(DroppingItemEventArgs ev)
    {
        if (ev.IsThrown)
        {
            var hits = Physics.RaycastAll(ev.Player.Position, ev.Player.CameraTransform.forward, 2f);
            
            if (hits == null)
            {
                return;
            }
            
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.name == MoneyCatcher && 
                    _schematic.AttachedBlocks.Contains(hit.collider.gameObject))
                {
                    ev.Item.Destroy();
                    AcceptMoney();
                    break;
                }
            }
        }
    }

    private void AcceptMoney()
    {
        if (_amountofmoney == 0)
        {
            _isActivated = true;
            return;
        }

        _amountofmoney++;
    }
}