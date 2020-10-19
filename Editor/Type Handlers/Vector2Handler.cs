using UnityEngine;

namespace Popcron.Settings
{
    public class Vector2Handler : TypeHandler<Vector2, float[]>
    {
        public override string DeclaredTypeName => "Vector2";

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
}