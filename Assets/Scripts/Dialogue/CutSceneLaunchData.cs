using UnityEngine;

public static class CutsceneLaunchData
{
    private static string conversationName;

    public static void SetConversation(string name)
    {
        conversationName = name;
    }

    public static string ConsumeConversation()
    {
        string result = conversationName;
        conversationName = null;
        return result;
    }
}
