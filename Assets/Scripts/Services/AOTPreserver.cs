using UnityEngine;
using Supabase.Gotrue;

public class AOTPreserver : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void PreserveTypes()
    {
        // Dummy-Referenzen für iOS AOT
        var session = new Session();
        var user = new User();
        var identity = new UserIdentity();
    }
}
