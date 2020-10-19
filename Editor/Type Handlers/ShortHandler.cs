namespace Popcron.Settings
{
    public class ShortHandler : TypeHandler<short, short>
    {
        public override string DeclaredTypeName => "short";

        public override short ConvertAwayFromReal(short real)
        {
            return real;
        }

        public override short ConvertIntoReal(short fake)
        {
            return fake;
        }

        public override string GetLine(short fake)
        {
            return fake.ToString();
        }
    }
}