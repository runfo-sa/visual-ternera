namespace Main.Models
{
    public record struct ModuleAction(string Name, DelegateCommand<string> Command);
}
