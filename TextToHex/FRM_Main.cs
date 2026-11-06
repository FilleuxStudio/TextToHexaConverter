using System;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TextToHex.UI;

namespace TextToHex
{
    public partial class FRM_Main : Form
    {
        private sealed class StatusInfo
        {
            public StatusKind Kind;
            public string Title, Detail;
        }

        private static readonly int[] Radixes = { 10, 16, 2, 8 };

        private readonly Font _headerTitleFont = Theme.Font(17f, FontStyle.Bold);
        private readonly Font _headerSubFont = Theme.Font(9.5f);
        private readonly Font _headerTagFont = Theme.Font(9f);

        private byte[] _bytes = new byte[0];   // dernière suite d'octets valide
        private bool _updating;                // évite les boucles texte <-> hexa
        private bool _lastEditWasHex;          // quelle zone l'utilisateur a modifiée en dernier

        private StatusInfo _textStatus = new StatusInfo
        {
            Kind = StatusKind.Info,
            Title = "Saisissez du texte ou de l'hexadécimal",
            Detail = "La conversion est instantanée dans les deux sens."
        };
        private StatusInfo _numberStatus = new StatusInfo
        {
            Kind = StatusKind.Info,
            Title = "Saisissez un nombre",
            Detail = "Décimal, hexadécimal (0x), binaire (0b) ou octal (0o)."
        };

        public FRM_Main()
        {
            InitializeComponent();

            Icon = LoadAppIcon();
            FitToScreen();
            ApplyStatus();
        }

