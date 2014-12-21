using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using OpenTK;
using GraphicsLibrary.Core;
using GraphicsLibrary.Collision;

namespace GraphicsLibrary.Content
{
	public static class ObjConverter
	{
		/// <summary>
		/// Converts the given .obj file to a Mesh object
		/// </summary>
		/// <param name="inputFile">.obj file (not path)</param>
		/// <returns>Generated Mesh</returns>
		public static Mesh ConvertObjToMesh(string inputFile)
		{
			return ConvertObjToMesh(inputFile, Vector3.Zero);
		}

		/// <summary>
		/// Converts the given .obj file to a Mesh object
		/// </summary>
		/// <param name="inputFile">.obj file (not path)</param>
		/// <param name="offset">Mesh offset</param>
		/// <returns>Generated Mesh</returns>
		public static Mesh ConvertObjToMesh(string inputFile, Vector3 offset)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			Mesh output = new Mesh();

			string[] inputElements = inputFile.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

			int vCount = 1;
			int vnCount = 1;
			int vtCount = 1;

			int totalVertices = 0;
			int totalNormals = 0;
			int totalTextCoordinates = 0;

			foreach(string s in inputElements)
			{
				string[] decomposed = s.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

				if(decomposed[0] == "v" | decomposed[0] == "V")
				{
					totalVertices++;
				}
				else if(decomposed[0] == "vt")
				{
					totalTextCoordinates++;
				}
				else if(decomposed[0] == "vn")
				{
					totalNormals++;
				}
			}

			//Vertex[] vertices = new Vertex[totalVertices + 1];
			Vector3[] vertices = new Vector3[totalVertices + 1];
			Vector3[] normals = new Vector3[totalNormals + 1];
			Vector2[] textCoords = new Vector2[totalTextCoordinates + 1];

			List<Face> faces = new List<Face>();

			foreach(string s in inputElements)
			{
				string[] decomposed = s.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

				switch(decomposed[0])
				{
					#region Comment
					case "#":

						break;
					#endregion
					#region Geometric vertex
					case "v":
					case "V":
						Vector3 pos = new Vector3((float)Convert.ToDouble(decomposed[1].Replace(",", ".")),
												  (float)Convert.ToDouble(decomposed[2].Replace(",", ".")),
												  (float)Convert.ToDouble(decomposed[3].Replace(",", ".")));

						vertices[vCount] = pos - offset;

						vCount++;


						break;
					#endregion
					#region Texture vertex
					case "vt":

						Vector2 tex = new Vector2((float)Convert.ToDouble(decomposed[1].Replace(",", ".")),
												  1f - (float)Convert.ToDouble(decomposed[2].Replace(",", ".")));

						textCoords[vtCount] = tex;

						vtCount++;

						break;
					#endregion
					#region Vertex normal
					case "vn":
						Vector3 nrm = new Vector3((float)Convert.ToDouble(decomposed[1].Replace(",", ".")),
												  (float)Convert.ToDouble(decomposed[2].Replace(",", ".")),
												  (float)Convert.ToDouble(decomposed[3].Replace(",", ".")));

						normals[vnCount] = nrm;

						vnCount++;

						break;
					#endregion
					#region Face
					case "f":
					case "F":

						int length = decomposed.Length - 1;
						int[] vertexIndices = new int[length];
						int[] normalIndices = new int[length];
						int[] textureIndices = new int[length];
						for(int i = 0; i < decomposed.Length - 1; i++)
						{
							string[] vertexString = decomposed[i + 1].Split(new string[] { "/" }, StringSplitOptions.None);
							vertexIndices[i] = Convert.ToInt32(vertexString[0]);
							textureIndices[i] = Convert.ToInt32(vertexString[1]);
							normalIndices[i] = Convert.ToInt32(vertexString[2]);
						}
						faces.Add(new Face(vertexIndices, textureIndices, normalIndices));



						break;
					#endregion
					#region Mtl library
					case "mtllib":

						//TODO: MTLLIB

						break;
					#endregion
					#region Mtl library reference
					case "usemtl":

						//TODO: USEMTL

						break;
					#endregion
				}
			}

