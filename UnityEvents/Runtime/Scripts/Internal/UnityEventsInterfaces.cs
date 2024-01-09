namespace UnityEvents.Internal
{
   /// <summary>
   /// Interface for an event system.
   /// </summary>
   public interface IEventSystem
   {
      void Reset();
      void ProcessEvents();
      void VerifyNoSubscribers();
   }

   /// <summary>
   /// Interface for an jobified event system.
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public interface IJobEventSystem<T> : IEventSystem where T : struct
   {
      void QueueEvent(EventTarget target, T ev);
   }

   /// <summary>
   /// Interface for a job that can be executed when an event occurs.
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public interface IJobForEvent<T> where T : struct
   {
      void ExecuteEvent(T ev);
   }

}
