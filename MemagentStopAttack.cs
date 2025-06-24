using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using CommandSystem;
using Exiled.API.Features;
using InventorySystem.Items.Firearms.Extensions;
using MEC;
using static EgorPlugin.MemagentVaccineItem;
using AudioPlayerApi;
using Exiled.Permissions.Extensions;
using Mirror;

namespace EgorPlugin;

public class MemagentStopAttack : ICommand, IUsageProvider
{
    public string Command { get; } = "stop";
    public string[] Aliases { get; } = null;
    public string Description { get; } = "Запускает деструктивный мемагент в аудиосистемы учреждения.";
    public string[] Usage { get; } = ["volume", "filename"];
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        
        if (!sender.CheckPermission("mem.attack"))
        {
            response = "Недостаточно прав.";
            return false;
        }
        
        Isattackactive = false;
        response = "Выключено";
        return true;

    }

    

    
}