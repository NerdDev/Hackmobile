using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using System.Diagnostics;

[AddComponentMenu ("Pathfinding/Seeker")]
/** Handles path calls for a single unit.
 * \ingroup relevant
 * This is a component which is meant to be attached to a single unit (AI, Robot, Player, whatever) to handle it's pathfinding calls.
 * It also handles post-processing of paths using modifiers.
 * \see \ref calling-pathfinding
 */
public class Seeker : MonoBehaviour {

	//====== SETTINGS ======
	
	/* Recalculate last queried path when a graph changes. \see AstarPath::OnGraphsUpdated */
	//public bool recalcOnGraphChange = true;
	
	public bool drawGizmos = true;
	public bool detailedGizmos = false;
	
	/** Saves nearest nodes for previous path to enable faster Get Nearest Node calls. */
	public bool saveGetNearestHints = true;
	
	public StartEndModifier startEndModifier = new StartEndModifier ();
	
	[HideInInspector]
	public TagMask traversableTags = new TagMask (-1,-1);
	
	[HideInInspector]
	/** Penalties for each tag.
	 * Tag 0 which is the default tag, will have added a penalty of tagPenalties[0].
	 * These should only be positive values since the A* algorithm cannot handle negative penalties.
	 * \note This array should always have a length of 32.
	 * \see Pathfinding::Path::tagPenalties
	 */
	public int[] tagPenalties = new int[32];
	
	//====== SETTINGS ======
	
	//public delegate Path PathReturn (Path p);
	
	/** Callback for when a path is completed. Movement scripts should register to this delegate.\n
	 * A temporary callback can also be set when calling StartPath, but that delegate will only be called for that path */
	public OnPathDelegate pathCallback;
	
	/** Called before pathfinding is started */
	public OnPathDelegate preProcessPath;
	
	/** For anything which requires the original nodes (Node[]) (before modifiers) to work */
	public OnPathDelegate postProcessOriginalPath;
	
	/** Anything which only modifies the positions (Vector3[]) */
	public OnPathDelegate postProcessPath;
	
	//public GetNextTargetDelegate getNextTarget;
	
	//DEBUG
	//public Path lastCompletedPath;
	[System.NonSerialized]
	public Vector3[] lastCompletedVectorPath;
	[System.NonSerialized]
	public Node[] lastCompletedNodePath;
	
	//END DEBUG
	
	/** The current path */
	[System.NonSerialized]
    protected PFPath path;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
	
	/** Returns #path */
    public PFPath GetCurrentPath()
    {
		return path;
	}
	
	private Node startHint;
	private Node endHint;
	
	/** Temporary callback only called for the current path. This value is set by the StartPath functions */
	private OnPathDelegate tmpPathCallback;
	
	/** The path ID of the last path queried */
	protected int lastPathID = 0;
	
#if PhotonImplementation
	public Seeker () {
		Awake ();
	}
#endif
	
	public void Awake () {
		startEndModifier.Awake (this);
	}
	
	public void OnDestroy () {
		startEndModifier.OnDestroy (this);
	}
	
	private List<IPathModifier> modifiers = new List<IPathModifier> ();
	
	public void RegisterModifier (IPathModifier mod) {
		if (modifiers == null) {
			modifiers = new List<IPathModifier> (1);
		}
		
		modifiers.Add (mod);
	}
	
	public void DeregisterModifier (IPathModifier mod) {
		if (modifiers == null) {
			return;
		}
		modifiers.Remove (mod);
	}
	
	public enum ModifierPass {
		PreProcess,
		PostProcessOriginal,
		PostProcess
	}
	
