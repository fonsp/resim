using System;
using System.Diagnostics;
using System.Management;
using System.Collections.Generic;
using GraphicsLibrary.Core;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GraphicsLibrary.Hud
{
	public class HudDebug:HudElement
	{
		public Dictionary<string, HudDebugField> fields = new Dictionary<string, HudDebugField>();

		public HudGraph fpsGraph = new HudGraph("fpsGraph");
		public HudGraph vGraph = new HudGraph("vGraph");
		public HudGraph xGraph = new HudGraph("xGraph");
		public HudGraph yGraph = new HudGraph("yGraph");
		public HudGraph zGraph = new HudGraph("zGraph");

		public HudDebug(string name)
			: base(name)
		{
			NewField("fps", 0, AlignMode.Left, "", " fps");
			NewField("position", 1, AlignMode.Left, "Camera: (", ")");

			NewField("version", 3, AlignMode.Left, "ReSim v0.2.0; github.com/fons-/resim", "");
			NewField("cpuVendor", 4, AlignMode.Left, "CPU: ", "");
			foreach(ManagementObject service in new ManagementObjectSearcher("select * from Win32_Processor").Get())
			{
				fields["cpuVendor"].value = service["Name"].ToString();
			}
			NewField("ram", 5, AlignMode.Left, "RAM: ", " MB");

			
			NewField("gpuVendor", 6, AlignMode.Left, "GPU: ", "");
			fields["gpuVendor"].value = GL.GetString(StringName.Vendor) + "; " + GL.GetString(StringName.Renderer) + "; " + GL.GetString(StringName.Version);
			NewField("display", 7, AlignMode.Left, "Display: ", "");
			NewField("window", 8, AlignMode.Left, "Window: ", "");
			NewField("vsync", 9, AlignMode.Left, "VSync: ", "");

			NewField("tl", 0, AlignMode.Right, "Local time: ", " s");
			NewField("tw", 1, AlignMode.Right, "World time: ", " s");
			NewField("td", 2, AlignMode.Right, "Ahead by: ", " s");

			NewField("lf", 4, AlignMode.Right, "Lorentz factor: ", "");
			NewField("warp", 5, AlignMode.Right, "Warp factor: ", "");
			NewField("v", 6, AlignMode.Right, "v: ", " /s");
			NewField("vc", 7, AlignMode.Right, "v/c: ", "");
			NewField("c", 8, AlignMode.Right, "c: ", " /s");
			NewField("timeMult", 9, AlignMode.Right, "timeMult: ", "");

			NewField("enAb", 11, AlignMode.Right, "Relativistic aberration: ", "");
			NewField("enBr", 12, AlignMode.Right, "Relativistic brightness: ", "");
			NewField("enDo", 13, AlignMode.Right, "Doppler effect: ", "");

			fpsGraph.position.Y = 10 * 14;
			Add(fpsGraph);
			vGraph.position.Y = 10 * 14 + 128;
			Add(vGraph);
			xGraph.position.Y = 10 * 14 + 256;
			xGraph.color = Color4.Red;
			Add(xGraph);
			yGraph.position.Y = 10 * 14 + 256;
			yGraph.backgroundColor = Color4.Transparent;
			yGraph.color = Color4.Green;
			Add(yGraph);
			zGraph.position.Y = 10 * 14 + 256;
			zGraph.backgroundColor = Color4.Transparent;
			zGraph.color = Color4.Blue;
			Add(zGraph);
		}

		/// <summary>
		/// Adds a new field.
		/// </summary>
		/// <param name="fieldName">Field identifier</param>
		/// <param name="lineOffset">Number of lines below the top edge of the screen</param>
		/// <param name="align">Left/Right align</param>
		/// <param name="prefix">Displayed text will be: [prefix][value][suffix]</param>
		/// <param name="suffix">Displayed text will be: [prefix][value][suffix]</param>
		private void NewField(string fieldName, int lineOffset, AlignMode align, string prefix, string suffix)
		{
			if(fields.ContainsKey(fieldName))
			{
				throw new ArgumentException("Name already exists");
			}
			fields.Add(fieldName, new HudDebugField(fieldName, lineOffset, align));
			fields[fieldName].prefix = prefix;
			fields[fieldName].suffix = suffix;
			Add(fields[fieldName]);
		}

		/// <summary>
		/// Set the value of the specified field.
		/// </summary>
		/// <param name="fieldName">Name of the field</param>
		/// <param name="value">New value</param>
		public void SetValue(string fieldName, string value)
		{
			fields[fieldName].value = value;
		}

		/// <summary>
		/// Gets the value of the specified field.
		/// </summary>
		/// <param name="fieldName">Name of the field</param>
		/// <returns>The value of the specified field</returns>
		public string GetValue(string fieldName)
		{
			return fields[fieldName].value;
		}

		public int fps = 0;
		private int fCount = 0;
		private float fTime = 0f;

		public override void Update(float timeSinceLastUpdate)
		{
			base.Update(timeSinceLastUpdate);

			fCount++;
			if(fTime >= 0.5f)
			{
				fTime -= 0.5f;
				fps = fCount * 2;
				fpsGraph.value = (byte)((fps*256)/1000);
				fCount = 0;
			}
			fTime += timeSinceLastUpdate;

			fields["fps"].value = fps.ToString("D");
			fields["position"].value = Camera.Instance.derivedPosition.X.ToString("F1") + ", " +
									   Camera.Instance.derivedPosition.Y.ToString("F1") + ", " +
									   Camera.Instance.derivedPosition.Z.ToString("F1");

			fields["ram"].value = ((float)Process.GetCurrentProcess().PrivateMemorySize64 / 1024f / 1024f).ToString("F2");
			fields["display"].value = RenderWindow.Instance.Width.ToString("D") + "x" + RenderWindow.Instance.Height.ToString("D");
			fields["window"].value = RenderWindow.Instance.WindowState + ", " + RenderWindow.Instance.WindowBorder;
			fields["vsync"].value = RenderWindow.Instance.VSync.ToString();

			fields["tl"].value = RenderWindow.Instance.localTime.ToString("F2");
			fields["tw"].value = RenderWindow.Instance.worldTime.ToString("F2");
			fields["td"].value = (RenderWindow.Instance.worldTime - RenderWindow.Instance.localTime).ToString("F2");
			fields["lf"].value = RenderWindow.Instance.lf.ToString("F3");
			fields["warp"].value = Math.Pow(RenderWindow.Instance.b, 1.0 / 3.0).ToString("F3");
			fields["v"].value = RenderWindow.Instance.v.ToString("F1");
			fields["vc"].value = RenderWindow.Instance.b.ToString("F4");
			fields["c"].value = RenderWindow.Instance.c.ToString("F1");
			fields["timeMult"].value = RenderWindow.Instance.timeMultiplier.ToString("F4");

			fields["enAb"].value = RenderWindow.Instance.enableRelAberration ? "on " : "off";
			fields["enBr"].value = RenderWindow.Instance.enableRelBrightness ? "on " : "off";
			fields["enDo"].value = RenderWindow.Instance.enableDoppler ? "on " : "off";

			vGraph.value = (byte)((256 * RenderWindow.Instance.v) / RenderWindow.Instance.c);
			xGraph.value = (byte)((256 * (Camera.Instance.position.X - 0030)) / (4200 - 0030));
			yGraph.value = (byte)((256 * (Camera.Instance.position.Y - 0000)) / (0750 - 0000));
			zGraph.value = (byte)((256 * (Camera.Instance.position.Z + 7200)) / (7200 - 5000));
		}
	}
}