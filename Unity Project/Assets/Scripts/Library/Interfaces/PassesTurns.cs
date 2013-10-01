public interface PassesTurns
{
    void UpdateTurn();
    int CurrentPoints { get; set; }
    int BasePoints { get; set; }
    bool IsActive { get; set; }
}
