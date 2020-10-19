using UnityEngine;

namespace Popcron.Settings
{
    public class ColorHandler : TypeHandler<Color, float[]>
    {
        public override string DeclaredTypeName => "Color";

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
}