			foreach(Face f in faces)
			{
				Vertex[] vertexArr = new Vertex[f.vIndices.Length];

				for(int i = 0; i < f.vIndices.Length; i++)
				{
					vertexArr[i] = new Vertex(vertices[f.vIndices[i]], normals[f.vnIndices[i]], textCoords[f.vtIndices[i]]);
				}

				output.polygonList.Add(new Polygon(vertexArr));
			}

			return output;
		}

		/// <summary>
		/// Converts the given .obj file to a Mesh object that supports VBO rendering
		/// </summary>
		/// <param name="inputFile">.obj file (not path)</param>
		/// <param name="offset">Mesh offset</param>
		/// <returns>Generated Mesh</returns>
		public static Mesh ConvertObjToVboMesh(string inputFile, Vector3 offset)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			Mesh output = new Mesh();
			List<Vertex> vOuput = new List<Vertex>();

			string[] inputElements = inputFile.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

			int vCount = 1;
			int vnCount = 1;
			int vtCount = 1;

			int totalVertices = 0;
			int totalNormals = 0;
			int totalTextCoordinates = 0;

			foreach(string s in inputElements)
			{
				string[] decomposed = s.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

				if(decomposed[0] == "v" | decomposed[0] == "V")
				{
					totalVertices++;
				}
				else if(decomposed[0] == "vt")
				{
					totalTextCoordinates++;
				}
				else if(decomposed[0] == "vn")
				{
					totalNormals++;
				}
			}

			//Vertex[] vertices = new Vertex[totalVertices + 1];
			Vector3[] vertices = new Vector3[totalVertices + 1];
			Vector3[] normals = new Vector3[totalNormals + 1];
			Vector2[] textCoords = new Vector2[totalTextCoordinates + 1];

			List<Face> faces = new List<Face>();

			foreach(string s in inputElements)
			{
				string[] decomposed = s.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

				switch(decomposed[0])
				{
					#region Comment
					case "#":

						break;
					#endregion
					#region Geometric vertex
					case "v":
					case "V":
						Vector3 pos = new Vector3((float)Convert.ToDouble(decomposed[1].Replace(",", ".")),
												  (float)Convert.ToDouble(decomposed[2].Replace(",", ".")),
												  (float)Convert.ToDouble(decomposed[3].Replace(",", ".")));

						vertices[vCount] = pos - offset;

						vCount++;


						break;
					#endregion
					#region Texture vertex
					case "vt":

						Vector2 tex = new Vector2((float)Convert.ToDouble(decomposed[1].Replace(",", ".")),
												  1f - (float)Convert.ToDouble(decomposed[2].Replace(",", ".")));

						textCoords[vtCount] = tex;

						vtCount++;

						break;
					#endregion
					#region Vertex normal
					case "vn":
						Vector3 nrm = new Vector3((float)Convert.ToDouble(decomposed[1].Replace(",", ".")),
												  (float)Convert.ToDouble(decomposed[2].Replace(",", ".")),
												  (float)Convert.ToDouble(decomposed[3].Replace(",", ".")));

						normals[vnCount] = nrm;

						vnCount++;

						break;
					#endregion
					#region Face
					case "f":
					case "F":

						int length = decomposed.Length - 1;
						int[] vertexIndices = new int[length];
						int[] normalIndices = new int[length];
						int[] textureIndices = new int[length];
						for(int i = 0; i < decomposed.Length - 1; i++)
						{
							string[] vertexString = decomposed[i + 1].Split(new string[] { "/" }, StringSplitOptions.None);
							vertexIndices[i] = Convert.ToInt32(vertexString[0]);
							textureIndices[i] = Convert.ToInt32(vertexString[1]);
							normalIndices[i] = Convert.ToInt32(vertexString[2]);
						}
						faces.Add(new Face(vertexIndices, textureIndices, normalIndices));



						break;
					#endregion
					#region Mtl library
					case "mtllib":

						//TODO: MTLLIB

						break;
					#endregion
					#region Mtl library reference
					case "usemtl":

						//TODO: USEMTL

						break;
					#endregion
				}
			}

