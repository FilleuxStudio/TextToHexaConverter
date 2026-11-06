using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace TextToHex
{
    /// <summary>Encodage utilisé pour transformer le texte en octets.</summary>
    internal enum TextEncodingKind { Utf8, Ascii, Utf16, Windows1252 }

    /// <summary>Présentation de la sortie hexadécimale.</summary>
    internal enum HexFormat
    {
        /// <summary>48 65 6C 6C 6F</summary>
        Spaced,
        /// <summary>48656C6C6F</summary>
        Compact,
        /// <summary>0x48 0x65 0x6C</summary>
        Prefix0x,
        /// <summary>\x48\x65\x6C</summary>
        EscapeX,
        /// <summary>0x48, 0x65, 0x6C (tableau C / C# / Java)</summary>
        CArray
    }

    /// <summary>
    /// Conversions texte &lt;-&gt; octets &lt;-&gt; hexadécimal.
    /// Aucune dépendance à l'interface : toutes les méthodes sont testables seules.
    /// </summary>
    internal static class HexConverter
    {
        /// <summary>Taille maximale acceptée en entrée (caractères), pour garder une interface fluide.</summary>
        public const int MaxInputLength = 2000000;

        // ------------------------------------------------------------------
        //  Encodages
        // ------------------------------------------------------------------

        public static string EncodingLabel(TextEncodingKind kind)
        {
            switch (kind)
            {
                case TextEncodingKind.Ascii: return "ASCII";
                case TextEncodingKind.Utf16: return "UTF-16";
                case TextEncodingKind.Windows1252: return "Windows-1252";
                default: return "UTF-8";
            }
        }

        private static Encoding GetEncoding(TextEncodingKind kind, bool strict)
        {
            EncoderFallback ef = strict ? (EncoderFallback)new EncoderExceptionFallback() : new EncoderReplacementFallback("?");
            DecoderFallback df = strict ? (DecoderFallback)new DecoderExceptionFallback() : new DecoderReplacementFallback("�");

            switch (kind)
            {
                case TextEncodingKind.Ascii:
                    return Encoding.GetEncoding("us-ascii", ef, strict ? df : new DecoderReplacementFallback("?"));
                case TextEncodingKind.Utf16:
                    return new UnicodeEncoding(false, false, strict);      // little-endian, sans BOM
                case TextEncodingKind.Windows1252:
                    return Encoding.GetEncoding(1252, ef, df);
                default:
                    return new UTF8Encoding(false, strict);                // sans BOM
            }
        }

        /// <summary>Texte -> octets. Un avertissement est fourni si des caractères ont dû être remplacés.</summary>
        public static byte[] Encode(string text, TextEncodingKind kind, out string warning)
        {
            warning = null;
            if (string.IsNullOrEmpty(text)) return new byte[0];

            try
            {
                return GetEncoding(kind, true).GetBytes(text);
            }
            catch (EncoderFallbackException)
            {
                warning = "Certains caractères n'existent pas en " + EncodingLabel(kind) + " : ils ont été remplacés par « ? ».";
                return GetEncoding(kind, false).GetBytes(text);
            }
        }

        /// <summary>Octets -> texte affichable (fins de ligne normalisées, octets de contrôle masqués).</summary>
        public static string Decode(byte[] bytes, TextEncodingKind kind, out string warning)
        {
            warning = null;
            if (bytes == null || bytes.Length == 0) return string.Empty;

            string text;
            try
            {
                text = GetEncoding(kind, true).GetString(bytes);
            }
            catch (DecoderFallbackException)
            {
                warning = "Ces octets ne forment pas un texte valide en " + EncodingLabel(kind) + " : les octets invalides sont remplacés.";
                text = GetEncoding(kind, false).GetString(bytes);
            }

            // Une zone de texte Windows ne gère que les fins de ligne \r\n.
            text = Regex.Replace(text, "\r\n|\r|\n", "\r\n");

            // Les caractères de contrôle (00, 07, 1B…) tronquent ou cassent l'affichage : on les remplace par « · ».
            int masked = 0;
            var sb = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                if (c < ' ' && c != '\r' && c != '\n' && c != '\t' || c == '\u007F')
                {
                    sb.Append('·');
                    masked++;
                }
                else sb.Append(c);
            }
            if (masked > 0)
            {
                string note = masked + (masked > 1 ? " octets de contrôle non affichables sont représentés par « · »." : " octet de contrôle non affichable est représenté par « · ».");
                warning = warning == null ? note : warning + " " + note;
            }
            return sb.ToString();
        }

        // ------------------------------------------------------------------
        //  Octets -> hexadécimal
        // ------------------------------------------------------------------

        public static string ToHex(byte[] bytes, HexFormat format, bool upperCase)
        {
            if (bytes == null || bytes.Length == 0) return string.Empty;

            string digits = upperCase ? "0123456789ABCDEF" : "0123456789abcdef";
            string prefix = "", separator = "";
            switch (format)
            {
                case HexFormat.Spaced: separator = " "; break;
                case HexFormat.Compact: break;
                case HexFormat.Prefix0x: prefix = "0x"; separator = " "; break;
                case HexFormat.EscapeX: prefix = "\\x"; break;
                case HexFormat.CArray: prefix = "0x"; separator = ", "; break;
            }

            var sb = new StringBuilder(bytes.Length * (prefix.Length + 2 + separator.Length));
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i > 0) sb.Append(separator);
                sb.Append(prefix);
                sb.Append(digits[bytes[i] >> 4]);
                sb.Append(digits[bytes[i] & 0x0F]);
            }
            return sb.ToString();
        }

        // ------------------------------------------------------------------
        //  Hexadécimal -> octets (saisie tolérante)
        // ------------------------------------------------------------------

        private static bool IsHexDigit(char c)
        {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }

        private static bool IsSeparator(char c)
        {
            return char.IsWhiteSpace(c) || ",;:-_{}[]()".IndexOf(c) >= 0;
        }

        /// <summary>
        /// Accepte : "48 65 6C", "48656C", "0x48, 0x65", "\x48\x65", "{0x48,0x65}", majuscules ou minuscules.
        /// Une chaîne vide est valide (zéro octet).
        /// </summary>
        public static bool TryParseHex(string input, out byte[] bytes, out string error)
        {
            bytes = new byte[0];
            error = null;
            if (string.IsNullOrWhiteSpace(input)) return true;

            // Les préfixes 0x et \x deviennent des séparateurs : "\x48\x65" -> " 48  65".
            bool hadPrefix = Regex.IsMatch(input, @"0[xX]|\\[xX]");
            string s = Regex.Replace(input, @"0[xX]|\\[xX]", " ");

            var tokens = new List<string>();
            var cur = new StringBuilder();
            foreach (char c in s)
            {
                if (IsHexDigit(c)) { cur.Append(c); continue; }
                if (IsSeparator(c))
                {
                    if (cur.Length > 0) { tokens.Add(cur.ToString()); cur.Length = 0; }
                    continue;
                }
                error = "Caractère non hexadécimal : « " + c + " ». Chiffres autorisés : 0-9 et A-F.";
                bytes = null;
                return false;
            }
            if (cur.Length > 0) tokens.Add(cur.ToString());

            // Avec préfixes, ou si chaque groupe fait 1 ou 2 chiffres, un groupe = un octet ("0xF" -> 0F).
            bool perToken = hadPrefix || tokens.TrueForAll(t => t.Length <= 2);
            var digits = new StringBuilder();
            foreach (string t in tokens)
            {
                if (perToken && t.Length == 1) { digits.Append('0').Append(t); continue; }
                if (perToken && t.Length % 2 != 0)
                {
                    error = "Le groupe « " + t + " » a un nombre impair de chiffres : un octet s'écrit avec 2 chiffres.";
                    bytes = null;
                    return false;
                }
                digits.Append(t);
            }

            if (digits.Length % 2 != 0)
            {
                error = "Nombre impair de chiffres hexadécimaux (" + digits.Length + ") : un octet s'écrit avec 2 chiffres.";
                bytes = null;
                return false;
            }

            var result = new byte[digits.Length / 2];
            for (int i = 0; i < result.Length; i++)
                result[i] = (byte)((HexValue(digits[2 * i]) << 4) | HexValue(digits[2 * i + 1]));
            bytes = result;
            return true;
        }

        private static int HexValue(char c)
        {
            if (c <= '9') return c - '0';
            return (c | 0x20) - 'a' + 10;
        }
    }

    /// <summary>Conversion d'entiers de taille quelconque entre décimal, hexadécimal, binaire et octal.</summary>
    internal static class NumberConverter
    {
        public const int MaxDigits = 4096;

        public static string RadixName(int radix)
        {
            switch (radix)
            {
                case 2: return "binaire";
                case 8: return "octale";
                case 16: return "hexadécimale";
                default: return "décimale";
            }
        }

        /// <summary>
        /// Lit un entier. Les préfixes 0x et 0o sont toujours reconnus, 0b sauf en base 16 (où « 0b » est un nombre valide).
        /// Les espaces et « _ » sont ignorés. Une saisie vide retourne false sans message d'erreur.
        /// </summary>
        public static bool TryParse(string input, int radix, out BigInteger value, out int usedRadix, out string error)
        {
            value = BigInteger.Zero;
            usedRadix = radix;
            error = null;

            if (string.IsNullOrWhiteSpace(input)) return false;

            var sb = new StringBuilder();
            foreach (char c in input)
                if (!char.IsWhiteSpace(c) && c != '_') sb.Append(c);
            string s = sb.ToString();

            bool negative = false;
            if (s.Length > 0 && (s[0] == '-' || s[0] == '+'))
            {
                negative = s[0] == '-';
                s = s.Substring(1);
            }

            if (s.Length >= 2 && s[0] == '0')
            {
                char p = char.ToLowerInvariant(s[1]);
                if (p == 'x') { usedRadix = 16; s = s.Substring(2); }
                else if (p == 'o') { usedRadix = 8; s = s.Substring(2); }
                else if (p == 'b' && radix != 16) { usedRadix = 2; s = s.Substring(2); }
            }

            if (s.Length == 0)
            {
                error = "Saisissez au moins un chiffre.";
                return false;
            }
            if (s.Length > MaxDigits)
            {
                error = "Nombre trop long (maximum " + MaxDigits + " chiffres).";
                return false;
            }

            BigInteger result = BigInteger.Zero;
            BigInteger r = usedRadix;
            foreach (char c in s)
            {
                int d = DigitValue(c);
                if (d < 0 || d >= usedRadix)
                {
                    error = "Chiffre « " + c + " » invalide en base " + usedRadix + " (" + RadixName(usedRadix) + ").";
                    return false;
                }
                result = result * r + d;
            }

            value = negative ? BigInteger.Negate(result) : result;
            return true;
        }

        private static int DigitValue(char c)
        {
            if (c >= '0' && c <= '9') return c - '0';
            if (c >= 'a' && c <= 'z') return c - 'a' + 10;
            if (c >= 'A' && c <= 'Z') return c - 'A' + 10;
            return -1;
        }

        /// <summary>
        /// Écrit un entier dans une base. group : paquets lisibles (binaire par 8 bits, hexadécimal par octet,
        /// décimal par milliers). prefix : 0b / 0o / 0x.
        /// </summary>
        public static string Format(BigInteger value, int radix, bool group, bool prefix, bool upperCase)
        {
            bool negative = value.Sign < 0;
            BigInteger v = BigInteger.Abs(value);

            string digits;
            if (radix == 10) digits = v.ToString();
            else if (v.IsZero) digits = "0";
            else
            {
                var chars = new List<char>();
                BigInteger r = radix;
                string alphabet = upperCase ? "0123456789ABCDEF" : "0123456789abcdef";
                while (!v.IsZero)
                {
                    BigInteger rem;
                    v = BigInteger.DivRem(v, r, out rem);
                    chars.Add(alphabet[(int)rem]);
                }
                chars.Reverse();
                digits = new string(chars.ToArray());
            }

            if (group)
            {
                int size = radix == 2 ? 8 : radix == 16 ? 2 : radix == 10 ? 3 : 0;
                if (size > 0)
                {
                    if (radix != 10 && digits.Length % size != 0)
                        digits = digits.PadLeft(digits.Length + size - digits.Length % size, '0');
                    digits = GroupFromRight(digits, size);
                }
            }

            string head = "";
            if (prefix)
                head = radix == 16 ? "0x" : radix == 2 ? "0b" : radix == 8 ? "0o" : "";
            return (negative ? "-" : "") + head + digits;
        }

        private static string GroupFromRight(string s, int size)
        {
            var sb = new StringBuilder(s.Length + s.Length / size);
            int first = s.Length % size;
            if (first > 0) sb.Append(s, 0, first);
            for (int i = first; i < s.Length; i += size)
            {
                if (sb.Length > 0) sb.Append(' ');
                sb.Append(s, i, size);
            }
            return sb.ToString();
        }

        /// <summary>Nombre de bits nécessaires (valeur absolue), au moins 1.</summary>
        public static int BitLength(BigInteger value)
        {
            BigInteger v = BigInteger.Abs(value);
            int bits = 0;
            byte[] b = v.ToByteArray();
            int top = b.Length - 1;
            while (top > 0 && b[top] == 0) top--;
            if (b[top] == 0) return 1;
            bits = top * 8;
            byte t = b[top];
            while (t != 0) { bits++; t >>= 1; }
            return bits;
        }
    }
}
