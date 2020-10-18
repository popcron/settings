using UnityEngine;

namespace Popcron.Settings
{
    public class SupportedTypes : ScriptableObject
    {
        [SerializeField]
        private byte first;

        public byte byteValue;
        public byte[] byteArrayValue;

        public sbyte sbyteValue;
        public sbyte[] sbyteArrayValue;

        public short shortValue;
        public short[] shortArrayValue;

        public ushort ushortValue;
        public ushort[] ushortArrayValue;

        public int intValue;
        public int[] intArrayValue;

        public uint uintValue;
        public uint[] uintArrayValue;

        public long longValue;
        public long[] longArrayValue;

        public bool boolValue;
        public bool[] boolArrayValue;

        public string stringValue;
        public string[] stringListValue;

        public char charValue;
        public char[] charArrayValue;

        public float floatValue;
        public float[] floatArrayValue;

        public double doubleValue;
        public double[] doubleArrayValue;

        public Color colorValue;
        public Color[] colorArrayValue;

        public Rect rectValue;
        public Rect[] rectArrayValue;

        public Bounds boundsValue;
        public Bounds[] boundsArrayValue;

        public Vector2 vector2Value;
        public Vector2[] vector2ArrayValue;

        public Vector3 vector3Value;
        public Vector3[] vector3ArrayValue;

        public Vector4 vector4Value;
        public Vector4[] vector4ArrayValue;

        public Quaternion quaternionValue;
        public Quaternion[] quaternionArrayValue;
    }
}