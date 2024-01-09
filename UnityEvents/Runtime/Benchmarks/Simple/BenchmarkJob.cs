using UnityEngine;
using UnityEvents.Internal;

namespace UnityEvents.Benchmark
{
   /// <summary>
   /// Simple example of using jobs with the event system 
   /// </summary>
   public class BenchmarkJob : MonoBehaviour
   {
      // I have to be an unmanaged type! Need references? Use an id and have a lookup database system.
      private struct EvBenchmarkEvent
      {
         public int jobValue;

         public EvBenchmarkEvent(int jobValue)
         {
            this.jobValue = jobValue;
         }
      }

      private EvBenchmarkEvent _ev = new EvBenchmarkEvent(1);

      private struct BenchJob : IJobForEvent<EvBenchmarkEvent>
      {
         // This result is stored across jobs, wipe it out at the beginning of each job if this isn't wanted!
         public int jobCount;

         public void ExecuteEvent(EvBenchmarkEvent ev)
         {
            jobCount += ev.jobValue;
         }
      }

      // I have to be an unmanaged type! Need references? Use an id and have a lookup database system.
      private struct EvBenchmarkEvent1
      {
         public int jobValue;

         public EvBenchmarkEvent1(int jobValue)
         {
            this.jobValue = jobValue;
         }
      }

      private EvBenchmarkEvent1 _ev1 = new EvBenchmarkEvent1(2);

      private struct BenchJob1 : IJobForEvent<EvBenchmarkEvent1>
      {
         // This result is stored across jobs, wipe it out at the beginning of each job if this isn't wanted!
         public int jobCount;

         public void ExecuteEvent(EvBenchmarkEvent1 ev)
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
         GlobalEventSystem.SubscribeWithJob<BenchJob1, EvBenchmarkEvent1>(new BenchJob1(), OnJobFinished);
      }

      private void OnDisable()
      {
         GlobalEventSystem.UnsubscribeWithJob<BenchJob, EvBenchmarkEvent>(OnJobFinished);
         GlobalEventSystem.UnsubscribeWithJob<BenchJob1, EvBenchmarkEvent1>(OnJobFinished);
      }

      private void FixedUpdate()
      {
         SendEvents();
      }

      public void SendEvents()
      {
         // Job listeners trigger on events like anything else. You can have job listeners and regular listeners to
         // a single event.
         for (int i = 0; i < 50; i++)
         {
            GlobalEventSystem.SendEvent(_ev);
            GlobalEventSystem.SendEvent(_ev1);
         }
      }

      private void OnJobFinished(BenchJob ev)
      {
         Debug.Log("BenchJob finished! Value: " + ev.jobCount);
      }
      private void OnJobFinished(BenchJob1 ev)
      {
         Debug.Log("BenchJob1 finished! Value: " + ev.jobCount);
      }
   }
}
