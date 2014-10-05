using System;
using Resim.Logic;
using GraphicsLibrary.Collision;
using GraphicsLibrary.Core;
using OpenTK;

namespace Resim.Program
{
	public partial class Game
	{
		private readonly Weapon[,] weapons = new Weapon[13, 3];
		private Weapon playerWeapon;

		/* Hier worden de verschillende weapons gedefinieerd
		 * De meeste code wordt nog niet gebruikt
		 */
		private void InitializeWeapons()
		{
			weapons[0, 0] = new MeleeWeapon("Fist level 1", 0, 1, 300, 2);
			weapons[0, 1] = new MeleeWeapon("Fist level 2", 20, 4, 300, 2);
			weapons[0, 2] = new MeleeWeapon("Fist level 3", 100, 12, 300, 2);

			weapons[1, 0] = new MeleeWeapon("Bat level 1", 50, 10, 300, 1);
			weapons[1, 1] = new MeleeWeapon("Bat level 2", 300, 25, 300, 1.8f);
			weapons[1, 2] = new MeleeWeapon("Bat level 3", 1500, 50, 300, 2.3f);

			weapons[2, 0] = new MeleeWeapon("Knife level 1", 250, 15, 300, 3);
			weapons[2, 1] = new MeleeWeapon("Knife level 2", 750, 50, 300, 3.2f);
			weapons[2, 2] = new MeleeWeapon("Knife level 3", 2500, 50, 300, 4);

			weapons[3, 0] = new MeleeWeapon("Sword level 1", 700, 40, 300, 1);
			weapons[3, 1] = new MeleeWeapon("Sword level 2", 3000, 75, 300, 1.4f);
			weapons[3, 2] = new MeleeWeapon("Sword level 3", 7000, 120, 300, 2f);

			weapons[4, 0] = new HitScanWeapon("Pistol level 1", 1500, 50, 8, false, false, 999);
			weapons[4, 1] = new HitScanWeapon("Pistol level 2", 4000, 75, 8.5f, false, false, 999);
			weapons[4, 2] = new HitScanWeapon("Pistol level 3", 10000, 120, 9f, false, false, 999);

			//TODO: Grenades

			//TODO: Mines

			weapons[7, 0] = new HitScanWeapon("M1 level 1", 1500, 50, 2.2f, false, false, 999);
			weapons[7, 1] = new HitScanWeapon("M1 level 2", 4000, 75, 10, false, false, 999);
			weapons[7, 2] = new HitScanWeapon("M1 level 3", 10000, 120, 10, false, false, 999);



			//TODO
			/*weapons[4, 0] = new HitScanWeapon("Sniper rifle level 1", 1500, 50, 2, false, false, 999);
			weapons[4, 1] = new HitScanWeapon("Sniper rifle level 2", 4000, 75, 3, false, false, 999);
			weapons[4, 2] = new HitScanWeapon("Sniper rifle level 3", 10000, 120, 4.5f, false, false, 999);
			*/

			//TODO
			weapons[10, 0] = new HitScanWeapon("M16 level 1", 1500, 50, 11.7f, true, false, 999);
			weapons[10, 1] = new HitScanWeapon("M16 level 2", 4000, 75, 13, true, false, 999);
			weapons[10, 2] = new HitScanWeapon("M16 level 3", 10000, 120, 16, true, false, 999);


			playerWeapon = weapons[7, 0];
		}

		/* Deze functie wordt opgeroepen als je klikt
		 * Hier wordt berekend of het schot raak was
		 */
		private bool FireWeapon()
		{
			if(playerWeapon.WeaponType == WeaponType.Hitscan)
			{
				flashA.materialAge = flashB.materialAge = 0;
				HitScanWeapon weapon = (HitScanWeapon)playerWeapon;

				Vector3 dir = new Vector3(
					(float)Math.Sin(fpsCam.X),
					(float)Math.Tan(fpsCam.Y),
					(float)Math.Cos(fpsCam.X)
				);
				CollisionRay collisionRay = new CollisionRay(Camera.Instance.position, dir);

				#region map
				float mapHitDistance = float.MaxValue;
				foreach(CollisionAABB collisionBox in mapCollision)
				{
					float hitDistance = collisionBox.Intersect(collisionRay);
					if(hitDistance != -1)
					{
						mapHitDistance = Math.Min(hitDistance, mapHitDistance);//(hitDistance < mapHitDistance) ? hitDistance : mapHitDistance;
					}
				}
				Console.WriteLine("m{0}", mapHitDistance);
				#endregion
				return false;
			}
			else if(playerWeapon.WeaponType == WeaponType.Melee)
			{
				MeleeWeapon weapon = (MeleeWeapon)playerWeapon;

				Vector3 dir = Vector3.Zero;

				fpsCam.Y = Math.Min(fpsCam.Y, 1.57f);
				fpsCam.Y = Math.Max(fpsCam.Y, -1.57f);

				dir.X = (float)Math.Sin(fpsCam.X);
				dir.Z = (float)Math.Cos(fpsCam.X);
				dir.Y = (float)Math.Tan(fpsCam.Y);

				CollisionRay collisionRay = new CollisionRay(Camera.Instance.position, dir);
				float hitDistance = monsterAABB.Intersect(collisionRay);
				//TODO: map
				if(hitDistance != -1 && hitDistance < weapon.Range)
				{
					ShowCombo(weapon.Damage, crossHair.position);
					return true;
				}
				return false;
			}
			else
			{
				return false;
			}
		}
	}
}