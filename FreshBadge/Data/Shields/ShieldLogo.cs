using System.Xml;
using System.Xml.Linq;

namespace FreshBadge.Data.Shields;

public class ShieldLogo {

    public string? logoXml { get; }
    public string? simpleIconSlug { get; }
    public bool autoSizeSimpleIcon { get; }

    public ShieldLogo(XmlDocument xml) {
        logoXml = xml.OuterXml;
    }

    public ShieldLogo(XDocument xml) {
        logoXml = xml.ToString(SaveOptions.DisableFormatting);
    }

    public ShieldLogo(string xml) {
        logoXml = xml;
    }

    public ShieldLogo(string simpleIconSlug, bool autoSize) {
        this.simpleIconSlug = simpleIconSlug;
        autoSizeSimpleIcon  = autoSize;
    }

}