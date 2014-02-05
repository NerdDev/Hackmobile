using UnityEditor;
using UnityEngine;
using System.Collections;


//Properties MUST have both getter and setter accessors AND [ExposeProperty] attribute set otherwise the property will not be shown.
	// http://www.unifycommunity.com/wiki/index.php?title=Expose_properties_in_inspector
//Example is in TimeManager.cs

[CustomEditor( typeof( TimeManager ) )]
public class TimeManagerEditor : Editor {


    TimeManager m_Instance;
    PropertyField[] m_fields;
    
    
    public void OnEnable()
    {
        m_Instance = target as TimeManager;
        m_fields = ExposeProperties.GetProperties( m_Instance );
    }
    
    public override void OnInspectorGUI () {

        if ( m_Instance == null )
            return;
        
        this.DrawDefaultInspector();
        
        ExposeProperties.Expose( m_fields );
        
    }
}