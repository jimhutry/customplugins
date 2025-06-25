using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CommandSystem;
using Exiled.Permissions.Extensions;


namespace EgorPlugin;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class TechFeaturesParentCommand : ParentCommand
{
    
    public new List<ICommand> Commands { get; } = [new BreakDoorsCommand()];
    public override void LoadGeneratedCommands()
    {
        foreach (ICommand command in Commands)
        {
            RegisterCommand(command);
        }
    }

    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!sender.CheckPermission("tf"))
        {
            response = "Недостаточно прав.";
            return false;
        }

        var b = new StringBuilder("Введите суб-команду. Доступные суб-команды:\n");
        foreach (ICommand command in Commands)
        {
            b.Append(command.Command + ": " + command.Description + "\n");
        }
        response = b.ToString();
        return true;
    }

    public override string Command { get; } = "techfeatures";
    public override string[] Aliases { get; } = ["techf", "tf"];
    public override string Description { get; } = "Команда, отвечающая за техническое взаимодействие с комплексом.";
}