using Hazel;
using System.Collections.Generic;
using UnityEngine;

namespace HardelAPI.Utility.Utils {
	public static class RpcUtils {
		private static readonly FloatRange XRange = new FloatRange(-40f, 40f);
		private static readonly FloatRange YRange = new FloatRange(-40f, 40f);
		private static readonly FloatRange ZRange = new FloatRange(-40f, 40f);

		// Vector2
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

		// Vector3
		public static void WriteVector3(this MessageWriter writer, Vector3 vec) {
			ushort value = (ushort) (XRange.ReverseLerp(vec.x) * 65535f);
			ushort value2 = (ushort) (YRange.ReverseLerp(vec.y) * 65535f);
			ushort value3 = (ushort) (ZRange.ReverseLerp(vec.z) * 65535f);
			writer.Write(value);
			writer.Write(value2);
			writer.Write(value3);
		}

		public static Vector3 ReadVector3(this MessageReader reader) {
			float v = (float) reader.ReadUInt16() / 65535f;
			float v2 = (float) reader.ReadUInt16() / 65535f;
			float v3 = (float) reader.ReadUInt16() / 65535f;
			return new Vector3(XRange.Lerp(v), YRange.Lerp(v2), ZRange.Lerp(v3));
		}

		public static void WriteListVector3(this MessageWriter writer, List<Vector3> vectors) {
			writer.Write(vectors.Count);
			foreach (var vector in vectors)
				writer.WriteVector3(vector);
		}

		public static List<Vector3> ReadListVector3(this MessageReader reader) {
			int size = reader.ReadInt32();
			List<Vector3> vectors = new List<Vector3>();

			for (int i = 0; i < size; i++) {
				Vector3 position = reader.ReadVector3();
				vectors.Add(position);
			}

			return vectors;
		}
	}
}
