using System;
using System.Runtime.CompilerServices;
using UnityEvents.Internal;
using Object = UnityEngine.Object;

namespace UnityEvents
{
   /// <summary>
   /// Used to represent the target of events. This is something that be subscribed to, unsubscribed from, and have
   /// events sent to. For example, the global event system uses an event target to maintain this. The GameObject event
   /// system has an event target for each GameObject.
   /// </summary>x
   public struct EventTarget : IEquatable<EventTarget>
   {
      public readonly ulong id;

      public static readonly EventTarget NULL_TARGET = new EventTarget(ulong.MaxValue);

      private static ulong _ids = (ulong)uint.MaxValue + 1;

      /// <summary>
      /// Creates a target with a specific ID. This CAN clash with a previously created target of an id.
      /// Creating new target of the same id intentionally is fine and will work interchangeably.
      /// </summary>
      /// <param name="id">Id to bind the target to.</param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public EventTarget(ulong id)
      {
         this.id = id;
      }

      /// <summary>
      /// Creates a target with a specific ID. This CAN clash with a previously created target of an id.
      /// Creating new target of the same id intentionally is fine and will work interchangeably.
      /// </summary>
      /// <param name="id">Id to bind the target to.</param>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public EventTarget(int id) : this((ulong)id) { }

      /// <summary>
      /// Creates a new unique event target.
      /// </summary>
      /// <returns>The new target.</returns>s
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static EventTarget CreateTarget()
      {
         return new EventTarget(_ids++);
      }

      /// <summary>
      /// Creates an event target associated with the supplied Unity Object.
      /// </summary>
      /// <param name="obj">The object to associate the target with.</param>
      /// <returns>The new target.</returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static EventTarget CreateTarget(Object obj)
      {
         return obj == null ? NULL_TARGET : new EventTarget((ulong)obj.GetInstanceID());
      }

      /// <summary>
      /// Reserves a reservation of targets to be leveraged by the caller. For custom management for some number of targets.
      /// </summary>
      /// <param name="count">The number of targets to reserve.</param>
      /// <returns>The starting id of the reservation, this should be </returns>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static EventTargetReservation ReserveTargets(ulong count)
      {
         ulong start = _ids;
#if !DISABLE_EVENT_SAFETY_CHKS
         checked
         {
            _ids += count;
         }
#else
      				_ids += count;
#endif

         return new EventTargetReservation(start, count);
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool Equals(EventTarget other)
      {
         return id == other.id;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public override bool Equals(object obj)
      {
         return obj is EventTarget other && Equals(other);
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public override int GetHashCode()
      {
         return id.GetHashCode();
      }
   }

   /// <summary>
   /// Is a reservation of a block of event targets.
   /// </summary>
   public struct EventTargetReservation : IEquatable<EventTargetReservation>
   {
      private readonly ulong _reservationStart;
      private readonly ulong _reservationCount;

      private const int HASHCODE_MULTIPLIER = 397;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public EventTargetReservation(ulong reservationStart, ulong reservationCount)
      {
         _reservationStart = reservationStart;
         _reservationCount = reservationCount;
      }

      /// <summary>
      /// Get an event target from the reservation.
      /// </summary>
      /// <param name="index">The index of the event target.</param>
      /// <returns>The event Target.</returns>
      /// <exception cref="IndexOutOfReservedTargetsException">Throws if an invalid index.</exception>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public EventTarget GetEntityTarget(int index)
      {
#if !DISABLE_EVENT_SAFETY_CHKS
         if (index < 0 || (ulong)index >= _reservationCount)
         {
            throw new IndexOutOfReservedTargetsException();
         }
#endif

         return new EventTarget(_reservationStart + (ulong)index);
      }

      /// <summary>
      /// Get an event target from the reservation.
      /// </summary>
      /// <param name="index">The index of the event target.</param>
      /// <returns>The event Target.</returns>
      /// <exception cref="IndexOutOfReservedTargetsException">Throws if an invalid index.</exception>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public EventTarget GetEntityTarget(ulong index)
      {
#if !DISABLE_EVENT_SAFETY_CHKS
         if (index >= _reservationCount)
         {
            throw new IndexOutOfReservedTargetsException();
         }
#endif

         return new EventTarget(_reservationStart + index);
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool Equals(EventTargetReservation other)
      {
         return _reservationStart == other._reservationStart && _reservationCount == other._reservationCount;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public override bool Equals(object obj)
      {
         return obj is EventTargetReservation other && Equals(other);
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public override int GetHashCode()
      {
         unchecked
         {
            return ((int)_reservationStart * HASHCODE_MULTIPLIER) ^ (int)_reservationCount;
         }
      }
   }
}
