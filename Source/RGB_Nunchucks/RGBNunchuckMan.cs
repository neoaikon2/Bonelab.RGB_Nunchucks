#if !UNITY_EDITOR
using MelonLoader;
#endif
using System;
using UnityEngine;

namespace RGB_Nunchucks
{
	[RegisterTypeInIl2Cpp]
	public class RGBNunchuckMan : MonoBehaviour
	{
		public GameObject chakuPrimary = null;
		public LineRenderer lineRenderer = null;
		public GameObject chakuSecondary = null;
		public ConfigurableJoint chakuJoint = null;

		private Color rgbCycle = Color.red;

#if !UNITY_EDITOR
		public RGBNunchuckMan(IntPtr intPtr) : base(intPtr) { }
#endif

		private void Start()
		{
			// Get object references
			chakuPrimary = transform.Find("Chaku Primary").gameObject;
			lineRenderer = chakuPrimary.GetComponent<LineRenderer>();
			chakuSecondary = transform.Find("Chaku Secondary").gameObject;
			createLink();
		}

		private void drawLine()
		{
			if (!lineRenderer.enabled)
				return;
			lineRenderer.SetPosition(0, chakuPrimary.transform.position + (chakuPrimary.transform.forward * -.156f));
			lineRenderer.SetPosition(1, chakuSecondary.transform.position + (chakuSecondary.transform.forward * -.156f));
			lineRenderer.material.SetColor("_EmissionColor", rgbCycle);
		}

		private void createLink()
		{
			if (chakuJoint != null)
				return;

			// Create the spring joint
			chakuJoint = chakuPrimary.AddComponent<ConfigurableJoint>();

			// Attach it to the secondary chacku
			chakuJoint.connectedBody = chakuSecondary.GetComponent<Rigidbody>();
			chakuJoint.autoConfigureConnectedAnchor = false;
			chakuJoint.anchor = new Vector3(0, 0, -.156f);
			chakuJoint.connectedAnchor = new Vector3(0, 0, -.156f);
			chakuJoint.breakForce = float.PositiveInfinity;
			chakuJoint.breakTorque = float.PositiveInfinity;
			chakuJoint.massScale = 1;
			chakuJoint.connectedMassScale = 1;
			chakuJoint.enablePreprocessing = true;

			// Configurable Joint only
			chakuJoint.xMotion = ConfigurableJointMotion.Limited;
			chakuJoint.yMotion = ConfigurableJointMotion.Limited;
			chakuJoint.zMotion = ConfigurableJointMotion.Limited;
			SoftJointLimitSpring limitSpring = new SoftJointLimitSpring();
			limitSpring.spring = 800;
			limitSpring.damper = 5;
			SoftJointLimit linearLimit = new SoftJointLimit();
			linearLimit.limit = .006f;
			chakuJoint.linearLimitSpring = limitSpring;
			chakuJoint.linearLimit = linearLimit;

			// Spring joint only
			//chakuJoint.spring = 800;
			//chakuJoint.damper = 5;
			//chakuJoint.maxDistance = .006f;
			//chakuJoint.tolerance = .025f;


#if DEBUG
			RGBNunchuckMan.Msg("Relinking...");
#endif
		}

		public void Stow()
		{
			// Disable the secondary chaku on stow so it doesn't flop around
			chakuSecondary.SetActive(false);
			lineRenderer.enabled = false;

#if DEBUG
			RGBNunchuckMan.Msg("Stowing nunchakus...");
#endif
		}

		public void Unstow()
		{
			// Reparent the primary chaku to the RGB Nunchaku object            
			chakuPrimary.transform.SetParent(chakuSecondary.transform.parent);
			chakuSecondary.transform.localPosition = chakuPrimary.transform.localPosition;

			// Activate the chaku and line renderer
			chakuSecondary.SetActive(true);
			lineRenderer.enabled = true;

			// Relink them
			createLink();

#if DEBUG
			RGBNunchuckMan.Msg("Unstowing nunchakus...");
#endif
		}

		float t = 0;

		private void Heartbeat()
		{
			t += Time.deltaTime;
			if (t >= 3)
			{
				t = 0;
				RGBNunchuckMan.Msg("Heartbeat...");
			}
		}

		private void Update()
		{
#if DEBUG
			Heartbeat();
#endif

			// RGB cycle
			Color.RGBToHSV(rgbCycle, out float H, out float S, out float V);
			H = H + .001f <= 1.0f ? H + .001f : 0.0f;
			rgbCycle = Color.HSVToRGB(H, S, V);

			// Update the lights and colors
			chakuPrimary.transform.Find("Models").Find("Tang").GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", rgbCycle);
			chakuPrimary.transform.Find("COM").GetComponent<Light>().color = rgbCycle;
			chakuSecondary.transform.Find("Models").Find("Tang").GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", rgbCycle);
			chakuSecondary.transform.Find("COM").GetComponent<Light>().color = rgbCycle;

			// Draw the line between the nunchakus
			drawLine();
		}

		private static void Msg(string msg)
		{
#if UNITY_EDITOR
			Debug.Log(msg);
#else
			MelonLogger.Msg(msg);
#endif
		}
	}
}
