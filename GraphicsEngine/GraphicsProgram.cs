using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using GraphicsLibrary.Core;

namespace GraphicsLibrary
{
	public class GraphicsProgram:IDisposable
	{
		public bool enableLogging;
		public string logFilename;
		public string[] programArguments;

		public Config config;

		public GraphicsProgram(string[] arguments, bool enableLogging, string logFilename)
		{

			RenderWindow.Instance.program = this;
			this.enableLogging = enableLogging;
			this.logFilename = logFilename;
			programArguments = arguments;

			Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
			if(enableLogging)
			{
				try
				{
					StreamWriter streamWriter = new StreamWriter(new FileStream(logFilename, FileMode.Append)) { AutoFlush = true };
					Debug.Listeners.Add(new TextWriterTraceListener(streamWriter));
				}
				catch(Exception exception)
				{
					Debug.WriteLine("Failed to create log file. Make sure you have admin rights and close all processes using this file: {0}", exception.Message);
				}
			}
			Debug.WriteLine("---------------");
			Debug.WriteLine("A game by Fons van der Plas");
			Debug.WriteLine("---------------");
			Debug.WriteLine("Copyright 2013 Fons van der Plas");
			Debug.WriteLine("Fons van der Plas, fonsvdplas@gmail.com");
			Debug.WriteLine("---------------");
			Debug.WriteLine("Program launched at " + DateTime.Now);
			Debug.Write("Received arguments: ");
			foreach(string s in programArguments)
			{
				Debug.Write(s + ", ");
			}
			Debug.WriteLine("");
		}

		public virtual void LoadResources()
		{


		}

		public virtual void InitGame()
		{

		}

		public virtual void Update(float timeSinceLastUpdate)
		{

		}

		public virtual void Resize(Rectangle newDimensions)
		{

		}

		public void Run()
		{
			RenderWindow.Instance.Run();
		}

		public virtual void Dispose()
		{
			RenderWindow.Instance.Dispose();
			//throw new NotImplementedException(); //TODO
		}
	}
}