namespace Popcron.Settings
{
    public class FloatHandler : TypeHandler<float, float>
    {
        public override string DeclaredTypeName => "float";

        public override float ConvertAwayFromReal(float real)
        {
            return real;
        }

        public override float ConvertIntoReal(float fake)
        {
            return fake;
        }

        public override string GetLine(float fake)
        {
            return fake.ToString() + "f";
        }
    }
}