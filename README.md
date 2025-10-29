# LoaderTest
Repo to show how blazor keep unloaded collectible assembly load context alive

### How build and run

1. build `Loader` project 1st
2. build `TestGui` project 2nd
1. run `Loader` (see below screenshots)

### How it works

The `DynamicForm.razor` page uses 

    <DynamicComponent Type="FormType"  @rendermode="RenderMode.InteractiveServer" />

where `FormType` is loaded in a collectible assembly load context. When you navigate away from the `DynamicForm` page,
the collectible assembly kept in a disposed list in class `DynamicFormRepo`.

Pressing the `Collect` button will try to unload the disposed assembly load contexts, and force a garbage collection.
Pressing the `Refresh` all get 'alive' info about all the unloaded conexts, in the form `IsAlive:True/False | ...`

If `clear framework cache` is checked, before a garbage collection, the blazor caches I know about are cleared (via reflection, just to test):

>  `Microsoft.AspNetCore.Components.Reflection.ComponentProperties._cachedWritersByType`
   `Microsoft.AspNetCore.Components.ComponentFactory._cachedComponentTypeInfo`
   `Microsoft.AspNetCore.Components.CascadingParameterState._cachedInfos`

### Expected behavior

All the collectible assembly load contexts should be unloaded after pressing `Collect` and `Refresh`, showing `IsAlive:False` for all the disposed contexts.

### Actual behavior

All the collectible assembly load contexts stay alive after pressing `Collect` and `Refresh`, showing `IsAlive:True` for all the disposed contexts, even
if checking `clear framework cache`.

*Note 1* If a hot reload is forced, the contexts are no more alive after a while
*Note 2* Using Visual Studio 2022 Memory Tool & WinDbg I get no clue why the contexts are kept alive:



### Screenshots
