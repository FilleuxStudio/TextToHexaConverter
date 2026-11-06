using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace TextToHex.UI
{
    /// <summary>Palette, polices et utilitaires de dessin partagés par tous les contrôles.</summary>
    internal static class Theme
    {
        // --- Palette -------------------------------------------------------
        public static readonly Color Background   = ColorTranslator.FromHtml("#F2F6F9");
        public static readonly Color Surface      = Color.White;
        public static readonly Color Border       = ColorTranslator.FromHtml("#DDE5EB");
        public static readonly Color BorderStrong = ColorTranslator.FromHtml("#BFCBD6");
        public static readonly Color Text         = ColorTranslator.FromHtml("#14212B");
        public static readonly Color TextMuted    = ColorTranslator.FromHtml("#5E6F7D");
        public static readonly Color Accent       = ColorTranslator.FromHtml("#0E7490");
        public static readonly Color AccentHover  = ColorTranslator.FromHtml("#155E75");
        public static readonly Color AccentPress  = ColorTranslator.FromHtml("#164E63");
        public static readonly Color AccentSoft   = ColorTranslator.FromHtml("#E3F4F8");
        public static readonly Color AccentDisabled = ColorTranslator.FromHtml("#9CC9D4");
        public static readonly Color Success      = ColorTranslator.FromHtml("#0F9D58");
        public static readonly Color Warning      = ColorTranslator.FromHtml("#C77700");
        public static readonly Color Danger       = ColorTranslator.FromHtml("#D92D20");
        public static readonly Color LogoStart    = ColorTranslator.FromHtml("#0891B2");
        public static readonly Color LogoEnd      = ColorTranslator.FromHtml("#4F46E5");

        public static readonly Color CheckerLight = Color.White;
        public static readonly Color CheckerDark  = ColorTranslator.FromHtml("#E4EAF0");

        // --- Échelle DPI ---------------------------------------------------
        private static float _scale;

        public static float Scale
        {
            get
            {
                if (_scale <= 0f)
                {
                    try
                    {
                        using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                            _scale = g.DpiX / 96f;
                    }
                    catch { _scale = 1f; }
                    if (_scale < 1f) _scale = 1f;
                }
                return _scale;
            }
        }

        /// <summary>Convertit des pixels "96 dpi" en pixels réels de l'écran.</summary>
        public static int Dp(int value)
        {
            return (int)Math.Round(value * Scale);
        }

        public static float Dp(float value)
        {
            return value * Scale;
        }

        // --- Polices -------------------------------------------------------
        private static string _family;

        public static string FontFamilyName
        {
            get
            {
                if (_family == null)
                {
                    _family = "Segoe UI";
                    try
                    {
                        using (var f = new FontFamily("Segoe UI")) { }
                    }
                    catch { _family = "Microsoft Sans Serif"; }
                }
                return _family;
            }
        }

        public static Font Font(float points, FontStyle style)
        {
            return new Font(FontFamilyName, points, style, GraphicsUnit.Point);
        }

        public static Font Font(float points)
        {
            return Font(points, FontStyle.Regular);
        }

        // --- Dessin --------------------------------------------------------
        public static void HighQuality(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        }

        public static GraphicsPath RoundRect(RectangleF r, float radius)
        {
            var path = new GraphicsPath();
            float d = Math.Min(radius * 2f, Math.Min(r.Width, r.Height));
            if (d <= 0.5f)
            {
                path.AddRectangle(r);
                return path;
            }
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        public static void FillRound(Graphics g, Color color, RectangleF r, float radius)
        {
            using (GraphicsPath p = RoundRect(r, radius))
            using (var b = new SolidBrush(color))
                g.FillPath(b, p);
        }

        public static void StrokeRound(Graphics g, Color color, float width, RectangleF r, float radius)
        {
            using (GraphicsPath p = RoundRect(r, radius))
            using (var pen = new Pen(color, width))
                g.DrawPath(pen, p);
        }

        /// <summary>Damier de transparence, style éditeurs d'images.</summary>
        public static void DrawChecker(Graphics g, Rectangle bounds, int cell)
        {
            using (var light = new SolidBrush(CheckerLight))
            using (var dark = new SolidBrush(CheckerDark))
            {
                g.FillRectangle(light, bounds);
                int rows = (bounds.Height + cell - 1) / cell;
                int cols = (bounds.Width + cell - 1) / cell;
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        if (((x + y) & 1) == 0) continue;
                        int w = Math.Min(cell, bounds.Width - x * cell);
                        int h = Math.Min(cell, bounds.Height - y * cell);
                        g.FillRectangle(dark, bounds.X + x * cell, bounds.Y + y * cell, w, h);
                    }
                }
            }
        }

        public static void DrawText(Graphics g, string text, Font font, Color color, Rectangle bounds, TextFormatFlags flags)
        {
            TextRenderer.DrawText(g, text, font, bounds, color,
                flags | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis | TextFormatFlags.PreserveGraphicsClipping);
        }

        /// <summary>Police à chasse fixe (Consolas, sinon Courier New).</summary>
        public static Font MonoFont(float points, FontStyle style)
        {
            string family = "Consolas";
            try { using (var f = new FontFamily("Consolas")) { } }
            catch { family = "Courier New"; }
            return new Font(family, points, style, GraphicsUnit.Point);
        }

        public static Font MonoFont(float points)
        {
            return MonoFont(points, FontStyle.Regular);
        }

        /// <summary>Logo de l'application : carré dégradé avec « 0x », repris pour l'icône de l'exécutable.</summary>
        public static void DrawLogo(Graphics g, RectangleF r)
        {
            HighQuality(g);
            using (GraphicsPath p = RoundRect(r, r.Width * 0.24f))
            using (var br = new LinearGradientBrush(r, LogoStart, LogoEnd, 45f))
                g.FillPath(br, p);

            using (Font f = MonoFont(1f, FontStyle.Bold))
            using (var big = new Font(f.FontFamily, r.Width * 0.40f, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.DrawString("0x", big, Brushes.White, new RectangleF(r.X, r.Y - r.Height * 0.03f, r.Width, r.Height), sf);
            }
        }
    }
}
