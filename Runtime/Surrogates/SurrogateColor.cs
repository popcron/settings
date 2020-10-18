using UnityEngine;

namespace Popcron.Settings
{
    public class SurrogateColor : SurrogateType<Color, float[]>
    {
        public override float[] ConvertAwayFromReal(Color real)
        {
            return new float[] { real.r, real.g, real.b, real.a };
        }

        public override Color ConvertIntoReal(float[] fake)
        {
            return new Color(fake[0], fake[1], fake[2], fake[3]);
        }

        public override string GetLine(float[] fake)
        {
            return $"new Color({fake[0]}f, {fake[1]}f, {fake[2]}f, {fake[3]}f)";
        }
    }

    public class SurrogateRect : SurrogateType<Rect, float[]>
    {
        public override float[] ConvertAwayFromReal(Rect real)
        {
            return new float[] { real.x, real.y, real.width, real.height };
        }

        public override Rect ConvertIntoReal(float[] fake)
        {
            return new Rect(fake[0], fake[1], fake[2], fake[3]);
        }

        public override string GetLine(float[] fake)
        {
            return $"new Rect({fake[0]}f, {fake[1]}f, {fake[2]}f, {fake[3]}f)";
        }
    }

    public class SurrogateBounds : SurrogateType<Bounds, float[]>
    {
        public override float[] ConvertAwayFromReal(Bounds real)
        {
            return new float[] { real.center.x, real.center.y, real.center.z, real.size.x, real.size.y, real.size.z };
        }

        public override Bounds ConvertIntoReal(float[] fake)
        {
            return new Bounds(new Vector3(fake[0], fake[1], fake[2]), new Vector3(fake[3], fake[4], fake[5]));
        }

        public override string GetLine(float[] fake)
        {
            return $"new Bounds(new Vector3({fake[0]}f, {fake[1]}f, {fake[2]}f), new Vector3({fake[3]}f, {fake[4]}f, {fake[5]}f))";
        }
    }

    public class SurrogateVector2 : SurrogateType<Vector2, float[]>
    {
        public override float[] ConvertAwayFromReal(Vector2 real)
        {
            return new float[] { real.x, real.y };
        }

        public override Vector2 ConvertIntoReal(float[] fake)
        {
            return new Vector2(fake[0], fake[1]);
        }

        public override string GetLine(float[] fake)
        {
            return $"new Vector2({fake[0]}f, {fake[1]}f)";
        }
    }

    public class SurrogateVector3 : SurrogateType<Vector3, float[]>
    {
        public override float[] ConvertAwayFromReal(Vector3 real)
        {
            return new float[] { real.x, real.y, real.z };
        }

        public override Vector3 ConvertIntoReal(float[] fake)
        {
            return new Vector3(fake[0], fake[1], fake[2]);
        }

        public override string GetLine(float[] fake)
        {
            return $"new Vector3({fake[0]}f, {fake[1]}f, {fake[2]}f)";
        }
    }

    public class SurrogateVector4 : SurrogateType<Vector4, float[]>
    {
        public override float[] ConvertAwayFromReal(Vector4 real)
        {
            return new float[] { real.x, real.y, real.z, real.w };
        }

        public override Vector4 ConvertIntoReal(float[] fake)
        {
            return new Vector4(fake[0], fake[1], fake[2], fake[3]);
        }

        public override string GetLine(float[] fake)
        {
            return $"new Vector4({fake[0]}f, {fake[1]}f, {fake[2]}f, {fake[3]}f)";
        }
    }

    public class SurrogateQuaternion : SurrogateType<Quaternion, float[]>
    {
        public override float[] ConvertAwayFromReal(Quaternion real)
        {
            return new float[] { real.x, real.y, real.z, real.w };
        }

        public override Quaternion ConvertIntoReal(float[] fake)
        {
            return new Quaternion(fake[0], fake[1], fake[2], fake[3]);
        }

        public override string GetLine(float[] fake)
        {
            return $"new Quaternion({fake[0]}f, {fake[1]}f, {fake[2]}f, {fake[3]}f)";
        }
    }
}