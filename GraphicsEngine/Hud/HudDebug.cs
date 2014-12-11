using System;
using System.Collections.Generic;
using OpenTK;

namespace GraphicsLibrary.Hud
{
	public class HudDebug:HudElement
	{
		public Dictionary<string, HudDebugField> fields = new Dictionary<string, HudDebugField>();

		public HudDebug(string name)
			: base(name)
		{
			NewField("test", 0, AlignMode.Left, "Test: ");
			NewField("width", 0, AlignMode.Right, "Width: ", 12);
			NewField("height", 1, AlignMode.Right, "Height: ", 12);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fieldName">Field identifier</param>
		/// <param name="lineOffset">Number of lines below the top edge of the screen</param>
		/// <param name="align">Left/Right align</param>
		/// <param name="prefix">Displayed text will be: [prefix][value]</param>
		/// <param name="width">Field width in characters (16 by default)</param>
		private void NewField(string fieldName, int lineOffset, AlignMode align, string prefix, int width = 16)
		{
			if(fields.ContainsKey(fieldName))
			{
				throw new ArgumentException("Name already exists");
			}
			fields.Add(fieldName, new HudDebugField(fieldName, lineOffset, align));
			fields[fieldName].prefix = prefix;
			fields[fieldName].width = 12 * width;
			Add(fields[fieldName]); // Add newly created hudfield node to this huddebug node
		}

		public void SetValue(string fieldName, string value)
		{
			fields[fieldName].value = value;
		}

		public string GetValue(string fieldName)
		{
			return fields[fieldName].value;
		}

		public override void Update(float timeSinceLastUpdate)
		{
			base.Update(timeSinceLastUpdate);

			// Update code here
			fields["test"].value = "swag";
			fields["width"].value = RenderWindow.Instance.Width.ToString();
			fields["height"].value = RenderWindow.Instance.Height.ToString();
		}
	}
}