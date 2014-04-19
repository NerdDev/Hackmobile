using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIMovementCore
{
    List<AIMovement> movements = new List<AIMovement>();

    public void AddMovement(AIMovement movement)
    {
        movements.Add(movement);
    }
}
