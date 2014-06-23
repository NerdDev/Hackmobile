public interface PassesTurns
{
    void UpdateTurn();
    bool IsActive { get; set; }
    int Rate { get; set; }
}
