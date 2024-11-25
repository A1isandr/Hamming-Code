using System;
using System.Linq;
using DynamicData;
using Task5.Models;

namespace Task5.Components;

public class ReceiverComponent
{
    private readonly SenderComponent _senderComponent;
    
    private readonly IObservableCache<ReceiverStats, int> _stats;


    public ReceiverComponent(SenderComponent senderComponent)
    {
        _senderComponent = senderComponent;

        _stats = _senderComponent
            .Connect()
            .Transform(DecodeHammingCode)
            .AsObservableCache();
    }
    
    
    public IObservable<IChangeSet<ReceiverStats, int>> ConnectToStats() => _stats.Connect();
    
    private static ReceiverStats DecodeHammingCode(Message message)
    {
        var (numberOfErrors, errorSyndrome) = CalculateNumberOfErrors(message.Word);
        byte[]? correctedHammingCode = null;
        
        if (numberOfErrors == 1)
        {
            correctedHammingCode = CorrectError(message.Word, errorSyndrome);
        }
        
        return new ReceiverStats(
            Id: message.Id,
            HammingCode: message.Word,
            NumberOfErrors: numberOfErrors,
            ErrorSyndrome: errorSyndrome,
            CorrectedHammingCode: correctedHammingCode);
    }
    
    private static int CalculateNumberOfParityBits(int numberOfBits)
    {
        var numberOfParityBits = 0;
        
        while (1 << numberOfParityBits < numberOfBits)
        {
            numberOfParityBits++;
        }
        
        return numberOfParityBits;
    }
    
    private static (int, int) CalculateNumberOfErrors(byte[] hammingCode)
    {
        var errorSyndrome = CalculateErrorSyndrome(hammingCode);
        var isOverallParityMatches = CalculateOverallParity(hammingCode);
        var numberOfErrors = 1;
        
        // 0 - нет ошибок, 1 - однократная, 2 - двукратная
        if (errorSyndrome == 0 && isOverallParityMatches)
        {
            numberOfErrors = 0;
        }

        if (errorSyndrome != 0 && isOverallParityMatches)
        {
            numberOfErrors = 2;
        }

        if (errorSyndrome == 0 && !isOverallParityMatches)
        {
            errorSyndrome = hammingCode.Length;
        }

        return (numberOfErrors, errorSyndrome);
    }
    
    public static int CalculateErrorSyndrome(byte[] hammingCode)
    {
        var numberOfBitsWithoutOverallParity = hammingCode.Length - 1;
        var numberOfParityBits = CalculateNumberOfParityBits(numberOfBitsWithoutOverallParity);
        var syndrome = 0;
    
        for (int i = 0; i < numberOfParityBits; i++)
        {
            var parityPos = 1 << i;
            var paritySum = 0;
        
            for (int j = parityPos - 1; j < numberOfBitsWithoutOverallParity; j += 2 * parityPos)
            {
                for (int k = 0; k < parityPos && j + k < numberOfBitsWithoutOverallParity; k++)
                {
                    paritySum += hammingCode[j + k];
                }
            }
        
            if (paritySum % 2 != 0)
            {
                syndrome += parityPos;
            }
        }
        
        return syndrome;
    }
    
    private static bool CalculateOverallParity(byte[] hammingCode)
    {
        var overallParity = hammingCode
            .Take(hammingCode.Length - 1)
            .Aggregate(0, (current, bit) => current ^ bit);

        return overallParity == hammingCode.Last();
    }
    
    private static byte[] CorrectError(byte[] hammingCode, int syndrome)
    {
        var correctedHammingCode = (byte[])hammingCode.Clone();
        
        if (syndrome > 0 && syndrome <= hammingCode.Length)
        {
            correctedHammingCode[syndrome - 1] ^= 1;
        }
        
        return correctedHammingCode;
    }
}