namespace Popcron.Settings
{
    public class IntHandler : TypeHandler<int, int>
    {
        public override string DeclaredTypeName => "int";

        public override int ConvertAwayFromReal(int real)
        {
            return real;
        }

        public override int ConvertIntoReal(int fake)
        {
            return fake;
        }

        public override string GetLine(int fake)
        {
            return fake.ToString();
        }
    }
}