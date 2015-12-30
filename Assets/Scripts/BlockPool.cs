using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Klax.Common;
using Klax.World;

namespace Klax.Core
{
	public class BlockPool : MonoBehaviour
	{
		List<Block> pool = new List<Block>();
		[SerializeField]
		Block prefabBlock;
		[SerializeField]
		MaterialContainer materials;

		public void Return(Block block)
		{
			var go = block.GameObject; 
			go.SetActive(false);
			var rigidbody = block.Rigidbody;
			rigidbody.useGravity = false;
			var collider = block.Collider;
			collider.enabled = false;
			pool.Add(block);
		}

		public Block GetRandom()
		{
			var random = Random.Range(0, materials.Count);
			return Get((BlockColor)random);
		}

		public Block Get(BlockColor color)
		{
			Block block;
			if (pool.Count > 0)
			{
				block = pool[0];
				pool.RemoveAt(0);
				block.GameObject.SetActive(true);
			}
			else
				block = Instantiate<Block>(prefabBlock);

			block.Color = color; 
			block.Renderer.material = materials[color];

			return null;
		}
	}
}
