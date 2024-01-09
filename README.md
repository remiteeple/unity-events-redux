# Unity Events Redux Enhanced #
*Based on the original [Unity Events 2.0](https://github.com/GalvanicGames/unity-events) by GalvanicGames, this is an enhanced version.*

Unity Events Redux Enhanced is a highly efficient, code-centric, and strongly typed publisher/subscriber event system designed to minimize direct object-to-object communication. It features both global and GameObject-specific event systems, capable of sending deferred events to be processed during FixedUpdate, Update, or LateUpdate. This system facilitates both standard callback events and multithreaded jobs that are activated by specific events.

Customizable Event Systems can be created, allowing for control over event processing times beyond the standard update cycles.

The codebase is **fully Unit Tested** for reliability and stability.

## Caution
This package leverages Unity's Job System, a feature currently in preview and experimental stages. **Usage is at your own discretion and risk.**

## Installation
### Automated Method
Use package identifier `com.remiteeple.unity-events-redux`.

### Manual Method
Access [Releases](https://github.com/remiteeple/unity-events-redux/releases) for manual download.

## Configuration
Post-import, Unity Events Redux is immediately operational.

### Dependencies ###
Requires installation of the following Unity packages:
- Jobs
- Mathematics
- Collections
- Burst

## Usage Examples
Numerous [Simple](Assets/UnityEvents/Examples/Simple) and [Advanced](Assets/UnityEvents/Examples/Advance) examples are available in the repository for reference.

Here's a straightforward illustration of dispatching events to both Global and GameObject-specific event systems:
```csharp
// Events must be unmanaged types. For references, use an identifier and a lookup system.
private struct EvExampleEvent
{
  public int exampleValue;

  public EvExampleEvent(int exampleValue)
  {
    this.exampleValue = exampleValue;
  }
}

// Callback function for event handling
private void OnExampleEvent(EvExampleEvent ev)
{
  Debug.Log("Event received! Value: " + ev.exampleValue);
}

private void OnEnable()
{
  // Subscribe to the global event system for FixedUpdate handling
  GlobalEventSystem.Subscribe<EvExampleEvent>(OnExampleEvent);

  // Subscribe to this GameObject's local event system for FixedUpdate handling
  gameObject.Subscribe<EvExampleEvent>(OnExampleEvent);
}

public void TriggerEvents()
{
  // Dispatch event globally, processed next FixedUpdate
  GlobalEventSystem.SendEvent(new EvExampleEvent(10));

  // Dispatch event to this specific GameObject, processed next FixedUpdate
  gameObject.SendEvent(new EvExampleEvent(99));
}
```

## Blittable Event Requirement
Unity Events Redux mandates the use of [blittable](https://docs.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types) types for events/jobs, ensuring compatibility with the burst compiler and Unity's Job system. This encourages better programming practices, as deferred events might encounter stale references or destroyed GameObjects. Events should represent data, not object references. For necessary references, utilize a lookup database system and send object IDs.

## _'DISABLE_EVENT_SAFETY_CHKS'_ Symbol
Unity Events Redux conducts various safety checks by default. To disable these for performance gains, define 'DISABLE_EVENT_SAFETY_CHKS' in the compiler (or in Unity: 'Player Settings > Scripting Define Symbols'). Disabling checks can boost performance but increases the risk of misuse. Use with caution!
