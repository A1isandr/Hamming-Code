using System;
using System.Collections.ObjectModel;
using DynamicData;
using Task5.Components;
using Task5.Models;

namespace Task5.ViewModels;

public class SenderViewModel : ViewModelBase
{
    private readonly SenderComponent _senderComponent;

    private readonly ReadOnlyObservableCollection<SenderStats> _stats;
    public ReadOnlyObservableCollection<SenderStats> Stats => _stats;

    public SenderViewModel(SenderComponent senderComponent)
    {
        _senderComponent = senderComponent;

        _senderComponent
            .ConnectToStats()
            .Bind(out _stats)
            .Subscribe();
    }
}