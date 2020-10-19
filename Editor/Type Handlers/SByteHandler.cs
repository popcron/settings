namespace Popcron.Settings
{
    public class SByteHandler : TypeHandler<sbyte, sbyte>
    {
        public override string DeclaredTypeName => "sbyte";

        public override sbyte ConvertAwayFromReal(sbyte real)
        {
            return real;
        }

        public override sbyte ConvertIntoReal(sbyte fake)
        {
            return fake;
        }

        public override string GetLine(sbyte fake)
        {
            return fake.ToString();
        }
    }
}