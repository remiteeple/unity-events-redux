using UnityEngine;
using UnityEvents.Internal;


namespace UnityEvents.Benchmark
{

   /// <summary>
   /// Simple example of using the global and GameObject event systems 
   /// </summary>
   public class Benchmark : MonoBehaviour
   {
      [SerializeField] private int _eventCount = 100;
      [SerializeField] private EventUpdateTick _eventUpdateTick = EventUpdateTick.FixedUpdate;

      // I have to be an unmanaged type! Need` references? Use an id and have a lookup database system.
      private struct EvBenchmarkEvent
      {
         public short exampleValue;

         public EvBenchmarkEvent(short exampleValue)
         {
            this.exampleValue = exampleValue;
         }
      }

      int exampleValue = 0;

      private void OnEnable()
      {
         // Subscribes to the global event system, handles events in FixedUpdate
         GlobalEventSystem.Subscribe<EvBenchmarkEvent>(OnExampleEvent);
      }

      private void OnDisable()
      {
         // Should always unsubscribe

         // Unsubscribe from the global system
         GlobalEventSystem.Unsubscribe<EvBenchmarkEvent>(OnExampleEvent);
         gameObject.Unsubscribe<EvBenchmarkEvent>(OnExampleEvent);
      }

      private void Update()
      {
         if (_eventUpdateTick == EventUpdateTick.Update)
         {
            SendEvents();
         }
      }

      private void FixedUpdate()
      {
         if (_eventUpdateTick == EventUpdateTick.FixedUpdate)
         {
            SendEvents();
         }
      }

      private void LateUpdate()
      {
         if (_eventUpdateTick == EventUpdateTick.LateUpdate)
         {
            SendEvents();
         }
      }
      
      public void SendEvents()
      {
         for (int i = 0; i < _eventCount; i++)
            GlobalEventSystem.SendEvent(new EvBenchmarkEvent(1));
      }

      private void OnExampleEvent(EvBenchmarkEvent ev)
      {
         exampleValue += ev.exampleValue;
         Debug.Log("Benchmark Event received! Value: " + exampleValue);
      }
   }
}
