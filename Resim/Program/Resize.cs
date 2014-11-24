using System.Drawing;
using OpenTK;

namespace Resim.Program
{
	public partial class Game
	{
		public override void Resize(Rectangle newDimensions)
		{
			//crossHair.position = new Vector2(newDimensions.Width / 2 - 16, newDimensions.Height / 2 - 16);
			hudConsole.position = new Vector2(0, newDimensions.Height - hudConsole.height);
		}
	}
}