			foreach(Face f in faces)
			{
				Vertex[] vertexArr = new Vertex[f.vIndices.Length];

				if(f.vIndices.Length == 3)
				{
					for(int i = 0; i < 3; i++)
					{
						vertexArr[i] = new Vertex(vertices[f.vIndices[i]], normals[f.vnIndices[i]], textCoords[f.vtIndices[i]]);
					}
				}
				else
				{
					Debugger.Break();
				}
				vOuput.AddRange(vertexArr);
				output.polygonList.Add(new Polygon(vertexArr));
			}
			output.vertexArray = vOuput.ToArray();
			Debug.WriteLine("Obj conversion complete: " + faces.Count + " faces were converted.");
			return output;
		}

		/// <summary>
		/// Converts the given .obj file to a Mesh object that supports VBO rendering
		/// </summary>
		/// <param name="inputFile">.obj file (not path)</param>
		/// <returns>Generated Mesh</returns>
		public static Mesh ConvertObjToVboMesh(string inputFile)
		{
			return ConvertObjToVboMesh(inputFile, Vector3.Zero);
		}

		/// <summary>
		/// Converts the given .obj file to an array of collision boxes. A box is created for every group in the .obj file.
		/// </summary>
		/// <param name="inputFile">.obj file (not path)</param>
		/// <returns>The generated array of collision boxes</returns>
		public static CollisionAABB[] ConvertObjToAABBarray(string inputFile)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			Vector3 max = new Vector3(2147000000, 2147000000, 2147000000); //2147000000 is ongeveer de maximale waarde van een int
			Vector3 min = new Vector3(-2147000000, -2147000000, -2147000000);

			string[] inputElements = inputFile.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

			int vCount = 1;
			int gCount = -1;
			int totalVertices = 0;
			int totalGroups = 0;
			foreach(string s in inputElements)
			{
				string[] decomposed = s.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

				if(decomposed[0] == "v" | decomposed[0] == "V")
				{
					totalVertices++;
				}
				else if(decomposed[0] == "g")
				{
					totalGroups++;
				}
			}

			Vector3[] vertices = new Vector3[totalVertices + 1];
			CollisionAABB[] output = new CollisionAABB[totalGroups];

			foreach(string s in inputElements)
			{
				string[] decomposed = s.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

				switch(decomposed[0])
				{
					case "#":
						//Obj comment
						break;
					case "g":
						//Group (AABB)
						gCount++;
						output[gCount] = new CollisionAABB(max, min);
						break;
					case "v":
					case "V":
						//Vertex
						vertices[vCount] = new Vector3((float)Convert.ToDouble(decomposed[1]),
													   (float)Convert.ToDouble(decomposed[2]),
													   (float)Convert.ToDouble(decomposed[3]));
						vCount++;
						break;
					case "f":
					case "F":
						//Face (AABB side)
						if(gCount >= 0)
						{ //Skip vertices before first group
							int length = decomposed.Length - 1;
							for(int i = 0; i < length - 1; i++)
							{ //For each vertex set (v, vt, vn)
								string[] vertexString = decomposed[i + 1].Split(new string[] { "/" }, StringSplitOptions.None);
								try
								{
									Vector3 currentV = vertices[Convert.ToInt32(vertexString[0])]; //First vertex of set (v)

									output[gCount].lb.X = Math.Min(output[gCount].lb.X, currentV.X);
									output[gCount].lb.Y = Math.Min(output[gCount].lb.Y, currentV.Y);
									output[gCount].lb.Z = Math.Min(output[gCount].lb.Z, currentV.Z);
									output[gCount].rt.X = Math.Max(output[gCount].rt.X, currentV.X);
									output[gCount].rt.Y = Math.Max(output[gCount].rt.Y, currentV.Y);
									output[gCount].rt.Z = Math.Max(output[gCount].rt.Z, currentV.Z);
								}
								catch(Exception e)
								{
									Debug.WriteLine("WARNING: obj conversion error with {" + s + "}: " + e.Message);
								}
							}
							//Face created
						}
						break;
				}
			}
			return output;
		}
	}
}