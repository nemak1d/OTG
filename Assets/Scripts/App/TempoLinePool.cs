//-------------------------------------------
//
//  TempoLinePool
//
//-------------------------------------------
using UnityEngine;
using N1D.Framework.Core;
using N1D.App.UI;

namespace N1D.App
{
    public class TempoLinePool : GameObjectPool<TempoLine>
    {
		//-----------------------------------
		// MonoBehaviour
		//-----------------------------------

		//-----------------------------------
		// Method (public)
		//-----------------------------------
		public static TempoLinePool Create(int size, TempoLine original)
		{
			var poolObject = new GameObject("TempoLinePool");
			var pool = poolObject.AddComponent<TempoLinePool>();
			pool.Initialize(size, original);
			return pool;
		}

		//-----------------------------------
		// Property
		//-----------------------------------

		//-----------------------------------
		// Define
		//-----------------------------------

		//-----------------------------------
		// Field
		//-----------------------------------

		//-----------------------------------
		// Internal Class / Struct
		//-----------------------------------
	}
} // N1D.App