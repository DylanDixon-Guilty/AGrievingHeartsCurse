using UnityEngine;

public static class CutsceneLaunchData
{
    private static string conversationName;
    private static string returnConversationName;

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

    public static void SetReturnConversation(string name)
    {
        returnConversationName = name;
    }

    public static string ConsumeReturnConversation()
    {
        string result = returnConversationName;
        returnConversationName = null;
        return result;
    }
}