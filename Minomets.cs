using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MapEditorReborn;
using MapEditorReborn.Events.Handlers;
using PlayerRoles;
using UnityEngine;

namespace EgorPlugin;

public class Minomets : Plugin<Config>
{
    public bool IsPaused { get; set; } = false;
    public Vector3 spawn1;
    public Vector3 spawn2;
    public List<Player> team1 = [];
    public List<Player> team2 = [];
    public Vector3 mapspawn;
    public string mapname;
    
    public bool isStarted {get;set;} = false;
    

    public void SubscribeEvents()
    {
        PlayerEvents.Died += OnDie;
    }

    public void UnsubscribeEvents()
    {
        PlayerEvents.Died -= OnDie;
    }
    
    public void OnStart()
    {
        isStarted = true;
        IsPaused = false;
        MapEditorReborn.API.Features.ObjectSpawner.SpawnSchematic(mapname, mapspawn, null, new Vector3(1,1,1), null!);
        var plrlist = Player.List.ToArray();
        if (Player.List.Count > 2)
        {
            for (int i = 0; i < plrlist.Length - 1; i++)
            {
                plrlist[i].Role.Set(RoleTypeId.ClassD);
                plrlist[i].Position = spawn1;
                team1.Add(plrlist[i]);
                plrlist[i + 1].Role.Set(RoleTypeId.FacilityGuard);
                plrlist[i + 1].Position = spawn2;
                team1.Add(plrlist[i + 1]);


                CustomItem.TryGive(plrlist[i], 31);
                CustomItem.TryGive(plrlist[i + 1], 31);

            }
        }
    }

    public void OnEnd()
    {
        isStarted = false;
        
        var plrlist = Player.List.ToArray();
        
    }
    
    private void OnDie(DiedEventArgs ev)
    {
        ev.Player.Role.Set(RoleTypeId.Overwatch);
        var counter = 0;
        foreach (var plr in team1)
        {
            if (!plr.IsAlive)
            {
                counter++;
            }
        }
        if (counter == team1.Count)
        {
            Log.Info("конец");
        }

        counter = 0;
        
        foreach (var plr in team2)
        {
            if (!plr.IsAlive)
            {
                counter++;
            }
        }
        if (counter == team2.Count)
        {
            Log.Info("конец");
        }
    }
    
}



// Start
// End 
// Pause 
// Resume


