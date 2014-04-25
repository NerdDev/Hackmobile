using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

namespace Pathfinding {
//Mem - 4+1+4+1+[4]+[4]+1+1+4+4+4+4+4+  12+12+12+12+12+12+4+4+4+4+4+1+1+(4)+4+4+4+4+4+4+4 ? 166 bytes

	/** Basic path, finds the shortest path from A to B.
	 * \ingroup paths
	 * This is the most basic path object it will try to find the shortest path from A to B.\n
	 * All other path types inherit from this type.
	 * \see Seeker::StartPath
	 */
	public class PFPath {
		
		public NodeRunData runData;
		
		/** Callback to call when the path is complete.
		 * This is usually sent to the Seeker component which post processes the path and then calls a callback to the script which requested the path 
		*/
		public OnPathDelegate callback;
		
		/** Defines if start and end nodes will have their connection costs recalculated for this path.
		 * These connection costs will be more accurate and based on the exact start point and target point,
		 * however it should not be used when connection costs are not the default ones (all build in graph generators currently generate default connection costs).
		 * \see Int3.costMagnitude
		 * \version Added in 3.0.8.3 */
		public bool recalcStartEndCosts = true;
		
		/** If the path failed, this is true.
		  * \see #errorLog */
		public bool error = false;
		
		/** Additional info on what went wrong.
		  * \see #error */
		public string errorLog;
		
		/** Is the path completed? */
		public bool foundEnd = false;
		
		/** Holds the path as a Node array. All nodes the path traverses. This might not be the same as all nodes the smoothed path traverses. */
		public Node[] path;
		
		/** Holds the (eventually smoothed) path as a Vector3 array */
		public Vector3[] vectorPath;
		
		/** The max number of milliseconds per iteration (frame) */
		protected float maxFrameTime;
		
		public Node startNode; /**< Start node of the path */
		public Node endNode;   /**< End node of the path */
		
		/** The node currently being processed */
		protected NodeRun currentR;
		
		/** Hints can be set to enable faster Get Nearest Node queries. Only applies to some graph types */
		public Node startHint;
		/** Hints can be set to enable faster Get Nearest Node queries. Only applies to some graph types */
		public Node endHint;
		
		public Vector3 originalStartPoint;
		public Vector3 originalEndPoint;
		
		/** Exact start point of the path */
		public Vector3 startPoint;
		
		/** Exact end point of the path */
		public Vector3 endPoint;
		
		/** Determines if a search for an end node should be done.
		 * Set by different path types
		 * \version Added in 3.0.8.3 */
		protected bool hasEndPoint = true;
		
		public Int3 startIntPoint; /**< Start point in integer coordinates */
		public Int3 hTarget; /**< Target to use for H score calculations. \see Pathfinding::Node::h */
		
		/** Number of ms of computation time for this path */ 
		public float duration;			/**< The duration of this path in ms */
		public int searchIterations = 0;/**< The number of frames/iterations this path has executed */
		public int searchedNodes;		/**< Number of nodes this path has searched */
		
		/** When the call was made to start the pathfinding for this path */
		public System.DateTime callTime;
		
		/** True if the path has been calculated (even if it had an error).
		 * Used by the multithreaded pathfinder to signal that this path object is safe to return. */
		public bool processed = false;
		
		/** Constraint for how to search for nodes */
		public NNConstraint nnConstraint = PathNNConstraint.Default;
		
		/** The next path to be searched.
		 * Linked list implementation. You should never change this if you do not know what you are doing */
		public PFPath next;
		
		
		//These are variables which different scripts and custom graphs can use to get a bit more info about What is searching
		//Not all are used in the standard graph types
		//These variables are put here because it is a lot faster to access fields than, for example make use of a lookup table (e.g dictionary)
		//Note: These variables needs to be filled in by an external script to be usable
		public int radius;
		
		/** A mask for defining what type of ground a unit can traverse, not used in any default standard graph. \see #enabledTags */
		public int walkabilityMask = -1;
		
		/** Height of the character. Not used currently */
		public int height;
		
		/** Turning radius of the character. Not used currently */
		public int turnRadius;
		
		/** Speed of the character. Not used currently */
		public int speed;
		
