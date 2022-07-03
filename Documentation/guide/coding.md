# Coding Style Guide

## Naming Conventions
Names of classes, enumerations, structs, methods, and namespaces: `PascalCase`.

```csharp
public class MyClass
{
    ...
}
```

```csharp
public void MyFunction()
{
    ...
}
```

```csharp
namespace GrandmaGreen.ExampleNamespace
{
    ...
}
```

```csharp
enum MyEnum
{
    StateOne,
    StateTwo
}
```

Names of properties and all variables, including local and member variables: `camelCase`.

Private member variables should also append an m_ prefix ahead of the variable name. Similarly, static variables should append an s_ prefix ahead of the variable name.

```csharp
public int myInt;
```

```csharp
private string m_myString;
```

```csharp
static float s_myFloat;
```

```csharp
public void MyFunction(int myLocalVariable)
{
    ...
}
```

## Good Practice
For readability, please consider the following:
* One statement per line.
* Keep opening and closing braces on their own lines.

Avoid:

```csharp
if (condition) { DoSomething(); DoSomethingElse(); }
```


Instead, do:

```csharp
if (condition)
{
    DoSomething();
    DoSomethingElse();
}
```

We also encourage using the `sealed` keyword when possible. This is done to help prevent accidental derivative classes.

Similarly, use `structs` over classes when possible. Structs are sealed and self contained. We utilize structs frequently when multi-threading, but they are good for general optimization.

Our team prioritizes and values **modularity** and **reusability**. Please keep systems and classes as decoupled as possible to minimize risk of the whole game crashing when one feature bugs out.

That being said, please always reach out to any of the leads or other team members if you are stuck on the best way to implement something! Remember that everything is done best with another pair of eyes. :sunflower:

## Files and Organization
Anything that needs to go on this website should go in the `Assets/_GrandmaGreen/Scripts` folder. The script picks up anything in this folder that has a `.cs` ending.

File names and directory names should be in `PascalCase`, ie. `MyFile.cs`.

Where possible, the name of the file should be the same as the name of the main class in the file, ie. `MyClass.cs`. Try to keep to one core class per file.