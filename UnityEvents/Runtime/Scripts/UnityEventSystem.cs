using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEvents.Internal;

namespace UnityEvents
{
   /// <summary>
   /// The event system that handles subscribers for all events, can queue all events, and will process the events
   /// when told to.
   /// </summary>
   public class UnityEventSystem : IDisposable
   {
      private IEventSystem[] _systems = Array.Empty<IEventSystem>();
      private Dictionary<Type, IEventSystem> _systemsCache = new Dictionary<Type, IEventSystem>();
      private Dictionary<Type, IEventSystem> _jobSystemsCache = new Dictionary<Type, IEventSystem>();
      private Dictionary<Type, List<IEventSystem>> _eventToJobSystems = new Dictionary<Type, List<IEventSystem>>();

      private Type _cachedSystemEventType;
      private IEventSystem _cacheSystem;

      private Type _cachedJobSystemsType;
      private List<IEventSystem> _cachedJobSystems;

      private Type _cachedJobSystemType;
      private IEventSystem _cachedJobSystem;

      /// <summary>
      /// Subscribe a listener to an event.
      /// </summary>
      /// <param name="target">The target to subscribe to.</param>
      /// <param name="eventCallback">The event callback</param>
      /// <typeparam name="T_Event">The event</typeparam>
      public void Subscribe<T_Event>(EventTarget target, Action<T_Event> eventCallback) where T_Event : unmanaged
      {
         var system = GetSystem<T_Event>();
         system.Subscribe(target, eventCallback);
      }

      /// <summary>
      /// Subscribe a job that processes during an event.
      /// </summary>
      /// <param name="target">The target to subscribe to.</param>
      /// <param name="job">The job, and starting data, to run when the event fires.</param>
      /// <param name="onComplete">The callback that is invoked when the job has finished.</param>
      /// <typeparam name="T_Job">The event type.</typeparam>
      /// <typeparam name="T_Event">The job type.</typeparam>
      public void SubscribeWithJob<T_Job, T_Event>(EventTarget target, T_Job job, Action<T_Job> onComplete)
         where T_Job : unmanaged, IJobForEvent<T_Event>
         where T_Event : unmanaged
      {
         var handler = GetJobSystem<T_Job, T_Event>();
         handler.Subscribe(target, job, onComplete);
      }

      /// <summary>
      /// Unsubscribe a listener from an event. 
      /// </summary>
      /// <param name="target">The target to unsubscribe from.</param>
      /// <param name="eventCallback">The event callback</param>
      /// <typeparam name="T_Event">The event</typeparam>
      public void Unsubscribe<T_Event>(EventTarget target, Action<T_Event> eventCallback) where T_Event : unmanaged
      {
         var system = GetSystem<T_Event>();
         system.Unsubscribe(target, eventCallback);
      }

      /// <summary>
      /// Unsubscribe a job that processed during from an event.
      /// </summary>
      /// <param name="target">The target to unsubscribe from.</param>
      /// <param name="onComplete">The callback that is invoked when the job has finished.</param>
      /// <typeparam name="T_Job">The job type.</typeparam>
      /// <typeparam name="T_Event">The event type.</typeparam>
      public void UnsubscribeWithJob<T_Job, T_Event>(EventTarget target, Action<T_Job> onComplete)
         where T_Job : unmanaged, IJobForEvent<T_Event>
         where T_Event : unmanaged
      {
         var handler = GetJobSystem<T_Job, T_Event>();
         handler.Unsubscribe(target, onComplete);
      }

      /// <summary>
      /// Queue an event.
      /// </summary>
      /// <param name="target">The target to queue an event with.</param>
      /// <param name="ev">The event to queue.</param>
      /// <typeparam name="T_Event">The event type.</typeparam>
      public void QueueEvent<T_Event>(EventTarget target, T_Event ev) where T_Event : unmanaged
      {
         var system = GetSystem<T_Event>();
         system.QueueEvent(target, ev);

         var list = GetJobSystemsForEvent<T_Event>();

         for (int i = 0; i < list.Count; i++)
         {
            ((IJobEventSystem<T_Event>)list[i]).QueueEvent(target, ev);
         }
      }
      /// <summary>
      /// Process all queued events.
      /// </summary>
      public void ProcessEvents()
      {
         for (int i = 0; i < _systems.Length; i++)
         {
            _systems[i].ProcessEvents();
         }
      }

