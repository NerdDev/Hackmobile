// == TERRAVOL ==
// Copyright(c) Olivier Fuxet, 2013. Do not redistribute.
// terravol.unity@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TerraVol
{
	/// <summary>
	/// Store data in a dictionary representing a virtual 3D grid.</summary>
	internal class Grid3
	{
		
		private Dictionary<Vector3i, ChunkData> grid;
		
		private int minX;
		public int MinX {
			get { return minX; }
		}
	
		private int minY;
		public int MinY {
			get { return minY; }
			set { minY = value; }
		}
		
		private int minZ;
		public int MinZ {
			get { return minZ; }
		}
		
		private int maxX;
		public int MaxX {
			get { return maxX; }
		}
		
		private int maxY;
		public int MaxY {
			get { return maxY; }
			set { maxY = value; }
		}
		
		private int maxZ;
		public int MaxZ {
			get { return maxZ; }
		}
		
		
		public Grid3 (int minY)
		{
			this.minX = 0;
			this.minY = minY;
			this.minZ = 0;
			
			this.maxX = 0;
			this.maxY = 0;
			this.maxZ = 0;
			
			grid = new Dictionary<Vector3i, ChunkData>();
		}
		
		public void Set (ChunkData obj, Vector3i pos)
		{
			lock (grid)
			{
				grid[pos] = obj;
				updateMinMax(pos.x, pos.y, pos.z);
			}
		}

		public void Set (ChunkData obj, int x, int y, int z)
		{
			lock (grid)
			{
				grid[new Vector3i(x, y, z)] = obj;
				updateMinMax(x, y, z);
			}
		}
		
		public ChunkData GetCreate (TerraMap map, Vector3i pos)
		{
			lock (grid)
			{
				ChunkData obj;
				if (!grid.TryGetValue(pos, out obj)) {
					obj = new ChunkData(map, pos);
					grid.Add(pos, obj);
					updateMinMax(pos.x, pos.y, pos.z);
				}
				return obj;
			}
		}
		
		public ChunkData SafeGet (Vector3i pos)
		{
			lock (grid)
			{
				ChunkData obj;
				if (grid.TryGetValue(pos, out obj))
					return obj;
			}
			return default(ChunkData);
		}

		public ChunkData SafeGet (int x, int y, int z)
		{
			lock (grid)
			{
				ChunkData obj;
				if (grid.TryGetValue(new Vector3i(x, y, z), out obj))
					return obj;
			}
			return default(ChunkData);
		}
		
		public void AddOrReplace (ChunkData obj, Vector3i pos)
		{
			lock (grid)
			{
				grid[pos] = obj;
				updateMinMax(pos.x, pos.y, pos.z);
			}
		}

		public void AddOrReplace (ChunkData obj, int x, int y, int z)
		{
			lock (grid)
			{
				grid[new Vector3i(x, y, z)] = obj;
				updateMinMax(x, y, z);
			}
		}
		
		public Vector3i GetMin ()
		{
			return new Vector3i (minX, minY, minZ);
		}
		
		public Vector3i GetMax ()
		{
			return new Vector3i (maxX, maxY, maxZ);
		}
		
		private void updateMinMax(int x, int y, int z)
		{
			if (x < minX)
				minX = x;
			else if (x > maxX)
				maxX = x;
				
			/*if (y < minY)
				minY = y;
			else */if (y > maxY)
				maxY = y;
				
			if (z < minZ)
				minZ = z;
			else if (z > maxZ)
				maxZ = z;
		}
		
	}
	
}