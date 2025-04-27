using System;
using System.Threading.Tasks;
using WebApp.Common.Models;

namespace WebApp.Client.Components.Common.FormDialog;

public interface IFormDialogContent
{
    Action OnValidSubmitCallbackSuccess { get; set; }
    Func<ApiError, Task> OnValidSubmitCallbackErrorAsync { get; set; }
}