	/** Runs modifiers on path \a p */
	public void RunModifiers (ModifierPass pass, PFPath p) {
		
		
		//Sort the modifiers based on priority (bubble sort (slow but since it's a small list, it works good))
		bool changed = true;
		while (changed) {
			changed = false;
			for (int i=0;i<modifiers.Count-1;i++) {
				if (modifiers[i].Priority < modifiers[i+1].Priority) {
					IPathModifier tmp = modifiers[i];
					modifiers[i] = modifiers[i+1];
					modifiers[i+1] = tmp;
					changed = true;
				}
			}
		}
		
		//Call eventual delegates
		switch (pass) {
			case ModifierPass.PreProcess:
				if (preProcessPath != null) preProcessPath (p);
				break;
			case ModifierPass.PostProcessOriginal:
				if (postProcessOriginalPath != null) postProcessOriginalPath (p);
				break;
			case ModifierPass.PostProcess:
				if (postProcessPath != null) postProcessPath (p);
				break;
		}
		
		//No modifiers, then exit here
		if (modifiers.Count	== 0) return;
		
		ModifierData prevOutput = ModifierData.All;
		IPathModifier prevMod = modifiers[0];
		
		//Loop through all modifiers and apply post processing
		for (int i=0;i<modifiers.Count;i++) {
			//Cast to MonoModifier, i.e modifiers attached as scripts to the game object
			MonoModifier mMod = modifiers[i] as MonoModifier;
			
			//Ignore modifiers which are not enabled
			if (mMod != null && !mMod.enabled) continue;
			
			switch (pass) {
				case ModifierPass.PreProcess:
					modifiers[i].PreProcess (p);
					break;
				case ModifierPass.PostProcessOriginal:
					modifiers[i].ApplyOriginal (p);
					break;
				case ModifierPass.PostProcess:
					//UnityEngine.Debug.Log ("Applying Post");
					//Convert the path if necessary to match the required input for the modifier
					ModifierData newInput = ModifierConverter.Convert (p,prevOutput,modifiers[i].input);
					if (newInput != ModifierData.None) {
						modifiers[i].Apply (p,newInput);
						prevOutput = modifiers[i].output;
					} else {
						
						UnityEngine.Debug.Log ("Error converting "+(i > 0 ? prevMod.GetType ().Name : "original")+"'s output to "+(modifiers[i].GetType ().Name)+"'s input.\nTry rearranging the modifier priorities on the Seeker.");
					
						prevOutput = ModifierData.None;
					}
				
					prevMod = modifiers[i];
					
					break;
			}
			
			if (prevOutput == ModifierData.None) {
				break;
			}
		}
	}
	
	/** Is the current path done calculating.
	 * Returns if the current #path returns true on IsDone or there is no path (path is null)
	 * This method is not reliable when switching scenes and/or stopping and resuming pathfinding in any way
	 * \see Pathfinding::Path::IsDone
	 * \version Added in 3.0.8*/
	public bool IsDone () {
		return path == null || path.IsDone ();
	}
	
	/** Called when a path has completed.
	 * This should have been implemented as optional parameter values, but that didn't seem to work very well with delegates (the values weren't the default ones)
	 * \see OnPathComplete(Path,bool,bool) */
	public void OnPathComplete (PFPath p) {
		OnPathComplete (p,true,true);
	}
	
	/** Called when a path has completed.
	 * Will post process it and return it by calling #tmpPathCallback and #pathCallback */
	public void OnPathComplete (PFPath p, bool runModifiers, bool sendCallbacks) {
		
		if (this == null || p == null || p != path) {
			return;
		}
		
		startHint = p.startNode;
		endHint = p.endNode;
		
		if (!path.error && runModifiers) {
			//This will send the path for post processing to modifiers attached to this Seeker
			RunModifiers (ModifierPass.PostProcessOriginal, path);
			
			//This will send the path for post processing to modifiers attached to this Seeker
			RunModifiers (ModifierPass.PostProcess, path);
		}
		
		if (sendCallbacks) {
			
			//This will send the path to the callback (if any) specified when calling StartPath
			if (tmpPathCallback != null) {
				tmpPathCallback (p);
			}
			
			//This will send the path to any script which has registered to the callback
			if (pathCallback != null) {
				pathCallback (p);
			}
			
			lastCompletedNodePath = p.path;
			lastCompletedVectorPath = p.vectorPath;
		}
	}
	
