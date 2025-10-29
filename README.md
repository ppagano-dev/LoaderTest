# LoaderTest
Repo in .NET9 showing how blazor keeps unloaded collectible assembly load context alive

### How build and run

1. build `Loader` project 1st
2. build `TestGui` project 2nd
1. run `Loader` (see below screenshots)

### How it works

The `DynamicForm.razor` page uses 

    <DynamicComponent Type="FormType"  @rendermode="RenderMode.InteractiveServer" />

where `FormType` is loaded in a collectible assembly load context. When you navigate away from the `DynamicForm` page,
the collectible assembly is kept in a disposed list in class `DynamicFormRepo`.

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
if checking `clear framework cache`. This prevents discarding the unloaded context and its loaded assemblies that in our scenario changes vary frequently
and can be built at runtime with same name but diffenet versions, and this can happed for each user of each tenant.

**Note 1** if a hot reload is forced, the contexts are no more alive after a while

**Note 2** if in a collectible load context are loaded assemblies not referencing blazor in any way, the are correctly unloaded and reclaimed

**Note 3** using Visual Studio 2022 Memory Tool & WinDbg I get no clue why the contexts are kept alive:

<img width="901" height="205" alt="image" src="https://github.com/user-attachments/assets/e8493662-a555-46c8-bf2e-d8423a0f7bcd" />

<img width="1792" height="326" alt="image" src="https://github.com/user-attachments/assets/8fc346c6-2d61-4cd2-9ec8-8516e6d0932e" />



### Screenshots
1. open `Dynamic Form`, **Dynamic Form version / 1 ** will open in a new tab, then close the just open dynamic Form tab 
   <img width="1302" height="423" alt="image" src="https://github.com/user-attachments/assets/c95f5e53-4c5a-485e-a7e3-9762d3dffbd5" />

2. Press `Collect` in the home page as many times as you want, the `IsAlive` will be always true (even with 'clear framework cache' workaround enabled)

<img width="922" height="436" alt="image" src="https://github.com/user-attachments/assets/4b773010-d7ab-4875-a402-6680f7d2a05f" />

 

