namespace Popcron.Settings
{
    public class BooleanHandler : TypeHandler<bool, bool>
    {
        public override string DeclaredTypeName => "bool";

        public override bool ConvertAwayFromReal(bool real)
        {
            return real;
        }

        public override bool ConvertIntoReal(bool fake)
        {
            return fake;
        }

        public override string GetLine(bool fake)
        {
            return fake.ToString().ToLower();
        }
    }
}