	/** Called for each path in a MultiTargetPath. Only post processes the path, does not return it.
	  * \astarpro */
	public void OnPartialPathComplete (PFPath p) {
		OnPathComplete (p,true,false);
	}
	
	/** Called once for a MultiTargetPath. Only returns the path, does not post process.
	  * \astarpro */
	public void OnMultiPathComplete (PFPath p) {
		OnPathComplete (p,false,true);
	}
	
	/*public void OnEnable () {
		//AstarPath.OnGraphsUpdated += CheckPathValidity;
	}
	
	public void OnDisable () {
		//AstarPath.OnGraphsUpdated -= CheckPathValidity;
	}*/
	
	/*public void CheckPathValidity (AstarPath active) {
		
		/*if (!recalcOnGraphChange) {
			return;
		}
		
		
		
		//Debug.Log ("Checking Path Validity");
		//Debug.Break ();
		if (lastCompletedPath != null && !lastCompletedPath.error) {
			//Debug.Log ("Checking Path Validity");
			StartPath (transform.position,lastCompletedPath.endPoint);
			
			/*if (!lastCompletedPath.path[0].IsWalkable (lastCompletedPath)) {
				StartPath (transform.position,lastCompletedPath.endPoint);
				return;
			}
				
			for (int i=0;i<lastCompletedPath.path.Length-1;i++) {
				
				if (!lastCompletedPath.path[i].ContainsConnection (lastCompletedPath.path[i+1],lastCompletedPath)) {
					StartPath (transform.position,lastCompletedPath.endPoint);
					return;
				}
				Debug.DrawLine (lastCompletedPath.path[i].position,lastCompletedPath.path[i+1].position,Color.cyan);
			}*
		}*
	}*/
	
	//The frame the last call was made from this Seeker
	//private int lastPathCall = -1000;
	
	/** Returns a new path instance. The path will be taken from the path pool if path recycling is turned on.\n
	 * This path can be sent to #StartPath(Path,OnPathDelegate) with no change, but if no change is required #StartPath(Vector3,Vector3,OnPathDelegate) does just that.
	 * \code Seeker seeker = GetComponent (typeof(Seeker)) as Seeker;
	 * Path p = seeker.GetNewPath (transform.position, transform.position+transform.forward*100);
	 * p.nnConstraint = NNConstraint.Default; \endcode */
	public PFPath GetNewPath (Vector3 start, Vector3 end) {
		//Get a path from the Path Pool - If path recycling is off or if there are no paths in the pool, a new path will be created
		PFPath p = AstarPath.GetFromPathPool ();
		
		//Reset the path with new start and end points
		p.Reset (start,end,null);
		return p;
	}
	
	/** Call this function to start calculating a path.
	 * \param start		The start point of the path
	 * \param end		The end point of the path
	 */
	public PFPath StartPath (Vector3 start, Vector3 end) {
		return StartPath (start,end,null,-1);
	}
	
	/** Call this function to start calculating a path.
	 * \param start		The start point of the path
	 * \param end		The end point of the path
	 * \param callback	The function to call when the path has been calculated
	 * 
	 * \a callback will be called when the path has completed.
	 * \a Callback will not be called if the path is canceled (e.g when a new path is requested before the previous one has completed) */
	public PFPath StartPath (Vector3 start, Vector3 end, OnPathDelegate callback) {
		return StartPath (start,end,callback,-1);
	}
	
	/** Call this function to start calculating a path.
	 * \param start		The start point of the path
	 * \param end		The end point of the path
	 * \param callback	The function to call when the path has been calculated
	 * \param graphMask	Mask used to specify which graphs should be searched for close nodes. See Pathfinding::NNConstraint::graphMask. \astarproParam
	 * 
	 * \a callback will be called when the path has completed.
	 * \a Callback will not be called if the path is canceled (e.g when a new path is requested before the previous one has completed) */
	public PFPath StartPath (Vector3 start, Vector3 end, OnPathDelegate callback, int graphMask) {
		PFPath p = GetNewPath (start,end);
		return StartPath (p, callback, graphMask);
	}
	
