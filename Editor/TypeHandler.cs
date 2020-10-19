using System;
using System.Collections.Generic;

namespace Popcron.Settings
{
    public abstract class TypeHandler
    {
        private static Dictionary<string, TypeHandler> realTypeToSurrogate = null;

        public abstract Type RealType { get; }
        public abstract Type FakeType { get; }
        public abstract object ConvertAwayFromReal(object real);
        public abstract object ConvertIntoReal(object fake);
        public abstract string GetLine(object fake);
        public abstract string GetTypeName(bool isCollection);

        public static TypeHandler Find(string realType)
        {
            if (realTypeToSurrogate == null)
            {
                Type[] types = typeof(TypeHandler).Assembly.GetTypes();
                realTypeToSurrogate = new Dictionary<string, TypeHandler>();
                for (int i = 0; i < types.Length; i++)
                {
                    Type type = types[i];
                    if (!type.IsAbstract)
                    {
                        if (typeof(TypeHandler).IsAssignableFrom(type))
                        {
                            TypeHandler newSurrogate = (TypeHandler)Activator.CreateInstance(type);
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

            if (realTypeToSurrogate.TryGetValue(realType, out TypeHandler handler))
            {
                return handler;
            }

            return null;
        }

        public static TypeHandler Find(Type realType)
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
}