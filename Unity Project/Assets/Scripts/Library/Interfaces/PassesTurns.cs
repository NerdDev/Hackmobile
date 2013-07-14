public interface PassesTurns
{
    void UpdateTurn();
    //public int currentPoints;
    //public int basePoints;
    int CurrentPoints { get; set; }
    int BasePoints { get; set; }
    bool IsActive { get; set; }
}