		/* To store additional data. Note: this is SLOW. About 10-100 times slower than using the fields above.
		 * \version Removed in 3.0.8
		  * Currently not used */
		//public Dictionary<string,int> customData = null;//new Dictionary<string,int>();
		
		/** Determines which heuristic to use */
		public Heuristic heuristic;
		/** Scale of the heuristic values */
		public float heuristicScale = 1F;
		
		/** ID of this path. Used to distinguish between different paths */
		public ushort pathID;
		
		/** Saved original costs for the end node. \see ResetCosts */
		protected int[] endNodeCosts;
		
		/** Which graph tags are traversable.
		 * This is a bitmask so -1 = all bits set = all tags traversable.
		 * For example, to set bit 5 to true, you would do
		 * \code myPath.enabledTags |= 1 << 5; \endcode
		 * To set it to false, you would do
		 * \code myPath.enabledTags &= ~(1 << 5); \endcode
		 * 
		 * \see CanTraverse
		 */
		public int enabledTags = -1;
		
		/** Penalties for each tag. */
		protected int[] _tagPenalties = new int[0];
		
		/** Penalties for each tag.
		 * Tag 0 which is the default tag, will have added a penalty of tagPenalties[0].
		 * These should only be positive values since the A* algorithm cannot handle negative penalties.
		 * \note This array will never be null. If you try to set it to null or with a lenght which is not 32. It will be set to "new int[0]".
		 * \see Seeker::tagPenalties
		 */
		public int[] tagPenalties {
			get {
				return _tagPenalties;
			}
			set {
				if (value == null || value.Length != 32) _tagPenalties = new int[0];
				else _tagPenalties = value;
			}
		}
		
		/** Total Length of the path.
		 * Calculates the total length of the #vectorPath.
		 * Cache this rather than call this function every time since it will calculate the length every time, no just returned a cached value.
		 * \returns Total length of #vectorPath, if #vectorPath is null positive infinity is returned.
		 */
		public float GetTotalLength () {
			if (vectorPath == null) return float.PositiveInfinity;
			float tot = 0;
			for (int i=0;i<vectorPath.Length-1;i++) tot += Vector3.Distance (vectorPath[i],vectorPath[i+1]);
			return tot;
		}
		
		public uint GetTagPenalty (int tag) {
			return tag < _tagPenalties.Length ? (uint)_tagPenalties[tag] : 0;
		}
		
		/** Returns if the node can be traversed.
		  * This per default equals to if the node is walkable and if the node's tag is included in #enabledTags */
#if ConfigureTagsAsMultiple
		public bool CanTraverse (Node node) {
			return node.walkable && (node.tags & enabledTags) != 0;
		}
#else
		public bool CanTraverse (Node node) {
			return node.walkable && (enabledTags >> node.tags & 0x1) != 0;
		}
#endif
		
		/** Returns if this path is done calculating.
		 * \returns If #foundEnd or #error is true.
		 * \note The path might not have been returned yet.
		 * \version Added in 3.0.8
		 * \see Seeker::IsDone
		 */
		public bool IsDone () {
			return foundEnd || error;
		}
		
		/** Sets #error to true and appends \a msg to #errorLog and logs \a msg to the console.
		 * Debug.Log call is only made if AstarPath::logPathResults is not equal to None and not equal to InGame */
		public void LogError (string msg) {
			error = true;
			errorLog += msg;
			
			if (AstarPath.active.logPathResults != PathLog.None && AstarPath.active.logPathResults != PathLog.InGame) {
				Debug.LogWarning (msg);
			}
		}
		
		/** Default constructor */
		public PFPath () {}
		
		/** Create a new path with a start and end point.
		 * The delegate will be called when the path has been calculated.
		 * Do not confuse it with the Seeker callback as they are sent at different times.
		 * If you are using a Seeker to start the path you can set \a callbackDelegate to null */
		public PFPath (Vector3 start, Vector3 end, OnPathDelegate callbackDelegate) {
			Reset (start,end,callbackDelegate,false);
		}
		
		/** Sets the start and end points.
		 * Sets #originalStartPoint, #originalEndPoint, #startPoint, #endPoint, #startIntPoint and #hTarget (to \a end ) */
		public virtual void UpdateStartEnd (Vector3 start, Vector3 end) {
			originalStartPoint = start;
			originalEndPoint = end;
			
			startPoint = start;
			endPoint = end;
			
			startIntPoint = (Int3)start;
			hTarget = (Int3)end;
		}
		
