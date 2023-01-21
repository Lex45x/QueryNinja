using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using QueryNinja.Core;
using QueryNinja.Sources.GraphQL.Controllers;

namespace QueryNinja.Sources.GraphQL.SchemaGeneration
{
    internal class ActionsScanner : IActionsScanner
    {
        private readonly IEnumerable<Func<IQueryNinjaController>> controllerFactories;

        public ActionsScanner(IEnumerable<Func<IQueryNinjaController>> controllerFactories)
        {
            this.controllerFactories = controllerFactories;
        }

        /// <inheritdoc />
        public IEnumerable<QueryRoot> GetQueryRoots()
        {

            foreach (var factory in controllerFactories)
            {
                factory.ge
            }




            foreach (var factory in controllerFactories)
            {
                var queryParameter = actionDescriptor?.MethodInfo.GetParameters()
                    .FirstOrDefault(parameter => typeof(IQuery).IsAssignableFrom(parameter.ParameterType));

                if (queryParameter == null)
                {
                    continue;
                }

                var modelType = queryParameter.ParameterType.IsGenericType
                    ? queryParameter.ParameterType.GetGenericArguments()[0]
                    : typeof(object);

                yield return new QueryRoot(modelType, actionDescriptor.ControllerTypeInfo, actionDescriptor.MethodInfo,
                    queryParameter?.ParameterType);
            }
        }
    }
}