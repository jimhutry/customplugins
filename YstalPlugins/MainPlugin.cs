using Exiled.API.Features;
using Exiled.CustomItems;
using Exiled.CustomItems.API;
using Exiled.CustomItems.API.Features;

namespace EgorPlugin;

public class MainPlugin : Plugin<Config>
{
    public override string Name { get; } = "даунскийплагин";
    public override string Author { get; } = "егор";
    public override void OnEnabled()
    {
        base.OnEnabled();
        CustomItem.RegisterItems();
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
        CustomItem.UnregisterItems();
    }
}