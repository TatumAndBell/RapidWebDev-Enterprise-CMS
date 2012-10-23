/***************************************************************************
	Copyright (C) 2010 Eunge (Email: eunge.liu@gmail.com, Legal Name: Jian Liu)

	This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <http://www.gnu.org/licenses/>.
 ***************************************************************************/

namespace Application.Framework.Common
{
	using System;
	using System.IO;
	using System.Text;
	using System.Collections.Generic;
	using System.Security.Cryptography;

	/// <summary>
	/// The encryption/decryption algorithm static class based on DES.
	/// </summary>
	public static class DesCryptor
	{
		private static byte[] key = ASCIIEncoding.ASCII.GetBytes("$*#^&#@~");
		private static byte[] iv = ASCIIEncoding.ASCII.GetBytes("eungeliu");

		/// <summary>
		/// Encrypt input string by DES algorithm.
		/// </summary>
		/// <param name="inputString">Input string is for encryption.</param>
		/// <returns>The encrypted string.</returns>
		public static string DESEncrypt(string inputString)
		{
			MemoryStream ms = null;
			CryptoStream cs = null;
			StreamWriter sw = null;

			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			try
			{
				ms = new MemoryStream();
				cs = new CryptoStream(ms, des.CreateEncryptor(key, iv), CryptoStreamMode.Write);
				sw = new StreamWriter(cs);
				sw.Write(inputString);
				sw.Flush();
				cs.FlushFinalBlock();
				return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
			}
			finally
			{
				if (sw != null)
				{
					sw.Close();
				}

				if (cs != null)
				{
					cs.Close();
				}

				if (ms != null)
				{
					ms.Close();
				}
			}
		}

		/// <summary>
		/// Decrypt input string by DES algorithm.
		/// </summary>
		/// <param name="inputString">Input string is for decryption.</param>
		/// <returns>Decrypted string.</returns>
		public static string DESDecrypt(string inputString)
		{
			MemoryStream ms = null;
			CryptoStream cs = null;
			StreamReader sr = null;

			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			try
			{
				ms = new MemoryStream(Convert.FromBase64String(inputString));
				cs = new CryptoStream(ms, des.CreateDecryptor(key, iv), CryptoStreamMode.Read);
				sr = new StreamReader(cs);
				return sr.ReadToEnd();
			}
			finally
			{
				if (sr != null)
				{
					sr.Close();
				}

				if (cs != null)
				{
					cs.Close();
				}

				if (ms != null)
				{
					ms.Close();
				}
			}
		}
	}
}

