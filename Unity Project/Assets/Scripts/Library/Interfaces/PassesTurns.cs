public interface PassesTurns
{
    void UpdateTurn();
    ulong CurrentPoints { get; set; }
    ulong BasePoints { get; set; }
    bool IsActive { get; set; }
}
