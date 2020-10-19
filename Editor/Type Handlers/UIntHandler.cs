namespace Popcron.Settings
{
    public class UIntHandler : TypeHandler<uint, uint>
    {
        public override string DeclaredTypeName => "uint";

        public override uint ConvertAwayFromReal(uint real)
        {
            return real;
        }

        public override uint ConvertIntoReal(uint fake)
        {
            return fake;
        }

        public override string GetLine(uint fake)
        {
            return fake.ToString();
        }
    }
}