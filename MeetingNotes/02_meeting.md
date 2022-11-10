# 02 meeting

## Current goals

* **cpp => java** (jni)
  * try out more complex scenarios (before another layer of abstraction and complexity is added by moving from cpp to .net)
    * throw exception in java code, handle it in cpp
    * pass string between cpp and java
    * pass object?
* **p/invoke**
  * call smt. native from .net using `LibraryImport` attribute 
* **nice to have if time permits**
  * roslyn hello world
  * invoke simple static java method from .net 

## General notes

* base project goal is to be able to generate invocations of static methods with string and number parameters
* no need to study java in advance - research problems when they come up
* **multiplatform solution**
  * native delegates  in c# 
  * different representations of jni function table struct - annotate delegates with different calling convention
  * Global vs Local jni handle - jni pointer to object, which keeps the object alive (global vs local scope) - I need global handles to work with .net objects in java
    * when working with java objects in .net, I need to use `GCHandle` to keep objects alive - I wanna use strong not pinned mode - I will always work with the java object via handle, I don't mind that the .net pointer gets invalidated (via CG eg during the heap defragmentation)
* **How to get java metadata (reflection) to .Net source generator so that it can generate proxies and wrappers**?
  * xamarin java interop has .java file parser  - could it be used? - to extract metadata from user written .java files
  * I also need to generate proxies for library methods - I won't have access to their source code (.java)
  * `.jar` - zip file, contains `.class` files and binary files for library methods - to get metadata for library methods.  
    * problem - **does not contain names of methods parameters** - unfortunate, because I wanna generate user friendly API. Problem can probably be bypassed somehow. How?

## Difficult problems to address or to avoid when a time comes

* Garbage collection - must have
  * but no need to address the issue of cycles between two heaps - way too complex, probably impossible to solve efficiently. GCs are complex and carefully optimized. Strategies of java and .net CGs are not compatible
* Generics - would be nice to support generic collections but:
  *  java and .net type systems are not compatible when it comes to generics - **type constraints** work differently?
  * java forgets type information for generics in runtime (.net does not and needs the type info)
* .dll hell and java class loadres
  * mapping between class and its implementation if done by a class name
  * java class loader reads `.class` files and carries out the mapping between class and implementation, then it loads the implementation to JVM
    * there can exists multiple `.java` files for one class name - alternative implementations
  * Multiple components in one project may depend each on a different version of the same package - how to deal with a necessity to have two incompatible implementations of the same class in the project?
    * java uses sub-loader for each project to provide projects with appropriate versions of their dependencies 
  * The problem is - we use `Jni` to create instances of java objects and to find method implementations. Jni gets a type and method information from a class  loader. if there are multiple class subloaders in the project, how do we know, which one will provide data about the version of the class which the caller expects? We can easily get a different implementation of the same "interface".
* It generally does not make that much sense to work with objects when doing interop between languages. OOP tends to invoke a huge amount of methods to carry out simple task. Overhead of method invocation via interop is not trivial (incomparably larger than the overhead of normal method invocation). But when enabling the interop between two object oriented languages such as C# and java, support for objects must be implemented, otherwise interop does not make sense. 

