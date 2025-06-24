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

public class MemagentStartAttack : ICommand, IUsageProvider
{
    public string Command { get; } = "attack";
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
        
        if (arguments.Count < 2 || 
            !float.TryParse(arguments.At(0), out float volume))
        {
            response = $"Помощь: {arguments.Array[0]} {this.DisplayCommandUsage()}";
            return false; 
        }
        
        if (Isattackactive)
        {
            response = "Атака уже включена.";
            return false;
        }
        var filepath = Path.Combine(Paths.Configs, "AudioFiles", string.Join(" ", arguments.Segment(1, arguments.Count - 1)));
        if (!File.Exists(filepath))
        {
            response = "Ошибка. Файл не найден.";
            return false;
        }
        Isattackactive = true;
        InitiateMemagentPlayback(filepath, volume);
        response = "Успешно.";
        return true;

    }

    

    
}
