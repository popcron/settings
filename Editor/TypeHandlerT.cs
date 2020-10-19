using System;
using System.Text;

namespace Popcron.Settings
{
    public abstract class TypeHandler<R, F> : TypeHandler
    {
        public sealed override string GetTypeName(bool isCollection)
        {
            string typeName = DeclaredTypeName;
            if (isCollection)
            {
                return $"List<{typeName}>";
            }
            else
            {
                return typeName;
            }
        }

        public virtual string DeclaredTypeName => RealType.FullName;

        public sealed override object ConvertAwayFromReal(object real)
        {
            if (real == null)
            {
                return null;
            }

            if (real is Array realArray)
            {
                Array fakeArray = Array.CreateInstance(FakeType, realArray.Length);
                for (int i = 0; i < realArray.Length; i++)
                {
                    object convertedElement = ConvertAwayFromReal((R)realArray.GetValue(i));
                    fakeArray.SetValue(convertedElement, i);
                }

                return fakeArray;
            }
            else
            {
                return ConvertAwayFromReal((R)real);
            }
        }

        public sealed override object ConvertIntoReal(object fake)
        {
            if (fake == null)
            {
                return null;
            }

            if (fake is Array fakeArray)
            {
                Array realArray = Array.CreateInstance(RealType, fakeArray.Length);
                for (int i = 0; i < fakeArray.Length; i++)
                {
                    object convertedElement = ConvertIntoReal((F)fakeArray.GetValue(i));
                    realArray.SetValue(convertedElement, i);
                }

                return realArray;
            }
            else
            {
                return ConvertIntoReal((F)fake);
            }
        }

        public sealed override string GetLine(object fake)
        {
            if (fake == null)
            {
                return "default";
            }

            if (fake is Array fakeArray)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("new List<");
                builder.Append(DeclaredTypeName);
                builder.AppendLine(">()");
                builder.AppendLine("    {");

                for (int i = 0; i < fakeArray.Length; i++)
                {
                    object fakeElement = fakeArray.GetValue(i);
                    builder.Append("        ");
                    builder.Append(GetLine((F)fakeElement));

                    if (i != fakeArray.Length)
                    {
                        builder.AppendLine(",");
                    }
                }

                builder.Append("    }");
                return builder.ToString();
            }
            else
            {
                return GetLine((F)fake);
            }
        }

        public abstract F ConvertAwayFromReal(R real);
        public abstract R ConvertIntoReal(F fake);
        public abstract string GetLine(F fake);
        public override Type RealType => typeof(R);
        public override Type FakeType => typeof(F);
    }
}