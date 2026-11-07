using System.Drawing;

namespace EmployeePortal.Integrations.Imaging;

/// <summary>
/// Exercises types from System.Drawing.Common 4.7.0 (SCA test case for CVE-2021-24112 / GHSA-rxg9-xrhp-64gj).
/// Uses value types (Size/Rectangle/Color) so it executes safely across platforms without invoking native GDI+ handles.
/// </summary>
public class BadgeAvatarProcessor : IAvatarProcessor
{
    public string GetBadgeDimensionsMetadata(int width, int height)
    {
        var badgeSize = new Size(Math.Max(width, 64), Math.Max(height, 64));
        var boundingBox = new Rectangle(Point.Empty, badgeSize);
        var borderColor = Color.FromArgb(255, 30, 64, 175);
        return $"BadgeBounds={boundingBox.Width}x{boundingBox.Height};Color=#{borderColor.R:X2}{borderColor.G:X2}{borderColor.B:X2}";
    }
}

