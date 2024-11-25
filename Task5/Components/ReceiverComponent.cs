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
        var syndrome = CalculateSyndrome(message.Word);
        var isOverallParityMatches = CalculateOverallParity(message.Word);
        var numberOfErrors = CalculateNumberOfErrors(syndrome, isOverallParityMatches);
        byte[]? correctedHammingCode = null;
        
        if (numberOfErrors == 1)
        {
            correctedHammingCode = CorrectError(message.Word, syndrome);
        }
        
        return new ReceiverStats(
            Id: message.Id,
            HammingCode: message.Word,
            NumberOfErrors: numberOfErrors,
            Syndrome: syndrome == 0 ? null : syndrome,
            CorrectedHammingCode: correctedHammingCode);
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
    
    private static int CalculateSyndrome(byte[] hammingCode)
    {
        var numberOfParityBits = CalculateNumberOfParityBits(hammingCode.Length - 1);
        var syndrome = 0;

        for (int i = 0; i < numberOfParityBits; i++)
        {
            var parityPos = 1 << i;
            var paritySum = 0;

            for (int j = parityPos - 1; j < hammingCode.Length; j += 2 * parityPos)
            {
                for (int k = 0; k < parityPos && j + k < hammingCode.Length; k++)
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

    private static int CalculateNumberOfErrors(int syndrome, bool isOverallParityMatches)
    {
        if (syndrome == 0 && isOverallParityMatches)
        {
            return 0;
        }

        if (syndrome != 0 && isOverallParityMatches)
        {
            return 2;
        }

        return 1;
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