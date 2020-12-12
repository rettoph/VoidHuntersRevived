using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Effects
{
    public static class EffectHelper
    {
		/// <summary>
		/// Open the recieved file and return the raw content in byte form.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Byte[] GetFileResourceBytes(String path)
		{
			Byte[] bytes;

			using (var stream = File.Open(path, FileMode.Open))
			{
				using (var ms = new MemoryStream())
				{
					stream.CopyTo(ms);
					bytes = ms.ToArray();
				}
			}

			return bytes;
		}
	}
}
