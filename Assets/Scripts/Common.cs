using UnityEngine;

namespace Klax.Common
{
	public delegate void WinLineDelegate(LineType Line, int blockCount);

	public static class BlockColorExtention
	{
		public static Color ToUnityColor(this BlockColor color)
		{
			switch (color)
			{
			case BlockColor.Black:
				return Color.black;
			case BlockColor.Blue:
				return Color.blue;
			default:
				return Color.white;
			}
		}
	}

	public struct Point
	{
		public int x;
		public int y;
		
		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	public enum LineType
	{
		MainDiagonal,
		Vertical,
		SecondaryDiagonal,
		Horizontal
	}

	public enum BlockColor
	{
		Black,
		Blue,
		Green,
		Yellow,
		Red,
		Cyan,
		Magenta,
		White
	}
}
