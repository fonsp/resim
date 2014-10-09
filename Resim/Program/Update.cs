using System;
using OpenTK;
using OpenTK.Input;
using GraphicsLibrary;
using GraphicsLibrary.Collision;
using GraphicsLibrary.Core;
using GraphicsLibrary.Input;

namespace Resim.Program
{
	public partial class Game
	{
		private Vector2 fpsCam = Vector2.Zero;
		private double bobbingTimer;
		private double bobbingFactor;
		private int walkSpeed = 400;
		private int playerHeight = 170;
		private Vector2 mouseSensitivity = new Vector2(200, 200); //TODO: config
		private Vector3 cameraBobDelta = Vector3.Zero;
		private int comboFieldCounter;
		private CollisionAABB monsterAABB = new CollisionAABB(new Vector3(-50, 0, -50), new Vector3(50, 110, 50));

		public override void Update(float timeSinceLastUpdate)
		{
			skybox.position = Camera.Instance.position;
			Camera.Instance.position -= cameraBobDelta;

			InputManager.UpdateToggleStates();

			if(InputManager.IsKeyDown(Key.R))
			{
				Respawn();
			}

			#region FPS camera

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

			if(InputManager.IsButtonDown(MouseButton.Right))
			{
				if(Camera.Instance.Fov != 50)
				{
					Camera.Instance.Fov = 50;
				}
				crossHair.isVisible = false;
			}
			else
			{
				if(Camera.Instance.Fov != 90)
				{
					Camera.Instance.Fov = 90;
				}
				crossHair.isVisible = true;
			}

			if(InputManager.IsKeyDown(Key.Left))
			{
				monster.position.X -= 500f * timeSinceLastUpdate;
				monsterAABB.Translate(new Vector3(-500f * timeSinceLastUpdate, 0f, 0f));
			}

			if(InputManager.IsKeyDown(Key.Right))
			{
				monster.position.X += 500f * timeSinceLastUpdate;
				monsterAABB.Translate(new Vector3(500f * timeSinceLastUpdate, 0f, 0f));
			}

			if(InputManager.IsKeyDown(Key.Up))
			{
				monster.position.Y += 500f * timeSinceLastUpdate;
				monsterAABB.Translate(new Vector3(0f, 500f * timeSinceLastUpdate, 0f));
			}

			if(InputManager.IsKeyDown(Key.Down))
			{
				monster.position.Y -= 500f * timeSinceLastUpdate;
				monsterAABB.Translate(new Vector3(0f, -500f * timeSinceLastUpdate, 0f));
			}

			#endregion
			#region Movement

			walkSpeed = config.GetInt(InputManager.IsKeyDown(Key.ShiftLeft) ? "runningSpeed" : "walkingSpeed");

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
			Camera.Instance.velocity.X += delta.X * walkSpeed * timeSinceLastUpdate * (float)Math.Cos(fpsCam.X); //1
			Camera.Instance.velocity.Z += delta.X * walkSpeed * timeSinceLastUpdate * (float)Math.Sin(fpsCam.X); //0

			#endregion
			#region Collision

			bool grounded = false;
			playerHeight = config.GetInt(InputManager.IsKeyDown(Key.LControl) ? "crouchHeight" : "walkHeight");
			Vector3 feetPosHorizontal = Camera.Instance.position;
			feetPosHorizontal.Y -= playerHeight * 0.75f;

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
			#region Jumping/Gravity
			if(grounded)
			{
				Camera.Instance.velocity.Y = 0;

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

			if(delta == Vector2.Zero || !grounded)
			{
				bobbingFactor = bobbingFactor * Math.Pow(0.1, timeSinceLastUpdate);
			}
			else
			{
				bobbingFactor = -((1 - bobbingFactor) * Math.Pow(0.1, timeSinceLastUpdate) - 1);
				bobbingTimer += timeSinceLastUpdate;
			}

			Vector2 bobbingStrength = new Vector2(10, 10); //TODO: config

			cameraBobDelta = new Vector3
			(
				(float)(Math.Cos(fpsCam.X) * bobbingFactor * Math.Sin(bobbingTimer * 6)) * bobbingStrength.X,
				(float)(bobbingFactor * Math.Cos(bobbingTimer * 12)) * bobbingStrength.Y,
				(float)(Math.Sin(fpsCam.X) * bobbingFactor * Math.Sin(bobbingTimer * 6)) * bobbingStrength.X
			);

			Camera.Instance.position += cameraBobDelta;

			#endregion
			#region HUD

			// Grain
			grainImage.position.X = -random.Next(0, (int)Math.Max(0, grainImage.width - RenderWindow.Instance.ClientRectangle.Width));
			grainImage.position.Y = -random.Next(0, (int)Math.Max(0, grainImage.height - RenderWindow.Instance.ClientRectangle.Height));

			#endregion
		}
	}
}