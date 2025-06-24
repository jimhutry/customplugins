using System;
using System.Collections.Generic;
using System.Linq;
using AudioPlayerApi;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using UnityEngine;

namespace EgorPlugin;


public class MemagentVaccineItem : CustomItem
{
    public const float MaxMemagentDistance = 20f;
    
    public const float VaccineDistance = 2f;
    public static MemagentVaccineItem Singleton { get; private set; }
    
    public PluginPriority Priority { get; set; } = PluginPriority.Default;

    public MemagentVaccineItem()
    {
        Singleton = this;
    }
    
    public static AudioPlayer MemagentExample;
    
    public override uint Id { get; set; } = 555;

    public override string Name { get; set; } = "\uf070 Контр-мемагент";

    public override string Description { get; set; } =
        "Прививка от меметического эффекта, наделяет иммунитетом к опасной информации.";

    public override float Weight { get; set; } = 0.1f;

    public override ItemType Type { get; set; } = ItemType.Coin;

    public override SpawnProperties SpawnProperties { get; set; }

    private List<Player> _immunedPlayers;

    public static bool Isattackactive;
    
    public void RegisterEvents()
    {
        this.Register();
        _immunedPlayers = new();
        PlayerEvents.FlippingCoin += OnFlippingCoin;
        PlayerEvents.ChangingRole += OnChangingRole;
        PlayerEvents.Destroying += OnDestroying;


    }

    public void UnregisterEvents()
    {
        this.Unregister();
        _immunedPlayers = null;
        PlayerEvents.FlippingCoin -= OnFlippingCoin;
        PlayerEvents.ChangingRole -= OnChangingRole;
        PlayerEvents.Destroying -= OnDestroying;
    }

    protected override void ShowSelectedMessage(Player player) { }
    protected override void ShowPickedUpMessage(Player player) { }

    protected override void OnPickingUp(PickingUpItemEventArgs ev)
    {
        base.OnPickingUp(ev);
        ev.Player.ShowHint("Вы подобрали <color=#b56e69>\uf070 Контр-Мемагент</color>", 3);
    }

    protected override void OnChanging(ChangingItemEventArgs ev)
    {
        base.OnChanging(ev);
        ev.Player.ShowHint("У вас в руках <color=#b56e69>\uf070 Контр-Мемагент</color>", 3);
    }

    protected override void OnDroppingItem(DroppingItemEventArgs ev)
    {
        base.OnDroppingItem(ev);
        ev.Player.ShowHint("Вы выбросили <color=#b56e69>\uf070 Контр-Мемагент</color>", 3);
    }


    public static void InitiateMemagentPlayback(string fileName, float volume)
    {
        var audioPlayer = AudioPlayer.Create("MemagentPlayback", fileName,
            destroyWhenAllClipsPlayed: true);
        var speakers = new List<Speaker>(); 
        MemagentExample = audioPlayer;
        var lenghtofsound = (float) MemagentExample.ClipsById[0].Duration.TotalSeconds;
        foreach (var speaker in Room.List.SelectMany(room => room.Speakers))
        {
            speakers.Add(audioPlayer.AddSpeaker($"Memagent_{Guid.NewGuid()}", 
                speaker.Position, volume, maxDistance: MaxMemagentDistance));
        } 
        Timing.RunCoroutine(AffectingCoroutine(lenghtofsound, speakers, audioPlayer), nameof(MemagentVaccineItem));
    }

    public static void DestroyMemagentPlayback(AudioPlayer audioPlayer)
    {
        audioPlayer.Destroy();
    }
    private void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        if (!Check(ev.Item))
        {
            return;
        }

        ev.IsAllowed = false;
        foreach (var player in Player.List)
        {
            if (Vector3.Distance(ev.Player.Position, player.Position) <= VaccineDistance)
            {
                _immunedPlayers.Append(player);
                player.ShowHint($"Вы взглянули на изображение из рук {ev.Player.Nickname}");
            }
        }
    }

    private void OnChangingRole(ChangingRoleEventArgs ev)
    {
        if (_immunedPlayers.Contains(ev.Player))
        {
            _immunedPlayers.Remove(ev.Player);
        }
    }

    
    
    private void OnDestroying(DestroyingEventArgs ev)
    {
        if (_immunedPlayers.Contains(ev.Player))
        {
            _immunedPlayers.Remove(ev.Player);
        }
    }
    
    public static bool IsPlayerInSpeakerRadius(Player player, List<Speaker> speakers)
    {
        foreach (var speaker in speakers)
        {
            if (Vector3.Distance(speaker.Position, player.Position) <= MaxMemagentDistance)
            {
                return true;
            }
        }
        return false;
    }
    
    public static bool IsPlayerImmuned(Player player)
    {
        return Singleton._immunedPlayers.Contains(player);
    }
    
    public static void EnableMemEffects(Player player)
    {        
        Timing.RunCoroutine(AffectMemCoroutine(player), nameof(MemagentVaccineItem));
    }

    public static void RemoveMemEffects(Player player)
    {
        Timing.RunCoroutine(RemoveMemCoroutine(player), nameof(MemagentVaccineItem) + "1");
    }
    
    
    private static IEnumerator<float> RemoveMemCoroutine(Player player)
    {
        Timing.KillCoroutines(nameof(MemagentVaccineItem));
        yield return Timing.WaitForSeconds(3f);
        
        player.DisableEffect<Blurred>();
        
        yield return Timing.WaitForSeconds(1.5f);
        
        player.DisableEffect<Concussed>();
        
        yield return Timing.WaitForSeconds(3.5f);
        
        player.DisableEffect<Burned>();
        player.DisableEffect<Deafened>();
    }
    
    private static IEnumerator<float> AffectMemCoroutine(Player player)
    {
        player.EnableEffect<Burned>();
        
        yield return Timing.WaitForSeconds(7.5f);
        
        player.DisableEffect<Concussed>();
        
        yield return Timing.WaitForSeconds(3f);
        
        player.EnableEffect<Deafened>();
        player.EnableEffect<Blurred>(duration: 15f);
        
        yield return Timing.WaitForSeconds(1.5f);

        while (player.Health > 10)
        {
            player.Hurt(5);
            yield return Timing.WaitForSeconds(0.5f);
        }
    }
    
    public static IEnumerator<float> AffectingCoroutine(float duration, List<Speaker> speakers, AudioPlayer audioPlayer)
    {
            for (var i = 0; i < duration * 10; i++)
            {
                if (Isattackactive)
                {
                    Log.Info(duration);
                    Log.Info("hui");
                    Log.Info(Singleton._immunedPlayers);
                    foreach (var player in Player.List)
                    {
                        if (!IsPlayerInSpeakerRadius(player, speakers)) continue;
                        if (!IsPlayerImmuned(player))
                        {
                            EnableMemEffects(player);
                        }
                        else
                        {
                            RemoveMemEffects(player);
                        }
                    } 
                    yield return Timing.WaitForSeconds(0.1f);
                }
                else
                {
                    Log.Info("hui2");
                    DestroyMemagentPlayback(audioPlayer);
                    break;
                }
            }

            Isattackactive = false;
    }
}