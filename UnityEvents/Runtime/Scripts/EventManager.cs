using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEvents.Internal;

namespace UnityEvents
{
   public class EventManager : MonoBehaviour
   {
      private static UnityEventSystem[] _systems = new UnityEventSystem[3] {
            new UnityEventSystem(),
            new UnityEventSystem(),
            new UnityEventSystem()
        };

      /// <summary>
      /// Subscribe a listener to an event in the specific update tick.
      /// </summary>
      /// <typeparam name="T_Event"></typeparam>
      /// <param name="target"></param>
      /// <param name="eventCallback"></param>
      /// <param name="tick"></param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void Subscribe<T_Event>(EventTarget target, Action<T_Event> eventCallback, EventUpdateTick tick)
          where T_Event : unmanaged
      {
         _systems[(int)tick].Subscribe(target, eventCallback);
      }

      /// <summary>
      /// Subscribe a job that processes during an event in the specific update tick.
      /// </summary>
      /// <typeparam name="T_Job"></typeparam>
      /// <typeparam name="T_Event"></typeparam>
      /// <param name="target"></param>
      /// <param name="job"></param>
      /// <param name="onComplete"></param>
      /// <param name="tick"></param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void SubscribeWithJob<T_Job, T_Event>(
          EventTarget target,
          T_Job job,
          Action<T_Job> onComplete,
          EventUpdateTick tick)
          where T_Job : unmanaged, IJobForEvent<T_Event>
          where T_Event : unmanaged
      {
         _systems[(int)tick].SubscribeWithJob<T_Job, T_Event>(target, job, onComplete);
      }

      /// <summary>
      /// Unsubscribe a listener from an event in the specific update tick.
      /// </summary>
      /// <typeparam name="T_Event"></typeparam>
      /// <param name="target"></param>
      /// <param name="eventCallback"></param>
      /// <param name="tick"></param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void Unsubscribe<T_Event>(EventTarget target, Action<T_Event> eventCallback, EventUpdateTick tick)
          where T_Event : unmanaged
      {
         _systems[(int)tick].Unsubscribe(target, eventCallback);
      }

      /// <summary>
      /// Unsubscribe a job that processed during from an event in the specific update tick.
      /// </summary>
      /// <typeparam name="T_Job"></typeparam>
      /// <typeparam name="T_Event"></typeparam>
      /// <param name="target"></param>
      /// <param name="onComplete"></param>
      /// <param name="tick"></param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void UnsubscribeWithJob<T_Job, T_Event>(EventTarget target, Action<T_Job> onComplete,
          EventUpdateTick tick)
          where T_Job : unmanaged, IJobForEvent<T_Event>
          where T_Event : unmanaged
      {
         _systems[(int)tick].UnsubscribeWithJob<T_Job, T_Event>(target, onComplete);
      }

      /// <summary>
      /// Send an event to be processed in a specific update tick.
      /// </summary>
      /// <typeparam name="T_Event"></typeparam>
      /// <param name="target"></param>
      /// <param name="ev"></param>
      /// <param name="tick"></param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void SendEvent<T_Event>(EventTarget target, T_Event ev, EventUpdateTick tick)
          where T_Event : unmanaged
      {
         _systems[(int)tick].QueueEvent(target, ev);
      }

      /// <summary>
      /// Flushes all currently queued events NOW
      /// </summary>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void FlushAll()
      {
         for (int i = 0; i < _systems.Length; i++)
         {
            _systems[i].ProcessEvents();
         }
      }

      /// <summary>
      /// Reset all the event systems with all update types.
      /// </summary>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void ResetAll()
      {
         for (int i = 0; i < _systems.Length; i++)
         {
            _systems[i].Reset();
         }
      }

      /// <summary>
      /// Debug function to verify there are no lingering listeners. Throws an exception if there's a listener.
      /// </summary>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void VerifyNoSubscribersAll()
      {
         for (int i = 0; i < _systems.Length; i++)
         {
            _systems[i].VerifyNoSubscribers();
         }
      }

      /// <summary>
      /// Debug function to verify there are no lingering listeners. Logs each offending system instead of throwing an
      /// exception.
      /// </summary>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void VerifyNoSubscribersAllLog()
      {
         for (int i = 0; i < _systems.Length; i++)
         {
            _systems[i].VerifyNoSubscribersLog();
         }
      }

      private void FixedUpdate()
      {
         _systems[0].ProcessEvents();
      }

      private void Update()
      {
         _systems[1].ProcessEvents();
      }

      private void LateUpdate()
      {
         _systems[2].ProcessEvents();
      }

      private void OnDestroy()
      {
         ResetAll();

         for (int i = 0; i < _systems.Length; i++)
         {
            _systems[i].Dispose();
         }
      }

      /// <summary>
      /// Initialize the event manager.
      /// </summary>
      [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
      private static void Initialize()
      {
         GameObject em = new GameObject("[EVENT MANAGER DO]", typeof(EventManager));
         DontDestroyOnLoad(em);
      }
   }

   public enum EventUpdateTick
   {
      FixedUpdate,
      Update,
      LateUpdate
   }
}
