using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoCatalogLib.Exchange
{
    public static class GeneralizedTypes
    {
        private static readonly IDictionary<Type, GeneralizedType> ConcreteToGeneralizedTypesLookup =
            new Dictionary<Type, GeneralizedType>();

        public static Type GetConcreteType(GeneralizedType type)
        {
            var isArray = type >= GeneralizedType.ArrayOfUnknown;
            var elementType = isArray ? (GeneralizedType)(type - GeneralizedType.ArrayOfUnknown) : type;
            Type concreteType;

            switch (elementType)
            {
                case GeneralizedType.Boolean:
                    concreteType = typeof(bool);
                    break;

                case GeneralizedType.DateTime:
                    concreteType = typeof(DateTime);
                    break;

                case GeneralizedType.Float:
                    concreteType = typeof(double);
                    break;

                case GeneralizedType.Integer:
                    concreteType = typeof(long);
                    break;

                case GeneralizedType.String:
                    concreteType = typeof(string);
                    break;

                case GeneralizedType.Unknown:
                    concreteType = typeof(object);
                    break;

                default:
                    throw new Exception(@"No target type defined yet");
            }

            return isArray ? concreteType.MakeArrayType() : concreteType;
        }

        public static GeneralizedType GetGeneralizedTypeValue(Type concreteType)
        {
            return ConcreteToGeneralizedTypesLookup[concreteType];
        }

        static GeneralizedTypes()
        {
            foreach (var value in Enum.GetValues(typeof(GeneralizedType)).Cast<GeneralizedType>())
                ConcreteToGeneralizedTypesLookup.Add(GetConcreteType(value), value);
        }
    }
}