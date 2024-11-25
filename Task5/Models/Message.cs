namespace Task5.Models;

public class Message(int id, byte[] word)
{
    public int Id { get; } = id;
    
    public int NumberOfDataDigits { get; } = word.Length;

    public byte[] Word { get; set; } = word;
}