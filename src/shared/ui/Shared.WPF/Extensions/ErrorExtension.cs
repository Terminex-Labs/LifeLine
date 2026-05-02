using System.Text;
using System.Windows;
using Terminex.Common.Results;

namespace Shared.WPF.Extensions
{
    public static class ErrorExtension
    {
        public static void ShowError(this IList<Error> errors)
        {
            if (errors.Count > 0)
            {
                var sb = new StringBuilder();

                foreach (var item in errors)
                    sb.AppendLine($"ErrorCode: {item.ErrorCode}").AppendLine($"ErrorMessage: {item.Message}");

                MessageBox.Show(sb.ToString());
            }
        }
    }
}
