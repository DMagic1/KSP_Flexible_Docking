using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlexoTubes
{
    public class FlexoTube : ModuleDockingNode
    {
		[KSPField]
		public float maxTranslate;
		[KSPField]
		public float maxRotate;
		[KSPField(isPersistant = true)]
		public int targetYTransFrame;
		[KSPField(isPersistant = true)]
		public int targetXTransFrame;
		[KSPField(isPersistant = true)]
		public int targetYRotFrame;
		[KSPField(isPersistant = true)]
		public int targetXRotFrame;
		[KSPField(isPersistant = true)]
		public bool IsDeployed;
		[KSPField(guiActive = true)]
		public string Status = "Retracted (Basic Mode)";
		[KSPField(guiActive = true)]
		public string Magnets = "Enabled";
		[KSPField(isPersistant = true)]
		public bool MagnetsEnabled = true;

		[KSPField]
		public float activeForce = 0.2f;
		[KSPField]
		public float activeTorque = 0.2f;
		[KSPField]
		public float activeRange = 1f;
		[KSPField]
		public float activeReEngage = 1.5f;

		[KSPField(guiActive = true)]
		public string BaseState = "Ready";
		[KSPField(guiActive = true)]
		public string OtherNodeID = "";
		[KSPField(guiActive = true)]
		public string Enabled = "...";
		//[KSPField(guiActive = true)]
		//public string Angle = "0";
		//[KSPField(guiActive = true)]
		//public string Distance = "0";
		//[KSPField(guiActive = true)]
		//public string DistanceFixed = "0";
		[KSPField(guiActive = true)]
		public string Pos = "0";
		[KSPField(guiActive = true)]
		public string OtherPos = "0";
		//[KSPField(guiActive = true)]
		//public string PosOtherPos = "0";
		//[KSPField(guiActive = true)]
		//public string Projected = "0";
		//[KSPField(guiActive = true)]
		//public string AdjProjected = "0";
		//[KSPField(guiActive = true)]
		//public string Front = "0";
		//[KSPField(guiActive = true)]
		//public string FlatLine = "0";
		[KSPField(guiActive = true)]
		public string YTime = "0";
		[KSPField(guiActive = true)]
		public string YFrame = "0";
		[KSPField(guiActive = true)]
		public string XTime = "0";
		[KSPField(guiActive = true)]
		public string XFrame = "0";
		[KSPField(guiActive = true)]
		public string RotDir = "";
		[KSPField(guiActive = true)]
		public string AngleX = "";
		[KSPField(guiActive = true)]
		public string AngleY = "";
		[KSPField(guiActive = true)]
		public string NewAngleX = "";
		[KSPField(guiActive = true)]
		public string NewAngleY = "";
		[KSPField(guiActive = true)]
		public string TransRot = "";
		[KSPField(guiActive = true)]
		public string TransRotStart = "";
		[KSPField(guiActive = true)]
		public string FrontX = "";
		[KSPField(guiActive = true)]
		public string FrontY = "";
		[KSPField(guiActive = true)]
		public string DirX = "";
		[KSPField(guiActive = true)]
		public string DirY = "";

		private const int frames = 401;
		private const int startFrame = 200;
		private const int tranYClip = 1;
		private const int tranXClip = 2;
		private const int rotYClip = 3;
		private const int rotXClip = 4;

		private float frameDist;
		private float frameRot;

		private const string extendName = "Extend";
		private const string yTranslateName = "TranslateYAxis";
		private const string xTranslateName = "TranslateXAxis";
		private const string rotYName = "RotateYAxis";
		private const string rotXName = "RotateXAxis";

		public int lastXTransFrame;
		public int lastYTransFrame;
		public int lastYRotFrame;
		public int lastXRotFrame;

		private Animation anim;
		private Transform referenceTranslateTransform;
		private Transform referenceRotationTransform;
		private float cachedAcquireForce;
		private float cachedAcquireTorque;
		private float cachedAcquireRange;
		private float cachedReEngage;
		private bool moving;
		private bool resetting;

		private ModuleDockingNode cachedOtherNode;
		private float cachedOtherNodeForce = 2;
		private float cachedOtherNodeTorque = 2;
		private float cachedOtherNodeRange = 0.5f;

		public float theta = 0;
		public float dist = 0;

		public bool setRest;
		public bool showLineSet = true;
		public bool rotTrans = true;
		private Vector3 refDir;

		private FlexoTestWindow flexoWindow;

		public override void OnStart(PartModule.StartState state)
		{
			base.OnStart(state);

			anim = part.FindModelAnimators()[0];
			referenceTranslateTransform = part.FindModelTransform("ReferenceTransform");
			referenceRotationTransform = part.FindModelTransform("ReferenceRotation");
			TransRotStart = referenceTranslateTransform.rotation.eulerAngles.ToString("F3");
			refDir = referenceTranslateTransform.up;

			frameDist = (maxTranslate * 2) / (float)((frames - 1) / 2);
			frameRot = (maxRotate * 2) / (float)((frames - 1) / 2);

			cachedAcquireForce = acquireForce;
			cachedAcquireTorque = acquireTorque;
			cachedAcquireRange = acquireRange;
			cachedReEngage = minDistanceToReEngage;

			lastYTransFrame = startFrame;
			lastXTransFrame = startFrame;
			lastYRotFrame = startFrame;
			lastXRotFrame = startFrame;

			Events["ToggleMagnets"].guiName = "Toggle Magnets";

			if (!MagnetsEnabled)
			{
				acquireForce = 0;
				acquireTorque = 0;
				Magnets = "Disabled";
			}
			else
				Magnets = "Enabled";

			if (IsDeployed)
			{
				extensionAnimator(1, 1);
				setRest = true;

				if (MagnetsEnabled)
				{
					acquireForce = activeForce;
					acquireTorque = activeTorque;
				}

				acquireRange = activeRange;
				minDistanceToReEngage = activeReEngage;
				switch (this.state)
				{
					case "PreAttached":
					case "Disabled":
					case "Ready":
					case "Disengage":
					case "Acquire":
					case "Acquire (dockee)":
						lastYTransFrame = startFrame;
						lastXTransFrame = startFrame;
						lastYRotFrame = startFrame;
						lastXRotFrame = startFrame;
						break;
					case "Docked (same vessel)":
					case "Docked (docker)":
					case "Docked (dockee)":
						lastYTransFrame = targetYTransFrame;
						lastXTransFrame = targetXTransFrame;
						lastYRotFrame = targetYRotFrame;
						lastXRotFrame = targetXRotFrame;
						setDockedPosition();
						break;
				}
			}
			else
			{
				if (this.state == "PreAttached")
				{
					Events["Deploy"].active = false;
					Events["Retract"].active = false;
				}
			}

			Events["OpenWindow"].active = true;
			Events["CloseWindow"].active = false;
		}

		public override string GetInfo()
		{
			string s = base.GetInfo();

			s += string.Format("\nMax Translation: {0:F2}m", maxTranslate);

			s += string.Format("\nMax Rotation: {0:F2}°", maxRotate);

			return s;
		}

		public override void OnUpdate()
		{
			BaseState = this.state;
			Status = setStatus();

			Enabled = (!base.IsDisabled).ToString();

			if (otherNode == null)
			{
				OtherNodeID = "Not Set";
				if (cachedOtherNode != null)
				{
					cachedOtherNode.acquireForce = cachedOtherNodeForce;
					cachedOtherNode.acquireTorque = cachedOtherNodeTorque;
					cachedOtherNode.acquireRange = cachedOtherNodeRange;
					cachedOtherNode = null;
				}
			}
			else
			{
				if (cachedOtherNode != otherNode)
				{
					cachedOtherNode = otherNode;
					cachedOtherNodeForce = otherNode.acquireForce;
					cachedOtherNodeTorque = otherNode.acquireTorque;
					cachedOtherNodeRange = otherNode.acquireRange;

					if (MagnetsEnabled)
					{
						otherNode.acquireForce = activeForce;
						otherNode.acquireTorque = activeTorque;
					}
					else
					{
						otherNode.acquireForce = 0;
						otherNode.acquireTorque = 0;
					}
					otherNode.acquireRange = activeRange;
				}

				OtherNodeID = otherNode.part.flightID.ToString();
			}

			if (showLineSet)
			{
				//drawLine(nodeTransform.position, nodeTransform.position + nodeTransform.forward.normalized, XKCDColors.LightGreen);
				//drawLine(nodeTransform.position, nodeTransform.position + nodeTransform.up.normalized, XKCDColors.DarkGreen);
				drawLine(referenceRotationTransform.position, referenceRotationTransform.position + referenceRotationTransform.forward.normalized, XKCDColors.DarkRed);
			}
			//else
			//{
			//	drawLine(referenceTransform.position, referenceTransform.position + referenceTransform.forward.normalized, XKCDColors.LightGreen);
			//	drawLine(referenceTransform.position, referenceTransform.position + referenceTransform.up.normalized, XKCDColors.DarkGreen);
			//}

			if (anim == null)
			{
				moving = false;
				return;
			}

			if (anim.IsPlaying(extendName))
				return;

			if (IsDeployed)
			{
				if (!anim.IsPlaying(extendName))
					moving = false;
			}
			else
			{
				if (!anim.IsPlaying(extendName))
					moving = false;
			}

		}

		private void LateUpdate()
		{
			if (!IsDeployed)
				return;

			if (moving)
				return;

			if (resetting)
			{
				setRestPosition(2);
				return;
			}

			switch(state)
			{
				case "PreAttached":
				case "Disabled":
					return;
				case "Docked (same vessel)":
				case "Docked (docker)":
				case "Docked (dockee)":
					setDockedPosition();
					break;
				case "Acquire":
					setAcquiringPosition();
					break;
				case "Acquire (dockee)":
					if (otherNode == null)
					{
						if (setRest)
							setRestPosition(1);

						break;
					}
					if (otherNode.GetType() == typeof(FlexoTube))
					{
						if (setRest)
							setRestPosition(1);

						break;
					}
					setAcquiringPosition();
					break;
				case "Ready":
				case "Disengage":
					if (setRest)
						setRestPosition(1);
					break;
			}

		}

		private string setStatus()
		{
			if (moving)
				return "Moving...";

			switch (state)
			{
				case "PreAttached":
					Events["Deploy"].active = false;
					Events["Retract"].active = false;
					return "Attached";
				case "Docked (same vessel)":
				case "Docked (docker)":
				case "Docked (dockee)":
					Events["Deploy"].active = false;
					Events["Retract"].active = false;
					return "Docked";
				case "Acquire":
				case "Acquire (dockee)":
					return "Acquiring...";
			}

			if (IsDeployed)
			{
				Events["Deploy"].active = false;
				Events["Retract"].active = true;
				return "Deployed (Flexible Mode)";
			}
			else
			{
				Events["Deploy"].active = true;
				Events["Retract"].active = false;
				return "Retracted (Basic Mode)";
			}
		}

		private void setDockedPosition()
		{
			setTranslation(targetYTransFrame, targetXTransFrame);
			setRotation(targetYRotFrame, targetXRotFrame);
		}

		private void setAcquiringPosition()
		{
			calculateNewTarget();

			int newYTranTarget = getNewFrame(targetYTransFrame, lastYTransFrame, 2);

			int newXTranTarget = getNewFrame(targetXTransFrame, lastXTransFrame, 2);

			setTranslation(newYTranTarget, newXTranTarget);

			//calculateNewRotation();

			int newYRotTarget = getNewFrame(targetYRotFrame, lastYRotFrame, 2);

			int newXRotTarget = getNewFrame(targetXRotFrame, lastXRotFrame, 2);

			setRotation(newYRotTarget, newXRotTarget);
		}

		private void calculateNewTarget()
		{
			if (nodeTransform == null || referenceTranslateTransform == null || otherNode.nodeTransform == null)
			{
				targetYTransFrame = startFrame;
				targetXTransFrame = startFrame;
				return;
			}

			if (rotTrans)
			{ 
				referenceTranslateTransform.rotation = Quaternion.LookRotation(otherNode.nodeTransform.forward * -1f, otherNode.nodeTransform.up);
			}

			TransRot = referenceTranslateTransform.rotation.eulerAngles.ToString("F3");

			if (showLineSet)
			{
				//drawLine(referenceTranslateTransform.position, referenceTranslateTransform.position + refDir.normalized, XKCDColors.Black);
				//drawLine(referenceTranslateTransform.position, referenceTranslateTransform.position + referenceTranslateTransform.forward.normalized, XKCDColors.LightishRed);
				//drawLine(referenceTranslateTransform.position, referenceTranslateTransform.position + referenceTranslateTransform.up.normalized, XKCDColors.DarkishRed);
			}

			Vector3 pos = referenceTranslateTransform.position;
			Vector3 otherPos = otherNode.nodeTransform.position;

			//Pos = pos.ToString("F3");
			//OtherPos = otherPos.ToString("F3");

			Vector3 line = otherPos - pos;

			//PosOtherPos = line.ToString("F3");

			Vector3 front = referenceTranslateTransform.forward;

			//Front = front.ToString("F3");

			Vector3 projected = Vector3.Project(line, front);

			//Projected = projected.ToString("F3");

			Vector3 adjustedProjection = pos + projected;

			//AdjProjected = adjustedProjection.ToString("F3");			
			
			Vector3 v1 = otherPos - adjustedProjection;
			Vector3 v2 = refDir * -1f;
			Vector3 v3 = front;

			theta = Vector3.Angle(v2, v1);

			Vector3 up = Vector3.Cross(v2, v1);
			float sign = Mathf.Sign(Vector3.Dot(v3, up));

			theta *= sign;

			Vector3 flatLine = otherPos - adjustedProjection;

			//FlatLine = flatLine.ToString("F3");

			dist = flatLine.magnitude;

			//Angle = theta.ToString("F4");
			//Distance = dist.ToString("F4");

			if (dist >= maxTranslate)
				dist = maxTranslate;

			//DistanceFixed = dist.ToString("F4");

			theta = theta * Mathf.Deg2Rad;

			float XDist = dist * Mathf.Sin(theta) * -1f;
			float YDist = dist * Mathf.Cos(theta) * -1f;

			XDist *= 2;
			YDist *= 2;

			float XShift = XDist / frameDist;
			float YShift = YDist / frameDist;

			targetXTransFrame = startFrame + (int)XShift;
			targetYTransFrame = startFrame + (int)YShift;

			//if (showLineSet)
			//{
			//	drawLine(pos, otherPos, Color.red);
			//}
			//else
			//{
			//	drawLine(pos, pos + referenceTranslateTransform.forward.normalized, Color.black);
			//	drawLine(adjustedProjection, otherPos, Color.blue);
			//	//drawLine(adjustedProjection, adjustedProjection + v2, Color.green);
			//	//drawLine(adjustedProjection, adjustedProjection + v3, Color.red);
			//	drawLine(pos, otherPos, Color.red);
			//	//drawLine(adjustedProjection, adjustedProjection + up, Color.gray);
			//}

			targetYRotFrame = startFrame;
			targetXRotFrame = startFrame;

			calculateNewRotation(front);
		}

		private void calculateNewRotation(Vector3 dir)
		{
			if (referenceRotationTransform == null || referenceTranslateTransform == null)
			{
				targetYTransFrame = startFrame;
				targetXTransFrame = startFrame;
				return;
			}

			//Vector3 dir = referenceTranslateTransform.forward;

			Vector3 ori = referenceRotationTransform.position;

			Vector3 newZ = referenceRotationTransform.forward;
			Vector3 newY = referenceRotationTransform.up;
			Vector3 newX = referenceRotationTransform.right;

			if (!showLineSet)
			{
				drawLine(ori, ori + newZ, Color.black);
				drawLine(ori, ori + newY, Color.black);
				drawLine(ori, ori + newX, Color.black);
			}

			float rotXX = Vector3.Dot(newZ, dir);
			float rotXY = Vector3.Dot(newY, dir);

			float rotYX = Vector3.Dot(newZ, dir);
			float rotYY = Vector3.Dot(newX, dir);

			DirX = new Vector2(rotXY, rotXX).ToString("F3");
			DirY = new Vector2(rotYY, rotYX).ToString("F3");

			FrontX = new Vector2(Vector3.Dot(newY, newZ), Vector3.Dot(newZ, newZ)).ToString("F3");
			FrontY = new Vector2(Vector3.Dot(newX, newZ), Vector3.Dot(newZ, newZ)).ToString("F3");

			float angX = Mathf.Atan2(rotXY, rotXX);
			float angY = Mathf.Atan2(rotYY, rotYX);

			angX *= Mathf.Rad2Deg;
			angY *= Mathf.Rad2Deg;

			AngleX = angX.ToString("F3");
			AngleY = angY.ToString("F3");

			if (!showLineSet)
			{
				//drawLine(p2, p1, XKCDColors.DarkGreen);
				//drawLine(nodeTransform.position, nodeTransform.position + rot, Color.blue);
			}
			else
			{
				drawLine(nodeTransform.position, nodeTransform.position + dir, Color.blue);
			}

			float radius = Mathf.Sqrt(angX * angX + angY * angY);

			if (radius > 10)
			{
				float theta = Mathf.Atan2(angY, angX);
				radius = 10;
				angX = radius * Mathf.Cos(theta);
				angY = radius * Mathf.Sin(theta);
			}

			NewAngleX = angX.ToString("F3");
			NewAngleY = angY.ToString("F3");

			angX *= -2;
			angY *= -2;

			float XShift = angX / frameRot;
			float YShift = angY / frameRot;

			if (XShift > 200)
				XShift = 200;
			else if (XShift < -200)
				XShift = -200;

			if (YShift > 200)
				YShift = 200;
			else if (YShift < -200)
				YShift = -200;

			targetXRotFrame = startFrame + (int)XShift;
			targetYRotFrame = startFrame + (int)YShift;
		}

		private void drawLine(Vector3 start, Vector3 end, Color c)
		{
			GameObject line = new GameObject();
			line.transform.position = start;
			line.AddComponent<LineRenderer>();
			LineRenderer lr = line.GetComponent<LineRenderer>();
			lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
			lr.SetColors(c, c * 0.3f);
			lr.SetWidth(0.1f, 0.05f);

			lr.SetPosition(0, start);
			lr.SetPosition(1, end);
			Destroy(line, TimeWarp.deltaTime);
		}

		private void setRestPosition(int step)
		{
			int newXTranTarget = getNewFrame(startFrame, lastXTransFrame, step);

			int newYTranTarget = getNewFrame(startFrame, lastYTransFrame, step);

			setTranslation(newYTranTarget, newXTranTarget);

			int newYRotTarget = getNewFrame(startFrame, lastYRotFrame, step);

			int newXRotTarget = getNewFrame(startFrame, lastXRotFrame, step);

			setRotation(newYRotTarget, newXRotTarget);
		}

		private int getNewFrame(int target, int last, int step)
		{
			int i = last;

			if (target > last)
			{
				i = last + step;
				if (i > target)
					i = target;
			}
			else if (last > target)
			{
				i = last - step;
				if (i < target)
					i = target;
			}

			return i;
		}

		private void setTranslation(int yTrans, int xTrans)
		{

			float yTime = (1f * yTrans) / (1f * frames);
			float xTime = (1f * xTrans) / (1f * frames);

			if (xTime > 0.99f)
				xTime = 0.99f;
			else if (xTime < 0.01f)
				xTime = 0.01f;
			if (yTime > 0.99f)
				yTime = 0.99f;
			else if (yTime < 0.01f)
				yTime = 0.01f;

			anim[xTranslateName].speed = 0;
			anim[xTranslateName].normalizedTime = xTime;
			anim.Blend(xTranslateName, 0.5f);

			anim[yTranslateName].speed = 0;
			anim[yTranslateName].normalizedTime = yTime;
			anim.Blend(yTranslateName, 0.5f);

			YTime = anim[yTranslateName].normalizedTime.ToString("F4");
			YFrame = (anim[yTranslateName].normalizedTime * frames).ToString("F1");

			XTime = anim[xTranslateName].normalizedTime.ToString("F4");
			XFrame = (anim[xTranslateName].normalizedTime * frames).ToString("F1");

			lastYTransFrame = yTrans;
			lastXTransFrame = xTrans;
		}

		private void setRotation(int yAxis, int xAxis)
		{
			float yTime = (1f * yAxis) / (1f * frames);
			float xTime = (1f * xAxis) / (1f * frames);

			if (xTime > 0.99f)
				xTime = 0.99f;
			else if (xTime < 0.01f)
				xTime = 0.01f;
			if (yTime > 0.99f)
				yTime = 0.99f;
			else if (yTime < 0.01f)
				yTime = 0.01f;

			anim[rotYName].speed = 0;
			anim[rotYName].normalizedTime = yTime;
			anim.Blend(rotYName, 1);

			anim[rotXName].speed = 0;
			anim[rotXName].normalizedTime = xTime;
			anim.Blend(rotXName, 1);

			YTime = anim[rotYName].normalizedTime.ToString("F4");
			YFrame = (anim[rotYName].normalizedTime * frames).ToString("F1");

			XTime = anim[rotXName].normalizedTime.ToString("F4");
			XFrame = (anim[rotXName].normalizedTime * frames).ToString("F1");

			lastYRotFrame = yAxis;
			lastXRotFrame = xAxis;
		}

		new public void Decouple()
		{
			base.Decouple();

			if (IsDeployed)
			{
				Events["Deploy"].active = false;
				Events["Retract"].active = true;
			}
			else
			{
				Events["Deploy"].active = true;
				Events["Retract"].active = false;
			}
		}

		new public void Undock()
		{
			base.Undock();

			if (IsDeployed)
			{
				Events["Deploy"].active = false;
				Events["Retract"].active = true;
			}
			else
			{
				Events["Deploy"].active = true;
				Events["Retract"].active = false;
			}
		}

		new public void UndockSameVessel()
		{
			base.UndockSameVessel();

			if (IsDeployed)
			{
				Events["Deploy"].active = false;
				Events["Retract"].active = true;
			}
			else
			{
				Events["Deploy"].active = true;
				Events["Retract"].active = false;
			}
		}

		[KSPEvent(guiActive = true, guiActiveUnfocused = true, externalToEVAOnly = true, active = true)]
		public void Deploy()
		{
			if (resetting)
				StopCoroutine("restThenRetract");

			extensionAnimator(1, 0);

			setRest = true;

			IsDeployed = true;

			if (MagnetsEnabled)
			{
				acquireForce = activeForce;
				acquireTorque = activeTorque;
			}
			else
			{
				acquireForce = 0;
				acquireTorque = 0;
			}

			acquireRange = activeRange;
			minDistanceToReEngage = activeReEngage;

			Events["Deploy"].active = false;
			Events["Retract"].active = true;
		}

		[KSPEvent(guiActive = true, guiActiveUnfocused = true, externalToEVAOnly = true, active = false)]
		public void Retract()
		{
			if (resetting)
				return;

			Events["Deploy"].active = true;
			Events["Retract"].active = false;

			if (this.state == "Acquire" || this.state == "Acquire (dockee)")
				StartCoroutine("restThenRetract");
			else
			{
				setRest = false;

				stopAllAnimators();

				extensionAnimator(-1, 1);

				IsDeployed = false;

				if (MagnetsEnabled)
				{
					acquireForce = cachedAcquireForce;
					acquireTorque = cachedAcquireTorque;
				}
				else
				{
					acquireForce = 0;
					acquireTorque = 0;
				}

				acquireRange = cachedAcquireRange;
				minDistanceToReEngage = cachedReEngage;
			}
		}

		[KSPEvent(guiActive = true, guiActiveUnfocused = true, externalToEVAOnly = true, active = true)]
		public void ToggleMagnets()
		{
			if (MagnetsEnabled)
			{
				MagnetsEnabled = false;
				Magnets = "Disabled";
				acquireForce = 0;
				acquireTorque = 0;
			}
			else
			{
				MagnetsEnabled = true;
				Magnets = "Enabled";
				if (IsDeployed)
				{
					acquireForce = activeForce;
					acquireTorque = activeTorque;
				}
				else
				{
					acquireForce = cachedAcquireForce;
					acquireTorque = cachedAcquireTorque;
				}
			}
		}

		[KSPAction("Deploy Tube")]
		public void DeployAction(KSPActionParam param)
		{
			Deploy();
		}

		[KSPAction("Retract Tube")]
		public void RetractAction(KSPActionParam param)
		{
			Retract();
		}

		[KSPAction("Toggle Tube")]
		public void ToggleAction(KSPActionParam param)
		{
			if (IsDeployed)
				Retract();
			else
				Deploy();
		}

		[KSPAction("Toggle Magnets")]
		public void MagnetsAction(KSPActionParam param)
		{
			ToggleMagnets();
		}

		private void stopAllAnimators()
		{
			anim.Stop(yTranslateName);
			anim.Stop(xTranslateName);
			anim.Stop(rotYName);
			anim.Stop(rotXName);
		}

		private IEnumerator restThenRetract()
		{
			resetting = true;

			while (lastYTransFrame != startFrame || lastXTransFrame != startFrame || lastYRotFrame != startFrame || lastXRotFrame != startFrame)
				yield return null;

			setRest = false;

			stopAllAnimators();

			extensionAnimator(-1, 1);
			IsDeployed = false;

			if (MagnetsEnabled)
			{
				acquireForce = cachedAcquireForce;
				acquireTorque = cachedAcquireTorque;
			}
			else
			{
				acquireForce = 0;
				acquireTorque = 0;
			}

			acquireRange = cachedAcquireRange;
			minDistanceToReEngage = cachedReEngage;

			resetting = false;
		}

		private void extensionAnimator(float speed, float time)
		{
			if (anim == null)
				return;

			anim[extendName].speed = speed;

			if (!anim.IsPlaying(extendName))
			{
				anim[extendName].normalizedTime = time;
				anim[extendName].wrapMode = WrapMode.Once;
				anim.Blend(extendName, 1);
				moving = true;
			}
		}

		[KSPEvent(active = true, guiActive = true)]
		public void OpenWindow()
		{
			if (flexoWindow == null)
			{
				flexoWindow = gameObject.AddComponent<FlexoTestWindow>();
				flexoWindow.setup(this);
			}

			flexoWindow.Visible = true;

			Events["OpenWindow"].active = false;
			Events["CloseWindow"].active = true;
		}

		[KSPEvent(active = false, guiActive = true)]
		public void CloseWindow()
		{
			if (flexoWindow == null)
			{
				Events["OpenWindow"].active = true;
				Events["CloseWindow"].active = false;
				return;
			}

			flexoWindow.Visible = false;

			Events["OpenWindow"].active = true;
			Events["CloseWindow"].active = false;
		}

		public Animation Anim
		{
			get { return anim; }
		}

		public string ExtendName
		{
			get { return extendName; }
		}

		public string XTranslateName
		{
			get { return xTranslateName; }
		}

		public string YTranslateName
		{
			get { return yTranslateName; }
		}

		public string RotYName
		{
			get { return rotYName; }
		}

		public string RotZName
		{
			get { return rotXName; }
		}

    }
}
