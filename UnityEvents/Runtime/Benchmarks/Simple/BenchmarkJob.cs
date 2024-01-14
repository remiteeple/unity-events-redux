using UnityEngine;
using UnityEvents.Internal;

namespace UnityEvents.Benchmark
{
   /// <summary>
   /// Simple example of using jobs with the event system 
   /// </summary>
   public class BenchmarkJob : MonoBehaviour
   {
      [SerializeField] private int _eventCount = 100;
      [SerializeField] private EventUpdateTick _eventUpdateTick = EventUpdateTick.FixedUpdate;


      // I have to be an unmanaged type! Need references? Use an id and have a lookup database system.
      private struct EvBenchmarkEvent
      {
         public short jobValue;

         public EvBenchmarkEvent(short jobValue)
         {
            this.jobValue = jobValue;
         }
      }

      private struct BenchJob : IJobForEvent<EvBenchmarkEvent>
      {
         // This result is stored across jobs, wipe it out at the beginning of each job if this isn't wanted!
         public int jobCount;

         public void ExecuteEvent(EvBenchmarkEvent ev)
         {
            jobCount += ev.jobValue;
         }
      }

      private void OnEnable()
      {
         // Jobs work with the global simulation and global UI event systems as well as the GameObject system. This
         // will just show examples with the global simulation system.
         //
         // When an event is fired jobs will processed in parallel using the burst compiler. Can make otherwise
         // long tasks very short. Afterwards the callback functions are invoked so the listener can use the results
         // of the job.
         GlobalEventSystem.SubscribeWithJob<BenchJob, EvBenchmarkEvent>(new BenchJob(), OnJobFinished);
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
         // Job listeners trigger on events like anything else. You can have job listeners and regular listeners to
         // a single event.
         for (int i = 0; i < _eventCount; i++)
            GlobalEventSystem.SendEvent(new EvBenchmarkEvent(1));
      }

      private void OnJobFinished(BenchJob ev)
      {
         Debug.Log("Benchmark Job finished! Value: " + ev.jobCount);
      }
   }
}
