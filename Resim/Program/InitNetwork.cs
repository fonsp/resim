using System;
using OpenTK;
using GraphicsLibrary.Core;

namespace Resim.Program
{
	public partial class Game
	{
		private Vector3[] possibleSpawns = new Vector3[] {
			/*new Vector3(770, 450, -850),
			new Vector3(-1007, 369, -72),
			new Vector3(-749, 475, 1117),
			new Vector3(859, 394, 610),
			new Vector3(441, 169, -621),
			new Vector3(-153, 91, 1108),
			new Vector3(-338, 394, -809),
			new Vector3(-519, 475, 1232),
			new Vector3(710, 394, 1249),
			new Vector3(798, 169, 264),
			new Vector3(-50, 169, -508),
			new Vector3(-96, 394, -621)*/
			new Vector3(2700, 250, -6075) 
		};

		private void Respawn()
		{
			Camera.Instance.position = possibleSpawns[new Random().Next(possibleSpawns.Length - 1)];
			Camera.Instance.velocity = Vector3.Zero;
		}
	}
}