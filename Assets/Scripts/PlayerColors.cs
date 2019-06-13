using UnityEngine;

public static class PlayerColors {
    private static string[] colors = new string[] {
        "#4F705E",
        "#60EFA0",
        "#ECFD6D",
        "#F06C60",
        "#70322D",
        "#6E27A0",
        "#39BBBD",
        "#57F6F8",
        "#C479F2",
        "#BA9E2A"
    };

    public static Color RandomColor() {
        ColorUtility.TryParseHtmlString(colors[Random.Range(0, colors.Length)], out Color c);
        return c;
    }

    public static Color NextColor(Color color) {
        string hex = ColorUtility.ToHtmlStringRGB(color);
        int index = 0;
        for (int i = 0; i < colors.Length; i++) {
            if (colors[i].Equals(hex)) {
                if (i == colors.Length - 1) {
                    index = 0;
                } else {
                    index = i + 1;
                }
            }
        }
        ColorUtility.TryParseHtmlString(colors[index], out Color c);
        return c;
    }

    public static Color PreviousColor(Color color) {
        string hex = ColorUtility.ToHtmlStringRGB(color);
        int index = 0;
        for (int i = 0; i < colors.Length; i++) {
            if (colors[i].Equals(hex)) {
                if (i == 0) {
                    index = colors.Length - 1;
                } else {
                    index = i - 1;
                }
            }
        }
        ColorUtility.TryParseHtmlString(colors[index], out Color c);
        return c;
    }
}
