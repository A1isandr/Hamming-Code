using System.Collections.Immutable;

namespace Task5.Models;

public record SenderStats(
    int Id,
    int NumberOfParityBits,
    byte[] HammingCode,
    byte[] HammingCodeForTwoBitError,
    byte[]? HammingCodeWithErrors,
    int[]? ErrorPositions)
{
    public bool HasErrors => ErrorPositions is not null;
}