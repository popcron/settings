namespace Popcron.Settings
{
    public class CharHandler : TypeHandler<char, char>
    {
        public override string DeclaredTypeName => "char";

        public override char ConvertAwayFromReal(char real)
        {
            return real;
        }

        public override char ConvertIntoReal(char fake)
        {
            return fake;
        }

        public override string GetLine(char fake)
        {
            return fake.ToString();
        }
    }
}