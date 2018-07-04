﻿using System;
using System.Net;
using Butterfly.Client.AspNetCore;
using Butterfly.Client.Tracing;
using Butterfly.OpenTracing;
using Microsoft.Extensions.Options;

namespace Shriek.ServiceProxy.Http.Tracer.Butterfly
{
    public class ConsoleServiceTracerProvider : IServiceTracerProvider
    {
        private readonly ITracer _tracer;
        private readonly ButterflyOptions _options;

        public ConsoleServiceTracerProvider(ITracer tracer, IOptions<ButterflyOptions> options)
        {
            _tracer = tracer;
            _options = options.Value;
        }

        public IServiceTracer GetServiceTracer()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.UserDomainName;
            return new ServiceTracer(_tracer, _options.Service, environmentName, Environment.UserName, Dns.GetHostName());
        }
    }
}