        private static Icon LoadAppIcon()
        {
            try { return Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { return null; }   // icône par défaut
        }

        /// <summary>Évite une fenêtre plus grande que l'écran (portables 1366x768).</summary>
        private void FitToScreen()
        {
            Rectangle wa = Screen.FromPoint(Cursor.Position).WorkingArea;
            int chromeW = Width - ClientSize.Width, chromeH = Height - ClientSize.Height;
            int w = Math.Min(ClientSize.Width, wa.Width - chromeW - Theme.Dp(16));
            int h = Math.Min(ClientSize.Height, wa.Height - chromeH - Theme.Dp(16));
            MinimumSize = new Size(Math.Min(MinimumSize.Width, w + chromeW), Math.Min(MinimumSize.Height, h + chromeH));
            ClientSize = new Size(w, h);
        }

        // ------------------------------------------------------------------
        //  En-tête / pied de page
        // ------------------------------------------------------------------

        private void PNL_Header_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Theme.HighQuality(g);

            int logo = Theme.Dp(46);
            int x = Theme.Dp(24);
            int y = (PNL_Header.Height - logo) / 2;
            Theme.DrawLogo(g, new RectangleF(x, y, logo, logo));

            int tx = x + logo + Theme.Dp(14);
            int tw = Math.Max(50, PNL_Header.Width - tx - Theme.Dp(120));
            Theme.DrawText(g, "Texte vers Hexadécimal", _headerTitleFont, Theme.Text,
                new Rectangle(tx, y - Theme.Dp(3), tw, Theme.Dp(28)), TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            Theme.DrawText(g, "Convertissez du texte lisible en octets hexadécimaux, et inversement", _headerSubFont, Theme.TextMuted,
                new Rectangle(tx, y + Theme.Dp(26), tw, Theme.Dp(20)), TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            string[] v = Application.ProductVersion.Split('.');
            string tag = "v" + v[0] + "." + (v.Length > 1 ? v[1] : "0");
            Size ts = TextRenderer.MeasureText(tag, _headerTagFont);
            var pill = new RectangleF(PNL_Header.Width - Theme.Dp(24) - ts.Width - Theme.Dp(20), (PNL_Header.Height - Theme.Dp(26)) / 2f, ts.Width + Theme.Dp(20), Theme.Dp(26));
            Theme.FillRound(g, Theme.AccentSoft, pill, pill.Height / 2f);
            Theme.DrawText(g, tag, _headerTagFont, Theme.Accent, Rectangle.Round(pill), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private void PNL_Footer_Layout(object sender, LayoutEventArgs e)
        {
            int margin = Theme.Dp(24);
            int h = PNL_Footer.ClientSize.Height;
            BT_About.Location = new Point(PNL_Footer.ClientSize.Width - margin - BT_About.Width, (h - BT_About.Height) / 2);
            LBL_Status.SetBounds(margin, Theme.Dp(8), Math.Max(50, BT_About.Left - margin - Theme.Dp(16)), h - Theme.Dp(16));
        }

        private void BT_About_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this,
                "Texte vers Hexadécimal  v" + Application.ProductVersion + "\n\n" +
                "Convertit du texte en hexadécimal (et inversement) selon l'encodage choisi, " +
                "et convertit des nombres entre décimal, hexadécimal, binaire et octal.\n\n" +
                "FilleuxStudio — http://www.filleuxstudio.fr/",
                "À propos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ------------------------------------------------------------------
        //  Statut
        // ------------------------------------------------------------------

        private bool OnTextPage { get { return SEG_Tabs.SelectedIndex == 0; } }

        private void SetStatus(bool textPage, StatusKind kind, string title, string detail)
        {
            StatusInfo s = textPage ? _textStatus : _numberStatus;
            s.Kind = kind; s.Title = title; s.Detail = detail;
            if (textPage == OnTextPage) ApplyStatus();
        }

        private void ApplyStatus()
        {
            StatusInfo s = OnTextPage ? _textStatus : _numberStatus;
            LBL_Status.Set(s.Kind, s.Title, s.Detail);
        }

        // ------------------------------------------------------------------
        //  Onglets
        // ------------------------------------------------------------------

        private void SEG_Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            PAGE_Text.Visible = OnTextPage;
            PAGE_Numbers.Visible = !OnTextPage;
            ApplyStatus();
        }

        // ------------------------------------------------------------------
        //  Page 1 : texte <-> hexadécimal
        // ------------------------------------------------------------------

        private TextEncodingKind CurrentEncoding { get { return (TextEncodingKind)SEG_Encoding.SelectedIndex; } }
        private HexFormat CurrentFormat { get { return (HexFormat)SEG_Format.SelectedIndex; } }
        private bool UpperCase { get { return SEG_Case.SelectedIndex == 0; } }

        private void SetHexSilently(string hex)
        {
            _updating = true;
            try { TXT_Hex.Value = hex; }
            finally { _updating = false; }
        }

        private void SetTextSilently(string text)
        {
            _updating = true;
            try { TXT_Text.Value = text; }
            finally { _updating = false; }
        }

        private void TXT_Text_TextChanged(object sender, EventArgs e)
        {
            if (_updating) return;
            _lastEditWasHex = false;
            RecomputeFromText();
        }

        private void TXT_Hex_TextChanged(object sender, EventArgs e)
        {
            if (_updating) return;
            _lastEditWasHex = true;
            RecomputeFromHex(false);
        }

        /// <summary>La zone « Texte » fait foi : on l'encode puis on met en forme l'hexadécimal.</summary>
        private void RecomputeFromText()
        {
            try
            {
                string text = TXT_Text.Value;
                string warning;
                _bytes = HexConverter.Encode(text, CurrentEncoding, out warning);
                SetHexSilently(HexConverter.ToHex(_bytes, CurrentFormat, UpperCase));
                ShowCounts(text, warning);
            }
            catch (Exception ex)
            {
                SetStatus(true, StatusKind.Error, "Conversion impossible", ex.Message);
            }
        }

        /// <summary>
        /// La zone « Hexadécimal » fait foi : on la lit puis on décode le texte.
        /// reformat = true : on réécrit aussi l'hexadécimal dans la présentation choisie.
        /// </summary>
        private void RecomputeFromHex(bool reformat)
        {
            try
            {
                byte[] bytes;
                string error;
                if (!HexConverter.TryParseHex(TXT_Hex.Value, out bytes, out error))
                {
                    SetStatus(true, StatusKind.Error, "Hexadécimal invalide", error);
                    return;
                }

                _bytes = bytes;
                string warning;
                string text = HexConverter.Decode(bytes, CurrentEncoding, out warning);
                SetTextSilently(text);
                if (reformat) SetHexSilently(HexConverter.ToHex(bytes, CurrentFormat, UpperCase));
                ShowCounts(text, warning);
            }
            catch (Exception ex)
            {
                SetStatus(true, StatusKind.Error, "Conversion impossible", ex.Message);
            }
        }

        private void ShowCounts(string text, string warning)
        {
            int chars = 0;
            try { chars = string.IsNullOrEmpty(text) ? 0 : new StringInfo(text).LengthInTextElements; }
            catch { chars = text.Length; }

            string title = chars + (chars > 1 ? " caractères" : " caractère") + "  ·  " +
                           _bytes.Length + (_bytes.Length > 1 ? " octets" : " octet") +
                           "  ·  " + HexConverter.EncodingLabel(CurrentEncoding);

            if (warning != null) SetStatus(true, StatusKind.Warning, title, warning);
            else SetStatus(true, StatusKind.Info, title, _bytes.Length == 0 ? "Saisissez du texte ou de l'hexadécimal : la conversion est instantanée." : "Conversion à jour.");
        }

        private void SEG_Encoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Un autre encodage change les octets (é : C3 A9 en UTF-8, E9 en Windows-1252).
            if (_lastEditWasHex) RecomputeFromHex(false);
            else RecomputeFromText();
        }

        private void SEG_Format_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Présentation seule : on réécrit l'hexadécimal à partir de ce qui est valide.
            byte[] bytes;
            string error;
            if (HexConverter.TryParseHex(TXT_Hex.Value, out bytes, out error))
            {
                _bytes = bytes;
                SetHexSilently(HexConverter.ToHex(bytes, CurrentFormat, UpperCase));
            }
        }

        private void BT_ClearText_Click(object sender, EventArgs e)
        {
            TXT_Text.Value = "";
            TXT_Hex.Value = "";
            _bytes = new byte[0];
            TXT_Text.Inner.Focus();
            ShowCounts("", null);
        }

        private void BT_PasteText_Click(object sender, EventArgs e) { PasteInto(TXT_Text, true); }
        private void BT_PasteHex_Click(object sender, EventArgs e) { PasteInto(TXT_Hex, true); }
        private void BT_CopyHex_Click(object sender, EventArgs e) { CopyText(TXT_Hex.Value, true, "Hexadécimal copié"); }

        // ------------------------------------------------------------------
        //  Page 2 : nombres
        // ------------------------------------------------------------------

        private void NumberInput_Changed(object sender, EventArgs e)
        {
            UpdateNumbers();
        }

        private void UpdateNumbers()
        {
            try
            {
                int radix = Radixes[SEG_Base.SelectedIndex];
                BigInteger value;
                int usedRadix;
                string error;

                if (!NumberConverter.TryParse(TXT_Number.Value, radix, out value, out usedRadix, out error))
                {
                    TXT_Dec.Value = TXT_HexNum.Value = TXT_Bin.Value = TXT_Oct.Value = "";
                    if (error != null) SetStatus(false, StatusKind.Error, "Nombre invalide", error);
                    else SetStatus(false, StatusKind.Info, "Saisissez un nombre", "Décimal, hexadécimal (0x), binaire (0b) ou octal (0o).");
                    return;
                }

                bool group = SW_Group.Checked, prefix = SW_Prefix.Checked, upper = SW_Upper.Checked;
                TXT_Dec.Value = NumberConverter.Format(value, 10, group, false, upper);
                TXT_HexNum.Value = NumberConverter.Format(value, 16, group, prefix, upper);
                TXT_Bin.Value = NumberConverter.Format(value, 2, group, prefix, upper);
                TXT_Oct.Value = NumberConverter.Format(value, 8, group, prefix, upper);

                int bits = NumberConverter.BitLength(value);
                int octets = (bits + 7) / 8;
                string detail = usedRadix != radix ? "Préfixe reconnu : saisie lue en base " + usedRadix + " (" + NumberConverter.RadixName(usedRadix) + ")." : "Conversion à jour.";
                if (value.Sign < 0) detail += " Nombre négatif : signe « - » devant la valeur absolue.";
                SetStatus(false, StatusKind.Info, bits + (bits > 1 ? " bits" : " bit") + "  ·  " + octets + (octets > 1 ? " octets" : " octet"), detail);
            }
            catch (Exception ex)
            {
                SetStatus(false, StatusKind.Error, "Conversion impossible", ex.Message);
            }
        }

        private void BT_ClearNumber_Click(object sender, EventArgs e)
        {
            TXT_Number.Value = "";
            TXT_Number.Inner.Focus();
        }

        private void BT_CopyDec_Click(object sender, EventArgs e) { CopyText(TXT_Dec.Value, false, "Valeur décimale copiée"); }
        private void BT_CopyHexNum_Click(object sender, EventArgs e) { CopyText(TXT_HexNum.Value, false, "Valeur hexadécimale copiée"); }
        private void BT_CopyBin_Click(object sender, EventArgs e) { CopyText(TXT_Bin.Value, false, "Valeur binaire copiée"); }
        private void BT_CopyOct_Click(object sender, EventArgs e) { CopyText(TXT_Oct.Value, false, "Valeur octale copiée"); }

        // ------------------------------------------------------------------
        //  Presse-papiers (peut être verrouillé par une autre application)
        // ------------------------------------------------------------------

        private void CopyText(string text, bool textPage, string successTitle)
        {
            if (string.IsNullOrEmpty(text))
            {
                SetStatus(textPage, StatusKind.Warning, "Rien à copier", "Le résultat est vide.");
                return;
            }
            try
            {
                Clipboard.SetDataObject(text, true, 5, 100);
                SetStatus(textPage, StatusKind.Success, successTitle, "Le contenu est dans le presse-papiers.");
            }
            catch (ExternalException)
            {
                SetStatus(textPage, StatusKind.Error, "Presse-papiers indisponible", "Une autre application l'utilise. Réessayez dans un instant.");
            }
        }

        private void PasteInto(FieldBox target, bool textPage)
        {
            try
            {
                if (!Clipboard.ContainsText())
                {
                    SetStatus(textPage, StatusKind.Warning, "Presse-papiers vide", "Il ne contient pas de texte.");
                    return;
                }
                string text = Clipboard.GetText();
                if (text.Length > HexConverter.MaxInputLength)
                {
                    SetStatus(textPage, StatusKind.Error, "Contenu trop volumineux", "Maximum " + HexConverter.MaxInputLength.ToString("N0", CultureInfo.CurrentCulture) + " caractères.");
                    return;
                }
                target.Value = text;     // déclenche la conversion
                target.Inner.Focus();
            }
            catch (ExternalException)
            {
                SetStatus(textPage, StatusKind.Error, "Presse-papiers indisponible", "Une autre application l'utilise. Réessayez dans un instant.");
            }
        }

        // ------------------------------------------------------------------
        //  Raccourcis clavier
        // ------------------------------------------------------------------

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.D1)) { SEG_Tabs.SelectedIndex = 0; return true; }
            if (keyData == (Keys.Control | Keys.D2)) { SEG_Tabs.SelectedIndex = 1; return true; }
            if (keyData == (Keys.Control | Keys.Shift | Keys.C) && OnTextPage) { BT_CopyHex_Click(this, EventArgs.Empty); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (OnTextPage) TXT_Text.Inner.Focus();
        }
    }
}
