namespace Task5.Models;

public record Message(
    int Id, 
    int NumberOfDataBits, 
    byte[] Word)
{ }