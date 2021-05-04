using System.Collections.Generic;
using System.Linq;
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
            return from actionDescriptor in provider.ActionDescriptors.Items.OfType<ControllerActionDescriptor>()
                
                let queryParameter = actionDescriptor.MethodInfo
                    .GetParameters()
                    .FirstOrDefault(parameter => !typeof(IQuery).IsAssignableFrom(parameter.ParameterType))
                
                where queryParameter != null
                
                let modelType = queryParameter.ParameterType.IsGenericType
                    ? queryParameter.ParameterType.GetGenericArguments()[0]
                    : typeof(object)

                select new QueryRoot(modelType,
                    actionDescriptor.ControllerTypeInfo,
                    actionDescriptor.MethodInfo,
                    queryParameter?.ParameterType);
        }
    }
}