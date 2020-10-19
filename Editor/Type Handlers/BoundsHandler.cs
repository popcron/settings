using UnityEngine;

namespace Popcron.Settings
{
    public class BoundsHandler : TypeHandler<Bounds, float[]>
    {
        public override string DeclaredTypeName => "Bounds";

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
}