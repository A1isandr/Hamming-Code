using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DynamicData;
using Task5.Common;
using Task5.Models;

namespace Task5.Components;

public class SenderComponent
{
    private readonly InformationSourceComponent _informationSource;
    
    private readonly IObservableCache<Message, int> _messages;
    private readonly SourceCache<SenderStats, int> _stats = 
        new(record => record.Id);
    
    
    public SenderComponent(InformationSourceComponent informationSource)
    {
        _informationSource = informationSource;

        _messages = _informationSource
            .Connect()
            .Transform(ToHammingCode)
            .AsObservableCache();
    }
    
    
    public IObservable<IChangeSet<Message, int>> Connect() => _messages.Connect();
    
    public IObservable<IChangeSet<SenderStats, int>> ConnectToStats() => _stats.Connect();
    
    private Message ToHammingCode(Message message)
    {
        if (message.Id == 1)
        {
            _stats.Clear();
        }
        
        var numberOfParityBits = CalculateNumberOfParityBits(message.NumberOfDataDigits);
        var newWord = InsertParityBits(message.Word, numberOfParityBits);
        var hammingCode = CalculateCheckBits(newWord, numberOfParityBits);
        var hammingCodeForTwoBitError = AddParityBitForTwoBitError(hammingCode);
        byte[]? hammingCodeWithErrors = null;
        int[]? errorPositions = null;
        var random = new Random();

        if (random.Next() % 2 == 0)
        {
            (hammingCodeWithErrors, errorPositions) = IntroduceErrors(hammingCodeForTwoBitError, 2);
        }
        
        _stats.AddOrUpdate(new SenderStats(
            Id: message.Id,
            NumberOfParityBits: numberOfParityBits,
            HammingCode: hammingCodeForTwoBitError,
            HammingCodeWithErrors: hammingCodeWithErrors,
            ErrorPositions: errorPositions));
        
        message.Word = hammingCodeWithErrors ?? hammingCode;
        
        return message;
    }
    
    private static int CalculateNumberOfParityBits(int numberOfDataDigits)
    {
        var p = 0;
        
        while (1 << p < numberOfDataDigits + p + 1)
        {
            p++;
        }
        
        return p;
    }
    
    private static byte[] InsertParityBits(byte[] data, int numberOfParityBits)
    {
        var word = new List<byte>();
        var dataIdx = 0;

        for (int i = 1; i <= data.Length + numberOfParityBits; i++)
        {
            if (i.IsPowerOfTwo())
            {
                word.Add(0);
            }
            else
            {
                word.Add(data[dataIdx]);
                dataIdx++;
            }
        }

        return [..word];
    }
    
    private static byte[] CalculateCheckBits(byte[] word, int numberOfParityBits)
    {
        var hammingCode = (byte[])word.Clone();
        
        for (int i = 0; i < numberOfParityBits; i++)
        {
            var parityPos = 1 << i;
            var paritySum = 0;
            
            for (int j = parityPos - 1; j < word.Length; j += 2 * parityPos)
            {
                for (int k = 0; k < parityPos && j + k < word.Length; k++)
                {
                    paritySum += hammingCode[j + k];
                }
            }
            
            hammingCode[parityPos - 1] = (byte)(paritySum % 2);
        }
        
        return word;
    }
    
    private static byte[] AddParityBitForTwoBitError(byte[] hammingCode)
    {
        var hammingCodeForTwoBitError = new byte[hammingCode.Length + 1];
        hammingCode.CopyTo(hammingCodeForTwoBitError, 0);
        
        var parityBit = (byte)hammingCode
            .Aggregate(0, (current, bit) => current ^ bit);

        hammingCodeForTwoBitError[hammingCode.Length] = parityBit;;
        
        return hammingCodeForTwoBitError;
    }
    
    private static (byte[], int[]) IntroduceErrors(byte[] hammingCode, int maxErrors)
    {
        var random = new Random();
        var errorCount = random.Next(1, maxErrors + 1);
        var errorPositions = new int[errorCount];
        var hammingCodeWithErrors = (byte[])hammingCode.Clone();

        for (int i = 0; i < errorCount; i++)
        {
            var errorPosition = random.Next(0, hammingCode.Length);

            if (errorPositions.Contains(errorPosition + 1)) continue;
            
            hammingCodeWithErrors[errorPosition] ^= 1;
            errorPositions[i] = errorPosition + 1;
        }

        return (hammingCodeWithErrors, errorPositions.Order().ToArray());
    }
}