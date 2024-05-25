using UnityEngine;

public static class Global
{
    // Set this to false for "release" version.
    public static bool DEV = true;

    public static float width = 306.0f;
    public static float height = 544.0f;

    public struct Colors
    {
        public static Color Background = new(0.97f, 0.87f, 0.41f);

        public struct ButtonBack
        {
            public static Color Normal = new(0.78f, 0.48f, 0.39f);
            public static Color Pressed = new(0.91f, 0.65f, 0.59f);
            public static Color Hover = new(0.84f, 0.49f, 0.38f);
            public static float width = 0.1f;
            public static float height = 0.1f;
        }

        public struct ButtonNavigation
        {
            public static Color Normal = new(0.39f, 0.18f, 0.91f);
            public static Color Pressed = new(0.96f, 0.75f, 0.42f);
            public static Color Hover = new(0.71f, 0.46f, 1.00f);
        }

        public struct ButtonQuestion
        {
            public static Color Normal = new(0.39f, 0.18f, 0.91f);
            public static Color Pressed = new(0.96f, 0.75f, 0.42f);
            public static Color Hover = new(0.71f, 0.46f, 1.00f);
        }

    }




}