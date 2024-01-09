using UnityEngine;
using UnityEvents.Internal;


namespace UnityEvents.Benchmark
{

   /// <summary>
   /// Simple example of using the global and GameObject event systems 
   /// </summary>
   public class Benchmark : MonoBehaviour
   {
      // I have to be an unmanaged type! Need` references? Use an id and have a lookup database system.
      private struct EvBenchmarkEvent
      {
         public int exampleValue;

         public EvBenchmarkEvent(int exampleValue)
         {
            this.exampleValue = exampleValue;
         }
      }

      private void OnEnable()
      {
         // Subscribes to the global event system, handles events in FixedUpdate
         GlobalEventSystem.Subscribe<EvBenchmarkEvent>(OnExampleEvent);

         // Subscribes to THIS GameObject's event system! Also Fixed Update
         gameObject.Subscribe<EvBenchmarkEvent>(OnExampleEvent);

         // Is the game paused but still need events for UI? There's a global UI system. Handles events in
         // LateUpdate
         GlobalEventSystem.SubscribeUI<EvBenchmarkEvent>(OnExampleEvent);

         // There's also local event system for each GameObject that run in LateUpdate.
         gameObject.SubscribeUI<EvBenchmarkEvent>(OnExampleEvent);
      }

      private void OnDisable()
      {
         // Should always unsubscribe

         // Unsubscribe from the global system
         GlobalEventSystem.Unsubscribe<EvBenchmarkEvent>(OnExampleEvent);
         gameObject.Unsubscribe<EvBenchmarkEvent>(OnExampleEvent);

         GlobalEventSystem.UnsubscribeUI<EvBenchmarkEvent>(OnExampleEvent);
         gameObject.UnsubscribeUI<EvBenchmarkEvent>(OnExampleEvent);
      }

      private void Update()
      {
         SendEvents();
      }

      public void SendEvents()
      {
         // Send an event to the global event system, will be processed in the next FixedUpdate
         GlobalEventSystem.SendEvent(new EvBenchmarkEvent(10));

         // Send an event to a specific GameObject, only listeners subscribed to that gameobject will get
         // this event. Also will be processed in the next FixedUpdate
         gameObject.SendEvent(new EvBenchmarkEvent(99));

         // Can send events to the global UI event system. These will be processed in LateUpdate which allows the
         // game to paused.
         GlobalEventSystem.SendEventUI(new EvBenchmarkEvent(-1));

         // Similarly can send to a specific GameObject to be processed in LateUpdate
         gameObject.SendEventUI(new EvBenchmarkEvent(999999));
      }

      private void OnExampleEvent(EvBenchmarkEvent ev)
      {
         Debug.Log("Simple Event received! Value: " + ev.exampleValue);
      }
   }
}
