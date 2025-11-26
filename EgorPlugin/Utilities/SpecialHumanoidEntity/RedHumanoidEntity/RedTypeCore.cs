using System;
using System.Collections.Generic;
using EgorPlugin.Utilities.SpecialEntity.OperationalSpecialEntity.Coroutines;
using EgorPlugin.Utilities.SpecialHumanoidEntity.OperationalSpecialEntity.Enums;
using Exiled.Events;
using Exiled.Events.EventArgs.Player;
using HUD;
using UnityEngine;
using static EgorPlugin.Utilities.SpecialHumanoidEntity.OperationalSpecialEntity.Enums.RedHumanoidEntityPowerTypes;
using Object = UnityEngine.Object;
namespace EgorPlugin.Utilities.SpecialHumanoidEntity.RedHumanoidEntity;

public class RedTypeCore
{
    internal static readonly Dictionary<Player, RedHumanoidEntityPowerTypes> RedType = [];
    internal static Dictionary<Player, ExpandedRegenerationThreads> ChosenExpandedPower = [];
    public static void AssignRegeneration(RedHumanoidEntityPowerTypes power, Player player)
    {
        
        switch (power)
                { 
                    // Ущербная регенерация
                    case Damaging:
                        RedType[player] = power;
                        player.ShowHint("Вы ощутили слабое чувство регенерировать к <b><color=#ff8f87>регенерации</color></b>.", 
                            7, DrawUIComponent.HintPosition.CENTER, nameof(RedTypeCore));
                        player.GameObject.AddComponent<RedRegenerationComponent>();
                        break;
                    // Ограниченная регенерация
                    case Limited:
                        RedType[player] = power;
                        player.MaxHealth += 125;
                        player.Heal(player.MaxHealth);
                        player.ShowHint("Вы ощутили силы к восстановлению разрушенных тканей тела <b><color=#ff7369>регенерацией</color></b>", 
                            7, DrawUIComponent.HintPosition.CENTER, nameof(RedTypeCore));
                        player.GameObject.AddComponent<RedRegenerationComponent>();
                        break;
                    
                    // Полная регенерация
                    case Full:
                        RedType[player] = power;
                        player.MaxHealth += 425;
                        player.Heal(player.MaxHealth);
                        player.ShowHint("Вы чувствуете аномальные способности к восстановлению вашего тела <b><color=#ff4a3d>регенерацией</color></b>.", 
                            7, DrawUIComponent.HintPosition.CENTER, nameof(RedTypeCore));
                        player.GameObject.AddComponent<RedRegenerationComponent>();
                        // отключить реал урон
                        break;
                    
                    // Расширенная регенерация
                    case Expanded:
                        while (!ChosenExpandedPower.ContainsKey(player))
                        {
                            player.ShowHint("Выберите, какой аспект своего тела будете прокачивать (.c <аспект>):" +
                                            "\n<b><color=#a6263b>Органы Чувств</color></b>" +
                                            "\n<b><color=#a423a8>Конечности</color></b>" +
                                            "\n<b><color=#572791>Крепость</color></b>" +
                                            "\n<b><color=#609120>Мутация</color></b>",
                                7, DrawUIComponent.HintPosition.CENTER, nameof(RedTypeCore) + ".Expanded");
                            // подвязать выбор мощи
                        }
                        player.ShowHint("Вы ощутили мощь <b><color=#ff1100>регенерации</color></b> вашего тела.", 
                            7, DrawUIComponent.HintPosition.CENTER, nameof(RedTypeCore));
                        RedType[player] = power;
                        player.MaxHealth += 905;
                        // отключить реал урон
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(power), power, null);
                }
    }

    public static void RemoveRegeneration(Player player)
    {
        player.GameObject.GetComponent<RedRegenerationComponent>().StopAllCoroutines();
        player.GameObject.GetComponent<RedRegenerationComponent>().Destroy();
        // вернуть максимальное хп для настоящего класса player.MaxHealth =
    }
    
    protected void SubscribeEvents()
    {
        PlayerEvents.Destroying += OnDestroying;
        PlayerEvents.ChangingRole += OnChangingRole;

    }

    protected void UnsubscribeEvents()
    {   
        PlayerEvents.Destroying -= OnDestroying;
        PlayerEvents.ChangingRole -= OnChangingRole;
    }

    private void OnDestroying(DestroyingEventArgs e)
    {
        RemoveRegeneration(e.Player);
    }

    private void OnChangingRole(ChangingRoleEventArgs e)
    {
        RemoveRegeneration(e.Player);
    }
}