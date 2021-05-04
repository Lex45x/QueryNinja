// ReSharper disable InconsistentNaming as reserved '__' prefix for GraphQL internals
// https://spec.graphql.org/June2018/#sec-Introspection

using System.Collections.Generic;
using System.Linq;

namespace QueryNinja.Sources.GraphQL.Introspection
{
    internal class __Type
    {
        public static __Type Scalar(string name, string description = null)
        {
            return new(name, __TypeKind.SCALAR, description);
        }

        public static __Type Object(string name,
            IReadOnlyList<__Field> fields,
            IReadOnlyList<__Type> interfaces = null,
            string description = null)
        {
            return new(name, __TypeKind.OBJECT, description, fields, interfaces: interfaces);
        }

        public static __Type Union(string name, IReadOnlyList<__Type> possibleTypes, string description = null)
        {
            return new(name, __TypeKind.UNION, description, possibleTypes: possibleTypes);
        }

        public static __Type Interface(string name,
            IReadOnlyList<__Field> fields,
            IReadOnlyList<__Type> possibleTypes,
            string description = null)
        {
            return new(name, __TypeKind.INTERFACE, description, fields, possibleTypes: possibleTypes);
        }

        public static __Type Enum(string name,
            IReadOnlyList<__EnumValue> enumValues,
            string description = null)
        {
            return new(name, __TypeKind.ENUM, description, enumValues: enumValues);
        }

        public static __Type InputObject(string name,
            IReadOnlyList<__InputValue> inputFields = null,
            string description = null)
        {
            return new(name, __TypeKind.INPUT_OBJECT, description, inputFields: inputFields);
        }

        public static __Type List(__Type ofType)
        {
            return new(name: null, __TypeKind.LIST, ofType: ofType);
        }

        public static __Type NonNull(__Type ofType)
        {
            return new(name: null, __TypeKind.NON_NULL, ofType: ofType);
        }

        private __Type(string name,
            __TypeKind kind,
            string description = null,
            IReadOnlyList<__Field> fields = null,
            IReadOnlyList<__EnumValue> enumValues = null,
            IReadOnlyList<__Type> interfaces = null,
            IReadOnlyList<__Type> possibleTypes = null,
            IReadOnlyList<__InputValue> inputFields = null,
            __Type ofType = null)
        {
            this.fields = fields;
            this.enumValues = enumValues;
            Kind = kind;
            Name = name;
            Description = description;
            Interfaces = interfaces;
            PossibleTypes = possibleTypes;
            InputFields = inputFields;
            OfType = ofType;
        }

        public __TypeKind Kind { get; }
        public string Name { get; }
        public string Description { get; }

        private readonly IReadOnlyList<__Field> fields;
        private readonly IReadOnlyList<__EnumValue> enumValues;

        /// <summary>
        /// Works for Objects and Interfaces
        /// </summary>
        /// <param name="includeDeprecated"></param>
        /// <returns></returns>
        public IReadOnlyList<__Field> Fields(bool includeDeprecated = false)
        {
            return includeDeprecated
                ? fields
                : fields?.Where(field => field.IsDeprecated == false).ToList();
        }

        /// <summary>
        /// Works for Objects only
        /// </summary>
        public IReadOnlyList<__Type> Interfaces { get; }

        /// <summary>
        /// Works for interfaces and Unions
        /// </summary>
        public IReadOnlyList<__Type> PossibleTypes { get; }

        /// <summary>
        /// Works on Enums only
        /// </summary>
        /// <param name="includeDeprecated"></param>
        /// <returns></returns>
        public IReadOnlyList<__EnumValue> EnumValues(bool includeDeprecated = false)
        {
            return includeDeprecated
                ? enumValues
                : enumValues?.Where(field => field.IsDeprecated == false).ToList();
        }

        /// <summary>
        /// Works on InputObjects only
        /// </summary>
        public IReadOnlyList<__InputValue> InputFields { get; }

        /// <summary>
        /// Works for NonNull and Lists only
        /// </summary>
        public __Type OfType { get; }
    }
}