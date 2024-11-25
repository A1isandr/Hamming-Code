using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Task5.Components;
using Task5.Models;

namespace Task5.ViewModels;

public class InformationSourceViewModel : ViewModelBase
{
    private readonly InformationSourceComponent _informationSourceComponent;

    private readonly ReadOnlyObservableCollection<Message> _messages;
    public ReadOnlyObservableCollection<Message> Messages => _messages;
    
    [Reactive]
    public int NumberOfDigits { get; set; } = 15;
    
    [Reactive]
    public int NumberOfMessages { get; set; } = 6;
    
    [ObservableAsProperty]
    public bool IsBusy { get; set; }
    
    public ReactiveCommand<Unit, Unit> GenerateMessages { get; }
    
    public ReactiveCommand<Unit, Unit> GenerateMessagesCancelable { get; }
    
    public ReactiveCommand<Unit, Unit> CancelMessageGeneration { get; }
    
    
    public InformationSourceViewModel(InformationSourceComponent informationSourceComponent)
    {
        _informationSourceComponent = informationSourceComponent;
        
        var canGenerate = this
            .WhenAnyValue(
                x => x.NumberOfDigits,
                x => x.NumberOfMessages)
            .Select(number => number is {Item1: > 0, Item2: > 0});
        
        GenerateMessages = ReactiveCommand
            .CreateFromTask(GenerateMessagesImpl, canGenerate);
        
        GenerateMessagesCancelable = ReactiveCommand
            .CreateFromObservable(() => 
                GenerateMessages
                    .Execute()
                    .TakeUntil(CancelMessageGeneration!),
                canGenerate);

        CancelMessageGeneration = ReactiveCommand
            .Create(() => { }, GenerateMessages.IsExecuting);

        GenerateMessages
            .IsExecuting
            .ToPropertyEx(this, x => x.IsBusy);
        
        _informationSourceComponent
            .Connect()
            .Bind(out _messages)
            .Subscribe();
    }


    private async Task GenerateMessagesImpl(CancellationToken ct)
    {
        try
        {
            await _informationSourceComponent.GenerateMessages(NumberOfMessages, NumberOfDigits, ct);
        }
        catch (OperationCanceledException)
        {
        }
    }
}