      /// <summary>
      /// Reset all systems in the collection.
      /// </summary>
      public void Reset()
      {
         for (int i = 0; i < _systems.Length; i++)
         {
            _systems[i].Reset();
         }
      }

      /// <summary>
      /// Verify there are no subscribers in the systems.
      /// </summary>
      public void VerifyNoSubscribers()
      {
         for (int i = 0; i < _systems.Length; i++)
         {
            _systems[i].VerifyNoSubscribers();
         }
      }

      /// <summary>
      /// Verify there are no subscribers. Logs instead of throwing an exception.
      /// </summary>
      public void VerifyNoSubscribersLog()
      {
         for (int i = 0; i < _systems.Length; i++)
         {
            try
            {
               _systems[i].VerifyNoSubscribers();
            }
            catch (Exception e)
            {
               Debug.LogException(e);
               throw;
            }
         }
      }

      /// <summary>
      /// Disposes any resources held by the systems.
      /// </summary>
      public void Dispose()
      {
         for (int i = 0; i < _systems.Length; i++)
         {
            if (_systems[i] is IDisposable disposable)
            {
               disposable.Dispose();
            }
         }
      }

      /// <summary>
      /// Get the system for the event type.
      /// </summary>
      /// <typeparam name="T_Event"></typeparam>
      /// <returns></returns>
      private EventHandlerStandard<T_Event> GetSystem<T_Event>() where T_Event : unmanaged
      {
         var evType = typeof(T_Event);

         if (evType == _cachedSystemEventType)
         {
            return (EventHandlerStandard<T_Event>)_cacheSystem;
         }

         if (!_systemsCache.TryGetValue(evType, out var system))
         {
            system = new EventHandlerStandard<T_Event>();
            var systemsList = new List<IEventSystem>(_systems);
            systemsList.Add(system);
            _systems = systemsList.ToArray();
            _systemsCache[evType] = system;
         }

         _cachedSystemEventType = evType;
         _cacheSystem = system;

         return (EventHandlerStandard<T_Event>)system;
      }

      /// <summary>
      /// Get the job system for the event type.
      /// </summary>
      /// <typeparam name="T_Job"></typeparam>
      /// <typeparam name="T_Event"></typeparam>
      /// <returns></returns>
      private EventHandlerJob<T_Job, T_Event> GetJobSystem<T_Job, T_Event>()
         where T_Job : unmanaged, IJobForEvent<T_Event>
         where T_Event : unmanaged
      {
         var jobType = typeof(T_Job);
         var eventType = typeof(T_Event);

         if (jobType == _cachedJobSystemType)
         {
            return (EventHandlerJob<T_Job, T_Event>)_cachedJobSystem;
         }

         if (!_jobSystemsCache.TryGetValue(jobType, out var system))
         {
            system = new EventHandlerJob<T_Job, T_Event>();
            var systemsList = new List<IEventSystem>(_systems);
            systemsList.Add(system);
            _systems = systemsList.ToArray();
            _jobSystemsCache[jobType] = system;

            if (!_eventToJobSystems.TryGetValue(eventType, out var list))
            {
               list = new List<IEventSystem>();
               _eventToJobSystems[eventType] = list;
            }
            list.Add(system);
         }

         _cachedJobSystemType = jobType;
         _cachedJobSystem = system;

         return (EventHandlerJob<T_Job, T_Event>)system;
      }

      /// <summary>
      /// Get the job systems for the event type.
      /// </summary>
      /// <typeparam name="T_Event"></typeparam>
      /// <returns></returns>
      private List<IEventSystem> GetJobSystemsForEvent<T_Event>() where T_Event : unmanaged
      {
         var evType = typeof(T_Event);

         if (evType == _cachedJobSystemsType)
         {
            return _cachedJobSystems;
         }

         if (!_eventToJobSystems.TryGetValue(evType, out var list))
         {
            list = new List<IEventSystem>();
            _eventToJobSystems[evType] = list;
         }

         _cachedJobSystemsType = evType;
         _cachedJobSystems = list;

         return list;
      }
   }
}