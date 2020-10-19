using UnityEngine;

namespace Popcron.Settings
{
    public class Vector4Handler : TypeHandler<Vector4, float[]>
    {
        public override string DeclaredTypeName => "Vector4";

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
}