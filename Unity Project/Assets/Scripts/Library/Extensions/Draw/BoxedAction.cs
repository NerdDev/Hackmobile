using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BoxedAction<T>
{
    public DrawAction<T> UnitAction;
    public DrawAction<T> FrontAction;
    public DrawAction<T> BackAction;
    public DrawAction<T> LeftAction;
    public DrawAction<T> RightAction;
    public DrawAction<T> FrontLeftAction;
    public DrawAction<T> FrontRightAction;
    public DrawAction<T> BackLeftAction;
    public DrawAction<T> BackRightAction;

    public BoxedAction()
    {
    }

    public BoxedAction(DrawAction<T> frontAction, DrawAction<T> restAction)
    {
        this.FrontAction = frontAction;
        this.FrontLeftAction = frontAction;
        this.FrontRightAction = frontAction;
        this.LeftAction = restAction;
        this.RightAction = restAction;
        this.UnitAction = restAction;
        this.BackAction = restAction;
        this.BackLeftAction = restAction;
        this.BackRightAction = restAction;
    }

    public BoxedAction(DrawAction<T> frontAction, DrawAction<T> middleAction, DrawAction<T> backAction)
    {
        this.FrontAction = frontAction;
        this.FrontLeftAction = frontAction;
        this.FrontRightAction = frontAction;
        this.LeftAction = middleAction;
        this.RightAction = middleAction;
        this.UnitAction = middleAction;
        this.BackAction = backAction;
        this.BackLeftAction = backAction;
        this.BackRightAction = backAction;
    }
}

