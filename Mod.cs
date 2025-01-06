using System;
using System.Collections.Generic;
using Modding;
using Modding.Blocks;
using UnityEngine;
using UnityEngine.UI;

namespace TBCStusSpace
{
	public class Mod : ModEntryPoint
	{
		public static GameObject TBCController;

		public static ModAssetBundle modAssetBundle;

		public static void Log(string msg)
		{
			Debug.Log("TBC Log: " + msg);
		}
		public static void Warning(string msg)
		{
			Debug.LogWarning("TBC Warning: " + msg);
		}
		public static void Error(string msg)
		{
			Debug.LogError("TBC Error: " + msg);
		}

		public override void OnLoad()
		{

			//各ModuleとBehaviourをセットにし、XML上で使えるように
			Modding.Modules.CustomModules.AddBlockModule<TBCAddProjectile, TBCAddProjectileBehaviour>("TBCAddProjectile", true);
			Modding.Modules.CustomModules.AddBlockModule<TBCAddAPModule, TBCAddAPBehaviour>("TBCAddAPModule", true);
			Modding.Modules.CustomModules.AddBlockModule<TBCAddHEModule, TBCAddHEBehaviour>("TBCAddHEModule", true);

			TBCController = new GameObject("TBCController");
			UnityEngine.Object.DontDestroyOnLoad(TBCController);

			SingleInstance<AdArmorModule>.Instance.transform.parent = TBCController.transform;
			// Called when the mod is loaded.
			switch (Application.platform)   //OS毎に変更
			{
				case RuntimePlatform.WindowsPlayer:
					modAssetBundle = ModResource.GetAssetBundle("myasset");
					break;
				case RuntimePlatform.OSXPlayer:
					modAssetBundle = ModResource.GetAssetBundle("myassetmac");
					break;
				case RuntimePlatform.LinuxPlayer:
					modAssetBundle = ModResource.GetAssetBundle("myassetmac");
					break;
				default:
					modAssetBundle = ModResource.GetAssetBundle("myasset");
					break;
			}
		}
	}
}
