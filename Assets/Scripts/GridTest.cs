using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Klax.Common;

namespace Klax.Test
{
	public class GridTest : MonoBehaviour
	{
		Action blockdDstroyCallback = () => {};
		Action overflowCallback = () => { Debug.Log("TEST: GAME OVER!"); };

		class Win
		{
			public LineType Line;
			public int BlockCount;
			public Win(LineType line, int blockCount)
			{
				Line = line;
				BlockCount = blockCount;
			}
		}

		List<Win> Wins = new List<Win>(){
			new Win(LineType.MainDiagonal, 3),
			new Win(LineType.Horizontal, 3),
			new Win(LineType.Vertical,3),
			new Win(LineType.Horizontal,4)
		};

		const string testFailed = "TEST: Grid test failed! {0}:{1},{2}:{3}";

		void WinLineCallback(LineType line, int blockCount)
		{
			var win = Wins[0];
			if (line == win.Line && blockCount == win.BlockCount)
				Wins.RemoveAt(0);
			else
			{
				var message = string.Format(testFailed, win.Line, line, win.BlockCount, blockCount);
				Debug.LogError(message);
			}
		}

		// Use this for initialization
		IEnumerator Start () 
		{
			Debug.Log("TEST: Grid test start");
			var grid = new Core.Grid(overflowCallback,WinLineCallback);
		
			grid.DropTo(1, BlockColor.Red, blockdDstroyCallback);
			grid.DropTo(1, BlockColor.Red, blockdDstroyCallback);
			grid.DropTo(1, BlockColor.Yellow, blockdDstroyCallback);
			grid.DropTo(0, BlockColor.Red, blockdDstroyCallback);
			grid.DropTo(0, BlockColor.Blue, blockdDstroyCallback);
			grid.DropTo(0, BlockColor.Red, blockdDstroyCallback);
			grid.DropTo(0, BlockColor.Blue, blockdDstroyCallback);
			grid.DropTo(0, BlockColor.Blue, blockdDstroyCallback);
			grid.DropTo(4, BlockColor.Yellow, blockdDstroyCallback);
			grid.DropTo(3, BlockColor.Yellow, blockdDstroyCallback);
			grid.DropTo(2, BlockColor.Red, blockdDstroyCallback);
			grid.DropTo(2, BlockColor.Yellow, blockdDstroyCallback);
			Debug.Log("TEST: Grid test end");
			yield break;
		}
	}
}
