using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Task5.Components;
using Task5.Models;
using Task5.Services;

namespace Task5.ViewModels;

public class SenderViewModel : ViewModelBase
{
    private readonly SenderComponent _senderComponent;
    private readonly SelectedMessageService _selectedMessageService;

    private readonly ReadOnlyObservableCollection<SenderStats> _stats;
    public ReadOnlyObservableCollection<SenderStats> Stats => _stats;
    
    [Reactive]
    public int? SelectedMessage { get; set; }
    
    
    public SenderViewModel(
        SenderComponent senderComponent,
        SelectedMessageService selectedMessageService)
    {
        _senderComponent = senderComponent;
        _selectedMessageService = selectedMessageService;

        _senderComponent
            .ConnectToStats()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _stats)
            .Subscribe();

        this
            .WhenAnyValue(x => x._selectedMessageService.SelectedMessage)
            .BindTo(this, x => x.SelectedMessage);
        
        this
            .WhenAnyValue(x => x.SelectedMessage)
            .Subscribe(x => _selectedMessageService.SelectedMessage = x);
    }
}