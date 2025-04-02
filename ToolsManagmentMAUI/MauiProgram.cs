using Microsoft.Extensions.Logging;
using ToolsManagmentMAUI.Services;

namespace ToolsManagmentMAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("fa-solid-900.otf", "FontAwesome");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<ToolService>();
        builder.Services.AddSingleton<AlertService>();
        builder.Services.AddSingleton<ShoppingCartService>();

        return builder.Build();
    }
}
