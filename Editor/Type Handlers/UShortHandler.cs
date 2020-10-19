namespace Popcron.Settings
{
    public class UShortHandler : TypeHandler<ushort, ushort>
    {
        public override string DeclaredTypeName => "ushort";

        public override ushort ConvertAwayFromReal(ushort real)
        {
            return real;
        }

        public override ushort ConvertIntoReal(ushort fake)
        {
            return fake;
        }

        public override string GetLine(ushort fake)
        {
            return fake.ToString();
        }
    }
}