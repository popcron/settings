using UnityEngine;

namespace Popcron.Settings
{
    public class Vector3Handler : TypeHandler<Vector3, float[]>
    {
        public override string DeclaredTypeName => "Vector3";

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
}