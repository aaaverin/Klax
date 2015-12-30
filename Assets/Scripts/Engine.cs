using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Klax.World;
using Klax.Common;

namespace Klax.Core
{
	public class Engine : MonoBehaviour 
	{
		[SerializeField]
		BlockPool blocks;
		[SerializeField]
		Vector3[] respawns;

		Grid grid;

		void Start () 
		{
			grid = new Grid (() => Debug.Log ("Game Over"), (x,y) => Debug.Log (x + ":" + y));
		}
		
		void StartRandom()
		{
			var randomLine = Random.Range (0, respawns.Length);
			var block = blocks.GetRandom ();
			var blockTransform = block.Transform;
			blockTransform.position = respawns [randomLine];
			block.StartMoving();
		}

		void Drop(int line, Block block)
		{
			grid.DropTo (line, block.Color, () => blocks.Return (block));
		}
	}
}
