# Unity Events Redux #
*Based on the original [Unity Events Redux](https://github.com/GalvanicGames/unity-events) by GalvanicGames*

A performant code focused strongly typed publisher/subscriber event system to decouple objects from talking directly to each other. Supports global event system and per GameObject event systems that send deferred events to be processed at a later tick (FixedUpdate, Update, or LateUpdate). Allows regular callback events and multithreaded jobs that trigger on events.

Custom Event Systems can be created to control when events are processed instead of relying on the update ticks.

The code is **fully Unit Tested**.

## Disclaimer
This package utilizes Unity's Job System. The Job System is a feature that is considered to be in preview and experimental. **Use at your own risk!**

## Obtain
### Automated
`com.remiteeple.unity-events-redux`

### Manual
[Releases](https://github.com/remiteeple/unity-events-redux/releases)

## Setup
Once the Unity Events asset has been imported into the project then the event system is ready to be used.

### Prerequisites ###
Requires the following Unity packages:
```
Jobs
Mathematics
Collections
Burst
```

## Examples
There are multiple [simple](Assets/UnityEvents/Examples/Simple) and [advanced](Assets/UnityEvents/Examples/Advance) examples in the repository and can be looked at for guidance.

As a simple example here is how an event can be sent to a Global event system and a GameObject's local event system.
```csharp
// I have to be an unmanaged type! Need references? Use an id and have a lookup database system.
private struct EvExampleEvent
{
  public int exampleValue;

  public EvExampleEvent(int exampleValue)
  {
    this.exampleValue = exampleValue;
  }
}

// The callback that will be invoked on an event
private void OnExampleEvent(EvExampleEvent ev)
{
  Debug.Log("Event received! Value: " + ev.exampleValue);
}

private void OnEnable()
{
  // Subscribes to the global event system, handles events in FixedUpdate
  GlobalEventSystem.Subscribe<EvExampleEvent>(OnExampleEvent);

  // Subscribes to THIS GameObject's event system! Also Fixed Update
  gameObject.Subscribe<EvExampleEvent>(OnExampleEvent);
}

public void SendEvents()
{
  // Send an event to the global event system, will be processed in the next FixedUpdate
  GlobalEventSystem.SendEvent(new EvExampleEvent(10));

  // Send an event to a specific GameObject, only listeners subscribed to that gameobject will get
  // this event. Also will be processed in the next FixedUpdate
  gameObject.SendEvent(new EvExampleEvent(99));
}

```

## Blittable Requirement
Unity Events Redux requires that events/jobs are [blittable](https://docs.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types) types. This is done to allow compatibility with the burst compiler and Unity's Job system. Also has a benefit of encouraging "better" programming practices since the events are deferred. References may become stale and GameObjects may have been destroyed and are "null" by the time the event is processed. Send the data the event represents rather than a reference to an object. If a reference is needed then create a look up database and send the id of the object for event listeners to look up to process on. If an array/list is needed then consider using something like [ValueTypeLists](https://gist.github.com/cjddmut/cb43af3ee191af78363f41a3188c0f7b).

## _'DISABLE_EVENT_SAFETY_CHKS'_ Symbol
Unity Events Redux performs various safety checks to make sure it isn't being used inappropriately. These can be turned off by defning 'DISABLE_EVENT_SAFETY_CHKS' with the compiler (or in Unity go to 'Player Settings > Scripting Define Symbols'). Turning it off can improve performance since no checks will always be faster than any check. Use at your own risk!