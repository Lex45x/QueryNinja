﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QueryNinja.Core.Attributes;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Reflection;

namespace QueryNinja.Sources.GraphQL.Introspection
{
    internal static class FiltersInputObjectResolver
    {
        private static readonly Dictionary<Type, __Type> inputObjectsCache = new();

        public static bool TryGetInputObject(Type fieldType, out __Type inputObjects)
        {
            if (inputObjectsCache.TryGetValue(fieldType, out inputObjects))
            {
                return true;
            }

            var definedComponents = new List<Type>();

            foreach (var queryComponent in QueryNinjaExtensions.KnownQueryComponents)
            {
                var definitionAttribute = queryComponent.GetCustomAttribute<DefinitionAttribute>();

                if (definitionAttribute != null && definitionAttribute.IsDefinedFor(fieldType))
                {
                    definedComponents.Add(queryComponent);
                }
            }

            //todo: extend support beyond IDefaultFilter
            definedComponents = definedComponents.Where(type => type.GetInterface("IDefaultFilter`1") != null).ToList();

            if (definedComponents.Count == 0)
            {
                return false;
            }

            var enumValues = new List<__EnumValue>();

            inputObjects = __Type.InputObject($"Filter{fieldType.GetFriendlyTypeName()}", new List<__InputValue>
                {
                    //todo: support type expected by filter. CollectionFilter will have type different from FieldType
                    new("value", __Type.FromType(typeof(string)), defaultValue: null, "Value to be used with Operator"),
                    new("operator", __Type.Enum($"Operators{fieldType.GetFriendlyTypeName()}", enumValues), defaultValue: null,
                        "Available operators")
                },
                "QueryNinja autogenerated filter");

            foreach (var enumNames in definedComponents
                .Select(definedComponent => definedComponent
                    .GetInterface("IDefaultFilter`1")?
                    .GetGenericArguments()[0])
                .Select(@enum => @enum.GetEnumNames()
                    .Select(s => new __EnumValue(s, isDeprecated: false, deprecationReason: null))))
            {
                enumValues.AddRange(enumNames);
            }

            inputObjectsCache[fieldType] = inputObjects;
            return true;
        }
    }
}