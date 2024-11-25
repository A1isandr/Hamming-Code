using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using Task5.Models;

namespace Task5.Components;

public class InformationSourceComponent
{
    private readonly Random _random = new();
    
    private readonly SourceCache<Message, int> _messages =
        new(message => message.Id);


    public IObservable<IChangeSet<Message, int>> Connect() => _messages.Connect();
    
    public async Task GenerateMessages(
        int numberOfMessages, 
        int numberOfDigits, 
        CancellationToken ct)
    {
        _messages.Clear();
        
        for (int i = 0; i < numberOfMessages; i++)
        {
            ct.ThrowIfCancellationRequested();
            
            await Task.Run(() => 
                _messages.AddOrUpdate(new Message(
                    id: i + 1,
                    word: _random
                        .GetItems(new byte[] { 0, 1 }, numberOfDigits)
                        .ToArray())), ct);
        }
    }
}