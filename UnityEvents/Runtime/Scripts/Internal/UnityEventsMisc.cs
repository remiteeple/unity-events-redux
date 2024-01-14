using System;

namespace UnityEvents.Internal
{
   /// <summary>
   /// A struct that represents a queued event.
   /// </summary>
   /// <typeparam name="T_Event"></typeparam>
   public struct QueuedEvent<T_Event> where T_Event : struct
   {
      public readonly T_Event ev; // Moved T_Event first due to potential size
      public readonly EventTarget target;

      public QueuedEvent(EventTarget target, T_Event ev)
      {
         this.ev = ev;
         this.target = target;
      }
   }

   /// <summary>
   /// A struct that represents an event.
   /// </summary>
   /// <typeparam name="T_Event"></typeparam>
   public struct UnityEvent<T_Event> where T_Event : struct
   {
      public readonly T_Event ev; // T_Event first due to potential size
      public readonly int subscriberIndex; // Integer field follows

      public UnityEvent(T_Event ev, int subscriberIndex)
      {
         this.ev = ev;
         this.subscriberIndex = subscriberIndex;
      }
   }

   /// <summary>
   /// A struct that represents a callback and a target.
   /// </summary>
   /// <typeparam name="T_Event"></typeparam>
   public struct EntityCallbackId<T_Event> : IEquatable<EntityCallbackId<T_Event>>
       where T_Event : struct
   {
      public readonly Action<T_Event> callback; // Moved Action<T_Event> first due to potential size
      public readonly EventTarget target;

      private const int HASHCODE_MULTIPLIER = 397;

      public EntityCallbackId(EventTarget target, Action<T_Event> callback)
      {
         this.callback = callback;
         this.target = target;
      }

      public bool Equals(EntityCallbackId<T_Event> other)
      {
         return target.Equals(other.target) && Equals(callback, other.callback);
      }

      public override bool Equals(object obj)
      {
         return obj is EntityCallbackId<T_Event> other && Equals(other);
      }

      public override int GetHashCode()
      {
         unchecked
         {
            int targetHashCode = target.GetHashCode();
            int callbackHashCode = callback?.GetHashCode() ?? 0;
            return (targetHashCode * HASHCODE_MULTIPLIER) ^ callbackHashCode;
         }
      }
   }
}
