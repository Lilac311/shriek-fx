﻿using Shriek.Messages;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Shriek.Events
{
    public class InMemoryEventBus : IEventBus, IDisposable
    {
        private readonly IMessagePublisher messageProcessor;
        private readonly ConcurrentDictionary<string, ConcurrentQueue<Event>> eventQueueDict;
        private readonly ConcurrentDictionary<string, Task> taskDict;

        public InMemoryEventBus(IMessagePublisher messageProcessor)
        {
            this.messageProcessor = messageProcessor;
            eventQueueDict = new ConcurrentDictionary<string, ConcurrentQueue<Event>>();
            taskDict = new ConcurrentDictionary<string, Task>();
        }

        public void Dispose()
        {
            messageProcessor.Dispose();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Publish<TEvent>(TEvent @event) where TEvent : Event
        {
            if (@event == null) return;

            var eventQueue = eventQueueDict.GetOrAdd(@event.EventId, new ConcurrentQueue<Event>());
            eventQueue.Enqueue(@event);

            if (!taskDict.TryGetValue(@event.EventId, out Task task) || task.IsCompleted || task.IsCanceled || task.IsFaulted)
            {
                task?.Dispose();

                taskDict[@event.EventId] = Task.Run(() =>
                {
                    while (!eventQueue.IsEmpty && eventQueue.TryDequeue(out var evt))
                        messageProcessor.Send(evt);
                });
            }
        }
    }
}