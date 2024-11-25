using System;
using System.Collections.ObjectModel;
using DynamicData;
using Task5.Components;
using Task5.Models;

namespace Task5.ViewModels;

public class ReceiverViewModel : ViewModelBase
{
    private readonly ReceiverComponent _receiverComponent;

    private readonly ReadOnlyObservableCollection<ReceiverStats> _stats;
    public ReadOnlyObservableCollection<ReceiverStats> Stats => _stats;

    public ReceiverViewModel(ReceiverComponent receiverComponent)
    {
        _receiverComponent = receiverComponent;

        _receiverComponent
            .ConnectToStats()
            .Bind(out _stats)
            .Subscribe();
    }
}