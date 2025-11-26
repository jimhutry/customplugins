using System.Collections.Generic;
using CustomPlayerEffects;
using EgorPlugin.Features.SpecialHumanoidEntity.OperationalSpecialEntity.Enums;
using HUD;
using MEC;
using Mirror;
using static EgorPlugin.Features.SpecialHumanoidEntity.RedHumanoidEntity.RedTypeCore;
using Random = UnityEngine.Random;

namespace EgorPlugin.Features.SpecialHumanoidEntity.OperationalSpecialEntity.Components;

public class RedRegenerationComponent : NetworkBehaviour
{

    public void Start()
    {
        var player = Player.Get(gameObject);
        switch (RedType[player])
        {
            case RedHumanoidEntityPowerTypes.Damaging:
                Timing.RunCoroutine(DamagingPowerCoroutine(player));
                break;
            case RedHumanoidEntityPowerTypes.Limited:
                Timing.RunCoroutine(LimitedPowerCoroutine(player));
                Timing.RunCoroutine(EffectsCleaningCoroutine(player));
                break;
            case RedHumanoidEntityPowerTypes.Full:
                Timing.RunCoroutine(FullPowerCoroutine(player));
                Timing.RunCoroutine(PoweredEffectsCleaningCoroutine(player));
                break;
            case RedHumanoidEntityPowerTypes.Expanded:
                Timing.RunCoroutine(ExpandedPowerCoroutine(player));
                Timing.RunCoroutine(PoweredEffectsCleaningCoroutine(player));
                break;
            default:
                Log.Error("Unknown Red Type");
                Destroy(this);
                break;
        }
    }

    public void Destroy()
    {
        Destroy(this);
    }
    
    private static readonly List<string> BadhintsList = ["Боль ваших ран утихла, но вы по-прежнему чувствуете себя странно.", 
        "Ваши раны затянулись, но вы по-прежнему чувствуете легкую боль.", 
        "Вы чувствуете, что можете двигать мускулами на месте вашей раны."];
    private static readonly List<string> BadcustominfoList = ["Отверстия от пуль заращены лобковым волосом.", 
        "На конечностях человека видны небольшие отметки из кости.", 
        "Порезы на шее и туловище \"зашиты\" сухожилиями."];
    private static readonly List<string> GoodhintsList = ["Ваши раны быстро зарастают...", "Кровотечение резко остановилось, а раны исчезли.", "Лоскуты кожи быстро заросли обратно.", "Ваша боль быстро утихает..."];
    
    private static IEnumerator<float> DamagingPowerCoroutine(Player player)
    {
        while (RedType.ContainsKey(player))
        {
            if (Random.Range(1, 100) < 65 && player.Health < player.MaxHealth * 0.90)
            {
                player.ShowHint(BadhintsList.RandomItem(), 5, DrawUIComponent.HintPosition.CENTER, nameof(DamagingPowerCoroutine));
                player.CustomInfo = BadcustominfoList.RandomItem();
                player.Health += 4;
                player.EnableEffect<CardiacArrest>(0.5f);
            } else { player.Health += Random.Range(1, 3); }
            
            yield return Timing.WaitForSeconds(1.5f);
        }
    }

    private static IEnumerator<float> LimitedPowerCoroutine(Player plr)
    {
        while (RedType.ContainsKey(plr))
        {
            plr.Health += Random.Range(1, 5);
            yield return Timing.WaitForSeconds(0.5f);
        }
    }

    private static IEnumerator<float> EffectsCleaningCoroutine(Player plr)
    {
        while (RedType.ContainsKey(plr))
        {
            foreach (var effect in plr.ActiveEffects)
            {
                effect.DisableEffect();
                yield return Timing.WaitForSeconds(Random.Range(2, 14)/10);
            }
            yield return Timing.WaitForSeconds(6.5f);
        }
    }

    private static IEnumerator<float> FullPowerCoroutine(Player plr)
    {
        while (RedType.ContainsKey(plr))
        {
            if (Random.Range(1, 100) > 65 && plr.Health < plr.MaxHealth * 0.85)
            {
                plr.ShowHint(GoodhintsList.RandomItem(), 5, DrawUIComponent.HintPosition.CENTER, nameof(FullPowerCoroutine));
                plr.Health += 9;
            } else { plr.Health += Random.Range(5, 8); }
            
            yield return Timing.WaitForSeconds(0.4f);
        }
    }

    private static IEnumerator<float> PoweredEffectsCleaningCoroutine(Player plr)
    {
        while (RedType.ContainsKey(plr))
        {
            foreach (var effect in plr.ActiveEffects)
            {
                effect.DisableEffect();
                yield return Timing.WaitForSeconds(Random.Range(1, 6)/10);
            }
            yield return Timing.WaitForSeconds(2.5f);
        }
    }

    private static IEnumerator<float> ExpandedPowerCoroutine(Player plr)
    {
        while (RedType.ContainsKey(plr))
        {
            if (Random.Range(1, 100) > 65 && plr.Health < plr.MaxHealth * 0.85)
            {
                plr.ShowHint(GoodhintsList.RandomItem(), 5, DrawUIComponent.HintPosition.CENTER, nameof(ExpandedPowerCoroutine));
                plr.Health += 30;
            } else { plr.Health += Random.Range(15, 25); }
            
            yield return Timing.WaitForSeconds(0.3f);
        }
    }
}