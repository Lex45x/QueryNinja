using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using QueryNinja.Core;

namespace QueryNinja.Sources.GraphQL.SchemaGeneration
{
    internal class ActionsScanner : IActionsScanner
    {
        private readonly IActionDescriptorCollectionProvider provider;

        public ActionsScanner(IActionDescriptorCollectionProvider provider)
        {
            this.provider = provider;
        }

        /// <inheritdoc />
        public IEnumerable<QueryRoot> GetQueryRoots()
        {
            foreach (var item in provider.ActionDescriptors.Items)
            {
                var actionDescriptor = item as ControllerActionDescriptor;

                var queryParameter = actionDescriptor?.MethodInfo.GetParameters()
                    .FirstOrDefault(parameter => typeof(IQuery).IsAssignableFrom(parameter.ParameterType));

                if (queryParameter == null)
                {
                    continue;
                }

                var modelType = queryParameter.ParameterType.IsGenericType
                    ? queryParameter.ParameterType.GetGenericArguments()[0]
                    : typeof(object);

                yield return new QueryRoot(modelType, actionDescriptor.ControllerTypeInfo, actionDescriptor.MethodInfo, queryParameter?.ParameterType);
            }
        }
    }
}