using System.Drawing;
using Unfucked;
using UnionTypes;

namespace FreshBadge.Data.Shields;

using BadgeColor = Union<ShieldColor, Color, string>;

/// <summary>
/// <para>Returned from server to Shields.io client to make a Dynamic JSON badge.</para>
/// <para><see href="https://shields.io/badges/endpoint-badge"/></para>
/// <para>Using the endpoint badge, you can provide content for a badge through a JSON endpoint. The content can be prerendered, or generated on the fly. To strike a balance between responsiveness and bandwidth utilization on one hand, and freshness on the other, cache behavior is configurable, subject to the Shields minimum. The endpoint URL is provided to Shields through the query string. Shields fetches it and formats the badge.</para>
/// </summary>
public class ShieldsBadgeResponse {

    /// <param name="label">The left text, or the empty string to omit the left side of the badge. This can be overridden by the query string.</param>
    /// <param name="message">Can't be empty. The right text.</param>
    /// <param name="messageColor">Defaults to <see cref="ShieldColor.LIGHT_GREY"/>. Can be a named color, RGB, or any CSS color (rgb(), hsl(), or CSS names). See <see href="https://github.com/badges/shields/blob/master/badge-maker/README.md#colors"/>.</param>
    /// <param name="labelColor">Defaults to <see cref="ShieldColor.GREY"/>. The left color.</param>
    /// <param name="isError"><c>true</c> to treat this as an error badge. This prevents the user from overriding the color. In the future, it may affect cache behavior.</param>
    /// <param name="logo">An SVG document or string containing a custom logo, or one of the simple-icons (<see href="https://simpleicons.org"/>) slugs.</param>
    /// <param name="style">Default: <see cref="ShieldStyle.FLAT"/>. The template to use.</param>
    public ShieldsBadgeResponse(string label,
                                string message,
                                BadgeColor? labelColor = null,
                                BadgeColor? messageColor = null,
                                ShieldLogo? logo = null,
                                bool isError = false,
                                ShieldStyle? style = null) {
        this.label      = label;
        this.message    = message;
        this.isError    = isError;
        this.style      = style;
        color           = serializeColor(messageColor);
        this.labelColor = serializeColor(labelColor);

        static string? serializeColor(BadgeColor? color) => color?.ValueIndex switch {
            Union3Index.Value1 => color.Value.Value1.toText(),
            Union3Index.Value2 => color.Value.Value2.toRgbHexColor(),
            Union3Index.Value3 => color.Value.Value3.EmptyToNull(),
            null               => null
        };

        switch (logo) {
            case { logoXml: { } xml }:
                logoSvg = xml;
                break;
            case { simpleIconSlug: { } iconName }:
                namedLogo = iconName;
                logoSize  = logo.autoSizeSimpleIcon ? "auto" : null;
                logoColor = logo.simpleIconColor;
                break;
        }
    }

    public int schemaVersion { get; } = 1;

    public string? color { get; }
    public string label { get; }
    public string message { get; }
    public string? labelColor { get; }
    public bool isError { get; }
    public string? logoSvg { get; }
    public string? namedLogo { get; }
    public string? logoSize { get; }
    public string? logoColor { get; }
    public ShieldStyle? style { get; }

}