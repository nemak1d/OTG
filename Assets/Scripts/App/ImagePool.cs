//-------------------------------------------
//
//  ImagePool
//
//-------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using N1D.Framework.Core;
using N1D.App.UI;

namespace N1D.App
{
    public class ImagePool : GameObjectPool<Image>
    {
        //-----------------------------------
        // MonoBehaviour
        //-----------------------------------
        
		//-----------------------------------
		// Method (private)
		//-----------------------------------
		public static ImagePool Create(int size, Image original)
		{
			var poolObject = new GameObject("ImagePool");
			var pool = poolObject.AddComponent<ImagePool>();
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