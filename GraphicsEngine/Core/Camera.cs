﻿using OpenTK;

namespace GraphicsLibrary.Core
{
	public class Camera:Node
	{
		#region SingleTon
		private static Camera instance;
		public static Camera Instance
		{
			get { return instance ?? (instance = new Camera("MainCamera")); }
		}
		#endregion

		public Matrix4 modelview;

		public Matrix4 projection
		{
			get
			{
				return Matrix4.CreatePerspectiveFieldOfView(fov * 3.14159f / 180f, width / height, zNear, zFar);
			}
		}

		private float zNear = 1f;
		public float ZNear
		{
			get
			{
				return zNear;
			}
			set
			{
				zNear = value;
				RenderWindow.Instance.UpdateViewport();
			}
		}
		private float zFar = 100000f;
		public float ZFar
		{
			get
			{
				return zFar;
			}
			set
			{
				zFar = value;
				RenderWindow.Instance.UpdateViewport();
			}
		}
		private float fov = 170f;
		public float Fov
		{
			get
			{
				return fov;
			}
			set
			{
				fov = value;
				RenderWindow.Instance.UpdateViewport();
			}

		}
		public float width;
		public float height;

		public Camera(string name)
			: base(name)
		{

		}
	}
}

