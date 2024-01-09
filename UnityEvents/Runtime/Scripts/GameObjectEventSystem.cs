using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEvents.Internal;

namespace UnityEvents
{
	public static class GameObjectEventSystem
	{
		/// <summary>
		/// Cache of event targets for GameObjects.
		/// </summary>
      private static readonly Dictionary<GameObject, EventTarget> eventTargetCache = new Dictionary<GameObject, EventTarget>();

		/// <summary>
		/// Get or create an event target for a GameObject.
		/// </summary>
		/// <param name="gObj"></param>
		/// <returns></returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      private static EventTarget GetOrCreateEventTarget(GameObject gObj)
      {
         if (!eventTargetCache.TryGetValue(gObj, out var target))
         {
            target = EventTarget.CreateTarget(gObj);
            eventTargetCache[gObj] = target;
         }
         return target;
      }

		/// <summary>
		/// Subscribe a listener to an event in the specific update tick.
		/// </summary>
		/// <typeparam name="T_Event"></typeparam>
		/// <param name="gObj"></param>
		/// <param name="callback"></param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void Subscribe<T_Event>(this GameObject gObj, Action<T_Event> callback) where T_Event : unmanaged
      {
			EventManager.Subscribe(GetOrCreateEventTarget(gObj), callback, EventUpdateTick.FixedUpdate);
		}

		/// <summary>
		/// Subscribe a job that processes during an event in the specific update tick.
		/// </summary>
		/// <typeparam name="T_Job"></typeparam>
		/// <typeparam name="T_Event"></typeparam>
		/// <param name="gObj"></param>
		/// <param name="job"></param>
		/// <param name="onComplete"></param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void SubscribeWithJob<T_Job, T_Event>(this GameObject gObj, T_Job job, Action<T_Job> onComplete)
			where T_Job : unmanaged, IJobForEvent<T_Event>
			where T_Event : unmanaged
      {
			EventManager.SubscribeWithJob<T_Job, T_Event>(
            GetOrCreateEventTarget(gObj),
            job,
				onComplete,
				EventUpdateTick.FixedUpdate);
		}

		/// <summary>
		/// Unsubscribe a listener from an event in the specific update tick.
		/// </summary>
		/// <typeparam name="T_Event"></typeparam>
		/// <param name="gObj"></param>
		/// <param name="callback"></param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void Unsubscribe<T_Event>(this GameObject gObj, Action<T_Event> callback)
			where T_Event : unmanaged
      {
			EventManager.Unsubscribe(GetOrCreateEventTarget(gObj), callback, EventUpdateTick.FixedUpdate);
		}

		/// <summary>
		/// Unsubscribe a job that processed during from an event in the specific update tick.
		/// </summary>
		/// <typeparam name="T_Job"></typeparam>
		/// <typeparam name="T_Event"></typeparam>
		/// <param name="gObj"></param>
		/// <param name="onComplete"></param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void UnsubscribeWithJob<T_Job, T_Event>(this GameObject gObj, Action<T_Job> onComplete)
			where T_Job : unmanaged, IJobForEvent<T_Event>
			where T_Event : unmanaged
      {
			EventManager.UnsubscribeWithJob<T_Job, T_Event>(
            GetOrCreateEventTarget(gObj),
            onComplete,
				EventUpdateTick.FixedUpdate);
		}

		/// <summary>
		/// Send an event to be processed in a specific update tick.
		/// </summary>
		/// <typeparam name="T_Event"></typeparam>
		/// <param name="gObj"></param>
		/// <param name="ev"></param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void SendEvent<T_Event>(this GameObject gObj, T_Event ev) where T_Event : unmanaged
      {
			EventManager.SendEvent(GetOrCreateEventTarget(gObj), ev, EventUpdateTick.FixedUpdate);
		}

		/// <summary>
		/// Subscribe a listener to an event in the specific update tick.
		/// </summary>
		/// <typeparam name="T_Event"></typeparam>
		/// <param name="gObj"></param>
		/// <param name="callback"></param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void SubscribeUI<T_Event>(this GameObject gObj, Action<T_Event> callback)	
			where T_Event : unmanaged
      {
			EventManager.Subscribe(GetOrCreateEventTarget(gObj), callback, EventUpdateTick.LateUpdate);
		}

		/// <summary>
		/// Subscribe a job that processes during an event in the specific update tick.
		/// </summary>
		/// <typeparam name="T_Job"></typeparam>
		/// <typeparam name="T_Event"></typeparam>
		/// <param name="gObj"></param>
		/// <param name="job"></param>
		/// <param name="onComplete"></param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void SubscribeUIWithJob<T_Job, T_Event>(this GameObject gObj, T_Job job, Action<T_Job> onComplete)
			where T_Job : unmanaged, IJobForEvent<T_Event>
			where T_Event : unmanaged
      {
			EventManager.SubscribeWithJob<T_Job, T_Event>(
            GetOrCreateEventTarget(gObj),
				job,
				onComplete,
				EventUpdateTick.LateUpdate);
		}

		/// <summary>
		/// Unsubscribe a listener from an event in the specific update tick.
		/// </summary>
		/// <typeparam name="T_Event"></typeparam>
		/// <param name="gObj"></param>
		/// <param name="callback"></param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void UnsubscribeUI<T_Event>(this GameObject gObj, Action<T_Event> callback)
			where T_Event : unmanaged
      {
			EventManager.Unsubscribe(
            GetOrCreateEventTarget(gObj),
            callback,
				EventUpdateTick.LateUpdate);
		}

		/// <summary>
		/// Unsubscribe a job that processed during from an event in the specific update tick.
		/// </summary>
		/// <typeparam name="T_Job"></typeparam>
		/// <typeparam name="T_Event"></typeparam>
		/// <param name="gObj"></param>
		/// <param name="onComplete"></param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void UnsubscribeUIWithJob<T_Job, T_Event>(this GameObject gObj, Action<T_Job> onComplete)
			where T_Job : unmanaged, IJobForEvent<T_Event>
			where T_Event : unmanaged
      {
			EventManager.UnsubscribeWithJob<T_Job, T_Event>(
            GetOrCreateEventTarget(gObj),
            onComplete,
				EventUpdateTick.LateUpdate);
		}

		/// <summary>
		/// Send an event to be processed in a specific update tick.
		/// </summary>
		/// <typeparam name="T_Event"></typeparam>
		/// <param name="gObj"></param>
		/// <param name="ev"></param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SendEventUI<T_Event>(this GameObject gObj, T_Event ev) where T_Event : unmanaged
      {
			EventManager.SendEvent(GetOrCreateEventTarget(gObj), ev, EventUpdateTick.LateUpdate);
		}
	}
}