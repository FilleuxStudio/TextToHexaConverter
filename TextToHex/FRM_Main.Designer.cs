using System;
using System.Drawing;
using System.Windows.Forms;
using TextToHex.UI;

namespace TextToHex
{
    partial class FRM_Main
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (disposing)
            {
                _headerTitleFont.Dispose();
                _headerSubFont.Dispose();
                _headerTagFont.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code de construction de l'interface

        // En-tête / pied de page
        private SurfacePanel PNL_Header;
        private SurfacePanel PNL_Footer;
        private StatusLine LBL_Status;
        private PillButton BT_About;

        // Corps
        private Panel PNL_Body;
        private Panel PNL_TabBar;
        private SegmentedControl SEG_Tabs;
        private Panel PNL_Pages;

        // Page 1 : texte <-> hexadécimal
        private TableLayoutPanel PAGE_Text;
        private CardPanel CARD_Text;
        private FieldBox TXT_Text;
        private PillButton BT_PasteText;
        private PillButton BT_ClearText;
        private CardPanel CARD_Hex;
        private FieldBox TXT_Hex;
        private PillButton BT_CopyHex;
        private PillButton BT_PasteHex;
        private CardPanel CARD_Options;
        private SegmentedControl SEG_Encoding;
        private SegmentedControl SEG_Format;
        private SegmentedControl SEG_Case;

        // Page 2 : nombres
        private TableLayoutPanel PAGE_Numbers;
        private CardPanel CARD_NumIn;
        private SegmentedControl SEG_Base;
        private FieldBox TXT_Number;
        private Label LBL_NumHint;
        private ToggleSwitch SW_Group;
        private ToggleSwitch SW_Prefix;
        private ToggleSwitch SW_Upper;
        private PillButton BT_ClearNumber;
        private CardPanel CARD_NumOut;
        private FieldBox TXT_Dec;
        private FieldBox TXT_HexNum;
        private FieldBox TXT_Bin;
        private FieldBox TXT_Oct;
        private PillButton BT_CopyDec;
        private PillButton BT_CopyHexNum;
        private PillButton BT_CopyBin;
        private PillButton BT_CopyOct;

        // --- Fabriques de contrôles ---------------------------------------

        private static Label MakeLabel(string text, bool bold, Color color)
        {
            return new Label
            {
                Text = text,
                AutoSize = false,
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                BackColor = Theme.Surface,
                ForeColor = color,
                Font = Theme.Font(bold ? 9f : 9.5f, bold ? FontStyle.Bold : FontStyle.Regular),
                TextAlign = ContentAlignment.BottomLeft
            };
        }

        private static PillButton MakeSmallButton(string text, ButtonKind kind, EventHandler onClick)
        {
            var b = new PillButton
            {
                Kind = kind,
                Text = text,
                Font = Theme.Font(9f, FontStyle.Bold),
                Margin = new Padding(0)
            };
            b.Size = new Size(TextRenderer.MeasureText(text, b.Font).Width + Theme.Dp(26), Theme.Dp(30));
            b.Click += onClick;
            return b;
        }

        private static TableLayoutPanel MakeTable(int cols, int rows)
        {
            var t = new TableLayoutPanel
            {
                ColumnCount = cols,
                RowCount = rows,
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                BackColor = Theme.Surface
            };
            return t;
        }

        private static CardPanel MakeCard(string title, Padding margin)
        {
            return new CardPanel { Title = title, Dock = DockStyle.Fill, Margin = margin };
        }

        private static void Abs(TableLayoutPanel t, int dp) { t.RowStyles.Add(new RowStyle(SizeType.Absolute, Theme.Dp(dp))); }
        private static void Fill(TableLayoutPanel t) { t.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); }
        private static void ColPct(TableLayoutPanel t, float pct) { t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, pct)); }
        private static void ColAbs(TableLayoutPanel t, int dp) { t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Theme.Dp(dp))); }

        private static string[] Arr(params string[] items) { return items; }

        private FieldBox MakeOutput(out PillButton copy, string copyText, EventHandler onCopy)
        {
            var f = new FieldBox { ReadOnly = true, Monospace = true };
            copy = new PillButton
            {
                Text = copyText,
                Font = Theme.Font(9f, FontStyle.Bold)
            };
            copy.Click += onCopy;
            return f;
        }

        // Pile verticale : des panneaux « Dock = Top » (plus fiable qu'un TableLayoutPanel à lignes dynamiques).
        private sealed class StackBuilder
        {
            private readonly System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<Control, int>> _rows = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<Control, int>>();

            public void Add(Control c, int heightDp) { _rows.Add(new System.Collections.Generic.KeyValuePair<Control, int>(c, heightDp)); }
            public void Gap(int heightDp) { Add(new Panel { BackColor = Theme.Surface }, heightDp); }

            public Panel Build()
            {
                var p = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Surface };
                // Le dernier contrôle ajouté se positionne en premier : on ajoute donc à l'envers.
                for (int i = _rows.Count - 1; i >= 0; i--)
                {
                    Control c = _rows[i].Key;
                    c.Dock = DockStyle.Top;
                    c.Height = Theme.Dp(_rows[i].Value);
                    p.Controls.Add(c);
                }
                return p;
            }
        }

        private void AddOutputRow(StackBuilder st, string label, FieldBox field, PillButton button)
        {
            st.Add(MakeLabel(label, true, Theme.TextMuted), 22);

            var line = new Panel { BackColor = Theme.Surface };
            field.Dock = DockStyle.Fill;
            button.Dock = DockStyle.Right;
            button.Width = Theme.Dp(92);
            var spacer = new Panel { Dock = DockStyle.Right, Width = Theme.Dp(8), BackColor = Theme.Surface };
            line.Controls.Add(field);     // Fill : ajouté en premier
            line.Controls.Add(spacer);
            line.Controls.Add(button);    // le plus à droite
            st.Add(line, 42);
            st.Gap(16);
        }

        private void AddToggleRow(StackBuilder st, ToggleSwitch sw, string text, EventHandler onChange)
        {
            sw.CheckedChanged += onChange;
            var l = MakeLabel(text, false, Theme.Text);
            l.TextAlign = ContentAlignment.MiddleLeft;
            l.Padding = new Padding(Theme.Dp(54), 0, 0, 0);
            l.Click += delegate { sw.Checked = !sw.Checked; };

            var line = new Panel { BackColor = Theme.Surface };
            sw.Location = new Point(0, (Theme.Dp(36) - sw.Height) / 2);
            line.Controls.Add(sw);
            line.Controls.Add(l);
            sw.BringToFront();
            st.Add(line, 36);
        }

        /// <summary>
        /// Construit toute l'interface. Les contrôles personnalisés sont dessinés dans le dossier UI.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();

            // ---------------- En-tête ----------------
            PNL_Header = new SurfacePanel { Dock = DockStyle.Top, Height = Theme.Dp(78), Edge = PanelEdge.Bottom, TabStop = false };
            PNL_Header.Paint += PNL_Header_Paint;

            // ---------------- Pied de page ----------------
            PNL_Footer = new SurfacePanel { Dock = DockStyle.Bottom, Height = Theme.Dp(70), Edge = PanelEdge.Top };
            LBL_Status = new StatusLine { TabStop = false };
            BT_About = new PillButton { Text = "À propos", Kind = ButtonKind.Ghost, Font = Theme.Font(9.5f, FontStyle.Bold), Size = new Size(Theme.Dp(96), Theme.Dp(38)), TabIndex = 90 };
            BT_About.Click += BT_About_Click;
            PNL_Footer.Controls.Add(BT_About);
            PNL_Footer.Controls.Add(LBL_Status);
            PNL_Footer.Layout += PNL_Footer_Layout;

            // ---------------- Corps : onglets + pages ----------------
            PNL_Body = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background, Padding = new Padding(Theme.Dp(24), Theme.Dp(8), Theme.Dp(24), Theme.Dp(24)) };
            PNL_Pages = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };
            PNL_TabBar = new Panel { Dock = DockStyle.Top, Height = Theme.Dp(58), BackColor = Theme.Background };
            SEG_Tabs = new SegmentedControl
            {
                Items = Arr("Texte  ↔  Hexadécimal", "Nombres"),
                ToolTips = Arr("Convertit un texte en octets hexadécimaux, et inversement", "Convertit un nombre entre décimal, hexadécimal, binaire et octal"),
                FontSize = 10f,
                Location = new Point(0, Theme.Dp(8)),
                Size = new Size(Theme.Dp(360), Theme.Dp(42)),
                TabIndex = 0
            };
            SEG_Tabs.SelectedIndexChanged += SEG_Tabs_SelectedIndexChanged;
            PNL_TabBar.Controls.Add(SEG_Tabs);

            BuildTextPage();
            BuildNumbersPage();
            PNL_Pages.Controls.Add(PAGE_Text);
            PNL_Pages.Controls.Add(PAGE_Numbers);
            PNL_Body.Controls.Add(PNL_Pages);
            PNL_Body.Controls.Add(PNL_TabBar);

            // ---------------- Fenêtre ----------------
            this.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = Theme.Background;
            this.ClientSize = new Size(Theme.Dp(960), Theme.Dp(700));
            this.Controls.Add(PNL_Body);
            this.Controls.Add(PNL_Footer);
            this.Controls.Add(PNL_Header);
            this.Font = Theme.Font(9f);
            this.KeyPreview = true;
            this.MinimumSize = new Size(Theme.Dp(880), Theme.Dp(620));
            this.Name = "FRM_Main";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Texte vers Hexadécimal";
            this.ResumeLayout(false);
        }

        private void BuildTextPage()
        {
            PAGE_Text = MakeTable(2, 2);
            PAGE_Text.Dock = DockStyle.Fill;
            PAGE_Text.BackColor = Theme.Background;
            ColPct(PAGE_Text, 50F); ColPct(PAGE_Text, 50F);
            Fill(PAGE_Text); Abs(PAGE_Text, 134);

            // Carte « Texte »
            CARD_Text = MakeCard("Texte", new Padding(0, 0, Theme.Dp(8), Theme.Dp(16)));
            TXT_Text = new FieldBox
            {
                Dock = DockStyle.Fill, Multiline = true, Placeholder = "Tapez ou collez votre texte ici…", TabIndex = 1
            };
            TXT_Text.Inner.MaxLength = HexConverter.MaxInputLength;
            TXT_Text.TextValueChanged += TXT_Text_TextChanged;
            CARD_Text.Controls.Add(TXT_Text);
            BT_ClearText = MakeSmallButton("Effacer", ButtonKind.Ghost, BT_ClearText_Click);
            BT_PasteText = MakeSmallButton("Coller", ButtonKind.Ghost, BT_PasteText_Click);
            CARD_Text.AddHeaderControl(BT_ClearText);
            CARD_Text.AddHeaderControl(BT_PasteText);

            // Carte « Hexadécimal »
            CARD_Hex = MakeCard("Hexadécimal", new Padding(Theme.Dp(8), 0, 0, Theme.Dp(16)));
            TXT_Hex = new FieldBox
            {
                Dock = DockStyle.Fill, Multiline = true, Monospace = true,
                Placeholder = "Les octets apparaissent ici. Collez de l'hexadécimal pour retrouver le texte.", TabIndex = 2
            };
            TXT_Hex.Inner.MaxLength = int.MaxValue;
            TXT_Hex.TextValueChanged += TXT_Hex_TextChanged;
            CARD_Hex.Controls.Add(TXT_Hex);
            BT_PasteHex = MakeSmallButton("Coller", ButtonKind.Ghost, BT_PasteHex_Click);
            BT_CopyHex = MakeSmallButton("Copier", ButtonKind.Secondary, BT_CopyHex_Click);
            CARD_Hex.AddHeaderControl(BT_CopyHex);
            CARD_Hex.AddHeaderControl(BT_PasteHex);

            // Carte « Options »
            CARD_Options = MakeCard("Options de conversion", new Padding(0));
            var opt = MakeTable(3, 2);
            ColPct(opt, 38F); ColPct(opt, 44F); ColPct(opt, 18F);
            Abs(opt, 22); Abs(opt, 38);

            SEG_Encoding = new SegmentedControl
            {
                Items = Arr("UTF-8", "ASCII", "UTF-16", "Win-1252"),
                ToolTips = Arr(
                    "UTF-8 : standard du web et de Windows récent (é = C3 A9)",
                    "ASCII : 7 bits, caractères anglais uniquement",
                    "UTF-16 : encodage interne de Windows et .NET, petit-boutiste (A = 41 00)",
                    "Windows-1252 : ANSI occidental, 1 octet par caractère (é = E9, € = 80)"),
                Dock = DockStyle.Fill, Margin = new Padding(0, 0, Theme.Dp(18), 0), TabIndex = 3
            };
            SEG_Format = new SegmentedControl
            {
                Items = Arr("48 65", "4865", "0x48", "\\x48", "0x48,"),
                ToolTips = Arr(
                    "Octets séparés par des espaces : 48 65 6C",
                    "Compact, sans séparateur : 48656C",
                    "Préfixe 0x : 0x48 0x65 0x6C",
                    "Échappements \\x (chaînes C, Python…) : \\x48\\x65\\x6C",
                    "Tableau C / C# / Java : 0x48, 0x65, 0x6C"),
                Dock = DockStyle.Fill, Margin = new Padding(0, 0, Theme.Dp(18), 0), TabIndex = 4
            };
            SEG_Case = new SegmentedControl
            {
                Items = Arr("ABC", "abc"),
                ToolTips = Arr("Lettres hexadécimales en majuscules (FF)", "Lettres hexadécimales en minuscules (ff)"),
                Dock = DockStyle.Fill, Margin = new Padding(0), TabIndex = 5
            };
            SEG_Encoding.SelectedIndexChanged += SEG_Encoding_SelectedIndexChanged;
            SEG_Format.SelectedIndexChanged += SEG_Format_SelectedIndexChanged;
            SEG_Case.SelectedIndexChanged += SEG_Format_SelectedIndexChanged;

            opt.Controls.Add(MakeLabel("Encodage du texte", true, Theme.TextMuted), 0, 0);
            opt.Controls.Add(MakeLabel("Présentation des octets", true, Theme.TextMuted), 1, 0);
            opt.Controls.Add(MakeLabel("Casse", true, Theme.TextMuted), 2, 0);
            opt.Controls.Add(SEG_Encoding, 0, 1);
            opt.Controls.Add(SEG_Format, 1, 1);
            opt.Controls.Add(SEG_Case, 2, 1);
            CARD_Options.Controls.Add(opt);

            PAGE_Text.Controls.Add(CARD_Text, 0, 0);
            PAGE_Text.Controls.Add(CARD_Hex, 1, 0);
            PAGE_Text.Controls.Add(CARD_Options, 0, 1);
            PAGE_Text.SetColumnSpan(CARD_Options, 2);
        }

        private void BuildNumbersPage()
        {
            PAGE_Numbers = MakeTable(2, 1);
            PAGE_Numbers.Dock = DockStyle.Fill;
            PAGE_Numbers.BackColor = Theme.Background;
            PAGE_Numbers.Visible = false;
            ColPct(PAGE_Numbers, 42F); ColPct(PAGE_Numbers, 58F);
            Fill(PAGE_Numbers);

            // Carte « Nombre à convertir » : une seule colonne, une ligne par élément
            CARD_NumIn = MakeCard("Nombre à convertir", new Padding(0, 0, Theme.Dp(8), 0));
            var inp = new StackBuilder();

            SEG_Base = new SegmentedControl
            {
                Items = Arr("Déc", "Hex", "Bin", "Oct"),
                ToolTips = Arr("Base 10 (0-9)", "Base 16 (0-9, A-F)", "Base 2 (0-1)", "Base 8 (0-7)"),
                TabIndex = 10
            };
            SEG_Base.SelectedIndexChanged += NumberInput_Changed;
            TXT_Number = new FieldBox { Monospace = true, Placeholder = "ex. 255, 0xFF, 0b1010", TabIndex = 11 };
            TXT_Number.TextValueChanged += NumberInput_Changed;
            LBL_NumHint = MakeLabel("Préfixes reconnus : 0x (hexadécimal), 0b (binaire), 0o (octal). Les espaces et « _ » sont ignorés ; les nombres négatifs et très grands sont acceptés.", false, Theme.TextMuted);
            LBL_NumHint.Font = Theme.Font(9f);
            LBL_NumHint.TextAlign = ContentAlignment.TopLeft;
            LBL_NumHint.Padding = new Padding(0, Theme.Dp(8), 0, 0);

            inp.Add(MakeLabel("Base de saisie", true, Theme.TextMuted), 22);
            inp.Add(SEG_Base, 40);
            inp.Gap(16);
            inp.Add(MakeLabel("Valeur", true, Theme.TextMuted), 22);
            inp.Add(TXT_Number, 42);
            inp.Add(LBL_NumHint, 66);

            SW_Group = new ToggleSwitch { TabIndex = 12 };
            SW_Prefix = new ToggleSwitch { TabIndex = 13 };
            SW_Upper = new ToggleSwitch { TabIndex = 14, Checked = true };
            AddToggleRow(inp, SW_Group, "Grouper les chiffres (octets, milliers)", NumberInput_Changed);
            AddToggleRow(inp, SW_Prefix, "Afficher les préfixes (0x, 0b, 0o)", NumberInput_Changed);
            AddToggleRow(inp, SW_Upper, "Hexadécimal en majuscules", NumberInput_Changed);
            inp.Gap(8);
            CARD_NumIn.Controls.Add(inp.Build());
            BT_ClearNumber = MakeSmallButton("Effacer", ButtonKind.Ghost, BT_ClearNumber_Click);
            CARD_NumIn.AddHeaderControl(BT_ClearNumber);

            // Carte « Résultats »
            CARD_NumOut = MakeCard("Résultats", new Padding(Theme.Dp(8), 0, 0, 0));
            var outp = new StackBuilder();

            PillButton c1, c2, c3, c4;
            TXT_Dec = MakeOutput(out c1, "Copier", BT_CopyDec_Click); BT_CopyDec = c1;
            TXT_HexNum = MakeOutput(out c2, "Copier", BT_CopyHexNum_Click); BT_CopyHexNum = c2;
            TXT_Bin = MakeOutput(out c3, "Copier", BT_CopyBin_Click); BT_CopyBin = c3;
            TXT_Oct = MakeOutput(out c4, "Copier", BT_CopyOct_Click); BT_CopyOct = c4;
            AddOutputRow(outp, "Décimal (base 10)", TXT_Dec, BT_CopyDec);
            AddOutputRow(outp, "Hexadécimal (base 16)", TXT_HexNum, BT_CopyHexNum);
            AddOutputRow(outp, "Binaire (base 2)", TXT_Bin, BT_CopyBin);
            AddOutputRow(outp, "Octal (base 8)", TXT_Oct, BT_CopyOct);
            CARD_NumOut.Controls.Add(outp.Build());

            PAGE_Numbers.Controls.Add(CARD_NumIn, 0, 0);
            PAGE_Numbers.Controls.Add(CARD_NumOut, 1, 0);
        }

        #endregion
    }
}
