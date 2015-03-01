using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace GraphicsLibrary.Content
{
	/// <summary>
	/// Transfers textures to and from the GPU, and stores the pointers from GPU memory.
	/// </summary>
	public static class TextureManager
	{
		static readonly Dictionary<string, int> mTexCache;

		/// <summary>
		/// The number of loaded textures
		/// </summary>
		public static int numberOfTextures
		{
			get
			{
				return mTexCache.Count;
			}
		}

		static TextureManager()
		{
			try
			{
				mTexCache = new Dictionary<string, int>();
				//GL.Enable(EnableCap.Texture2D);
			}
			catch(Exception exception)
			{
				Debug.WriteLine("WARNING: TextureManager could not be created: " + exception.Message + " @ " + exception.Source);
			}
		}

		/// <summary>
		/// Load an image file to GPU memory.
		/// </summary>
		/// <param name="name">New texture name</param>
		/// <param name="path">Path of the image source</param>
		public static void AddTexture(string name, string path)
		{
			AddTexture(name, path, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
		}

		/// <summary>
		/// Load an image file to GPU memory.
		/// </summary>
		/// <param name="name">New texture name</param>
		/// <param name="path">Path of the image source</param>
		/// <param name="textureMinFilter">Texture filtering: min</param>
		/// <param name="textureMagFilter">Texture filtering: max</param>
		public static void AddTexture(string name, string path, TextureMinFilter textureMinFilter, TextureMagFilter textureMagFilter)
		{
			try
			{
				if(String.IsNullOrEmpty(path))
				{
					throw new ArgumentException("Invalid path");
				}
				if(String.IsNullOrEmpty(name))
				{
					throw new ArgumentException("Invalid name");
				}

				int mTexBuffer = GL.GenTexture();
				GL.BindTexture(TextureTarget.Texture2D, mTexBuffer);

				Bitmap image = new Bitmap(path);
				BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly,
					System.Drawing.Imaging.PixelFormat.Format32bppArgb);

				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, imageData.Width, imageData.Height, 0,
					OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, imageData.Scan0);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)textureMinFilter);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)textureMagFilter);

				image.UnlockBits(imageData);

				mTexCache.Add(name, mTexBuffer);
			}
			catch(Exception exception)
			{
				Debug.WriteLine("WARNING: failed to load texture " + name + " at " + path + ": " + exception.Message);
			}

		}

		/// <summary>
		/// Remove a texture from GPU memory.
		/// </summary>
		/// <param name="name">Texture name</param>
		public static void RemoveTexture(string name)
		{
			try
			{
				if(mTexCache.ContainsKey(name) && GL.IsTexture(mTexCache[name]))
				{
					GL.DeleteTexture(mTexCache[name]);
					mTexCache.Remove(name);
				}
				else
				{
					throw new KeyNotFoundException("Texture key not found");
				}
			}
			catch(Exception exception)
			{
				Debug.WriteLine("WARNING: Failed to remove texture " + name + ": " + exception.Message);
			}

		}

		/// <summary>
		/// Remove all textures from GPU memory.
		/// </summary>
		public static void ClearTextureCache()
		{
			foreach(KeyValuePair<string, int> feTexBuffer in mTexCache)
			{
				GL.DeleteTexture(feTexBuffer.Value);
			}
			mTexCache.Clear();
		}

		/// <summary>
		/// Gets the pointer to the texture in GPU memory.
		/// </summary>
		/// <param name="name">Texture name</param>
		/// <returns>Pointer to the texture in GPU memory</returns>
		public static int GetTexture(string name)
		{
			if(mTexCache.ContainsKey(name))
			{
				return mTexCache[name];
			}
			Debug.WriteLine("WARNING: failed to get texture " + name);
			return mTexCache["default"];
		}
	}
}