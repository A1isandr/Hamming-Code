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

public class ReceiverViewModel : ViewModelBase
{
    private readonly ReceiverComponent _receiverComponent;
    private readonly SelectedMessageService _selectedMessageService;

    private readonly ReadOnlyObservableCollection<ReceiverStats> _stats;
    public ReadOnlyObservableCollection<ReceiverStats> Stats => _stats;
        
    [Reactive]
    public int? SelectedMessage { get; set; }
    
    
    public ReceiverViewModel(
        ReceiverComponent receiverComponent,
        SelectedMessageService selectedMessageService)
    {
        _receiverComponent = receiverComponent;
        _selectedMessageService = selectedMessageService;

        _receiverComponent
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