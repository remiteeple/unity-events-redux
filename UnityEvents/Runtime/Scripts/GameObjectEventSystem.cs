﻿using System;
using UnityEngine;
using UnityEvents.Internal;

namespace UnityEvents
{
	public static class GameObjectEventSystem
	{
		public static void Subscribe<T_Event>(this GameObject gObj, Action<T_Event> callback) where T_Event : unmanaged
      {
			EventManager.Subscribe(EventTarget.CreateTarget(gObj), callback, EventUpdateTick.FixedUpdate);
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
			EventManager.Unsubscribe(EventTarget.CreateTarget(gObj), callback, EventUpdateTick.FixedUpdate);
		}

		public static void UnsubscribeWithJob<T_Job, T_Event>(this GameObject gObj, Action<T_Job> onComplete)
			where T_Job : unmanaged, IJobForEvent<T_Event>
			where T_Event : unmanaged
      {
			EventManager.UnsubscribeWithJob<T_Job, T_Event>(
				EventTarget.CreateTarget(gObj),
				onComplete,
				EventUpdateTick.FixedUpdate);
		}

		public static void SendEvent<T_Event>(this GameObject gObj, T_Event ev) where T_Event : unmanaged
      {
			EventManager.SendEvent(EventTarget.CreateTarget(gObj), ev, EventUpdateTick.FixedUpdate);
		}

		public static void SubscribeUI<T_Event>(this GameObject gObj, Action<T_Event> callback)
			where T_Event : unmanaged
      {
			EventManager.Subscribe(EventTarget.CreateTarget(gObj), callback, EventUpdateTick.LateUpdate);
		}

		public static void SubscribeUIWithJob<T_Job, T_Event>(this GameObject gObj, T_Job job, Action<T_Job> onComplete)
			where T_Job : unmanaged, IJobForEvent<T_Event>
			where T_Event : unmanaged
      {
			EventManager.SubscribeWithJob<T_Job, T_Event>(
				EventTarget.CreateTarget(gObj),
				job,
				onComplete,
				EventUpdateTick.LateUpdate);
		}

		public static void UnsubscribeUI<T_Event>(this GameObject gObj, Action<T_Event> callback)
			where T_Event : unmanaged
      {
			EventManager.Unsubscribe(
				EventTarget.CreateTarget(gObj),
				callback,
				EventUpdateTick.LateUpdate);
		}

		public static void UnsubscribeUIWithJob<T_Job, T_Event>(this GameObject gObj, Action<T_Job> onComplete)
			where T_Job : unmanaged, IJobForEvent<T_Event>
			where T_Event : unmanaged
      {
			EventManager.UnsubscribeWithJob<T_Job, T_Event>(
				EventTarget.CreateTarget(gObj),
				onComplete,
				EventUpdateTick.LateUpdate);
		}

		public static void SendEventUI<T_Event>(this GameObject gObj, T_Event ev) where T_Event : unmanaged
      {
			EventManager.SendEvent(EventTarget.CreateTarget(gObj), ev, EventUpdateTick.LateUpdate);
		}
	}
}