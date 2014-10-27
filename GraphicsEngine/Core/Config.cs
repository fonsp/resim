using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

namespace GraphicsLibrary.Core
{
	public class Config
	{
		private readonly string fileName;
		public Config(string fileName)
		{
			this.fileName = fileName;
			Reload();
		}

		private Dictionary<string, bool> bools = new Dictionary<string, bool>();
		private Dictionary<string, string> strings = new Dictionary<string, string>();
		private Dictionary<string, int> ints = new Dictionary<string, int>();
		private Dictionary<string, double> doubles = new Dictionary<string, double>();

		public bool GetBool(string name)
		{
			if(!bools.ContainsKey(name))
			{
				throw new KeyNotFoundException();
			}
			return bools[name];
		}

		public string GetString(string name)
		{
			if(!strings.ContainsKey(name))
			{
				throw new KeyNotFoundException();
			}
			return strings[name];
		}

		public int GetInt(string name)
		{
			if(!ints.ContainsKey(name))
			{
				throw new KeyNotFoundException();
			}
			return ints[name];
		}

		public double GetDouble(string name)
		{
			if(!doubles.ContainsKey(name))
			{
				throw new KeyNotFoundException();
			}
			return doubles[name];
		}

		public void Reload()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			bools = new Dictionary<string, bool>();
			strings = new Dictionary<string, string>();
			ints = new Dictionary<string, int>();
			doubles = new Dictionary<string, double>();

			string file = File.ReadAllText(fileName);

			string[] fileSplit0 = file.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

			foreach(string line in fileSplit0)
			{
				string[] fileSplit1 = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				bool error = false;
				switch(fileSplit1[0])
				{
					case ";":
						//Comment
						break;
					case "":
					case null:
						//Empty line
						break;
					case "bool":
						//Boolean
						try
						{
							bools.Add(fileSplit1[1], Convert.ToBoolean(fileSplit1[2]));
						}
						catch(Exception)
						{
							error = true;
						}
						break;
					case "string":
						//String
						try
						{
							strings.Add(fileSplit1[1], fileSplit1[2]);
						}
						catch(Exception)
						{
							error = true;
						}
						break;
					case "int":
						try
						{
							ints.Add(fileSplit1[1], Convert.ToInt32(fileSplit1[2]));
						}
						catch(Exception)
						{
							error = true;
						}
						break;
					case "double":
						try
						{
							doubles.Add(fileSplit1[1], Convert.ToDouble(fileSplit1[2]));
						}
						catch(Exception)
						{
							error = true;
						}
						break;
					default:
						error = true;
						break;
				}
				if(error)
				{
					Debug.Write("Error in config file " + fileName + " at line: " + line);
				}
			}
		}
	}
}