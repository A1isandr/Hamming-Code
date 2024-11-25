using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Task5.Models;

namespace Task5.Services;

public class SelectedMessageService : ReactiveObject
{
    [Reactive]
    public int? SelectedMessage { get; set; }
}