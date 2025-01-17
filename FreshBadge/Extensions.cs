using System.Drawing;

namespace FreshBadge;

public static class Extensions {

    public static string toRgbHexColor(this Color color) {
        if (color.R % 0x11 == 0 && color.G % 0x11 == 0 && color.B % 0x11 == 0) {
            return $"{color.R / 0x11:x1}{color.G / 0x11:x1}{color.B / 0x11:x1}";
        } else {
            return $"{color.R:x2}{color.G:x2}{color.B:x2}";
        }
    }

}