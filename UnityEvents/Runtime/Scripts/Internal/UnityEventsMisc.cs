using System;
using System.Runtime.CompilerServices;

namespace UnityEvents.Internal
{
   public struct QueuedEvent<T_Event> where T_Event : struct
   {
      public EventTarget target;
      public T_Event ev;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public QueuedEvent(EventTarget target, T_Event ev)
      {
         this.target = target;
         this.ev = ev;
      }
   }

   public struct UnityEvent<T_Event> where T_Event : struct
   {
      public readonly T_Event ev;
      public readonly int subscriberIndex;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public UnityEvent(T_Event ev, int subscriberIndex)
      {
         this.ev = ev;
         this.subscriberIndex = subscriberIndex;
      }
   }

   public struct EntityCallbackId<T_Event> : IEquatable<EntityCallbackId<T_Event>>
       where T_Event : struct
   {
      public EventTarget target;
      public Action<T_Event> callback;

      private const int HASHCODE_MULTIPLIER = 397;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public EntityCallbackId(EventTarget target, Action<T_Event> callback)
      {
         this.target = target;
         this.callback = callback;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool Equals(EntityCallbackId<T_Event> other)
      {
         return target.Equals(other.target) && Equals(callback, other.callback);
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public override bool Equals(object obj)
      {
         return obj is EntityCallbackId<T_Event> other && Equals(other);
      }

      public override int GetHashCode()
      {
         unchecked
         {
            int callbackHashCode = callback?.GetHashCode() ?? 0;
            return (target.GetHashCode() * HASHCODE_MULTIPLIER) ^ callbackHashCode;
         }
      }
   }
}