	/** Call this function to start calculating a path.
	 * \param p			The path to start calculating
	 * \param callback	The function to call when the path has been calculated
	 * \param graphMask	Mask used to specify which graphs should be searched for close nodes. See Pathfinding::NNConstraint::graphMask. \astarproParam
	 * 
	 * \a callback will be called when the path has completed.
	 * \a Callback will not be called if the path is canceled (e.g when a new path is requested before the previous one has completed) */
	public PFPath StartPath (PFPath p, OnPathDelegate callback = null, int graphMask = -1) {
		p.enabledTags = traversableTags.tagsChange;
		p.tagPenalties = tagPenalties;
		
		//In case a multi target path has been specified, call special logic
		if (p.GetType () == typeof (MultiTargetPath)) {
			return StartMultiTargetPath (p as MultiTargetPath,callback);
		}
		//Cancel a previously requested path is it has not been processed yet and also make sure that it has not been recycled and used somewhere else
		if (path != null && !path.processed && lastPathID == path.pathID) {
			path.errorLog += "Canceled path because a new one was requested\nGameObject: "+gameObject.name;
			path.error = true;
			//No callback should be sent for the canceled path
		}
		
		path = p;
		path.callback += OnPathComplete;
		path.nnConstraint.graphMask = graphMask;
		
		tmpPathCallback = callback;
		
		//Set the Get Nearest Node hints if they have not already been set
		if (path.startHint == null)
			path.startHint = startHint;
			
		if (path.endHint == null) 
			path.endHint = endHint;
		
		//Save the path id so we can make sure that if we cancel a path (see above) it should not have been recycled yet.
		lastPathID = path.pathID;
		
		//Delay the path call by one frame if it was sent the same frame as the previous call
		/*if (lastPathCall == Time.frameCount) {
			StartCoroutine (DelayPathStart (path));
			return path;
		}*/
		
		//lastPathCall = Time.frameCount;
		
		//Pre process the path
		RunModifiers (ModifierPass.PreProcess, path);
		
		//Send the request to the pathfinder
		AstarPath.StartPath (path);
		
		return path;
	}
	
	/** Starts a Multi Target Path from one start point to multiple end points. A Multi Target Path will search for all the end points in one search and will return all paths if \a pathsForAll is true, or only the shortest one if \a pathsForAll is false.\n
	 * \param start			The start point of the path
	 * \param endPoints		The end points of the path
	 * \param pathsForAll	Indicates whether or not a path to all end points should be searched for or only to the closest one
	 * \param callback		The function to call when the path has been calculated
	 * \param graphMask		Mask used to specify which graphs should be searched for close nodes. See Pathfinding::NNConstraint::graphMask. \astarproParam
	 * 
	 * \a callback and #pathCallback will be called when the path has completed. \a Callback will not be called if the path is canceled (e.g when a new path is requested before the previous one has completed)
	 * \astarpro 
	 * \see Pathfinding::MultiTargetPath
	 * \see \ref MultiTargetPath.cs "Example of how to use multi-target-paths"
	 */
	public MultiTargetPath StartMultiTargetPath (Vector3 start, Vector3[] endPoints, bool pathsForAll, OnPathDelegate callback = null, int graphMask = -1) {
		MultiTargetPath p = new MultiTargetPath (start, endPoints, null, null);
		p.pathsForAll = pathsForAll;
		return StartMultiTargetPath (p, callback, graphMask);
	}
	
