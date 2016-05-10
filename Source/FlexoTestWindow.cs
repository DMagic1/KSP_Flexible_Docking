using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlexoTubes
{
	public class FlexoTestWindow : MonoBehaviour
	{
		private bool visible;
		private FlexoTube root;
		private Rect WindowRect = new Rect(50, 50, 600, 800);
		private string caption = "Flexo Tube";
		private GUILayoutOption[] options = null;
		private int windowID;
		private float[] times = new float[5];
		private float[] lengths = new float[5];
		private float[] weights = new float[5];
		private string[] animations = new string[5];

		private void Awake()
		{
			windowID = UnityEngine.Random.Range(1000, 2000000) + _AssemblyName.GetHashCode();
			WindowRect = new Rect(50, 50, 600, 700);
			options = new GUILayoutOption[2] { GUILayout.Width(600), GUILayout.Height(700) };
		}

		public void setup(FlexoTube tube)
		{
			root = tube;
			weights[0] = 1;
			weights[1] = 1;
			weights[2] = 1;
			weights[3] = 1;
			weights[4] = 1;
			lengths[0] = 50;
			lengths[1] = 401;
			lengths[2] = 401;
			lengths[3] = 401;
			lengths[4] = 401;
			animations[0] = root.ExtendName;
			animations[1] = root.YTranslateName;
			animations[2] = root.XTranslateName;
			animations[3] = root.RotYName;
			animations[4] = root.RotZName;
		}

		private static String _AssemblyName
		{ get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name; } }

		public bool Visible
		{
			get { return visible; }
			set { visible = value; }
		}

		private void OnGUI()
		{
			if (!visible)
				return;

			DrawGUI();
		}

		private void DrawGUI()
		{
			WindowRect = WindowRect.ClampToScreen();

			WindowRect = GUILayout.Window(windowID, WindowRect, DrawWindow, caption, options);
		}

		private void DrawWindow(int id)
		{
			DrawPre(id);

			Draw(id);

			DrawPost(id);

			GUI.DragWindow();
		}

		private void DrawPre(int id)
		{

		}

		private void Draw(int id)
		{
			GUILayout.BeginHorizontal();

			GUILayout.Space(600);

			GUILayout.EndHorizontal();

			GUILayout.Space(700);

			Rect r = new Rect(140, 60, 400, 20);

			times[4] = GUI.HorizontalSlider(r, times[4], -200, 200);

			r = new Rect(WindowRect.width - 80, 100, 20, 400);

			times[3] = GUI.VerticalSlider(r, times[3], -200, 200);

			r = new Rect(20, 100, 20, 400);

			times[0] = GUI.VerticalSlider(r, times[0], 50, 1);

			r = new Rect(80, 100, 400, 400);

			GUI.Box(r, "");

			float tempX = times[1];
			float tempY = times[2];

			if (r.Contains(Event.current.mousePosition))
			{
				float xPos = Event.current.mousePosition.x - r.x;
				float yPos = Event.current.mousePosition.y - r.y;

				if (xPos >= 0 && yPos >= 0 && xPos < r.width && yPos < r.height)
				{
					tempX = ((xPos / r.width) * 400) - 200;
					tempY = 200 - ((yPos / r.height) * 400);

					if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
					{
						times[2] = tempX;
						times[1] = tempY;
					}
				}
			}

			r = new Rect(30, 530, 220, 20);

			GUI.Label(r, animations[0] + " Weight: " + weights[0].ToString("F2"));

			r.width = 20;
			r.x += 250;

			if (GUI.Button(r, "-"))
			{
				weights[0] = weights[0] - 0.05f;
				if (weights[0] < 0)
					weights[0] = 0;
			}

			r.x += 30;

			if (GUI.Button(r, "+"))
			{
				weights[0] = weights[0] + 0.05f;
				if (weights[0] > 1)
					weights[0] = 1;
			}

			r.x = 30;
			r.y += 30;
			r.width = 220;

			GUI.Label(r, animations[1] + " Weight: " + weights[1].ToString("F2"));

			r.width = 20;
			r.x += 250;

			if (GUI.Button(r, "-"))
			{
				weights[1] = weights[1] - 0.05f;
				if (weights[1] < 0)
					weights[1] = 0;
			}

			r.x += 30;

			if (GUI.Button(r, "+"))
			{
				weights[1] = weights[1] + 0.05f;
				if (weights[1] > 1)
					weights[1] = 1;
			}

			r.x += 30;
			r.width = 200;

			GUI.Label(r, string.Format("Target: {0} - Last {1}", root.targetYTransFrame, root.lastYTransFrame));

			r.x = 30;
			r.y += 30;
			r.width = 220;

			GUI.Label(r, animations[2] + " Weight: " + weights[2].ToString("F2"));

			r.width = 20;
			r.x += 250;

			if (GUI.Button(r, "-"))
			{
				weights[2] = weights[2] - 0.05f;
				if (weights[2] < 0)
					weights[2] = 0;
			}

			r.x += 30;

			if (GUI.Button(r, "+"))
			{
				weights[2] = weights[2] + 0.05f;
				if (weights[2] > 1)
					weights[2] = 1;
			}

			r.x += 30;
			r.width = 200;

			GUI.Label(r, string.Format("Target: {0} - Last {1}", root.targetXTransFrame, root.lastXTransFrame));

			r.x = 30;
			r.y += 30;
			r.width = 220;

			GUI.Label(r, animations[3] + " Weight: " + weights[3].ToString("F2"));

			r.width = 20;
			r.x += 250;

			if (GUI.Button(r, "-"))
			{
				weights[3] = weights[3] - 0.05f;
				if (weights[3] < 0)
					weights[3] = 0;
			}

			r.x += 30;

			if (GUI.Button(r, "+"))
			{
				weights[3] = weights[3] + 0.05f;
				if (weights[3] > 1)
					weights[3] = 1;
			}

			r.x += 30;
			r.width = 200;

			GUI.Label(r, string.Format("Target: {0} - Last {1}", root.targetYRotFrame, root.lastYRotFrame));

			r.x = 30;
			r.y += 30;
			r.width = 220;

			GUI.Label(r, animations[4] + " Weight: " + weights[4].ToString("F2"));

			r.width = 20;
			r.x += 250;

			if (GUI.Button(r, "-"))
			{
				weights[4] = weights[4] - 0.05f;
				if (weights[4] < 0)
					weights[4] = 0;
			}

			r.x += 30;

			if (GUI.Button(r, "+"))
			{
				weights[4] = weights[4] + 0.05f;
				if (weights[4] > 1)
					weights[4] = 1;
			}

			r.x += 30;
			r.width = 200;

			GUI.Label(r, string.Format("Target: {0} - Last {1}", root.targetXRotFrame, root.lastXRotFrame));

			r = new Rect(40, 680, 300, 100);

			string label = string.Format("Y Roll: {0:F0} | X Roll: {1:F0}\nY Trans Frame: {2:F0} | X Trans Frame: {3:F0}", times[3], times[4], tempX, tempY);

			GUI.Label(r, label);

			r.height = 40;
			r.x += 310;
			r.width = 80;

			if (GUI.Button(r, "Set"))
			{
				animationSet();
			}

			r.x += 100;
			r.width = 120;
			r.height = 25;
			r.y -= 50;

			root.setRest = GUI.Toggle(r, root.setRest, "Set Rest");

			r.y += 30;
			root.showLineSet = GUI.Toggle(r, root.showLineSet, "Line Set 1");

			r.y += 25;
			root.rotTrans = GUI.Toggle(r, root.rotTrans, "Align Trans");
		}

		private void DrawPost(int id)
		{

		}

		private void animationSet()
		{
			if (root.Anim == null)
				return;

			for (int i = 0; i < 5; i++)
			{
				Debug.Log("Setting Animation State " + i + "...");

				float shiftedTime = times[i] + (i == 0 ? 0 : 200);

				float time = shiftedTime / lengths[i];

				root.Anim[animations[i]].speed = 0;
				root.Anim[animations[i]].normalizedTime = time;
				if (i == 0)
					root.Anim[animations[i]].wrapMode = WrapMode.Once;
				else
					root.Anim[animations[i]].wrapMode = WrapMode.ClampForever;
				root.Anim.Blend(animations[i], weights[i]);

				Debug.Log(string.Format("Animation Clip [{0}] Set To Time {1:F3} At Frame {2} And Weight {3:F3}", animations[i], time, times[i], weights[i]));
			}
		}

	}
}
