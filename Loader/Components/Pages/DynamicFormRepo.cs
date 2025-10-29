
namespace Loader.Components.Pages;

using System.Collections.Generic;
using System.Runtime.Loader;
using System.Linq;
using System;



internal class DynamicFormRepo
{

    internal static string Info => string.Join( '|', uloadedContexts.Select( c => $"IsAlive:{c.IsAlive}" ) );


    internal static AssemblyLoadContext CreateContext()
    {
        var ctx = new CollectibleLoadContext();

        liveContexts.Add( ctx );

        return ctx;
    }


    internal static void Cleanup( bool clearFrameworkCaches = false )
    {

        var toUnload = disposedContexts.ToArray();
        disposedContexts.Clear();
        
        foreach( var ctx in toUnload )
        {
            ctx.Unload();
            uloadedContexts.Add( new WeakReference( ctx, trackResurrection: true ) );
        }


        if( clearFrameworkCaches )
        {
            ClearFormsReferences( "Microsoft.AspNetCore.Components.Reflection.ComponentProperties", "_cachedWritersByType" );
            ClearFormsReferences( "Microsoft.AspNetCore.Components.ComponentFactory", "_cachedComponentTypeInfo" );

            ClearFormsReferences( "Microsoft.AspNetCore.Components.CascadingParameterState", "_cachedInfos" );

            // seems not exists
            ClearFormsReferences( "Microsoft.AspNetCore.Components.RootComponentTypeCache", "_typeToKeyLookUp" );
        }



        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();


        foreach( var wr in uloadedContexts.ToArray() )
        {
            //if( wr.IsAlive == false )
            //    uloadedContexts.Remove( wr );

            //var assemblies = (wr.Target as AssemblyLoadContext)?.Assemblies ?? [];
            //continue;
        }

    }





    internal static void ReleaseContext( AssemblyLoadContext? ctx )
    {
        lock( monitor )
        {
            if( ctx is null )
                return;

            var index = liveContexts.IndexOf( ctx );
            if( index < 0 )
                return;
        

            liveContexts.RemoveAt( index );
            disposedContexts.Add( ctx );
        
        }
    }

    
    private static void ClearFormsReferences( string typeName, string dictName )
    {
        var type = typeof( Microsoft.AspNetCore.Components.ComponentBase ).Assembly.GetType( typeName );
        if( type is null )
            return;

        var field = type.GetField( dictName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic );
        if( field?.GetValue( null ) is not System.Collections.IDictionary dict )
            return;

        dict.Clear();

    } //

    private static object monitor = new();


    private static List<AssemblyLoadContext> liveContexts = new ();
    private static List<AssemblyLoadContext> disposedContexts = new();

    private static List<WeakReference> uloadedContexts = new();

}
