using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confabulation.Chat
{
	public static class Utilities
	{
		/// <summary>
		/// Returns a string with a comma-separated list of the items provided.
		/// </summary>
		/// <param name="items">The items to generate the list from</param>
		/// <returns>A comma-separated list of items</returns>
		public static string GetCommaSeparatedList(IEnumerable<string> items)
		{
			StringBuilder combinedItems = new StringBuilder();

			foreach (string item in items)
			{
				combinedItems.Append(item);

				if (item != items.Last())
					combinedItems.Append(',');
			}

			return combinedItems.ToString();
		}

		/// <summary>
		/// Determine if a character is digit (a number from 0-9).
		/// </summary>
		/// <param name="b">The character</param>
		/// <returns>True if the character is a digit</returns>
		public static bool IsDigit(byte b)
		{
			if (b >= 0x30 && b <= 0x39)
				return true;

			return false;
		}

		/// <summary>
		/// Determine if a character is digit (a number from 0-9).
		/// </summary>
		/// <param name="c">The character</param>
		/// <returns>True if the character is a digit</returns>
		public static bool IsDigit(char c)
		{
			if (c >= 0x30 && c <= 0x39)
				return true;

			return false;
		}

		/// <summary>
		/// Determine if a character is letter (A-Z,a-z).
		/// </summary>
		/// <param name="b">The character</param>
		/// <returns>True if the character is a letter</returns>
		public static bool IsLetter(byte b)
		{
			if ((b >= 0x41 && b <= 0x5A) || (b >= 0x61 && b <= 0x7A))
				return true;

			return false;
		}

		/// <summary>
		/// Determine if a character is letter (A-Z,a-z).
		/// </summary>
		/// <param name="c">The character</param>
		/// <returns>True if the character is a letter</returns>
		public static bool IsLetter(char c)
		{
			if ((c >= 0x41 && c <= 0x5A) || (c >= 0x61 && c <= 0x7A))
				return true;

			return false;
		}

		/// <summary>
		/// Determine if a byte array contains a space character.
		/// </summary>
		/// <param name="array">A byte array</param>
		/// <returns>True if the byte array contains a space</returns>
		public static bool ContainsSpace(byte[] array)
		{
			foreach (byte b in array)
			{
				if (b == (byte)' ')
					return true;
			}

			return false;
		}
	}
}
