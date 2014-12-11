using OpenTK;
using OpenTK.Graphics;

namespace GraphicsLibrary.Hud
{
	public class HudDebugField:TextField
	{
		public string prefix = "";
		public string value = "";
		public int width = 122;
		public AlignMode align = AlignMode.Left;
		public int lineOffset;

		private HudImage back = new HudImage("asdf", "white");

		public HudDebugField(string name, int lineOffset, AlignMode align)
			: base(name)
		{
			this.align = align;
			this.lineOffset = lineOffset;
			back.color = new Color4(0f, 0f, 0f, 0.3f);
			back.height = 18;
			back.width = 128;
		}

		public override void Update(float timeSinceLastUpdate)
		{
			base.Update(timeSinceLastUpdate);

			text = prefix + value;

			// TODO: Update width?

			position.Y = 18 * lineOffset;
			if(align == AlignMode.Left)
			{
				position.X = 0;
			}
			else
			{
				position.X = RenderWindow.Instance.Width - width;
			}
			back.position = position;
			back.width = width;
		}

		public override void Render()
		{
			//render black background
			back.Render();

			position += new Vector2(1, 1); // padding
			base.Render();
			position -= new Vector2(1, 1);
		}
	}

	public enum AlignMode
	{
		Right,
		Left
	}
}