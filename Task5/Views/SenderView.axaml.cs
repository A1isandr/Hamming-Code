using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Task5.ViewModels;

namespace Task5.Views;

public partial class SenderView : ReactiveUserControl<SenderViewModel>
{
    public SenderView()
    {
        InitializeComponent();
        
        DataContext = App.ServiceProvider.GetRequiredService<SenderViewModel>();
    }
}