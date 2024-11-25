using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Task5.ViewModels;

namespace Task5.Views;

public partial class ReceiverView : ReactiveUserControl<ReceiverViewModel>
{
    public ReceiverView()
    {
        InitializeComponent();

        DataContext = App.ServiceProvider.GetRequiredService<ReceiverViewModel>();
    }
}