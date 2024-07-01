using Editor.Services;
using System.Drawing.Printing;
using System.Windows.Data;

namespace Editor.ViewModels
{
    public class ToolbarViewModel(ICommandService commandService) : BindableBase
    {
        public ICommandService CommandService => commandService;
        public static ListCollectionView Printers => new(PrinterSettings.InstalledPrinters.Cast<string>().ToList());
    }
}
