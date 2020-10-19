namespace Popcron.Settings
{
    public class StringHandler : TypeHandler<string, string>
    {
        public override string DeclaredTypeName => "string";

        public override string ConvertAwayFromReal(string real)
        {
            return real;
        }

        public override string ConvertIntoReal(string fake)
        {
            return fake;
        }

        public override string GetLine(string fake)
        {
            return $"\"{fake}\"";
        }
    }
}