using System.Drawing;
using System.Xml;
using System.Xml.Linq;
using UnionTypes;

namespace FreshBadge.Data.Shields;

/// <summary>
/// Custom icon on the left side of the badge.
/// </summary>
public class ShieldLogo {

    public string? logoXml { get; }
    public string? simpleIconSlug { get; }
    public bool autoSizeSimpleIcon { get; }
    public string? simpleIconColor { get; }

    /// <summary>
    /// Embed an SVG image in the badge as its logo.
    /// </summary>
    /// <param name="svg">An SVG XML document</param>
    public ShieldLogo(XmlDocument svg) {
        logoXml = svg.OuterXml;
    }

    /// <summary>
    /// Embed an SVG image in the badge as its logo.
    /// </summary>
    /// <param name="svg">An SVG XML document</param>
    public ShieldLogo(XDocument svg) {
        logoXml = svg.ToString(SaveOptions.DisableFormatting);
    }

    /// <summary>
    /// Embed an SVG image in the badge as its logo.
    /// </summary>
    /// <param name="svg">A serialized SVG XML document</param>
    public ShieldLogo(string svg) {
        logoXml = svg;
    }

    /// <summary>
    /// Use an icon from Simple Icons (<see href="https://simpleicons.org"/>) as the badge's logo.
    /// </summary>
    /// <param name="simpleIconSlug">The slug name of the icon in Simple Icons that appears in the file basename when you download the SVG, such as <c>dotnet</c> or <c>1dot1dot1dot1</c> (NOT the display name, like <c>.NET</c> or <c>1.1.1.1</c>).</param>
    /// <param name="autoSize"><c>true</c> to allow the icon to be wider than a square (fit to height), useful for wide logos like <c>amg</c> and <c>amd</c>. When set to <c>false</c>, constrains the icon to a 1:1 aspect ratio (fit to width and height).</param>
    /// <param name="color">To change the fill color of the icon, or <c>null</c> to use its original color (white).</param>
    public ShieldLogo(string simpleIconSlug, bool autoSize, Union<Color, string>? color = null) {
        this.simpleIconSlug = simpleIconSlug;
        autoSizeSimpleIcon  = autoSize;
        simpleIconColor = color switch {
            { HasValue1: true, Value1: var c } => c.toRgbHexColor(),
            { HasValue2: true, Value2: var c } => c,
            _                                  => null
        };
    }

}