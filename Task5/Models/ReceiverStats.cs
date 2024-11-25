﻿namespace Task5.Models;

public record ReceiverStats(
    int Id,
    byte[] HammingCode,
    int NumberOfErrors,
    int? Syndrome,
    byte[]? CorrectedHammingCode)
{
    public bool HasErrors => NumberOfErrors > 0;
    
    public bool HasExactlyOneError => NumberOfErrors == 1;
}