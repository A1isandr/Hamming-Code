using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Task5.ViewModels;

namespace Task5.Views;

public partial class InformationSourceView : ReactiveUserControl<InformationSourceViewModel>
{
    public InformationSourceView()
    {
        InitializeComponent();

        DataContext = App.ServiceProvider.GetRequiredService<InformationSourceViewModel>();

        this.WhenActivated(disposables =>
        {
            this
                .BindCommand(ViewModel,
                    viewModel => viewModel.BeginOrCancelMessageGeneration,
                    view => view.BeginButton)
                .DisposeWith(disposables);

            this
                .WhenAnyValue(x => x.ViewModel!.IsBusy)
                .Subscribe(isBusy =>
                {
                    BeginButtonIcon.Content = isBusy 
                        ? MaterialIconKind.Stop 
                        : MaterialIconKind.Play;
                });
        });
    }
}