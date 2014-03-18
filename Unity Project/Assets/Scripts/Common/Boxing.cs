using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Boxing : Bounding
{
    public GridLocation Front;

    public Boxing(Bounding rhs, GridLocation front)
        : base(rhs)
    {
        this.Front = front;
    }

    public Boxing(Boxing rhs)
        : this(rhs, rhs.Front)
    {
    }
}

