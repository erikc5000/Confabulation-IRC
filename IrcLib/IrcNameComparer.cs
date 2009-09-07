using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public class IrcNameComparer : IEqualityComparer<string>
	{
		public IrcNameComparer(IrcServerProperties properties)
		{
			this.properties = properties;
		}

		public bool Equals(string x, string y)
		{
			if (x == null && y == null)
				return true;
			else if (x == null || y == null)
				return false;

			if (x.Length != y.Length)
				return false;

			IrcCaseMapping caseMapping = properties.CaseMapping;
			int maxLowerCase;

			switch (caseMapping)
			{
				case IrcCaseMapping.Ascii:
					maxLowerCase = 90;
					break;

				case IrcCaseMapping.StrictRfc1459:
					maxLowerCase = 93;
					break;

				case IrcCaseMapping.Rfc1459:
				default:
					maxLowerCase = 94;
					break;
			}

			for (int i = 0; i < x.Length; i++)
			{
				char a = x[i];
				char b = y[i];

				if (a >= 65 && a <= maxLowerCase)
					a += (char)32;

				if (b >= 65 && b <= maxLowerCase)
					b += (char)32;

				if (a != b)
					return false;
			}

			return true;
		}

		public int GetHashCode(string obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			return obj.GetHashCode();
		}

		IrcServerProperties properties;
	}
}
