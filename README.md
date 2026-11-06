<div align="center">

<img src="icon.ico" width="96" alt="Logo Texte vers Hexadécimal" />

# Texte vers Hexadécimal

**Convertissez du texte lisible en code hexadécimal (et inversement), et vos nombres entre décimal, hexadécimal, binaire et octal.**

[![Release](https://img.shields.io/github/v/release/FilleuxStudio/TextToHexaConverter?include_prereleases&label=version&color=0E7490)](https://github.com/FilleuxStudio/TextToHexaConverter/releases/latest)
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4)](https://dotnet.microsoft.com/download/dotnet-framework/net48)
[![Windows](https://img.shields.io/badge/Windows-7%20SP1%20%7C%208.1%20%7C%2010%20%7C%2011-0078D4)](#-compatibilité)
[![Dernier commit](https://img.shields.io/github/last-commit/FilleuxStudio/TextToHexaConverter?color=4F46E5)](https://github.com/FilleuxStudio/TextToHexaConverter/commits/main)

[Télécharger](https://github.com/FilleuxStudio/TextToHexaConverter/releases/latest) ·
[Fonctionnalités](#-fonctionnalités) ·
[Exemples](#-exemples) ·
[Compatibilité](#-compatibilité) ·
[Compiler](#-compiler-le-projet)

<!-- Ajoutez une capture d'écran réalisée sous Windows : docs/screenshot.png -->
<!-- <img src="docs/screenshot.png" width="860" alt="Capture de l'application" /> -->

</div>

---

## Présentation

**Texte vers Hexadécimal** est un outil Windows pour passer d'un texte lisible à sa représentation en **octets
hexadécimaux** (le « code machine » du texte), et inversement. Il est pensé pour les développeurs, les étudiants
en informatique et toute personne qui doit lire, écrire ou vérifier des données brutes : protocoles, fichiers,
chaînes à intégrer dans du code, exercices de systèmes et de réseaux.

La conversion est **instantanée, dans les deux sens** : écrivez du texte pour obtenir l'hexadécimal, ou collez de
l'hexadécimal pour retrouver le texte.

## 🚀 Fonctionnalités

### Texte ↔ Hexadécimal

| | |
| --- | --- |
| **Conversion bidirectionnelle en direct** | texte → hexadécimal et hexadécimal → texte, sans bouton à presser |
| **Vrais octets, selon l'encodage** | UTF-8, ASCII, UTF-16 (little-endian) et Windows-1252 : `é` = `C3 A9` en UTF-8, `E9` en Windows-1252 |
| **Présentation au choix** | `48 65 6C`, `48656C`, `0x48 0x65`, `\x48\x65` ou tableau C/C#/Java `0x48, 0x65` |
| **Majuscules / minuscules** | `FF` ou `ff` |
| **Saisie hexadécimale tolérante** | accepte espaces, virgules, préfixes `0x` et `\x`, accolades, majuscules ou minuscules |
| **Messages clairs** | octets invalides, nombre impair de chiffres, caractères absents de l'encodage, octets non affichables |
| **Compteurs** | nombre de caractères et d'octets en temps réel |

### Nombres

- Saisie en **décimal, hexadécimal, binaire ou octal**, avec reconnaissance des préfixes `0x`, `0b` et `0o`
- Résultats simultanés dans les **quatre bases**, avec copie en un clic
- **Nombres de taille illimitée** (jusqu'à 4 096 chiffres) et **nombres négatifs**
- Options : regroupement des chiffres (octets, milliers), préfixes, majuscules
- Affichage de la taille : nombre de bits et d'octets nécessaires

### Confort

- Copier / coller en un clic, presse-papiers protégé contre les erreurs
- Interface moderne, adaptée aux **écrans haute résolution (DPI)**, navigation au clavier
- Raccourcis : `Ctrl+1` / `Ctrl+2` pour changer d'onglet, `Ctrl+Maj+C` pour copier l'hexadécimal

## Exemples

| Texte | Encodage | Résultat |
| --- | --- | --- |
| `Hello` | UTF-8 | `48 65 6C 6C 6F` |
| `é` | UTF-8 | `C3 A9` |
| `é` | Windows-1252 | `E9` |
| `€` | UTF-8 | `E2 82 AC` |
| `A` | UTF-16 | `41 00` |
| `Hi` | UTF-8, format `\x48` | `\x48\x69` |
| `Hi` | UTF-8, format `0x48,` | `0x48, 0x69` |

Une saisie à retour à la ligne produit `0D 0A` (retour chariot + saut de ligne), comme sous Windows.

## 📦 Installation

1. Vérifiez que **.NET Framework 4.8** est installé (voir ci-dessous).
2. Téléchargez la dernière version depuis la page [Releases](https://github.com/FilleuxStudio/TextToHexaConverter/releases/latest).
3. Lancez `TextToHex.exe` : aucune installation n'est nécessaire.

## Compatibilité

L'application cible **.NET Framework 4.8** et fonctionne sur les versions de Windows qui le prennent en charge :

| Windows | Prise en charge | .NET Framework 4.8 |
| --- | :---: | --- |
| **Windows 11** | ✅ | Inclus (4.8.1 sur les versions récentes, compatible) |
| **Windows 10** (1607 et ultérieures) | ✅ | Inclus à partir de la version 1903 ; à installer avant |
| **Windows 8.1** | ✅ | [À installer](https://dotnet.microsoft.com/download/dotnet-framework/net48) |
| **Windows 7 SP1** | ✅ | [À installer](https://dotnet.microsoft.com/download/dotnet-framework/net48) (Service Pack 1 et mises à jour Windows requis) |
| **Windows 8** (non 8.1) | ⚠️ | Non pris en charge par .NET 4.8 : mettez à jour vers Windows 8.1 |

> [!NOTE]
> Windows 7, 8 et 8.1 ne reçoivent plus de mises à jour de sécurité de Microsoft. Le logiciel y fonctionne,
> mais ces systèmes ne sont plus recommandés. Les architectures **32 bits et 64 bits** sont prises en charge
> (l'exécutable est compilé en *AnyCPU*).

**Quel est mon .NET Framework ?** Ouvrez une invite de commandes et tapez :

```bat
reg query "HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" /v Release
```

La valeur `Release` doit être **528040** ou plus (4.8 = 528040 et plus ; 4.8.1 = 533320 et plus).

## Bon à savoir

- Un texte n'a pas **un** hexadécimal : il dépend de l'**encodage**. Pour un fichier ou un protocole moderne, utilisez UTF-8.
- L'**ASCII** ne contient que les caractères anglais : les autres sont remplacés par `?` (avec un avertissement).
- Les octets de contrôle (`00`, `07`, `1B`…) ne sont pas affichables dans une zone de texte : ils sont représentés par `·`
  dans la zone de texte, mais **conservés** dans l'hexadécimal.
- Les nombres négatifs sont affichés avec un signe `-` devant la valeur absolue (pas en complément à deux).

## Compiler le projet

**Prérequis développeur**

- [Visual Studio 2022](https://visualstudio.microsoft.com/fr/downloads/) avec la charge de travail *Développement de
  bureau .NET*
- Le *pack de développement .NET Framework 4.8*

**Étapes**

```bat
git clone https://github.com/FilleuxStudio/TextToHexaConverter.git
cd TextToHexaConverter
msbuild TextToHex.sln /p:Configuration=Release
```

L'exécutable est généré dans `TextToHex\bin\Release\`. Vous pouvez aussi ouvrir `TextToHex.sln` dans Visual Studio
et appuyer sur `F5`.

## Structure du projet

```text
TextToHex/
├── HexConverter.cs          Moteur : encodages, hexadécimal, conversion de nombres (sans interface)
├── FRM_Main.cs              Logique de la fenêtre principale
├── FRM_Main.Designer.cs     Construction de l'interface
├── Program.cs               Point d'entrée
├── UI/
│   ├── Theme.cs             Palette, polices, échelle DPI, utilitaires de dessin
│   └── Controls.cs          Cartes, boutons, champs, sélecteurs, interrupteurs, ligne d'état
└── app.manifest             Compatibilité Windows et prise en charge DPI
```

Le moteur (`HexConverter.cs`) ne dépend pas de l'interface et peut être réutilisé tel quel dans un autre projet.

## Auteur

Développé par **[FilleuxStudio](http://www.filleuxstudio.fr/)**.