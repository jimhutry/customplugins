

using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;

namespace EgorPlugin.Utilites;

public class ParentSpecialEntityCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        throw new NotImplementedException();
    }

    public string Command { get; } = "setspecialentity";
    public string[] Aliases { get; } = { "setentity", "specialentity"};
    public string Description { get; } = "Установить гуманоидные угрожающие сущности";
    
}