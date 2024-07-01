namespace Main.Models
{
    public record struct ModuleAction(
        string Name,
        string DisplayName,
        DelegateCommand<string> Command,
        Material.Icons.MaterialIconKind? Icon = Material.Icons.MaterialIconKind.Help);
}
