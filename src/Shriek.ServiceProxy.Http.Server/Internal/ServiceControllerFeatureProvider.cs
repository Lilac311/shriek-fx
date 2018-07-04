﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shriek.ServiceProxy.Http.Server.Internal
{
    internal class ServiceControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private const string ControllerTypeNameSuffix = "Controller";
        private readonly IEnumerable<Type> ServiceTypes;

        public ServiceControllerFeatureProvider(IEnumerable<Type> ServiceTypes)
        {
            this.ServiceTypes = ServiceTypes;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var type in AppDomain.CurrentDomain.GetAllTypes())
            {
                if (IsController(type) || ServiceTypes.Any(o => type.IsClass && o.IsAssignableFrom(type)) && !feature.Controllers.Contains(type))
                {
                    feature.Controllers.Add(type.GetTypeInfo());
                }
            }
        }

        protected bool IsController(Type typeInfo)
        {
            if (!typeInfo.IsClass)
            {
                return false;
            }

            if (typeInfo.IsAbstract)
            {
                return false;
            }

            // We only consider public top-level classes as controllers. IsPublic returns false for nested
            // classes, regardless of visibility modifiers
            if (!typeInfo.IsPublic)
            {
                return false;
            }

            if (typeInfo.ContainsGenericParameters)
            {
                return false;
            }

            if (typeInfo.IsDefined(typeof(NonControllerAttribute)))
            {
                return false;
            }

            if (!typeInfo.Name.EndsWith(ControllerTypeNameSuffix, StringComparison.OrdinalIgnoreCase) &&
                !typeInfo.IsDefined(typeof(ControllerAttribute)))
            {
                return false;
            }

            return true;
        }
    }
}