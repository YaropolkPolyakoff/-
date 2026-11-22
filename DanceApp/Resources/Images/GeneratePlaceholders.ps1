# GeneratePlaceholders.ps1
# Run this script once to create placeholder PNG cover images.
# Requires .NET / PowerShell 5+
#
# Usage:  Set-Location DanceApp\Resources\Images
#         .\GeneratePlaceholders.ps1

Add-Type -AssemblyName System.Drawing

$items = @(
    @{ Name="hiphop";     Label="Hip-Hop";           BgR=40;  BgG=20;  BgB=10;  AccR=255; AccG=107; AccB=53  },
    @{ Name="breakdance"; Label="Breakdance";         BgR=10;  BgG=30;  BgB=40;  AccR=58;  AccG=255; AccB=216 },
    @{ Name="kpop";       Label="K-Pop Cover Dance";  BgR=40;  BgG=10;  BgB=30;  AccR=255; AccG=60;  AccB=172 },
    @{ Name="highheels";  Label="High Heels";         BgR=20;  BgG=10;  BgB=40;  AccR=168; AccG=85;  AccB=247 }
)

foreach ($item in $items) {
    $bmp  = New-Object System.Drawing.Bitmap(240, 300)
    $g    = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    # Background gradient
    $topColor    = [System.Drawing.Color]::FromArgb(255, $item.BgR,  $item.BgG,  $item.BgB)
    $bottomColor = [System.Drawing.Color]::FromArgb(255, [Math]::Max(0,$item.BgR-10),
                                                          [Math]::Max(0,$item.BgG-10),
                                                          [Math]::Max(0,$item.BgB-10))
    $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
        [System.Drawing.Point]::new(0,0),
        [System.Drawing.Point]::new(0,300),
        $topColor, $bottomColor)
    $g.FillRectangle($brush, 0, 0, 240, 300)
    $brush.Dispose()

    # Accent bar at top
    $accentBrush = New-Object System.Drawing.SolidBrush(
        [System.Drawing.Color]::FromArgb(255, $item.AccR, $item.AccG, $item.AccB))
    $g.FillRectangle($accentBrush, 0, 0, 240, 5)
    $accentBrush.Dispose()

    # Label text
    $font      = New-Object System.Drawing.Font("Segoe UI", 18, [System.Drawing.FontStyle]::Bold)
    $textBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
    $sf        = New-Object System.Drawing.StringFormat
    $sf.Alignment     = [System.Drawing.StringAlignment]::Center
    $sf.LineAlignment = [System.Drawing.StringAlignment]::Far
    $g.DrawString($item.Label, $font, $textBrush, [System.Drawing.RectangleF]::new(16, 0, 208, 280), $sf)
    $font.Dispose()
    $textBrush.Dispose()

    $g.Dispose()

    $outPath = Join-Path $PSScriptRoot "$($item.Name).png"
    $bmp.Save($outPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    Write-Host "Created $outPath"
}
Write-Host "Done."
