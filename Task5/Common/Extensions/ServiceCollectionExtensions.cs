using Microsoft.Extensions.DependencyInjection;
using SukiUI.Dialogs;
using Task5.Components;
using Task5.Services;
using Task5.ViewModels;

namespace Task5.Common;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<SenderViewModel>();
        services.AddSingleton<ReceiverViewModel>();
        services.AddSingleton<InformationSourceViewModel>();

        services.AddSingleton<InformationSourceComponent>();
        services.AddSingleton<SenderComponent>();
        services.AddSingleton<ReceiverComponent>();

        services.AddSingleton<SelectedMessageService>();

        services.AddSingleton<ISukiDialogManager, SukiDialogManager>();
    }
}