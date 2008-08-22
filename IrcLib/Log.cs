using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Confabulation.Chat
{
	public static class Log
	{
		public static void WriteLine(string text)
		{
			TextWriter syncWriter = TextWriter.Synchronized(writer);
			syncWriter.WriteLine(text);
			syncWriter.Flush();
		}

		private static StreamWriter writer = new StreamWriter("Confabulation.log");
	}
}
