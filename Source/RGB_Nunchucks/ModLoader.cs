using MelonLoader;
using UnityEngine;
using UnhollowerRuntimeLib;

namespace RGB_Nunchucks
{
	public class ModLoader : MelonMod
	{
		public override void OnInitializeMelon()
		{
#if DEBUG
			ClassInjector.RegisterTypeInIl2Cpp<RGBNunchuckMan>(true);
#else
			ClassInjector.RegisterTypeInIl2Cpp<RGBNunchuckMan>();
#endif
			base.OnInitializeMelon();
		}
	}
}
