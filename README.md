
# MonoInjection - Dependency Injection for Unity

MonoInjection is a lightweight dependency injection framework for Unity, designed to simplify dependency management while ensuring high efficiency

## Features
- Efficient dependency injection using precomputed lists
- Seamless integration with `MonoBehaviour` (support for other classes is being added)
- Simple and intuitive usage with the `[Inject]` attribute
- **Dependencies are resolved automatically** without requiring explicit registration

## Installation

### Using Git
Clone the repository directly into your Unity project:
```sh
cd YourUnityProject/Assets/
git clone https://github.com/weslleyxm/MonoInjection.git
```
  
#### Or use the Unity Package Manager (UPM):
1. Open Unity and go to **Window** > **Package Manager**
2. Click the **+** button and select **Add package from git URL**
3. Enter: `https://github.com/weslleyxm/MonoInjection.git` and click **Add**

## Usage

### Setting Up the Injection Manager
1. Add the `InjectionManager` script to a GameObject in your scene
2. The scene is now ready for dependency injection

### Injecting Dependencies
To inject a dependency, use the `[Inject]` attribute in your class:
```csharp
using MonoInjection;
using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    [Inject]
    private SomeDependency dependency;
    
    void Start()
    {
        Debug.Log(dependency);
    }
}
```

### Providing Dependencies
**Dependencies are resolved automatically**, but if needed, you can manually bind a service using `ServiceLocator.Bind<T>()`:
```csharp
using MonoInjection;
using UnityEngine;

public class SomeDependency : MonoBehaviour
{
    void Awake()
    {
        ServiceLocator.Bind<SomeDependency>().FromComponentInHierarchy();
    }
}
```
Alternatively, you can provide a specific instance:
```csharp
ServiceLocator.Bind<SomeDependency>().FromInstance(new SomeDependency());
```

### Resolving Dependencies Manually
While dependencies are injected automatically, you can manually resolve them when needed using `ServiceLocator.Resolve<T>()`:
```csharp
var dependency = ServiceLocator.Resolve<SomeDependency>();
```
You can also resolve dependencies dynamically by type:
```csharp
var dependency = ServiceLocator.Resolve(typeof(SomeDependency));
```

### Removing and Clearing Dependencies
If necessary, you can remove specific dependencies or clear all registered services:
```csharp
ServiceLocator.Remove<SomeDependency>(); // Removes a specific dependency
ServiceLocator.Clear(); // Clears all registered dependencies
```

### Handling Objects Instantiated After the Injection Frame
Objects instantiated after the initial injection frame will not be automatically injected. To ensure they receive dependencies, use:
```csharp
var instance = InjectionManager.Instantiate<YourClass>();
```
This ensures that any new objects created at runtime also receive their required dependencies.

## Notes
- The system efficiently manages dependencies without unnecessary scanning
- Support for non-`MonoBehaviour` classes is in development
- **Dependencies are resolved automatically**, but explicit binding is available when needed

## Contributions
Contributions are welcome! Feel free to submit pull requests or open issues