	/** Starts a Multi Target Path from multiple start points to a single target point. A Multi Target Path will search from all start points to the target point in one search and will return all paths if \a pathsForAll is true, or only the shortest one if \a pathsForAll is false.\n
	 * \param startPoints	The start points of the path
	 * \param end			The end point of the path
	 * \param pathsForAll	Indicates whether or not a path from all start points should be searched for or only to the closest one
	 * \param callback		The function to call when the path has been calculated
	 * \param graphMask		Mask used to specify which graphs should be searched for close nodes. See Pathfinding::NNConstraint::graphMask. \astarproParam
	 * 
	 * \a callback and #pathCallback will be called when the path has completed. \a Callback will not be called if the path is canceled (e.g when a new path is requested before the previous one has completed)
	 * \astarpro 
	 * \see Pathfinding::MultiTargetPath
	 * \see \ref MultiTargetPath.cs "Example of how to use multi-target-paths"
	 */
	public MultiTargetPath StartMultiTargetPath (Vector3[] startPoints, Vector3 end, bool pathsForAll, OnPathDelegate callback = null, int graphMask = -1) {
		MultiTargetPath p = new MultiTargetPath (startPoints, end, null, null);
		p.pathsForAll = pathsForAll;
		return StartMultiTargetPath (p, callback, graphMask);
	}
	
	/** Starts a Multi Target Path. Takes a MultiTargetPath and wires everything up for it to send callbacks to the seeker for post-processing.\n
	 * \param p				The path to start calculating
	 * \param callback		The function to call when the path has been calculated
	 * \param graphMask	Mask used to specify which graphs should be searched for close nodes. See Pathfinding::NNConstraint::graphMask. \astarproParam
	 * 
	 * \a callback and #pathCallback will be called when the path has completed. \a Callback will not be called if the path is canceled (e.g when a new path is requested before the previous one has completed)
	 * \astarpro
	 * \see Pathfinding::MultiTargetPath
	 * \see \ref MultiTargetPath.cs "Example of how to use multi-target-paths"
	 */
	public MultiTargetPath StartMultiTargetPath (MultiTargetPath p, OnPathDelegate callback = null, int graphMask = -1) {
			
		//Cancel a previously requested path is it has not been processed yet and also make sure that it has not been recycled and used somewhere else
		if (path != null && !path.processed && lastPathID == path.pathID) {
			path.errorLog += "Canceled path because a new one was requested";
			path.error = true;
			//No callback should be sent for the canceled path
		}
		
		OnPathDelegate[] callbacks = new OnPathDelegate[p.targetPoints.Length];
		for (int i=0;i<callbacks.Length;i++) {
			callbacks[i] = OnPartialPathComplete;
		}
		
		p.callbacks = callbacks;
		p.callback += OnMultiPathComplete;
		p.nnConstraint.graphMask = graphMask;
		
		path = p;
		
		tmpPathCallback = callback;
		
		//Save the path id so we can make sure that if we cancel a path (see above) it should not have been recycled yet.
		lastPathID = path.pathID;
		
		//Delay the path call by one frame if it was sent the same frame as the previous call
		/*if (lastPathCall == Time.frameCount) {
			StartCoroutine (DelayPathStart (path));
			return p;
		}
		
		lastPathCall = Time.frameCount;*/
		
		//Pre process the path
		RunModifiers (ModifierPass.PreProcess, path);
		
		//Send the request to the pathfinder
		AstarPath.StartPath (path);
		
		return p;
	}
	
	public IEnumerator DelayPathStart (PFPath p) {
		yield return 0;
		//lastPathCall = Time.frameCount;
		
		RunModifiers (ModifierPass.PreProcess, p);
		
		AstarPath.StartPath (p);
	}
	
#if !PhotonImplementation
	public void OnDrawGizmos () {
		if (lastCompletedNodePath == null || !drawGizmos) {
			return;
		}
		
		if (detailedGizmos) {
			Gizmos.color = new Color (0.7F,0.5F,0.1F,0.5F);
			
			if (lastCompletedNodePath!= null) {
				for (int i=0;i<lastCompletedNodePath.Length-1;i++) {
					Gizmos.DrawLine ((Vector3)lastCompletedNodePath[i].position,(Vector3)lastCompletedNodePath[i+1].position);
				}
			}
		}
		
		Gizmos.color = new Color (0,1F,0,1F);
		
		if (lastCompletedVectorPath != null) {
			for (int i=0;i<lastCompletedVectorPath.Length-1;i++) {
				Gizmos.DrawLine (lastCompletedVectorPath[i],lastCompletedVectorPath[i+1]);
			}
		}
	}
#endif
	
}

