namespace ToolsManagmentMAUI.Services;

public class AlertService
{
    public async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        return await Application.Current.MainPage.DisplayAlert(title, message,
            "Tak", "Nie");
    }

    public async Task ShowMessageAsync(string title, string message)
    {
        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }

    public async Task ShowSuccessMessageAsync(string message)
    {
        await Application.Current.MainPage.DisplayAlert("Sukces", message,
            "OK");
    }

    public async Task ShowErrorMessageAsync(string message)
    {
        await Application.Current.MainPage.DisplayAlert("B³¹d", message, "OK");
    }
}
