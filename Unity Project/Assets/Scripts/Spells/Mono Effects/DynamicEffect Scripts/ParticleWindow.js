

class ParticleWindow extends EditorWindow 
{
    var mScale = 1.0;
    
    @MenuItem ("Custom/particle scale")
    
    static function Init()
    {
        var window : ParticleWindow = EditorWindow.GetWindow( ParticleWindow );
    }
    
    function OnGUI()
    {
        var obj = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel )[0];
		var go = obj as GameObject;
		var name : String;
		
		if ( go )
			name = go.name;
		else
			name = "select go";
			
        GUILayout.Label ("Object name : " + name, EditorStyles.boldLabel);

        mScale = EditorGUILayout.Slider ("scale : ", mScale, 0.01f, 5.0f);        
        
		if ( GUI.Button( new Rect( 50, 50, 100, 40 ), "set value" ) )
		{
			var ok : boolean = false;
			
			for ( var child : Transform in go.transform )
			{
				if ( child.gameObject.particleEmitter )
				{
					child.gameObject.particleEmitter.minSize *= mScale;
					child.gameObject.particleEmitter.maxSize *= mScale;
					child.gameObject.particleEmitter.worldVelocity *= mScale;
					child.gameObject.particleEmitter.localVelocity *= mScale;
					child.gameObject.particleEmitter.rndVelocity *= mScale;
					child.gameObject.particleEmitter.angularVelocity *= mScale;
					child.gameObject.particleEmitter.rndAngularVelocity *= mScale;
					
					
					ok = true;				
				}
			}
			
			if ( ok )
				go.transform.localScale *= mScale;
				
			if ( ok )
				Debug.Log( "ok!" );
			else
				Debug.Log( "something is wrong!" );
			

		
		}
    }
    
}


