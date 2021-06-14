using Hazel;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace HardelAPI.Utility.Utils {
	public static class RpcUtils {
		private static readonly FloatRange XRange = new FloatRange(-40f, 40f);
		private static readonly FloatRange YRange = new FloatRange(-40f, 40f);

		public static void WriteVector2(this MessageWriter writer, Vector2 vec) {
			ushort value = (ushort) (XRange.ReverseLerp(vec.x) * 65535f);
			ushort value2 = (ushort) (YRange.ReverseLerp(vec.y) * 65535f);
			writer.Write(value);
			writer.Write(value2);
		}

		public static Vector2 ReadVector2(this MessageReader reader) {
			float v = (float) reader.ReadUInt16() / 65535f;
			float v2 = (float) reader.ReadUInt16() / 65535f;
			return new Vector2(XRange.Lerp(v), YRange.Lerp(v2));
		}

		public static void WriteListVector2(this MessageWriter writer, List<Vector2> vectors) {
			writer.Write(vectors.Count);
			foreach (var vector in vectors)
				writer.WriteVector2(vector);
		}

		public static List<Vector2> ReadListVector2(this MessageReader reader) {
			int size = reader.ReadInt32();
			List<Vector2> vectors = new List<Vector2>();
			
			for (int i = 0; i < size; i++) {
				Vector2 position = reader.ReadVector2();
				vectors.Add(position);
			}

			return vectors;
		}
	}
}
