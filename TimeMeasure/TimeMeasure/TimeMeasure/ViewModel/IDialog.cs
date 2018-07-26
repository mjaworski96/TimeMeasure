using System.Threading.Tasks;

namespace TimeMeasure.ViewModel
{
    public interface IDialog
    {
        Task<bool> YesNoDialog(string title, string message);
        Task WarningDialog(string v1, string v2);
    }
}
