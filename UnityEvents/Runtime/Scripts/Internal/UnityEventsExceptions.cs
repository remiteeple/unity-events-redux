using System;
using System.Collections.Generic;
using System.Text;

namespace UnityEvents.Internal
{
   /// <summary>
   /// Called when a subscriber tries to subscribe to the same event twice.
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public class MultipleSubscriptionsException<T> : Exception
   {
      public MultipleSubscriptionsException(Action<T> callback)
         : base($"Not allowed to subscribe the same callback to the same entity! Target: {callback.Target.GetType().Name} Event: {typeof(T).Name}")
      {

      }
   }

   /// <summary>
   /// Thrown when a subscriber is still listening to an event when the event system is disposed.
   /// </summary>
   /// <typeparam name="T_Callback"></typeparam>
   /// <typeparam name="T_Event"></typeparam>
   public class SubscriberStillListeningException<T_Callback, T_Event> : Exception
        where T_Event : struct
   {
      private readonly List<Action<T_Callback>> _listeners;

      public SubscriberStillListeningException(List<Action<T_Callback>> listeners)
          : base($"The following subscribers are still listening to the {typeof(T_Event).Name} system!")
      {
         _listeners = listeners;
      }

      public override string ToString()
      {
         var sb = new StringBuilder(base.ToString());
         foreach (var listener in _listeners)
         {
            sb.AppendLine(listener?.Method.Name ?? "<NULL>");
         }
         return sb.ToString();
      }
   }

   /// <summary>
   /// Called when index out of range exception for reserved targets.
   /// </summary>
   public class IndexOutOfReservedTargetsException : Exception
   {

   }

   /// <summary>
   /// Called when a subscriber tries to subscribe to an event that is not blittable.
   /// </summary>
   public class EventTypeNotBlittableException : Exception
   {
      public EventTypeNotBlittableException(Type type)
         : base($"Event type {type.Name} must be blittable!")
      { }
   }

   /// <summary>
   /// Called when a subscriber tries to subscribe to a job that is not blittable.
   /// </summary>
   public class JobTypeNotBlittableException : Exception
   {
      public JobTypeNotBlittableException(Type type)
         : base($"Job type {type.Name} must be blittable!")
      { }
   }
}