using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEvents.Internal;

namespace UnityEvents
{
	public static class GameObjectEventSystem
	{
		private static readonly Dictionary<GameObject, EventTarget> eventTargetCache = new Dictionary<GameObject, EventTarget>();

		private static EventTarget GetOrCreateEventTarget(GameObject gObj)
		{
			if (!eventTargetCache.TryGetValue(gObj, out var eventTarget))
			{
				eventTarget = EventTarget.CreateTarget(gObj);
				eventTargetCache[gObj] = eventTarget;
			}
			return eventTarget;
		}

		public static void Subscribe<T_Event>(this GameObject gObj, Action<T_Event> callback) where T_Event : unmanaged
		{
			EventManager.Subscribe(GetOrCreateEventTarget(gObj), callback, EventUpdateTick.FixedUpdate);
		}

		public static void SubscribeWithJob<T_Job, T_Event>(this GameObject gObj, T_Job job, Action<T_Job> onComplete)
			where T_Job : unmanaged, IJobForEvent<T_Event>
			where T_Event : unmanaged
		{
			EventManager.SubscribeWithJob<T_Job, T_Event>(
				EventTarget.CreateTarget(gObj),
				job,
				onComplete,
				EventUpdateTick.FixedUpdate);
		}

		public static void Unsubscribe<T_Event>(this GameObject gObj, Action<T_Event> callback)
			where T_Event : unmanaged
		{
			EventManager.Unsubscribe(GetOrCreateEventTarget(gObj), callback, EventUpdateTick.FixedUpdate);
		}

		public static void UnsubscribeWithJob<T_Job, T_Event>(this GameObject gObj, Action<T_Job> onComplete)
			where T_Job : unmanaged, IJobForEvent<T_Event>
			where T_Event : unmanaged
		{
			EventManager.UnsubscribeWithJob<T_Job, T_Event>(
				GetOrCreateEventTarget(gObj),
				onComplete,
				EventUpdateTick.FixedUpdate);
		}

		public static void SendEvent<T_Event>(this GameObject gObj, T_Event ev) where T_Event : unmanaged
		{
			EventManager.SendEvent(GetOrCreateEventTarget(gObj), ev, EventUpdateTick.FixedUpdate);
		}

		public static void SubscribeUI<T_Event>(this GameObject gObj, Action<T_Event> callback)
			where T_Event : unmanaged
		{
			EventManager.Subscribe(GetOrCreateEventTarget(gObj), callback, EventUpdateTick.LateUpdate);
		}

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

		public static void UnsubscribeUI<T_Event>(this GameObject gObj, Action<T_Event> callback)
			where T_Event : unmanaged
		{
			EventManager.Unsubscribe(
				GetOrCreateEventTarget(gObj),
				callback,
				EventUpdateTick.LateUpdate);
		}

		public static void UnsubscribeUIWithJob<T_Job, T_Event>(this GameObject gObj, Action<T_Job> onComplete)
			where T_Job : unmanaged, IJobForEvent<T_Event>
			where T_Event : unmanaged
		{
			EventManager.UnsubscribeWithJob<T_Job, T_Event>(
				GetOrCreateEventTarget(gObj),
				onComplete,
				EventUpdateTick.LateUpdate);
		}

		public static void SendEventUI<T_Event>(this GameObject gObj, T_Event ev) where T_Event : unmanaged
		{
			EventManager.SendEvent(GetOrCreateEventTarget(gObj), ev, EventUpdateTick.LateUpdate);
		}
	}
}