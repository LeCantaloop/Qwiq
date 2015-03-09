# Qwiq
Qwiq is a **Q**uick **W**ork **I**tem **Q**uery library for Team Foundation Server / Visual Studio Online. If you do a lot of reading
or writing of work items, this package is for you.

## How do I use it?
Grab the nuget package that best suits your needs over at [https://ms-nuget.cloudapp.net](https://ms-nuget.cloudapp.net). Search for Qwiq.

### Qwiq.Core
Qwiq.Core is the no-frills base package. Exposes the raw types needed to read and write work items.

### Qwiq.Identity
Adds methods to simplify converting between your preferred method of identity (display names, user names) and TFS's identity classes.

## Why use it?
### 1. Easier to consume
Let's be honest, the TFS libraries are a pain to use. There are a lot of them, several are dynamically loaded, and a few are native. While
we can't avoid it, you can! Just install the Qwiq.Core package and everything will be in your \bin folder when you need it.

### 2. Easier to test
Qwiq makes testing your apps a breeze. Everything has an interface. Everything uses factories (or factory methods) instead of constructors.
Just mock what you need for your tests and go. No more messy, temperamental fakes, or adapters cluttering your code.

### 3. Easier to understand
How often do you update a work item? How often do you create a new security group? We stripped out the rarely used stuff to make interfaces
cleaner and the relationships between types simpler. Missing something you can't live without? Send us a pull request!