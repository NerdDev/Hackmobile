class QuickEdit_Base extends EditorWindow 
{	
	//external variables
	var vertHandlePrefab = (Resources.LoadAssetAtPath("Assets/6by7/Shared/Prefabs/VertHandlePrefab.prefab", typeof(Object)));
	
	//states
	var editModeActive : boolean = false;
	var editingShared : boolean = false;
	
	//internally shared variables
	var sourceObject : GameObject;
	var vertHandles : Transform[];
	var newMeshName : String = "";
	var vertIndex : int = 0;
	var vertPositions : Vector3[];
	var sourceMesh : Mesh;
	var editMesh : Mesh;
	var backupMeshVertData : Vector3[];
	var backupMeshUVData : Vector2[];
	
	
	//selection variables
	var sel_Dirty : boolean = false;
	var storedSelection : GameObject;
	var storedLength : int;
	var vertHandleSelection : QuickEdit_VertHandleScript[];
	var activeObjectPrevPos : Vector3;
	
	//Update Loop
	function Update()
	{
		if(editModeActive)
		{
			//custom "OnSelectionChange"
			if(Selection.activeGameObject != storedSelection || Selection.gameObjects.length != storedLength)
			{
				storedSelection = Selection.activeGameObject;
				storedLength = Selection.gameObjects.length;
				FilterSelection();				
			}		
			//move verts
			if(Selection.activeTransform && !sel_Dirty)
			{
				activeObjectCurrentPos = Selection.activeTransform.position;
				if(activeObjectCurrentPos != activeObjectPrevPos)
				{
					UpdateVertHandles();
					activeObjectPrevPos = activeObjectCurrentPos; 
				}			
			}
		}
	}
	//
	
	function EnterEditMode()
	{
		editModeActive = true;

		sourceObject = Selection.activeGameObject;
		sourceMesh = sourceObject.GetComponent(MeshFilter).sharedMesh;
		
		//Undo.RegisterUndo(sourceObject.GetComponent(MeshFilter), "Edit Mesh");
		
		//save the starting mesh data, so it can be cancelled
		backupMeshVertData = sourceMesh.vertices;
		backupMeshUVData = sourceMesh.uv;
		
		
		if(!editingShared) //copy the source mesh, save it, and make it the new used mesh
		{
			editMesh = new Mesh();
			
			editMesh.vertices = sourceMesh.vertices;
			editMesh.uv = sourceMesh.uv;			
			editMesh.uv2 = sourceMesh.uv2;			
			editMesh.triangles = sourceMesh.triangles;
			editMesh.normals = sourceMesh.normals;
			editMesh.tangents = sourceMesh.tangents;
			
			//get/set triangles for submeshes (yayy!)
			editMesh.subMeshCount = sourceMesh.subMeshCount;
			for(var t : int = 0;t<sourceMesh.subMeshCount;t++)
			{
				editMesh.SetTriangles(sourceMesh.GetTriangles(t), t);
			}
			//

			AssetDatabase.CreateAsset(editMesh, "Assets/6by7/QuickEdit/Meshes/"+newMeshName+".asset");
			AssetDatabase.SaveAssets();

			sourceObject.GetComponent(MeshFilter).sharedMesh = editMesh;
		}		
		else //edit the source mesh directly
		{
			editMesh = sourceMesh;
		}

		vertPositions = editMesh.vertices;
		vertHandles = new Transform[0];
		vertIndex = 0;
		Selection.objects = new Array();
		vertHandleSelection = new QuickEdit_VertHandleScript[0];
		AssignVertHandles();
	}
	
	function CancelMeshEdit()
	{
		if(!editingShared)
		{
			sourceObject.GetComponent(MeshFilter).sharedMesh = sourceMesh;
		}
		else
		{
			sourceMesh.vertices = backupMeshVertData;
			sourceMesh.uv = backupMeshUVData;
		}
		
		for(theVertHandle in vertHandles)
		{
			if(theVertHandle)
			{
				DestroyImmediate(theVertHandle.gameObject);
			}
		}
		Selection.objects = new Array();
		Selection.activeObject = sourceObject;
		editModeActive = false;	
	}
	
	function ExitEditMode()
	{
		for(theVertHandle in vertHandles)
		{
			if(theVertHandle)
			{
				DestroyImmediate(theVertHandle.gameObject);
			}
		}
		Selection.objects = new Array();
		Selection.activeObject = sourceObject;
		editModeActive = false;
	}
	
	//check the pos of each vert, and assign to vert handle or make new vert handle as needed
	function AssignVertHandles()
	{		
		for(theVertPos in vertPositions)
		{			
			if(!VertHandleHere(theVertPos, vertIndex))
			{
				//create a new vert handle at this vert's pos and set it up
				var newVertHandle = Instantiate(vertHandlePrefab, Vector3.zero, Quaternion.identity);
				newVertHandle.transform.parent = sourceObject.transform;
				newVertHandle.transform.localPosition = theVertPos;
				newVertHandle.GetComponent(QuickEdit_VertHandleScript).Activate();
				newVertHandle.GetComponent(QuickEdit_VertHandleScript).AddVertIndex(vertIndex);
				newVertHandle.name = "VertHandle_"+vertIndex;
				
				//add the new vert handle to the vert handle array
				var tempHandles = new Array();
				tempHandles = vertHandles;
				tempHandles.Add(newVertHandle.transform);
				vertHandles = tempHandles;
			}
			vertIndex++;
		}
	}
	
	function VertHandleHere(theVertPos : Vector3, vertIndex : int)
	{
		for(theVertHandle in vertHandles)
		{
			if(theVertHandle.transform.localPosition == theVertPos)
			{
				theVertHandle.gameObject.GetComponent(QuickEdit_VertHandleScript).AddVertIndex(vertIndex);
				return true;
			}
		}
	}
	
	function NameIsUnique(newMeshName : String)
	{
		if(AssetDatabase.LoadAssetAtPath("Assets/6by7/QuickEdit/Meshes/"+newMeshName+".asset", typeof(Object)))
		{
			return false;
		}
		else
			return true;
	}
	
	function FilterSelection()
	{
		sel_Dirty = true;
		if(editModeActive)
		{
			for(var i : int = 0;i<vertHandleSelection.length;i++)
			{
				vertHandleSelection[i].isSelected = false;
			}
			
			var tempHandleSelection = new Array();
			for(var v=0;v<Selection.gameObjects.length;v++)
			{
				if(Selection.gameObjects[v].GetComponent(QuickEdit_VertHandleScript))
				{
					tempHandleSelection.Add(Selection.gameObjects[v].GetComponent(QuickEdit_VertHandleScript));
				}
			}
			vertHandleSelection = tempHandleSelection;
			
			if(Selection.activeTransform)
			{
				if(vertHandleSelection.length > 0)
				{
					var tempSel = new Array();
					for(tvh in vertHandleSelection)
					{
						tempSel.Add(tvh.gameObject);
					}
					Selection.objects = tempSel;
					activeObjectPrevPos = Selection.activeTransform.position;
					for(theVertHandle in vertHandleSelection)
					{
						theVertHandle.isSelected = true;
					}
					activeObjectCurrentPos = Selection.activeTransform.position;
				}
			}
			else
				Selection.objects = new Array();
		}
		sel_Dirty = false;
	}
	
	function UpdateVertHandles()
	{
		for(vertHandle in vertHandleSelection)
		{
			vertHandle.UpdateAttachedVerts(editMesh);
		}
	}
}
