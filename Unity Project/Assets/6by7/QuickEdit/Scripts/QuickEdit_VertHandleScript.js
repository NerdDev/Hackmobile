#pragma strict
@script ExecuteInEditMode

var isActive : boolean = false;
var isSelected : boolean = false;
var myVertIndices = new int[0];

function Activate()
{
	isActive = true;
}

function AddVertIndex(vertIndex : int)
{
	var tempIndices = new Array();
	tempIndices = myVertIndices;
	tempIndices.Add(vertIndex);
	myVertIndices = tempIndices;
}

function UpdateAttachedVerts(theMesh : Mesh) 
{	
	var meshVerts = theMesh.vertices;
	for(theVertIndex in myVertIndices)
	{
		meshVerts[theVertIndex] = transform.parent.InverseTransformPoint(transform.position);
	}
	theMesh.vertices = meshVerts;
}

function OnDrawGizmos()
{
	if(isActive)
	{
		if(isSelected)
		{
			Gizmos.DrawIcon(transform.position, "6by7/VertOn.tga", false);			
		}
		else
		{
			Gizmos.DrawIcon(transform.position, "6by7/VertOff.tga", false);
		}
	}
}