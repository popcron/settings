using UnityEngine;

namespace Popcron.Settings
{
    public class QuaternionHandler : TypeHandler<Quaternion, float[]>
    {
        public override string DeclaredTypeName => "Quaternion";

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