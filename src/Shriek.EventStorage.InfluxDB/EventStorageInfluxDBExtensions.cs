﻿using Microsoft.Extensions.DependencyInjection;
using Shriek.EventSourcing;
using Shriek.Storage;
using System;

namespace Shriek.EventStorage.InfluxDB
{
    public static class EventStorageInfluxDBExtensions
    {
        public static void UseInfluxDbEventStorage(this ShriekOptionBuilder builder, Action<InfluxDBOptions> optionAction)
        {
            var options = new InfluxDBOptions();
            optionAction(options);

            builder.Services.AddScoped(x => options);
            builder.Services.AddScoped<InfluxDbContext>();
            builder.Services.AddScoped<IEventStorageRepository, EventStorageRepository>();
            builder.Services.AddScoped<IMementoRepository, MementoRepository>();
            builder.Services.AddScoped<IEventStorage, DefalutEventStorage>();
        }
    }
}