namespace Loader.Components.Pages;

using System.Runtime.Loader;


internal class CollectibleLoadContext : AssemblyLoadContext
{
    internal CollectibleLoadContext() : base( isCollectible: true )
    {
    }
}
