using UnityEngine;
using System;
using System.Collections;

using Random = UnityEngine.Random;

using Klax.Common;

namespace Klax.Core
{
	public class MaterialContainer : ScriptableObject
	{
		public int Count = Enum.GetValues(typeof(BlockColor)).Length;
		[SerializeField]
		Material defaultMaterial;
		Material[] coloredMaterials;

		public Material this[BlockColor color] { get { return Get((int)color); } }

		public Material this[int index] { get { return Get(index); } }

		Material Get(int index)
		{
			if (coloredMaterials == null)
				Fill();
			
			return this.coloredMaterials[index];
		}

		void Fill()
		{
			coloredMaterials = new Material[Count];
			for (int i = 0; i < Count; i++)
			{
				var material = new Material(defaultMaterial);
				var color = (BlockColor)i;
				material.color = color.ToUnityColor();
				coloredMaterials[i] = material;
			}
		}
	}
}
