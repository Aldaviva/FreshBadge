namespace FreshBadge.Data.Shields;

public enum ShieldColor {

    BRIGHT_GREEN,
    GREEN,
    YELLOW,
    YELLOW_GREEN,
    ORANGE,
    RED,
    BLUE,
    GREY,
    LIGHT_GREY,
    CRITICAL,
    IMPORTANT,
    SUCCESS,
    INFORMATIONAL,
    INACTIVE,

}

public static class ShieldColorMethods {

    public static string toText(this ShieldColor color) => color switch {
        ShieldColor.BRIGHT_GREEN  => "brightgreen",
        ShieldColor.GREEN         => "green",
        ShieldColor.YELLOW        => "yellow",
        ShieldColor.YELLOW_GREEN  => "yellowgreen",
        ShieldColor.ORANGE        => "orange",
        ShieldColor.RED           => "red",
        ShieldColor.BLUE          => "blue",
        ShieldColor.GREY          => "grey",
        ShieldColor.LIGHT_GREY    => "lightgrey",
        ShieldColor.CRITICAL      => "critical",
        ShieldColor.IMPORTANT     => "important",
        ShieldColor.SUCCESS       => "success",
        ShieldColor.INFORMATIONAL => "informational",
        ShieldColor.INACTIVE      => "inactive",
        _                         => color.ToString()
    };

}