		public virtual void Reset (Vector3 start, Vector3 end, OnPathDelegate callbackDelegate) {
			Reset (start,end,callbackDelegate,true);
		}
		
		public virtual void Reset (Vector3 start, Vector3 end, OnPathDelegate callbackDelegate, bool reset) {
			
			if (reset) {
				processed = false;
				vectorPath = null;
				path = null;
				next = null;
				foundEnd = false;
				error = false;
				errorLog = "";
				callback = null;
				currentR = null;
				duration = 0;
				searchIterations = 0;
				searchedNodes = 0;
				
				startHint = null;
				endHint = null;
			}
			
			callTime = System.DateTime.Now;
			
			callback = callbackDelegate;
			
			/*if (AstarPath.active == null || AstarPath.active.graphs == null) {
				errorLog += "No NavGraphs have been calculated yet - Don't run any pathfinding calls in Awake";
				if (AstarPath.active.logPathResults != PathLog.None) {
					Debug.LogWarning (errorLog);
				}
				error = true;
				return;
			}*/
			
			pathID = AstarPath.active.GetNextPathID ();
			
			UpdateStartEnd (start,end);
			
			heuristic = AstarPath.active.heuristic;
			heuristicScale = AstarPath.active.heuristicScale;
			
		}
		
		
		/** Prepares the path. Searches for start and end nodes and does some simple checking if a path is at all possible */
		public virtual void Prepare () {
			
			
			//@pathStartTime = startTime;
			
			//maxAngle = NmaxAngle;
			//angleCost = NangleCost;
			//stepByStep = NstepByStep;
			//unitRadius = 0;//BETA, Not used
			nnConstraint.tags = enabledTags;
			NNInfo startNNInfo 	= AstarPath.active.GetNearest (startPoint,nnConstraint, startHint);
			
			//Tell the NNConstraint which node was found as the start node if it is a PathNNConstraint and not a normal NNConstraint
			PathNNConstraint pathNNConstraint = nnConstraint as PathNNConstraint;
			if (pathNNConstraint != null) {
				pathNNConstraint.SetStart (startNNInfo.node);
			}
			
			startPoint = startNNInfo.clampedPosition;
			startIntPoint = (Int3)startPoint;
			startNode = startNNInfo.node;
			
			if (hasEndPoint) {
				NNInfo endNNInfo	= AstarPath.active.GetNearest (endPoint,nnConstraint, endHint);
				endPoint = endNNInfo.clampedPosition;
				hTarget = (Int3)endPoint;
				endNode = endNNInfo.node;
			}
			
			
			#if DEBUGGING
			Debug.DrawLine (startNode.position,startPoint,Color.blue);
			Debug.DrawLine (endNode.position,endPoint,Color.blue);
			#endif
			
			if (startNode == null || (hasEndPoint == true && endNode == null)) {
				LogError ("Couldn't find close nodes to either the start or the end (start = "+(startNode != null)+" end = "+(endNode != null)+")");
				return;
			}
			
			if (!startNode.walkable) {
				Debug.DrawRay (startPoint,Vector3.up,Color.red);
				Debug.DrawLine (startPoint,(Vector3)startNode.position,Color.red);
				Debug.Break();
				LogError ("The node closest to the start point is not walkable");
				return;
			}
			
			if (hasEndPoint && !endNode.walkable) {
				LogError ("The node closest to the end point is not walkable");
				return;
			}
			
			if (hasEndPoint && startNode.area != endNode.area) {
				LogError ("There is no valid path to the target (start area: "+startNode.area+", target area: "+endNode.area+")");
				return;
			}
		}
		
