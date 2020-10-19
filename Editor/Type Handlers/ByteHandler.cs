namespace Popcron.Settings
{
    public class ByteHandler : TypeHandler<byte, byte>
    {
        public override string DeclaredTypeName => "byte";

        public override byte ConvertAwayFromReal(byte real)
        {
            return real;
        }

        public override byte ConvertIntoReal(byte fake)
        {
            return fake;
        }

        public override string GetLine(byte fake)
        {
            return fake.ToString();
        }
    }
}