// == TERRAVOL ==
// Copyright(c) Olivier Fuxet, 2013. Do not redistribute.
// terravol.unity@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TerraVol
{
	/// <summary>
	/// Store data in a dictionary representing a virtual 2D grid.</summary>
	internal class Grid2
	{
		
		private Dictionary<Vector2i, Chunk2D> grid;
		
		private int minX;
		public int MinX {
			get { return minX; }
		}
		
		private int minZ;
		public int MinZ {
			get { return minZ; }
		}
		
		private int maxX;
		public int MaxX {
			get { return maxX; }
		}
		
		private int maxZ;
		public int MaxZ {
			get { return maxZ; }
		}
		
		
		public Grid2 ()
		{
			this.minX = 0;
			this.minZ = 0;
			
			this.maxX = 0;
			this.maxZ = 0;
			
			grid = new Dictionary<Vector2i, Chunk2D>();
		}
		
		public void Set (Chunk2D obj, Vector2i pos)
		{
			lock (grid)
			{
				grid[pos] = obj;
				updateMinMax(pos.x, pos.y);
			}
		}

		public void Set (Chunk2D obj, int x, int z)
		{
			lock (grid)
			{
				grid[new Vector2i(x, z)] = obj;
				updateMinMax(x, z);
			}
		}
		
		public Chunk2D GetCreate (Vector2i pos)
		{
			lock (grid)
			{
				Chunk2D obj;
				if (!grid.TryGetValue(pos, out obj)) {
					obj = new Chunk2D(pos);
					grid.Add(pos, obj);
					updateMinMax(pos.x, pos.y);
				}
				return obj;
			}
		}
		
		public Chunk2D SafeGet (Vector2i pos)
		{
			lock (grid)
			{
				Chunk2D obj;
				if (grid.TryGetValue(pos, out obj))
					return obj;
			}
			return default(Chunk2D);
		}

		public Chunk2D SafeGet (int x, int z)
		{
			lock (grid)
			{
				Chunk2D obj;
				if (grid.TryGetValue(new Vector2i(x, z), out obj))
					return obj;
			}
			return default(Chunk2D);
		}
		
		public void AddOrReplace (Chunk2D obj, Vector2i pos)
		{
			lock (grid)
			{
				grid[pos] = obj;
				updateMinMax(pos.x, pos.y);
			}
		}

		public void AddOrReplace (Chunk2D obj, int x, int z)
		{
			lock (grid)
			{
				grid[new Vector2i(x, z)] = obj;
				updateMinMax(x, z);
			}
		}
		
		public Vector2i GetMin ()
		{
			return new Vector2i (minX, minZ);
		}
		
		public Vector2i GetMax ()
		{
			return new Vector2i (maxX, maxZ);
		}
		
		private void updateMinMax(int x, int z)
		{
			if (x < minX)
				minX = x;
			else if (x > maxX)
				maxX = x;
				
			if (z < minZ)
				minZ = z;
			else if (z > maxZ)
				maxZ = z;
		}
		
	}
	
}