using System.Drawing;
using OpenTK;

namespace Resim.Program
{
	/* Deze functie wordt gebruikt voor het verplaatsen van de HUD-elementen
	 * Op dit moment is er maar 1 element dat verplaatst moet worden
	 */
	public partial class Game
	{
		public override void Resize(Rectangle newDimensions)
		{
			crossHair.position = new Vector2(newDimensions.Width / 2 - 16, newDimensions.Height / 2 - 16);
		}
	}
}