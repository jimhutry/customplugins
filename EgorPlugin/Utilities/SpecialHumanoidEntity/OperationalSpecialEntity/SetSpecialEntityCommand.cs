using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using EgorPlugin.Utilites.SpecialEntity.OperationalSpecialEntity.Enums;
using EgorPlugin.Utilities.SpecialHumanoidEntity.OperationalSpecialEntity.Enums;
using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using Exiled.Permissions.Extensions;
using static EgorPlugin.Utilities.SpecialHumanoidEntity.RedHumanoidEntity.RedTypeCore;

namespace EgorPlugin.Utilities.SpecialHumanoidEntity.OperationalSpecialEntity;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class SetSpecialEntityCommand : ICommand, IUsageProvider, IHelpProvider
{
    public string Command { get; } = "set";
    public string[] Aliases { get; } = ["s"];
    public string Description { get; } = "";
    public string[] Usage { get; } = ["player_id", "entity_type"];
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!sender.CheckPermission("rp.playerutils"))
        {
            response = "Нет прав.";
            return false;
        }

        if (arguments.Count < 1 || !int.TryParse(arguments.At(0), out int id))
        {
            response = GetHelp(arguments);
            return false;
        }

        var player = Player.Get(id);
        
        if (!Enum.TryParse(arguments.At(1), out SpecialEntityTypes type))
        {
            response = "<b>Типы сущностей (читайте лор):</b> Green, Grey, Red.";
            return false;
        }
                
        if (player == null)
        {
            response = $"Игрок под идентификатором <b>{id}</b> не найден!";
            return false;
        }
        
        if (player.Role is not HumanRole && player.Role is not SpectatorRole) 
        {
            response = $"Команда применима только на игроках человеческого класса или на наблюдателях.";
            return false;
        }
        
        switch (type)
        {
            case SpecialEntityTypes.Grey:
                response = "В разработке";
                return true;
            case SpecialEntityTypes.Green:
                response = "В разработке";
                return true;
            case SpecialEntityTypes.Red:
                if (arguments.Count < 3 || !int.TryParse(arguments.At(2), out int powerLevel))
                {
                    response = "<b>Помощь по использованию:</b> specialentity set Red power(0-3)";
                    return false;
                }
                AssignRegeneration((RedHumanoidEntityPowerTypes) Convert.ToInt32(GlobalProjectFunctions.Clamp(powerLevel, 0, 3)), player);
                response = "Успешно.";  
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        response = "a";
        return true;
    }

    
    public string GetHelp(ArraySegment<string> arguments)
        {
            return $"<b>Помощь по использованию:</b> specialentity set {this.DisplayCommandUsage()}";
        }
    }
    