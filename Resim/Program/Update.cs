using System;
using Resim.Logic;
using GraphicsLibrary;
using GraphicsLibrary.Core;
using GraphicsLibrary.Input;
using OpenTK;
using OpenTK.Input;
using GraphicsLibrary.Collision;
using System.Collections.Generic;
using Resim.Logic;

namespace Resim.Program
{
	/* De Update functie wordt voor elk frame opgeroepen
	 * Op dit moment staat het grootste deel van het spel in deze functie
	 */
	public partial class Game
	{
		private Vector2 fpsCam = Vector2.Zero;
		private double bobbingTimer;
		private double bobbingFactor;
		private float deathTimer;
		private int walkSpeed = 400;
		private int playerHeight = 170;
		private Vector2 mouseSensitivity = new Vector2(200, 200); //TODO: config
		private Vector3 cameraBobDelta = Vector3.Zero;
		private Vector3 gunBobDelta = Vector3.Zero;
		private Vector3 recoilDelta = Vector3.Zero;
		private int comboFieldCounter;
		private bool previousButtonState;
		private float timeSinceLastShot;
		private CollisionAABB monsterAABB = new CollisionAABB(new Vector3(-50, 0, -50), new Vector3(50, 110, 50));

		public override void Update(float timeSinceLastUpdate)
		{
			skybox.position = Camera.Instance.position;

			Camera.Instance.position -= cameraBobDelta;
			gunBase.position -= gunBobDelta;
			gunBase.position -= recoilDelta;

			InputManager.UpdateToggleStates();

			#region FPS camera
			/* De camera beweegt met de muis mee
			 * zonder dat de muis het scherm verlaat
			 */
			if(!InputManager.IsKeyToggled(Key.Delete))
			{
				Vector2 mouseDelta = InputManager.GetMousePosition();
				mouseDelta.Y /= mouseSensitivity.Y;
				mouseDelta.X /= mouseSensitivity.X;
				fpsCam += mouseDelta;
				fpsCam.Y = (float)Math.Max(-1.57, Math.Min(1.57, fpsCam.Y));

				Camera.Instance.ResetOrientation();
				Camera.Instance.Pitch(-fpsCam.Y);
				Camera.Instance.Yaw(fpsCam.X);
				InputManager.HideCursor();
			}
			else
			{
				InputManager.ShowCursor();
			}

			/* Inzoomen met de rechter muisknop
			 */
			if(InputManager.IsButtonDown(MouseButton.Right))
			{
				if(Camera.Instance.Fov != 50)
				{
					Camera.Instance.Fov = 50;
				}
				gunBase.position = new Vector3(0, -10.1f, 10);
				crossHair.isVisible = false;
			}
			else
			{
				if(Camera.Instance.Fov != 90)
				{
					Camera.Instance.Fov = 90;
				}
				gunBase.position = new Vector3(-6, -10.5f, 12);
				crossHair.isVisible = true;
			}

			if(InputManager.IsKeyDown(Key.Left))
			{
				gunBase.Yaw(timeSinceLastUpdate);
				monster.position.X += 500 * timeSinceLastUpdate;
			}

			if(InputManager.IsKeyDown(Key.Right))
			{
				gunBase.Yaw(-timeSinceLastUpdate);
				monster.position.Y += 500 * timeSinceLastUpdate;
			}
			#endregion

			#region Movement
			/* Rennen/lopen*/
			if(InputManager.IsKeyDown(Key.ShiftLeft))
			{
				walkSpeed = config.GetInt("runningSpeed");
			}
			else
			{
				walkSpeed = config.GetInt("walkingSpeed");
			}
			bool w = InputManager.IsKeyDown(Key.W);
			bool a = InputManager.IsKeyDown(Key.A);
			bool s = InputManager.IsKeyDown(Key.S);
			bool d = InputManager.IsKeyDown(Key.D);
			Vector2 delta = new Vector2(0, 0);
			if(w)
			{
				delta.Y += 1;
			}
			if(a)
			{
				delta.X -= 1;
			}
			if(s)
			{
				delta.Y -= 1;
			}
			if(d)
			{
				delta.X += 1;
			}

			delta.NormalizeFast();

			// Y
			Camera.Instance.velocity.X += delta.Y * walkSpeed * timeSinceLastUpdate * (float)Math.Sin(fpsCam.X); //0
			Camera.Instance.velocity.Z -= delta.Y * walkSpeed * timeSinceLastUpdate * (float)Math.Cos(fpsCam.X); //1

			// X
			Camera.Instance.velocity.X += delta.X * walkSpeed * timeSinceLastUpdate * (float)Math.Cos(fpsCam.X);//1
			Camera.Instance.velocity.Z += delta.X * walkSpeed * timeSinceLastUpdate * (float)Math.Sin(fpsCam.X);//0
			#endregion

			#region Collision
			bool grounded = false;

			if(InputManager.IsKeyDown(Key.LControl))
			{
				playerHeight = config.GetInt("crouchHeight");
			}
			else
			{
				playerHeight = config.GetInt("walkHeight");
			}

			Vector3 feetPosHorizontal = Camera.Instance.position;
			feetPosHorizontal.Y -= playerHeight * 0.75f;

			/* De map is opgebouwd uit een hoop kubussen (CollisionBox.cs) waarop de spelers lopen
			 */
			foreach(CollisionAABB collisionBox in mapCollision)
			{
				Vector3 feetPosVertical = Camera.Instance.position;
				feetPosVertical.Y -= playerHeight;
				// Y
				if(collisionBox.isInside(feetPosVertical))
				{
					grounded = true;
					CollisionRay collisionRay = new CollisionRay(Camera.Instance.position, new Vector3(.00001f, -1, .00001f));
					float height = collisionBox.Intersect(collisionRay);
					Camera.Instance.position.Y -= height - playerHeight;
				}
				/* Door twee maal de Y-as te testen met een klein hoogteverschil kunnen de spelers trappen/hellingen op lopen
				 */
				feetPosVertical.Y += 3;//TODO
				// Y
				if(collisionBox.isInside(feetPosVertical))
				{
					grounded = true;
					CollisionRay collisionRay = new CollisionRay(Camera.Instance.position, new Vector3(.00001f, -1, .00001f));
					float height = collisionBox.Intersect(collisionRay);
					Camera.Instance.position.Y -= height - playerHeight;
				}
				// X
				if(Camera.Instance.velocity.X > 0)
				{
					if(collisionBox.isInside(feetPosHorizontal + new Vector3(Camera.Instance.velocity.X * timeSinceLastUpdate + 30, 0, 0)))
					{ //30 is de breedte van de speler
						Camera.Instance.velocity.X = 0;
					}
				}
				else
				{
					if(collisionBox.isInside(feetPosHorizontal + new Vector3(Camera.Instance.velocity.X * timeSinceLastUpdate - 30, 0, 0)))
					{ //TODO
						Camera.Instance.velocity.X = 0;
					}
				}
				// Z
				if(Camera.Instance.velocity.Z > 0)
				{
					if(collisionBox.isInside(feetPosHorizontal + new Vector3(0, 0, Camera.Instance.velocity.Z * timeSinceLastUpdate + 30)))
					{
						Camera.Instance.velocity.Z = 0;
					}
				}
				else
				{
					if(collisionBox.isInside(feetPosHorizontal + new Vector3(0, 0, Camera.Instance.velocity.Z * timeSinceLastUpdate - 30)))
					{
						Camera.Instance.velocity.Z = 0;
					}
				}
			}
			#endregion

			if(InputManager.IsKeyDown(Key.R))
			{
				Camera.Instance.position = new Vector3(-10, 1000, -10);
			}
			#region Jumping/Gravity
			if(grounded)
			{
				Camera.Instance.velocity.Y = 0;

				/* Springen*/
				if(InputManager.IsKeyDown(Key.Space))
				{
					Camera.Instance.velocity.Y = config.GetInt("jumpForce");
				}
			}
			else
			{
				Camera.Instance.velocity.Y -= config.GetInt("gravity") * timeSinceLastUpdate;
			}

			#endregion

			#region Bobbing
			/* Als de speler beweegt maakt de camera subtiele bewegingen
			 * hierdoor lijkt het alsof het hoofd meebeweegt met het lopen
			 */
			if(delta == Vector2.Zero || !grounded)
			{
				bobbingFactor = bobbingFactor * Math.Pow(0.1, timeSinceLastUpdate);
			}
			else
			{
				bobbingFactor = -((1 - bobbingFactor) * Math.Pow(0.1, timeSinceLastUpdate) - 1);
				bobbingTimer += timeSinceLastUpdate;
			}

			Vector2 bobbingStrength = new Vector2(10, 5);

			cameraBobDelta = new Vector3
			(
				(float)(bobbingFactor * Math.Cos(bobbingTimer * 12)) * bobbingStrength.Y,
				(float)(Math.Cos(fpsCam.X) * bobbingFactor * Math.Sin(bobbingTimer * 6)) * bobbingStrength.X,
				(float)(Math.Sin(fpsCam.X) * bobbingFactor * Math.Sin(bobbingTimer * 6)) * bobbingStrength.X
			);

			gunBobDelta = new Vector3(0, cameraBobDelta.Y / 50, 0);
			gunBobDelta.X = (float)(bobbingFactor * Math.Sin(bobbingTimer * 6) * bobbingStrength.X / 50);

			Camera.Instance.position += cameraBobDelta;
			#endregion

			#region Weapons
			bool currentButtonState = InputManager.IsButtonDown(MouseButton.Left);
			timeSinceLastShot += timeSinceLastUpdate;
			if(currentButtonState)
			{
				if(playerWeapon.WeaponType == WeaponType.Hitscan)
				{
					HitScanWeapon weapon = (HitScanWeapon)playerWeapon;
					if(weapon.Automatic)
					{
						while(timeSinceLastShot >= 1 / weapon.FireRate)
						{
							FireWeapon();
							//recoil
							recoilDelta.Z = -4;
							recoilDelta.X += (float)random.NextDouble() * .1f;
							recoilDelta.Y += (float)random.NextDouble() * .4f;
							timeSinceLastShot -= 1 / weapon.FireRate;
						}
					}
					else
					{
						if(!previousButtonState)
						{
							if(timeSinceLastShot >= 1 / weapon.FireRate)
							{
								FireWeapon();
								timeSinceLastShot = 0;
							}
						}
					}
				}
				else if(playerWeapon.WeaponType == WeaponType.Melee)
				{
					MeleeWeapon weapon = (MeleeWeapon)playerWeapon;
					if(!previousButtonState)
					{
						if(timeSinceLastShot >= 1 / weapon.FireRate)
						{
							FireWeapon();
							timeSinceLastShot = 0;
						}
					}
				}
			}
			else
			{
				timeSinceLastShot = Math.Min(timeSinceLastShot, 1 / playerWeapon.FireRate);
			}
			previousButtonState = currentButtonState;
			#endregion

			#region Recoil
			recoilDelta = Vector3.Multiply(recoilDelta, (float)Math.Pow(0.00001, timeSinceLastUpdate));
			gunBase.position += recoilDelta;
			gunBase.position += gunBobDelta;
			#endregion

			#region HUD
			// Shop/inventory
			//Console.WriteLine(InputManager.IsKeyToggled(Key.E));

			// Grain
			grainImage.position.X = -random.Next(0, (int)Math.Max(0, grainImage.width - RenderWindow.Instance.ClientRectangle.Width));
			grainImage.position.Y = -random.Next(0, (int)Math.Max(0, grainImage.height - RenderWindow.Instance.ClientRectangle.Height));
			#endregion
		}
	}
}