namespace Popcron.Settings
{
    public class LongHandler : TypeHandler<long, long>
    {
        public override string DeclaredTypeName => "long";

        public override long ConvertAwayFromReal(long real)
        {
            return real;
        }

        public override long ConvertIntoReal(long fake)
        {
            return fake;
        }

        public override string GetLine(long fake)
        {
            return fake.ToString();
        }
    }
}