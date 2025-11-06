
using InriaTools;

using UnityEditor;

public class ThreadingOptions : Editor
{
    private const string FULL_STACK_MENU_ENTRY = "Tools/Full Thread Stack Trace";

    // This allows to update the menu item before showing it
    [MenuItem(FULL_STACK_MENU_ENTRY)]
    private static void ToggleStackTraceOption()
    {
        bool enabled = !EditorPrefs.GetBool(SeparateThread.SHOW_FULL_STACK_PREF_KEY);
        EditorPrefs.SetBool(SeparateThread.SHOW_FULL_STACK_PREF_KEY, enabled);
        Menu.SetChecked(FULL_STACK_MENU_ENTRY, enabled);
    }

}