		/** Initializes the path. Sets up the open list and adds the first node to it */
		public virtual void Initialize () {
			
			runData.pathID = pathID;
			
			//Resets the binary heap, don't clear everything because that takes an awful lot of time, instead we can just change the numberOfItems in it (which is just an int)
			//Binary heaps are just like a standard array but are always sorted so the node with the lowest F value can be retrieved faster
			runData.open.Clear ();
			
			if (hasEndPoint && startNode == endNode) {
				NodeRun endNodeR = endNode.GetNodeRun(runData);
				endNodeR.parent = null;
				endNodeR.h = 0;
				endNodeR.g = 0;
				Trace (endNodeR);
				foundEnd = true;
				return;
			}
			
			//Adjust the costs for the end node
			/*if (hasEndPoint && recalcStartEndCosts) {
				endNodeCosts = endNode.InitialOpen (open,hTarget,(Int3)endPoint,this,false);
				callback += ResetCosts; /* \todo Might interfere with other paths since other paths might be calculated before #callback is called *
			}*/
			
			//Node.activePath = this;
			NodeRun startRNode = startNode.GetNodeRun (runData);
			startRNode.pathID = pathID;
			startRNode.parent = null;
			startRNode.cost = 0;
			startRNode.g = startNode.penalty;
			startNode.UpdateH (hTarget,heuristic,heuristicScale, startRNode);
			
			/*if (recalcStartEndCosts) {
				startNode.InitialOpen (open,hTarget,startIntPoint,this,true);
			} else {*/
				startNode.Open (runData,startRNode,hTarget,this);
			//}
			
			searchedNodes++;
			
			//any nodes left to search?
			if (runData.open.numberOfItems <= 1) {
				LogError ("No open points, the start node didn't open any nodes");
				return;
			}
			
			currentR = runData.open.Remove ();
		}
		
		/** Calculates the path until completed or until the time has passed \a targetTick.
		 * Usually a check is only done every 500 nodes if the time has passed \a targetTick.
		 * Time/Ticks are got from System.DateTime.Now.Ticks.
		 * 
		 * Basic outline of what the function does for the standard path (Pathfinding.Path).
\code
while the end has not been found and no error has ocurred
	check if we have reached the end
		if so, exit and return the path
	
	open the current node, i.e loop through its neighbours, mark them as visited and put them on a heap
	
	check if there are still nodes left to process (or have we searched the whole graph)
		if there are none, flag error and exit
		
	pop the next node of the heap and set it as current
	
	check if the function has exceeded the time limit
		if so, return and wait for the function to get called again
\endcode
		 */
		public virtual void CalculateStep (long targetTick) {
			
			int counter = 0;
			
			//Continue to search while there hasn't ocurred an error and the end hasn't been found
			while (!foundEnd && !error) {
				
				//@Performance Just for debug info
				searchedNodes++;
				
				//Close the current node, if the current node is the target node then the path is finnished
				if (currentR.node == endNode) {
					foundEnd = true;
					break;
				}
				
				//Loop through all walkable neighbours of the node and add them to the open list.
				currentR.node.Open (runData,currentR, hTarget,this);
				
				//any nodes left to search?
				if (runData.open.numberOfItems <= 1) {
					LogError ("No open points, whole area searched");
					
					return;
				}
				
				//Select the node with the lowest F score and remove it from the open list
				currentR = runData.open.Remove ();
				
				//Check for time every 500 nodes, roughly every 0.5 ms usually
				if (counter > 500) {
					
					//Have we exceded the maxFrameTime, if so we should wait one frame before continuing the search since we don't want the game to lag
					if (System.DateTime.Now.Ticks >= targetTick) {
						
						//Return instead of yield'ing, a separate function handles the yield (CalculatePaths)
						return;
					}
					
					counter = 0;
				}
				
				counter++;
			
			}
			
			if (foundEnd && !error) {
				Trace (currentR);
			}
		}
		
		/** Resets End Node Costs. Costs are updated on the end node at the start of the search to better reflect the end point passed to the path, the previous ones are saved in #endNodeCosts and are reset in this function which is called after the path search is complete */
		public void ResetCosts (PFPath p) {
			if (!hasEndPoint) return;
			
			endNode.ResetCosts (endNodeCosts);
		}
		
		/** Traces the calculated path from the end node to the start.
		 * This will build an array (#path) of the nodes this path will pass through and also set the #vectorPath array to the #path arrays positions */
		public virtual void Trace (NodeRun from) {
			
			int count = 0;
			
			NodeRun c = from;
			while (c != null) {
				c = c.parent;
				count++;
				if (count > 1024) {
					Debug.LogWarning ("Inifinity loop? >1024 node path");
					break;
				}
			}
			
			path = new Node[count];
			c = from;
			
			for (int i = 0;i<count;i++) {
				path[count-1-i] = c.node;
				c = c.parent;
			}
			
			vectorPath = new Vector3[count];
			
			for (int i=0;i<count;i++) {
				vectorPath[i] = (Vector3)path[i].position;
			}
		}
		
