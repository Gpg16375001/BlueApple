using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmileLab.Net.API;

using MessagePack;


public static partial class ServerModelCache
{
    private static Dictionary<System.Type, object> modelCaches;

    static partial void CreateTables ();

    static ServerModelCache()
    {
        MessagePack.Resolvers.CompositeResolver.RegisterAndSetAsDefault(
            ServerModelArrayResolver.Instance,
            MessagePack.Resolvers.StandardResolver.Instance
        );
    }

    public static void Init ()
    {
        revisionCache = new Dictionary<System.Type, int> ();
        modelCaches = new Dictionary<System.Type, object> ();
        SqliteConnectionManager.ShardInstanse.CreateConnection ("InMemory", ":memory:");
        CreateTables ();
    }

    public static void Set<T> (object data)
    {
        if (modelCaches.ContainsKey (typeof(T))) {
            modelCaches [typeof(T)] = data;
        } else {
            modelCaches.Add (typeof(T), data);
        }
        Modify<T> ();
    }

    public static T Get<T> ()
    {
        object ret;
        if (modelCaches.TryGetValue (typeof(T), out ret)) {
            return default(T);
        }
        return (T)ret;
    }


    private static Dictionary<System.Type, int> revisionCache;

    public static bool IsModify<T> (int revision)
    {
        int cacheRevision = 0;
        if (revisionCache.TryGetValue (typeof(T), out cacheRevision)) {
            return cacheRevision != revision;
        }
        return false;
    }

    public static void Modify<T> ()
    {
        if (revisionCache.ContainsKey (typeof(T))) {
            revisionCache [typeof(T)]++;
            return;
        }
        revisionCache [typeof(T)] = 1;
    }

    public static int GetRevision<T> ()
    {
        int cacheRevision = 0;
        if (revisionCache.TryGetValue (typeof(T), out cacheRevision)) {
            return cacheRevision;
        }
        return cacheRevision;
    }

    public static SqliteDatabase GetConnection ()
    {
        return SqliteConnectionManager.ShardInstanse.GetConnection ("InMemory");
    }
}
