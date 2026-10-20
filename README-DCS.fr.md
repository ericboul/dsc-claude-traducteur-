# OpenClaude DCS Traducteur

> **Traduisez automatiquement vos missions DCS World de l'anglais vers le français, sans quitter votre PC.**

Ce module ajoute à OpenClaude Portable un onglet **DCS** dans le tableau de bord web, ainsi qu'un lanceur
Windows prêt à l'emploi : `OpenClaude-DCS-Traducteur.exe`.

Il s'appuie sur [miz-translator](https://github.com/leonchen83/miz-translator) de **leonchen83** (licence GPL-3.0)
et sur une IA au choix :

- **DeepSeek** (cloud, rapide, nécessite une clé API)
- **Ollama** (100 % local, gratuit, plus lent sur CPU)

---

## Ce que fait l'outil

1. Vous choisissez un dossier contenant des missions `.miz`.
2. Vous cochez une ou plusieurs missions.
3. L'outil copie chaque mission dans un dossier de travail, extrait les textes
   (briefings, dialogues radio, descriptions), les traduit en français, puis reconstruit l'archive.
4. Le résultat est enregistré **à côté de la mission d'origine** sous la forme :

```
Ma Mission.miz      →  Ma Mission_FR.miz
```

La mission d'origine n'est **jamais modifiée** : vous pouvez comparer, revenir en arrière ou supprimer
le fichier `_FR` à tout moment.

La traduction française est stockée dans `l10n/FR/dictionary` à l'intérieur du `.miz`.
DCS l'utilise lorsque le jeu est configuré en français (voir plus bas).

---

## Prérequis

| Élément | Détail |
|---|---|
| **Système** | Windows 10 ou 11 (64 bits) |
| **DCS World** | Toute version (stable ou Open Beta) |
| **Internet** | Au premier lancement pour télécharger miz-translator (~150 Mo) et, si vous utilisez DeepSeek, pour les appels API |
| **Clé DeepSeek** | Créée sur [platform.deepseek.com](https://platform.deepseek.com) — ou utilisez Ollama en local |
| **Java** | Inutile : le JDK portable est inclus dans le paquet miz-translator |

---

## Démarrage rapide

### 1. Lancer le traducteur

> **Premier téléchargement du projet ?** Lancez d'abord `START.bat` une fois : il installe Node.js
> et le moteur OpenClaude dans `engine/`. Sans cela, le lanceur affichera « Node.js introuvable ».

Double-cliquez ensuite sur :

```
OpenClaude-DCS-Traducteur.exe
```

Au premier lancement :

- si miz-translator n'est pas encore installé, une fenêtre vous propose de le télécharger ;
  acceptez, puis patientez pendant le téléchargement (~150 Mo) et l'extraction ;
- l'installateur est `tools/install_miz_translator.bat` ; il peut aussi être lancé manuellement ;
- une fois l'installation terminée, relancez `OpenClaude-DCS-Traducteur.exe`.

Le lanceur démarre le serveur du tableau de bord en arrière-plan (port **3000**) puis ouvre votre
navigateur sur l'onglet DCS :

```
http://localhost:3000/#dcs
```

Si le serveur tourne déjà (par exemple lancé depuis `START.bat`), le lanceur réutilise l'instance existante.

### 2. Configurer l'IA (une seule fois)

#### Option A — DeepSeek (recommandé)

1. Ouvrez l'onglet **Setup** du tableau de bord (ou l'option 4 de `START.bat`).
2. Sélectionnez le fournisseur **DeepSeek**.
3. Collez votre clé API (`sk-...`).
4. Choisissez le modèle `deepseek-flash` (rapide) ou `deepseek-v4-pro` (plus puissant).

La clé est enregistrée uniquement dans `data/ai_settings.env`, sur votre support portable.

#### Option B — Ollama (hors ligne)

1. Ouvrez l'onglet **System** et démarrez Ollama.
2. Installez un modèle via **Setup Local Models** (par exemple `qwen2.5:7b` ou `gemma3:4b`).
3. Dans l'onglet DCS, choisissez le fournisseur **Ollama** : la liste des modèles installés apparaît.

> Ollama doit écouter sur `http://127.0.0.1:11434`. Aucun texte ne sort de votre machine.

### 3. Traduire des missions

1. Dans l'onglet **DCS**, vérifiez que le statut affiche **Prêt**
   (`miz-translator` + Java portable détectés).
2. Indiquez le dossier des missions :
   - cliquez sur une suggestion (par exemple `Saved Games\DCS\Missions`),
   - ou collez un chemin puis cliquez sur **Charger**.
3. Cochez les missions souhaitées (50 maximum par lot).
   Le badge **FR existe** signale qu'une version `_FR.miz` est déjà présente.
