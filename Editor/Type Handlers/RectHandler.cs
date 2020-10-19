using UnityEngine;

namespace Popcron.Settings
{
    public class RectHandler : TypeHandler<Rect, float[]>
    {
        public override string DeclaredTypeName => "Rect";

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
}