		/* String builder used for all debug logging */
		//public static System.Text.StringBuilder debugStringBuilder = new System.Text.StringBuilder ();
		
		/** Returns a debug string for this path.
		 */
		public virtual string DebugString (PathLog logMode) {
			
			if (logMode == PathLog.None || (!error && logMode == PathLog.OnlyErrors)) {
				return "";
			}
			
			//debugStringBuilder.Length = 0;
			
			System.Text.StringBuilder text = new System.Text.StringBuilder ();
			
			text.Append (error ? "Path Failed : " : "Path Completed : ");
			text.Append ("Computation Time ");
			
			text.Append ((duration).ToString (logMode == PathLog.Heavy ? "0.000" : "0.00"));
			text.Append (" ms Searched Nodes ");
			text.Append (searchedNodes);
			
			if (!error) {
				text.Append (" Path Length ");
				text.Append (path == null ? "Null" : path.Length.ToString ());
			
				if (logMode == PathLog.Heavy) {
					text.Append ("\nSearch Iterations "+searchIterations);
					
					if (hasEndPoint && endNode != null) {
						NodeRun nodeR = endNode.GetNodeRun (runData);
						text.Append ("\nEnd Node\n	G: ");
						text.Append (nodeR.g);
						text.Append ("\n	H: ");
						text.Append (nodeR.h);
						text.Append ("\n	F: ");
						text.Append (nodeR.f);
						text.Append ("\n	Point: ");
						text.Append (((Vector3)endPoint).ToString ());
						text.Append ("\n	Graph: ");
						text.Append (endNode.graphIndex);
					}
				
					text.Append ("\nStart Node");
					text.Append ("\n	Point: ");
					text.Append (((Vector3)startPoint).ToString ());
					text.Append ("\n	Graph: ");
					text.Append (startNode.graphIndex);
					text.Append ("\nBinary Heap size at completion: ");
					text.Append (runData.open == null ? "Null" : (runData.open.numberOfItems-2).ToString ());// -2 because numberOfItems includes the next item to be added and item zero is not used
				}
				
				/*"\nEnd node\n	G = "+p.endNode.g+"\n	H = "+p.endNode.h+"\n	F = "+p.endNode.f+"\n	Point	"+p.endPoint
				+"\nStart Point = "+p.startPoint+"\n"+"Start Node graph: "+p.startNode.graphIndex+" End Node graph: "+p.endNode.graphIndex+
				"\nBinary Heap size at completion: "+(p.open == null ? "Null" : p.open.numberOfItems.ToString ())*/
			}
			
			if (error) {
				text.Append ("\nError: ");
				text.Append (errorLog);
			}
			
			text.Append ("\nPath Number ");
			text.Append (pathID);
			
			return text.ToString ();
		}
		
		/** Calls callback to return the calculated path. \see #callback */
		public virtual void ReturnPath () {
			if (callback != null) {
				callback (this);
			}
		}
		
		//Movement stuff
		
		/** Returns in which direction to move from a point on the path.
		 * A simple and quite slow (well, compared to more optimized algorithms) algorithm first finds the closest path segment (from #vectorPath) and then returns
		 * the direction to the next point from there. The direction is not normalized.
		 * \returns Direction to move from a \a point, returns Vector3::zero if #vectorPath is null or has a length of 0 */
		public Vector3 GetMovementVector (Vector3 point) {
			
			if (vectorPath == null || vectorPath.Length == 0) {
				return Vector3.zero;
			}
			
			if (vectorPath.Length == 1) {
				return vectorPath[0]-point;
			}
			
			float minDist = float.PositiveInfinity;//Mathf.Infinity;
			int minSegment = 0;
			
			for (int i=0;i<vectorPath.Length-1;i++) {
				
				Vector3 closest = Mathfx.NearestPointStrict (vectorPath[i],vectorPath[i+1],point);
				float dist = (closest-point).sqrMagnitude;
				if (dist < minDist) {
					minDist = dist;
					minSegment = i;
				}
			}
			
			return vectorPath[minSegment+1]-point;
		}
		
	}
}