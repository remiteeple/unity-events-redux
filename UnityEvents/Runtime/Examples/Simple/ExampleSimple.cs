﻿using UnityEngine;
using UnityEvents.Internal;


namespace UnityEvents.Example
{

   /// <summary>
   /// Simple example of using the global and GameObject event systems 
   /// </summary>
   public class ExampleSimple : MonoBehaviour
   {
      // I have to be an unmanaged type! Need references? Use an id and have a lookup database system.
      private struct EvExampleEvent
      {
         public int exampleValue;

         public EvExampleEvent(int exampleValue)
         {
            this.exampleValue = exampleValue;
         }
      }

      private void OnEnable()
      {
         // Subscribes to the global event system, handles events in FixedUpdate
         GlobalEventSystem.Subscribe<EvExampleEvent>(OnExampleEvent);

         // Subscribes to THIS GameObject's event system! Also Fixed Update
         gameObject.Subscribe<EvExampleEvent>(OnExampleEvent);

         // Is the game paused but still need events for UI? There's a global UI system. Handles events in
         // LateUpdate
         GlobalEventSystem.SubscribeUI<EvExampleEvent>(OnExampleEvent);

         // There's also local event system for each GameObject that run in LateUpdate.
         gameObject.SubscribeUI<EvExampleEvent>(OnExampleEvent);
      }

      private void OnDisable()
      {
         // Should always unsubscribe

         // Unsubscribe from the global system
         GlobalEventSystem.Unsubscribe<EvExampleEvent>(OnExampleEvent);
         gameObject.Unsubscribe<EvExampleEvent>(OnExampleEvent);

         GlobalEventSystem.UnsubscribeUI<EvExampleEvent>(OnExampleEvent);
         gameObject.UnsubscribeUI<EvExampleEvent>(OnExampleEvent);
      }

      private void Update()
      {
         SendEvents();
      }

      public void SendEvents()
      {
         // Send an event to the global event system, will be processed in the next FixedUpdate
         GlobalEventSystem.SendEvent(new EvExampleEvent(10));

         // Send an event to a specific GameObject, only listeners subscribed to that gameobject will get
         // this event. Also will be processed in the next FixedUpdate
         gameObject.SendEvent(new EvExampleEvent(99));

         // Can send events to the global UI event system. These will be processed in LateUpdate which allows the
         // game to paused.
         GlobalEventSystem.SendEventUI(new EvExampleEvent(-1));

         // Similarly can send to a specific GameObject to be processed in LateUpdate
         gameObject.SendEventUI(new EvExampleEvent(999999));
      }

      private void OnExampleEvent(EvExampleEvent ev)
      {
         Debug.Log("Simple Event received! Value: " + ev.exampleValue);
      }
   }
}
