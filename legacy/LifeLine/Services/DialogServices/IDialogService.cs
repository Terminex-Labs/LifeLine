using LifeLine.Utils.Enum;

namespace LifeLine.Services.DialogServices
{
    public interface IDialogService
    {
        bool ShowMessage(string Message, string Title = "Предупреждение", MessageButtons messageButtons = MessageButtons.OK);
        MessageButtons ShowMessageButton(string Message, string Title = "Предупреждение", MessageButtons messageButtons = MessageButtons.OK);
    }
}
