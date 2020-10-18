using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Popcron.Settings
{
    public abstract class SurrogateType
    {
        private static Dictionary<string, SurrogateType> realTypeToSurrogate = null;

        public abstract Type RealType { get; }
        public abstract Type FakeType { get; }
        public abstract object ConvertAwayFromReal(object real);
        public abstract object ConvertIntoReal(object fake);
        public abstract string GetLine(object fake);

        public static SurrogateType Find(string realType)
        {
            if (realTypeToSurrogate == null)
            {
                Type[] types = typeof(SurrogateType).Assembly.GetTypes();
                realTypeToSurrogate = new Dictionary<string, SurrogateType>();
                for (int i = 0; i < types.Length; i++)
                {
                    Type type = types[i];
                    if (!type.IsAbstract)
                    {
                        if (typeof(SurrogateType).IsAssignableFrom(type))
                        {
                            SurrogateType newSurrogate = (SurrogateType)Activator.CreateInstance(type);
                            realTypeToSurrogate[newSurrogate.RealType.FullName] = newSurrogate;
                        }
                    }
                }
            }

            //remove the array brackets
            if (realType.EndsWith("[]"))
            {
                realType = realType.Substring(0, realType.Length - 2);
            }

            if (realTypeToSurrogate.TryGetValue(realType, out SurrogateType handler))
            {
                return handler;
            }

            return null;
        }

        public static SurrogateType Find(Type realType)
        {
            if (realType == null)
            {
                return null;
            }

            if (realType.IsArray)
            {
                realType = realType.GetElementType();
            }

            return Find(realType.FullName);
        }
    }

    public abstract class SurrogateType<R, F> : SurrogateType
    {
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
                builder.Append(RealType.FullName);
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