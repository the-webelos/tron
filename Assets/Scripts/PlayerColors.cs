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
}