4. Choisissez le fournisseur et le modèle.
5. Cliquez sur **Traduire en français (N)**.
6. Suivez la progression dans le journal. À la fin, chaque mission est listée avec le statut
   **OK** ou **ÉCHEC** et le chemin du fichier produit.

> Une mission de taille moyenne prend généralement de quelques dizaines de secondes à quelques minutes,
> selon le modèle et le nombre de textes (un essai sur une mission F-15E a pris environ 70 secondes
> avec `deepseek-flash`).

---

## Régler DCS en français

Pour que DCS affiche les textes traduits, le jeu doit être configuré en français :

1. Lancez DCS World.
2. Ouvrez **OPTIONS**.
3. Selon la version : onglet **Gameplay** (ou **MISC.**) → champ **Language** → **Français**.
4. Redémarrez DCS.
5. Lancez la mission `_FR.miz` dans l'éditeur ou depuis le menu des missions.

Sans ce réglage, DCS reste en anglais et continue d'afficher le dictionnaire d'origine
(`l10n/DEFAULT`), même si le fichier `_FR.miz` contient bien la traduction.

---

## Où sont les fichiers ?

| Chemin | Contenu |
|---|---|
| `<dossier missions>\NomMission_FR.miz` | Mission traduite, à côté de l'originale |
| `data/dcs-translate/<id>/job.log` | Journal détaillé d'une traduction |
| `data/dcs-translate/<id>/trans.conf` | Configuration générée pour miz-translator |
| `tools/miz-translator/miz-translator/` | Outil miz-translator et JDK portable (~150 Mo) |

Le dossier `data/dcs-translate` peut être supprimé sans risque lorsque vous n'avez plus besoin
des journaux ; il est recréé automatiquement.

---

## Dépannage

| Symptôme | Solution |
|---|---|
| Statut **Indisponible** dans l'onglet DCS | miz-translator n'est pas installé : lancez `tools/install_miz_translator.bat` ou relancez l'exe et acceptez le téléchargement |
| `ECHEC du telechargement` | Vérifiez la connexion Internet, le pare-feu ou l'antivirus ; téléchargez l'archive depuis [github.com/leonchen83/miz-translator/releases](https://github.com/leonchen83/miz-translator/releases) et extrayez-la dans `tools/miz-translator/` |
| `Cle API DeepSeek manquante` | Renseignez la clé dans l'onglet **Setup** |
| `Aucun modèle Ollama disponible` | Démarrez Ollama (onglet **System**) et vérifiez qu'un modèle est installé |
| La traduction reste en anglais dans DCS | Réglez la langue du jeu sur **Français** et redémarrez DCS |
| `Port 3000 déjà utilisé` | Le tableau de bord tourne déjà : ouvrez directement `http://localhost:3000/#dcs` |
| Une mission échoue | Consultez `data/dcs-translate/<id>/job.log`, puis réessayez avec un autre modèle ou une mission seule |
| L'antivirus bloque l'exe | Ajoutez une exception pour le dossier OpenClaude Portable (l'exe lance simplement Node.js en local) |

---

## Limites connues

- Traduction de l'**anglais vers le français** uniquement (le code source de miz-translator gère
  d'autres langues, mais cette intégration est réglée sur `fr-FR`).
- 50 missions maximum par lot.
- Les dialogues générés par script Lua ne sont pas traduits (`lua=false` dans la configuration).
- Certains textes très courts ou purement techniques peuvent rester en anglais : c'est normal,
  l'outil ignore les chaînes de moins de 8 caractères et certains sigles.

---

## Crédits et licences

- **OpenClaude Portable** — licence MIT.
- **miz-translator** — © [leonchen83](https://github.com/leonchen83/miz-translator), licence **GPL-3.0**.
  L'outil n'est pas redistribué dans ce dépôt : il est téléchargé à la demande depuis ses releases officielles.
  Si vous redistribuez une version modifiée de miz-translator, vous devez respecter les termes de la GPL-3.0.
- **DeepSeek** — service tiers soumis à ses propres conditions d'utilisation.
- **Ollama** — logiciel libre, exécution locale.

---

## Installation manuelle de miz-translator

Si vous préférez installer l'outil vous-même :

1. Téléchargez `miz-translator-release.zip` depuis
   [github.com/leonchen83/miz-translator/releases](https://github.com/leonchen83/miz-translator/releases).
2. Extrayez l'archive dans `tools/miz-translator/`.
3. Vérifiez que ce fichier existe :

```
tools\miz-translator\miz-translator\lib\miz-translator-fat.jar
```

4. Relancez `OpenClaude-DCS-Traducteur.exe`.

---

*Bonne traduction, et bons vols !*
