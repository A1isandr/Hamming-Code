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
using Task5.Services;

namespace Task5.ViewModels;

public class InformationSourceViewModel : ViewModelBase
{
    private readonly InformationSourceComponent _informationSourceComponent;
    private readonly SelectedMessageService _selectedMessageService;

    private readonly ReadOnlyObservableCollection<Message> _messages;
    public ReadOnlyObservableCollection<Message> Messages => _messages;
    
    [Reactive]
    public int NumberOfDigits { get; set; } = 15;
    
    [Reactive]
    public int NumberOfMessages { get; set; } = 6;
    
    [Reactive]
    public int? SelectedMessage { get; set; }
    
    [ObservableAsProperty]
    public bool IsBusy { get; set; }
    
    public ReactiveCommand<Unit, Unit> GenerateMessages { get; }
    
    public ReactiveCommand<Unit, Unit> GenerateMessagesCancelable { get; }
    
    public ReactiveCommand<Unit, Unit> CancelMessageGeneration { get; }
    
    public ReactiveCommand<Unit, Unit> BeginOrCancelMessageGeneration { get; }
    
    
    public InformationSourceViewModel(
        InformationSourceComponent informationSourceComponent,
        SelectedMessageService selectedMessageService)
    {
        _informationSourceComponent = informationSourceComponent;
        _selectedMessageService = selectedMessageService;
        
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
        
        BeginOrCancelMessageGeneration = ReactiveCommand
            .Create<Unit>(_ =>
            {
                if (IsBusy)
                {
                    CancelMessageGeneration.Execute().Subscribe();
                }
                else
                {
                    GenerateMessagesCancelable.Execute().Subscribe();
                }
            });

        GenerateMessages
            .IsExecuting
            .ToPropertyEx(this, x => x.IsBusy);
        
        _informationSourceComponent
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _messages)
            .Subscribe();

        this
            .WhenAnyValue(x => x._selectedMessageService.SelectedMessage)
            .DistinctUntilChanged()
            .BindTo(this, x => x.SelectedMessage);
        
        this
            .WhenAnyValue(x => x.SelectedMessage)
            .DistinctUntilChanged()
            .Subscribe(x => _selectedMessageService.SelectedMessage = x);
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