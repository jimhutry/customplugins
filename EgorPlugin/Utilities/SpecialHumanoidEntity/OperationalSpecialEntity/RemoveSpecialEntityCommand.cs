using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommandSystem;
using EgorPlugin.Utilites.SpecialEntity.OperationalSpecialEntity.Enums;
using EgorPlugin.Utilities.SpecialHumanoidEntity.OperationalSpecialEntity.Enums;
using Exiled.API.Features.Roles;
using Exiled.Permissions.Extensions;
using static EgorPlugin.Utilities.SpecialHumanoidEntity.RedHumanoidEntity.RedTypeCore;

namespace EgorPlugin.Utilities.SpecialHumanoidEntity.OperationalSpecialEntity;

public class RemoveSpecialEntityCommand : ICommand, IUsageProvider, IHelpProvider
{

    public string Command { get; } = "remove";
    public string[] Aliases { get; } = ["rm"];
    public string Description { get; } = "";
    public string[] Usage { get; } = ["player_id"];
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!sender.CheckPermission("rp.playerutils"))
        {
            response = "Нет прав.";
            return false;
        }

        if (arguments.Count < 1 || !int.TryParse(arguments.At(0), out var id))
        {
            response = GetHelp(arguments);
            return false;
        }
        
        var player = Player.Get(id);
                
        if(player == null)
        {
            response = $"Игрок под идентификатором <b>{id}</b> не найден!";
            return false;
        }
        
        if (player.Role is not HumanRole && player.Role is not SpectatorRole) 
        {
            response = $"Команда применима только на игроках человеческого класса или на наблюдателях.";
            return false;
        }

        if (RedType.ContainsKey(player))
        {
            RemoveRegeneration(player);
            response = "Успешно.";
            return true;
        }
        response = "Ничего не случилось (почему-то).";
        return true;
    }
    public string GetHelp(ArraySegment<string> arguments)
    {
        return $"<b>Помощь по использованию:</b> specialentity set {this.DisplayCommandUsage()}";
    }
}