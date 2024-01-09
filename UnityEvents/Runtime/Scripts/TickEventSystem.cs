using System;
using System.Runtime.CompilerServices;
using UnityEvents.Internal;

namespace UnityEvents
{
   /// <summary>
   /// Event System that runs in a given update tick (FixedUpdate, Update, or LateUpdate). Can be used to create
   /// custom event systems.
   /// </summary>x
   public struct TickEventSystem
   {
      private readonly EventUpdateTick _tick;
      private static readonly EventTarget _sharedTarget = EventTarget.CreateTarget(); // Shared target for all instances

      /// <summary>
      /// Create an tick based event system.
      /// </summary>
      /// <param name="updateTick">Which tick to run in.</param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public TickEventSystem(EventUpdateTick updateTick)
      {
         _tick = updateTick;
      }

      /// <summary>
      /// Subscribe a listener to the tick based event system.
      /// </summary>
      /// <param name="callback">The callback that's invoked when an event occurs.</param>
      /// <typeparam name="T_Event">The event type.</typeparam>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Subscribe<T_Event>(Action<T_Event> callback) where T_Event : unmanaged
      {
         EventManager.Subscribe(_sharedTarget, callback, _tick);
      }

      /// <summary>
      /// Subscribe a job to the tick based event system.
      /// </summary>
      /// <param name="job">The job that is processed when an event occurs.</param>
      /// <param name="onComplete">The callback that's invoked when the job is done.</param>
      /// <typeparam name="T_Job">The job type.</typeparam>
      /// <typeparam name="T_Event">The event type.</typeparam>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void SubscribeWithJob<T_Job, T_Event>(T_Job job, Action<T_Job> onComplete)
          where T_Job : unmanaged, IJobForEvent<T_Event>
          where T_Event : unmanaged
      {
         EventManager.SubscribeWithJob<T_Job, T_Event>(_sharedTarget, job, onComplete, _tick);
      }

      /// <summary>
      /// Unsubscribe a listener from the tick based event system.
      /// </summary>
      /// <param name="callback">The callback to unsubscribe.</param>
      /// <typeparam name="T_Event">The event type.</typeparam>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Unsubscribe<T_Event>(Action<T_Event> callback) where T_Event : unmanaged
      {
         EventManager.Unsubscribe(_sharedTarget, callback, _tick);
      }

      /// <summary>
      /// Unsubscribe a job from the tick based event system.
      /// </summary>
      /// <param name="onComplete">The on complete callback to unsubscribe</param>
      /// <typeparam name="T_Job">The job type to unsubscribe.</typeparam>
      /// <typeparam name="T_Event">The event type.</typeparam>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void UnsubscribeWithJob<T_Job, T_Event>(Action<T_Job> onComplete)
          where T_Job : unmanaged, IJobForEvent<T_Event>
          where T_Event : unmanaged
      {
         EventManager.UnsubscribeWithJob<T_Job, T_Event>(_sharedTarget, onComplete, _tick);
      }

      /// <summary>
      /// Send an event to the tick based event system.
      /// </summary>
      /// <param name="ev">The event to send.</param>
      /// <typeparam name="T_Event">The event type.</typeparam>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void SendEvent<T_Event>(T_Event ev) where T_Event : unmanaged
      {
         EventManager.SendEvent(_sharedTarget, ev, _tick);
      }
   }
}
