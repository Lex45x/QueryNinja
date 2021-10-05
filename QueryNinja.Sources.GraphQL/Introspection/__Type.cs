// ReSharper disable InconsistentNaming as reserved '__' prefix for GraphQL internals
// https://spec.graphql.org/June2018/#sec-Introspection

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QueryNinja.Core;

namespace QueryNinja.Sources.GraphQL.Introspection
{
    internal class __Type : IEquatable<__Type>
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
            return new(name, __TypeKind.OBJECT, description, fields, interfaces: interfaces ?? new List<__Type>());
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

        private static readonly Dictionary<Type, __Type> TypeToTypeCache = new();

        private static readonly HashSet<__Type> TypesSet = new();
        public static IReadOnlySet<__Type> All => TypesSet;

        public static __Type FromType(Type source)
        {
            if (TypeToTypeCache.ContainsKey(source))
            {
                return TypeToTypeCache[source];
            }

            if (source.IsPrimitive || source == typeof(string))
            {
                return Scalar(source.Name);
            }

            if (source.IsEnum)
            {
                //todo: enable deprecation
                var values = source.GetEnumNames()
                    .Select(name => new __EnumValue(name, isDeprecated: false, deprecationReason: null))
                    .ToList();

                return Enum(source.Name, values);
            }

            var enumerableInterface = source.GetInterface("IEnumerable`1");

            if (enumerableInterface != null)
            {
                return List(FromType(enumerableInterface.GetGenericArguments()[0]));
            }

            var fields = new List<__Field>();

            var type = Object(source.Name, fields);
            TypeToTypeCache[source] = type;

            foreach (var member in source.GetMembers(BindingFlags.Instance | BindingFlags.Public))
            {
                List<__InputValue> arguments = null;

                Type fieldType;

                switch (member)
                {
                    case MethodInfo methodInfo:

                        if (methodInfo.IsSpecialName || methodInfo.DeclaringType == typeof(object))
                        {
                            continue;
                        }

                        fieldType = methodInfo.ReturnType;
                        arguments = (from parameter in methodInfo.GetParameters()
                                select new __InputValue(parameter.Name, FromType(parameter.ParameterType),
                                    parameter.DefaultValue?.ToString()))
                            .ToList();
                        break;
                    case PropertyInfo propertyInfo:
                        fieldType = propertyInfo.PropertyType;
                        break;
                    default:
                        continue;
                }
                
                var success = FiltersInputObjectResolver.TryGetInputObject(fieldType, out var inputObject);

                if (success)
                {
                    var list = List(inputObject);

                    var queryArgument = new __InputValue("filters", list, defaultValue: null);

                    arguments ??= new List<__InputValue>(capacity: 1);

                    arguments.Add(queryArgument);
                }

                //todo: enable deprecation
                var field = new __Field(member.Name, arguments, FromType(fieldType), false, null);

                fields.Add(field);
            }

            return type;
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

            if (Kind != __TypeKind.LIST && Kind != __TypeKind.NON_NULL)
            {
                TypesSet.Add(this);
            }
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

        /// <inheritdoc />
        public bool Equals(__Type other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Kind switch
            {
                __TypeKind.LIST => false,
                __TypeKind.NON_NULL => false,
                _ => Name == other.Name
            };
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((__Type) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine((int) Kind, Name);
        }
    }
}