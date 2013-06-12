using UnityEngine;
using System.Collections;

public class GridSpace : MonoBehaviour
{

    [SerializeField] private string name_;
    public string gridName
    {
        get { return name_; }
    }
    [SerializeField] public GridType type_;
    public GridType type
    {
        get { return type_; }
    }

}
