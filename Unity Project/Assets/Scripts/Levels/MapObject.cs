using UnityEngine;
using System.Collections;

abstract public class MapObject {

    abstract public GridBox get(int x, int y);

    abstract public MultiMap<GridBox> getFlat();
}
