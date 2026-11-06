using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TextToHex.UI
{
    /// <summary>Carte blanche aux angles arrondis avec un titre numéroté ("1 · Image source").</summary>
    internal class CardPanel : Panel
    {
        private readonly Font _titleFont = Theme.Font(11f, FontStyle.Bold);
        private readonly Font _badgeFont = Theme.Font(9f, FontStyle.Bold);
        private string _title = "";
        private string _step = "";
        private readonly System.Collections.Generic.List<Control> _headerControls = new System.Collections.Generic.List<Control>();

        public CardPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            BackColor = Theme.Surface;   // les contrôles enfants se fondent dans la carte blanche
            Padding = new Padding(Theme.Dp(20), Theme.Dp(56), Theme.Dp(20), Theme.Dp(20));
        }

        [DefaultValue("")]
        public string Title { get { return _title; } set { _title = value ?? ""; Invalidate(); } }

        [DefaultValue("")]
        public string Step { get { return _step; } set { _step = value ?? ""; Invalidate(); } }

        /// <summary>Ajoute un contrôle aligné à droite dans la ligne de titre de la carte.</summary>
        public void AddHeaderControl(Control c)
        {
            _headerControls.Add(c);
            Controls.Add(c);
            PerformLayout();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            int x = Width - Theme.Dp(14);
            foreach (Control c in _headerControls)
            {
                x -= c.Width;
                c.Location = new Point(x, Theme.Dp(10));
                x -= Theme.Dp(4);
                c.BringToFront();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(Theme.Background);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Theme.HighQuality(g);

            var card = new RectangleF(0.5f, 0.5f, Width - 2f, Height - 2f);
            Theme.FillRound(g, Theme.Surface, card, Theme.Dp(14f));
            Theme.StrokeRound(g, Theme.Border, 1f, card, Theme.Dp(14f));

            int x = Theme.Dp(20);
            int badge = Theme.Dp(26);
            int y = Theme.Dp(15);
            if (_step.Length > 0)
            {
                Theme.FillRound(g, Theme.AccentSoft, new RectangleF(x, y, badge, badge), badge / 2f);
                Theme.DrawText(g, _step, _badgeFont, Theme.Accent, new Rectangle(x, y, badge, badge),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                x += badge + Theme.Dp(10);
            }
            Theme.DrawText(g, _title, _titleFont, Theme.Text, new Rectangle(x, y, Width - x - Theme.Dp(20), badge),
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { _titleFont.Dispose(); _badgeFont.Dispose(); }
            base.Dispose(disposing);
        }
    }

    internal enum PanelEdge { None, Top, Bottom }

    /// <summary>Panneau plat à double tampon avec un filet optionnel (en-tête / pied de page).</summary>
    internal class SurfacePanel : Panel
    {
        private PanelEdge _edge = PanelEdge.None;

        public SurfacePanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            BackColor = Theme.Surface;
        }

        [DefaultValue(PanelEdge.None)]
        public PanelEdge Edge { get { return _edge; } set { _edge = value; Invalidate(); } }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            base.OnPaint(e);
            using (var pen = new Pen(Theme.Border))
            {
                if (_edge == PanelEdge.Bottom) e.Graphics.DrawLine(pen, 0, Height - 1, Width, Height - 1);
                else if (_edge == PanelEdge.Top) e.Graphics.DrawLine(pen, 0, 0, Width, 0);
            }
        }
    }

    internal enum ButtonKind { Primary, Secondary, Ghost }

    /// <summary>Bouton moderne arrondi avec états survol / appui / désactivé / focus clavier.</summary>
    internal class PillButton : Control
    {
        private bool _hover, _pressed, _active, _keyboardFocus;
        private ButtonKind _kind = ButtonKind.Secondary;

        public PillButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);
            Cursor = Cursors.Hand;
            Font = Theme.Font(9.5f, FontStyle.Bold);
            TabStop = true;
            Size = new Size(Theme.Dp(120), Theme.Dp(38));
        }

        [DefaultValue(ButtonKind.Secondary)]
        public ButtonKind Kind { get { return _kind; } set { _kind = value; Invalidate(); } }

        /// <summary>Mis en évidence (ex. préréglage actuellement appliqué).</summary>
        [DefaultValue(false)]
        public bool Active { get { return _active; } set { if (_active != value) { _active = value; Invalidate(); } } }

        public void PerformClick() { if (Enabled) OnClick(EventArgs.Empty); }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; _pressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { if (e.Button == MouseButtons.Left) { _pressed = true; _keyboardFocus = false; Focus(); Invalidate(); } base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _pressed = false; Invalidate(); base.OnMouseUp(e); }
        protected override void OnEnabledChanged(EventArgs e) { _hover = _pressed = false; Cursor = Enabled ? Cursors.Hand : Cursors.Default; Invalidate(); base.OnEnabledChanged(e); }
        protected override void OnGotFocus(EventArgs e) { Invalidate(); base.OnGotFocus(e); }
        protected override void OnLostFocus(EventArgs e) { _pressed = false; _keyboardFocus = false; Invalidate(); base.OnLostFocus(e); }
        protected override void OnTextChanged(EventArgs e) { Invalidate(); base.OnTextChanged(e); }

        protected override bool IsInputKey(Keys keyData)
        {
            return keyData == Keys.Space || keyData == Keys.Enter || base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter) { _pressed = true; _keyboardFocus = true; Invalidate(); e.Handled = true; }
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter) && _pressed)
            {
                _pressed = false; Invalidate();
                PerformClick();
                e.Handled = true;
            }
            base.OnKeyUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Color parent = Parent != null ? Parent.BackColor : Theme.Surface;
            g.Clear(parent);
            Theme.HighQuality(g);

            Color fill, border, text;
            switch (_kind)
            {
                case ButtonKind.Primary:
                    fill = !Enabled ? Theme.AccentDisabled : _pressed ? Theme.AccentPress : _hover ? Theme.AccentHover : Theme.Accent;
                    border = fill;
                    text = Color.White;
                    break;
                case ButtonKind.Ghost:
                    fill = !Enabled ? parent : _pressed ? Theme.AccentSoft : _hover ? Theme.AccentSoft : parent;
                    border = fill;
                    text = Enabled ? Theme.Accent : Theme.TextMuted;
                    break;
                default:
                    if (_active) { fill = Theme.AccentSoft; border = Theme.Accent; text = Theme.Accent; }
                    else
                    {
                        fill = !Enabled ? ColorTranslator.FromHtml("#F6F7FB") : _pressed ? ColorTranslator.FromHtml("#E9ECF5") : _hover ? ColorTranslator.FromHtml("#F6F7FC") : Theme.Surface;
                        border = _hover && Enabled ? Theme.BorderStrong : Theme.Border;
                        text = Enabled ? Theme.Text : Theme.TextMuted;
                    }
                    break;
            }

            var r = new RectangleF(0.5f, 0.5f, Width - 2f, Height - 2f);
            float radius = Math.Min(Theme.Dp(10f), Height / 2f);
            Theme.FillRound(g, fill, r, radius);
            if (border != fill) Theme.StrokeRound(g, border, 1f, r, radius);

            if (Focused && _keyboardFocus)
            {
                var ring = new RectangleF(2.5f, 2.5f, Width - 6f, Height - 6f);
                Theme.StrokeRound(g, _kind == ButtonKind.Primary ? Color.White : Theme.Accent, 1.5f, ring, Math.Max(2f, radius - 2f));
            }

            Theme.DrawText(g, Text, Font, text, new Rectangle(Theme.Dp(8), 0, Width - Theme.Dp(16), Height),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    /// <summary>Interrupteur on/off.</summary>
    internal class ToggleSwitch : Control
    {
        private bool _checked;

        public event EventHandler CheckedChanged;

        public ToggleSwitch()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.StandardClick, true);
            Cursor = Cursors.Hand;
            TabStop = true;
            Size = new Size(Theme.Dp(42), Theme.Dp(24));
        }

        [DefaultValue(false)]
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked == value) return;
                _checked = value;
                Invalidate();
                var h = CheckedChanged;
                if (h != null) h(this, EventArgs.Empty);
            }
        }

        protected override void OnClick(EventArgs e) { Checked = !Checked; base.OnClick(e); }
        protected override void OnGotFocus(EventArgs e) { Invalidate(); base.OnGotFocus(e); }
        protected override void OnLostFocus(EventArgs e) { Invalidate(); base.OnLostFocus(e); }
        protected override bool IsInputKey(Keys keyData) { return keyData == Keys.Space || base.IsInputKey(keyData); }
        protected override void OnKeyUp(KeyEventArgs e) { if (e.KeyCode == Keys.Space) { Checked = !Checked; e.Handled = true; } base.OnKeyUp(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Parent != null ? Parent.BackColor : Theme.Surface);
            Theme.HighQuality(g);

            var track = new RectangleF(1f, 1f, Width - 2f, Height - 2f);
            Theme.FillRound(g, _checked ? Theme.Accent : ColorTranslator.FromHtml("#C9CFDF"), track, track.Height / 2f);

            float pad = Theme.Dp(3f);
            float d = track.Height - 2 * pad;
            float x = _checked ? track.Right - pad - d : track.X + pad;
            using (var b = new SolidBrush(Color.White))
                g.FillEllipse(b, x, track.Y + pad, d, d);

            if (Focused)
                Theme.StrokeRound(g, Theme.Accent, 1.5f, new RectangleF(0.75f, 0.75f, Width - 2.5f, Height - 2.5f), Height / 2f);
        }
    }

    /// <summary>Étiquette qui laisse passer les clics (texte indicatif d'un champ vide).</summary>
    internal class PlaceholderLabel : Label
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0084) { m.Result = new IntPtr(-1); return; }   // WM_NCHITTEST -> HTTRANSPARENT
            base.WndProc(ref m);
        }
    }

    /// <summary>Champ de saisie arrondi (une ou plusieurs lignes) avec bordure de focus et texte indicatif.</summary>
    internal class FieldBox : Panel
    {
        private readonly TextBox _inner = new TextBox();
        private readonly PlaceholderLabel _hint = new PlaceholderLabel();
        private bool _focused, _multiline;

        public FieldBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            Height = Theme.Dp(40);
            BackColor = Theme.Surface;

            _inner.BorderStyle = BorderStyle.None;
            _inner.Font = Theme.Font(10f);
            _inner.ForeColor = Theme.Text;
            _inner.BackColor = Theme.Surface;
            _inner.GotFocus += delegate { _focused = true; UpdateHint(); Invalidate(); };
            _inner.LostFocus += delegate { _focused = false; UpdateHint(); Invalidate(); };
            _inner.TextChanged += delegate { UpdateHint(); var h = TextValueChanged; if (h != null) h(this, EventArgs.Empty); };
            _inner.KeyDown += delegate (object s, KeyEventArgs e)
            {
                // Ctrl+A n'est pas géré nativement par les zones de texte multilignes.
                if (e.Control && e.KeyCode == Keys.A) { _inner.SelectAll(); e.SuppressKeyPress = true; e.Handled = true; }
            };
            Controls.Add(_inner);

            _hint.AutoSize = false;
            _hint.BackColor = Color.Transparent;
            _hint.ForeColor = Theme.TextMuted;
            _hint.Font = Theme.Font(10f);
            _hint.Visible = false;
            Controls.Add(_hint);
            _hint.BringToFront();
            _hint.Click += delegate { _inner.Focus(); };   // filet de sécurité si le clic atteint l'étiquette
            Cursor = Cursors.IBeam;
        }

        public event EventHandler TextValueChanged;

        public TextBox Inner { get { return _inner; } }

        [DefaultValue("")]
        public string Placeholder { get { return _hint.Text; } set { _hint.Text = value ?? ""; UpdateHint(); } }

        public string Value { get { return _inner.Text; } set { _inner.Text = value ?? ""; } }

        [DefaultValue(false)]
        public bool Multiline
        {
            get { return _multiline; }
            set
            {
                _multiline = value;
                _inner.Multiline = value;
                _inner.WordWrap = value;
                _inner.AcceptsReturn = value;
                _inner.ScrollBars = value ? ScrollBars.Vertical : ScrollBars.None;
                PerformLayout();
            }
        }

        /// <summary>Police à chasse fixe (Consolas) pour les octets, nombres et code.</summary>
        [DefaultValue(false)]
        public bool Monospace
        {
            set
            {
                Font old = _inner.Font;
                _inner.Font = value ? Theme.MonoFont(10.5f) : Theme.Font(10f);
                _hint.Font = _inner.Font;
                old.Dispose();
                PerformLayout();
            }
        }

        public bool ReadOnly
        {
            get { return _inner.ReadOnly; }
            set
            {
                _inner.ReadOnly = value;
                _inner.BackColor = value ? ColorTranslator.FromHtml("#F6F9FB") : Theme.Surface;
                Invalidate();
            }
        }

        private void UpdateHint()
        {
            _hint.Visible = _hint.Text.Length > 0 && _inner.TextLength == 0 && !_focused;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (_multiline)
            {
                int padH = Theme.Dp(14), padV = Theme.Dp(10);
                _inner.SetBounds(padH, padV, Math.Max(10, Width - padH * 2), Math.Max(10, Height - padV * 2));
                _hint.SetBounds(padH + Theme.Dp(2), padV, _inner.Width, Theme.Dp(56));
            }
            else
            {
                int pad = Theme.Dp(12);
                int h = _inner.PreferredHeight;
                _inner.SetBounds(pad, (Height - h) / 2, Math.Max(10, Width - pad * 2), h);
                _hint.SetBounds(pad + Theme.Dp(2), _inner.Top, _inner.Width, h);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e) { _inner.Focus(); base.OnMouseDown(e); }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(Parent != null ? Parent.BackColor : Theme.Surface);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Theme.HighQuality(g);
            var r = new RectangleF(0.75f, 0.75f, Width - 2.5f, Height - 2.5f);
            Color fill = _inner.ReadOnly ? ColorTranslator.FromHtml("#F6F9FB") : Theme.Surface;
            Theme.FillRound(g, fill, r, Theme.Dp(10f));
            Theme.StrokeRound(g, _focused ? Theme.Accent : Theme.Border, _focused ? 1.75f : 1.25f, r, Theme.Dp(10f));
        }
    }

    /// <summary>Sélecteur segmenté (une seule option active), avec info-bulle par segment.</summary>
    internal class SegmentedControl : Control
    {
        private readonly ToolTip _tip = new ToolTip { InitialDelay = 400, ReshowDelay = 200 };
        private string[] _items = new string[0];
        private string[] _tips = new string[0];
        private int _selected, _hover = -1, _lastTip = -1;
        private Font _font;

        public event EventHandler SelectedIndexChanged;

        public SegmentedControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
            Cursor = Cursors.Hand;
            TabStop = true;
            _font = Theme.Font(9.5f, FontStyle.Bold);
            Height = Theme.Dp(38);
        }

        public string[] Items { get { return _items; } set { _items = value ?? new string[0]; Invalidate(); } }

        public string[] ToolTips { get { return _tips; } set { _tips = value ?? new string[0]; } }

        public float FontSize { set { _font.Dispose(); _font = Theme.Font(value, FontStyle.Bold); Invalidate(); } }

        public int SelectedIndex
        {
            get { return _selected; }
            set
            {
                if (_items.Length == 0) return;
                value = Math.Max(0, Math.Min(_items.Length - 1, value));
                if (value == _selected) return;
                _selected = value;
                Invalidate();
                var h = SelectedIndexChanged;
                if (h != null) h(this, EventArgs.Empty);
            }
        }

        /// <summary>Change la sélection sans déclencher l'événement.</summary>
        public void SetSelectedSilently(int index)
        {
            if (index < 0 || index >= _items.Length) return;
            _selected = index;
            Invalidate();
        }

        private int IndexAt(Point p)
        {
            if (_items.Length == 0) return -1;
            int i = (int)((p.X - Theme.Dp(3)) / ((Width - Theme.Dp(6)) / (float)_items.Length));
            return i < 0 || i >= _items.Length ? -1 : i;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int i = IndexAt(e.Location);
            if (i != _hover) { _hover = i; Invalidate(); }
            if (i != _lastTip)
            {
                _lastTip = i;
                _tip.SetToolTip(this, i >= 0 && i < _tips.Length ? _tips[i] : "");
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e) { _hover = -1; _lastTip = -1; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            int i = IndexAt(e.Location);
            if (i >= 0) SelectedIndex = i;
            base.OnMouseDown(e);
        }

        protected override void OnGotFocus(EventArgs e) { Invalidate(); base.OnGotFocus(e); }
        protected override void OnLostFocus(EventArgs e) { Invalidate(); base.OnLostFocus(e); }

        protected override bool IsInputKey(Keys keyData)
        {
            return keyData == Keys.Left || keyData == Keys.Right || base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) { SelectedIndex = _selected - 1; e.Handled = true; }
            else if (e.KeyCode == Keys.Right) { SelectedIndex = _selected + 1; e.Handled = true; }
            base.OnKeyDown(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Parent != null ? Parent.BackColor : Theme.Surface);
            Theme.HighQuality(g);

            var track = new RectangleF(0.5f, 0.5f, Width - 2f, Height - 2f);
            float radius = Theme.Dp(10f);
            Theme.FillRound(g, ColorTranslator.FromHtml("#EAF0F4"), track, radius);

            if (Focused)
                Theme.StrokeRound(g, Theme.Accent, 1.5f, new RectangleF(0.75f, 0.75f, Width - 2.5f, Height - 2.5f), radius);

            if (_items.Length == 0) return;
            float pad = Theme.Dp(3f);
            float w = (Width - pad * 2) / _items.Length;
            for (int i = 0; i < _items.Length; i++)
            {
                var seg = new RectangleF(pad + i * w, pad, w, Height - pad * 2);
                bool sel = i == _selected;
                if (sel)
                {
                    var inner = new RectangleF(seg.X + 0.5f, seg.Y + 0.5f, seg.Width - 1f, seg.Height - 1f);
                    Theme.FillRound(g, Theme.Surface, inner, radius - 2);
                    Theme.StrokeRound(g, Theme.Border, 1f, inner, radius - 2);
                }
                Color text = sel ? Theme.Accent : (i == _hover ? Theme.Text : Theme.TextMuted);
                Theme.DrawText(g, _items[i], _font, text, Rectangle.Round(seg),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { _tip.Dispose(); _font.Dispose(); }
            base.Dispose(disposing);
        }
    }

    internal enum StatusKind { Info, Success, Warning, Error, Busy }

    /// <summary>Ligne d'état du pied de page : pastille colorée + message.</summary>
    internal class StatusLine : Control
    {
        private readonly Font _titleFont = Theme.Font(10f, FontStyle.Bold);
        private readonly Font _detailFont = Theme.Font(9f);
        private readonly Timer _timer = new Timer { Interval = 90 };
        private StatusKind _kind = StatusKind.Info;
        private string _title = "", _detail = "";
        private int _spin;

        public StatusLine()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            _timer.Tick += delegate { _spin = (_spin + 30) % 360; Invalidate(); };
        }

        public void Set(StatusKind kind, string title, string detail)
        {
            _kind = kind;
            _title = title ?? "";
            _detail = detail ?? "";
            _timer.Enabled = kind == StatusKind.Busy;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Parent != null ? Parent.BackColor : Theme.Surface);
            Theme.HighQuality(g);

            int d = Theme.Dp(34);
            var c = new RectangleF(0, (Height - d) / 2f, d, d);
            Color color;
            switch (_kind)
            {
                case StatusKind.Success: color = Theme.Success; break;
                case StatusKind.Warning: color = Theme.Warning; break;
                case StatusKind.Error: color = Theme.Danger; break;
                case StatusKind.Busy: color = Theme.Accent; break;
                default: color = Theme.TextMuted; break;
            }

            Theme.FillRound(g, Color.FromArgb(28, color), c, d / 2f);
            float cx = c.X + d / 2f, cy = c.Y + d / 2f;
            using (var pen = new Pen(color, Math.Max(1.8f, Theme.Dp(2f))) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round })
            using (var brush = new SolidBrush(color))
            {
                switch (_kind)
                {
                    case StatusKind.Success:
                        g.DrawLines(pen, new[] { new PointF(cx - d * 0.18f, cy + d * 0.01f), new PointF(cx - d * 0.04f, cy + d * 0.15f), new PointF(cx + d * 0.20f, cy - d * 0.14f) });
                        break;
                    case StatusKind.Error:
                        g.DrawLine(pen, cx - d * 0.14f, cy - d * 0.14f, cx + d * 0.14f, cy + d * 0.14f);
                        g.DrawLine(pen, cx + d * 0.14f, cy - d * 0.14f, cx - d * 0.14f, cy + d * 0.14f);
                        break;
                    case StatusKind.Warning:
                        g.DrawLine(pen, cx, cy - d * 0.20f, cx, cy + d * 0.04f);
                        g.FillEllipse(brush, cx - d * 0.045f, cy + d * 0.12f, d * 0.09f, d * 0.09f);
                        break;
                    case StatusKind.Busy:
                        g.DrawArc(pen, cx - d * 0.2f, cy - d * 0.2f, d * 0.4f, d * 0.4f, _spin, 250);
                        break;
                    default:
                        g.DrawLine(pen, cx, cy - d * 0.02f, cx, cy + d * 0.19f);
                        g.FillEllipse(brush, cx - d * 0.045f, cy - d * 0.22f, d * 0.09f, d * 0.09f);
                        break;
                }
            }

            int x = d + Theme.Dp(12);
            int w = Width - x;
            bool hasDetail = _detail.Length > 0;
            int lineH = Theme.Dp(20);
            int top = (Height - (hasDetail ? lineH * 2 - Theme.Dp(2) : lineH)) / 2;
            Theme.DrawText(g, _title, _titleFont, Theme.Text, new Rectangle(x, top, w, lineH), TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            if (hasDetail)
                Theme.DrawText(g, _detail, _detailFont, Theme.TextMuted, new Rectangle(x, top + lineH - Theme.Dp(2), w, lineH), TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { _timer.Dispose(); _titleFont.Dispose(); _detailFont.Dispose(); }
            base.Dispose(disposing);
        }
    }
}
