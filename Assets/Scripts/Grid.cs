using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Klax.Common;

namespace Klax.Core
{	
	public class Grid
	{
		const int WIDTH = 5;
		const int HEIGTH = 7;
		
		public enum NeighbourDirection
		{
			UpLeft,
			Up,
			UpRight,
			Right,
			DownRight,
			Down,
			DownLeft,
			Left
		}
		
		public class Node
		{
			public BlockColor Color;
			public Node[] Neighbours = new Node[Enum.GetValues(typeof(NeighbourDirection)).Length];
			public bool[] IgnoredLine = new bool[Enum.GetValues(typeof(LineType)).Length];
			public Point Point;
			public bool Winner;
			public Action WinCallback;
		}
		
		static int linesCount = Enum.GetValues(typeof(LineType)).Length;
		static int neighboursCount = Enum.GetValues(typeof(NeighbourDirection)).Length;
		
		delegate void ForEachNeighbourDelegate(Node neighbour, int dir);
		
		Node[,] nodes = new Node[WIDTH,HEIGTH];
		int[] heigths = new int[WIDTH];

		WinLineDelegate winLineCallback;
		Action overflowCallback;

		public Grid(Action overflowCallback, WinLineDelegate winLineCallback)
		{
			this.overflowCallback = overflowCallback;
			this.winLineCallback = winLineCallback;
		}

		public void DropTo(int line, BlockColor color, Action winCallback)
		{
			if (heigths [line] >= HEIGTH)
				Overflow ();
			else 
			{
				var node = new Node () 
				{ 
					Color = color,
					WinCallback = winCallback,
				};
				node.Point = new Point(line, heigths[line]);
				nodes[line, heigths[line]] = node;
				heigths[line]++;
				
				List<Node> changedNodes = new List<Node>();
				changedNodes.Add(node);

				while (changedNodes.Count > 0)
				{
					foreach (var changedNode in changedNodes)
						Link(changedNode);
					
					var winNodes = CalculateGrid(changedNodes);
					
					changedNodes.Clear();
					
					var droppedNodes = new List<Node>();
					
					foreach (var winNode in winNodes)
					{
						Unlink(winNode);
						
						var point = winNode.Point;
						
						nodes[point.x, point.y] = null;
						
						for (int i = point.y + 1; i < heigths[point.x]; i++)
						{
							var droppedNode = nodes[point.x, i];
							if (droppedNode != null)
							{
								if (!droppedNode.Winner)
								{
									droppedNodes.Add(droppedNode); //order is inportent
									Unlink(droppedNode);
									
									nodes[point.x, i] = null;
								}
							}
						}
						if ( heigths[point.x] > point.y )
							heigths[point.x] = point.y;

						winNode.WinCallback();
					}
					foreach (var droppedNode in droppedNodes)
					{
						var point = droppedNode.Point;
						droppedNode.Point = new Point(point.x, heigths[point.x]);
						nodes[point.x, heigths[point.x]] = droppedNode;
						heigths[point.x]++;
						changedNodes.Add(droppedNode);
					}
				}
			}
		}
		
		void Unlink(Node node)
		{
			ForEachNeighbour (node.Point, (neighbour, dir) => SetNeighbour (neighbour, null, dir));
		}
		
		void Link(Node node)
		{
			ForEachNeighbour(node.Point, (neighbour, dir) => {
				node.Neighbours[dir] = neighbour;
				SetNeighbour(neighbour, node, dir);
			});
		}
		
		void ForEachNeighbour(Point point, ForEachNeighbourDelegate action)
		{
			int dir = 0;
			foreach (var neighbour in GetNeigbours(point))
			{
				action(neighbour, dir);
				dir++;
			}
		}
		
		void SetNeighbour(Node neighbour, Node node, int dir)
		{
			if (neighbour != null)
			{
				var revertedDir = GetRevertedDirection(dir);
				neighbour.Neighbours[(int)revertedDir] = node;
			}
		}
		
		LineType GetLineOfDir(NeighbourDirection dir)
		{
			return (LineType)((int)dir % linesCount);
		}
		
		NeighbourDirection GetRevertedDirection(NeighbourDirection dir)
		{
			return GetRevertedDirection((int)dir);
		}
		
		NeighbourDirection GetRevertedDirection(int dir)
		{
			int result = (neighboursCount + dir - (neighboursCount / 2)) % neighboursCount;
			return (NeighbourDirection)result;
		}
		
		IEnumerable<Node> GetNeigbours(Point point)
		{
			for (int i = 0; i < neighboursCount; i++)
			{
				var neighbourPoint = GetNeighbourPoint(i, point.x, point.y);
				Node neighbour = null;
				if (neighbourPoint.x >= 0 && neighbourPoint.x < WIDTH)
					if (neighbourPoint.y >= 0 && neighbourPoint.y < HEIGTH)
						neighbour = nodes[neighbourPoint.x, neighbourPoint.y];
				yield return neighbour;
			}
		}
		
		List<Node> CalculateGrid(List<Node> changedNodes)
		{
			var winLine = new List<Node>();
			List<Node> winNodes = new List<Node>();
			foreach (var node in changedNodes)
			{
				for (int i = 0; i < linesCount; i++)
				{
					if (!node.IgnoredLine[i])
					{
						winLine.Clear();
						winLine.Add(node);
						for (int j = i; j < neighboursCount; j += linesCount)
						{
							var neighbour = node.Neighbours[j];
							while (neighbour != null && neighbour.Color == node.Color)
							{
								winLine.Add(neighbour);
								var n = neighbour;
								neighbour = neighbour.Neighbours[j];
								//TODO: Test, fix, remove
								if ( n == neighbour)
								{
									Debug.LogError(neighbour.Color + " : " + neighbour.Point + " _" + j);
									break;
								}
							}
						}
						if (winLine.Count > 2)
						{
							winLineCallback((LineType)i, winLine.Count);

							foreach (var winNode in winLine)
							{
								if (!winNode.Winner)
									winNodes.Add(winNode);
								winNode.Winner = true;
								winNode.IgnoredLine[i] = true;
							}
						}
					}
				}
			}
			return winNodes;
		}
		
		public static Point GetNeighbourPoint(int dir, int x, int y)
		{
			return GetNeighbourPoint ((NeighbourDirection)dir, x, y);
		}

		public static Point GetNeighbourPoint(NeighbourDirection dir, int x, int y)
		{
			//var __x = ((x + 7) % 8);
			//__x = Math.Sign (4 - __x) * Math.Sign ((__x % 4));
			//var __y = ...

			var _x = x;
			var _y = y;
			switch (dir)
			{
			case NeighbourDirection.UpLeft:
				_x -= 1;
				_y += 1;
				break;
			case NeighbourDirection.Up:
				_y += 1;
				break;
			case NeighbourDirection.UpRight:
				_x += 1;
				_y += 1;
				break;
			case NeighbourDirection.Right:
				_x += 1;
				break;
			case NeighbourDirection.DownRight:
				_x += 1;
				_y -= 1;
				break;
			case NeighbourDirection.Down:
				_y -= 1;
				break;
			case NeighbourDirection.DownLeft:
				_x -= 1;
				_y -= 1;
				break;
			case NeighbourDirection.Left:
				_x -= 1;
				break;
			}
			return new Point (_x, _y);
		}
		
		void Overflow()
		{
			overflowCallback ();
		}
	}
}