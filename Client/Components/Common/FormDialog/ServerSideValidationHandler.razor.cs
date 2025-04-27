using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using WebApp.Common.Models;

namespace WebApp.Client.Components.Common.FormDialog;

public class ServerSideValidationHandler : ComponentBase, IDisposable
{
    [CascadingParameter]
    private EditContext CurrentEditContext { get; set; }

    private ValidationMessageStore MessageStore { get; set; }

    /// <summary>
    /// Error not bound to any particular field
    /// </summary>
    public string UnboundError { get; set; }

    protected override void OnInitialized()
    {
        if (CurrentEditContext == null)
        {
            throw new InvalidOperationException($"{nameof(ServerSideValidationHandler)} must be placed inside of an {nameof(EditForm)}");
        }

        MessageStore = new ValidationMessageStore(CurrentEditContext);

        CurrentEditContext.OnValidationRequested += OnValidationRequested;
        CurrentEditContext.OnFieldChanged += OnFieldChanged;

        base.OnInitialized();
    }

    public void DisplayApiErrors(ApiError error)
    {
        if (error.Errors is not null)
        {
            foreach (var err in error.Errors)
            {
                MessageStore.Add(CurrentEditContext.Field(err.Key), err.Value);
            }
        }
        else
        {
            UnboundError = error.Message;
        }

        CurrentEditContext.NotifyValidationStateChanged();
    }

    public void ClearErrors()
    {
        MessageStore.Clear();
        UnboundError = string.Empty;
        CurrentEditContext.NotifyValidationStateChanged();
    }

    private void OnValidationRequested(object s, ValidationRequestedEventArgs e)
    {
        MessageStore.Clear();
    }

    private void OnFieldChanged(object s, FieldChangedEventArgs e)
    {
        MessageStore.Clear(e.FieldIdentifier);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            CurrentEditContext.OnValidationRequested -= OnValidationRequested;
            CurrentEditContext.OnFieldChanged -= OnFieldChanged;
        }
    }
}
