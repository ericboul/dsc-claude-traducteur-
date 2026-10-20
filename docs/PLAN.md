# Plan : Copilote vocal DCS World

> PRD source : `docs/PRD.md`

## Décisions architecturales

Décisions durables qui s'appliquent à toutes les phases :

- **Catalogue d'actions** : la *seule* interface entre le cerveau (IA) et le jeu. Chaque action porte un identifiant stable, un libellé en français, des paramètres, et une implémentation. Rien ne parle au jeu en dehors du catalogue.
- **Deux canaux vers le simulateur** : *agir* (entrées simulées — robuste, marche avec tout avion, aucun mod) et *lire* (script côté jeu → télémétrie).
- **Contrat script ↔ application** : le script vit dans le dossier du jeu ; l'application en est la source de vérité et le (ré)installe. Le bouton « Mettre à jour le script » répare une installation cassée sans édition manuelle.
- **Fichier de configuration de l'application** : chemin du simulateur, avion retenu, bindings d'entrées utilisés. Persistant entre les sessions.
- **Journal des échanges** : chaque action, son interprétation et son résultat sont journalisés et alimentent l'affichage.
- **Le copilote annonce et explique** : chaque action porte une explication en français du geste effectué dans le cockpit ; le journal dit *ce que je fais* avant le *résultat*. Une réponse « aide » liste les commandes et leur explication.

### Environnement constaté (cible)

- Simulateur : **DCS World** standalone (Eagle Dynamics), version **2.9.30.28536**, langue **FR**.
- Lancement : `Program Files\Eagle Dynamics\DCS World\bin-mt\DCS.exe`.
- Avion de la V1 : **F/A-18C** (installé).
- Dossier de jeu (`Saved Games\DCS`) : contient `Config\` (à lire/modifier) et `Scripts\` (où poser le script).
- Contrainte d'usage : le simulateur passe en **plein écran et prend le focus**.

---

## Phase 1 : Squelette + détection du jeu

**User stories** : US-8

### Ce qu'on livre

L'application démarre sur une fenêtre unique en français avec une zone de saisie et un journal vide. Elle localise le simulateur installé, reconnaît l'avion retenu, et affiche un indicateur d'état lisible (simulateur détecté/absent, jeu lancé/arrêté). Si rien n'est trouvé, elle explique quoi faire.

### Critères d'acceptation

- [x] Au premier lancement, l'application affiche « simulateur détecté » avec sa version et « F/A-18C présent ».
- [x] Si aucun simulateur n'est trouvé, un message explique comment corriger.
- [x] L'indicateur distingue « jeu lancé » de « jeu arrêté ».
- [x] L'application démarre **sans droits administrateur**.
- [x] La fenêtre reste visible et utilisable quand le simulateur est au premier plan.

## Bloquée par

- Aucune — démarrable immédiatement

---

## Phase 2 : Une action de bout en bout

**User stories** : US-1, US-3

### Ce qu'on livre

La première tranche complète : taper « train » (rentrer/sortir) et voir le train bouger réellement dans le simulateur. Le journal affiche la commande, l'action reconnue, puis le résultat. Si le jeu n'est pas lancé, l'application le dit clairement au lieu d'échouer en silence.

### Critères d'acceptation

- [x] Taper la commande et valider fait bouger le train d'atterrissage dans le simulateur.
- [x] Le journal affiche l'ordre saisi, l'action retenue et le résultat.
- [x] Simulateur non lancé → message clair, aucune tentative silencieuse.
- [x] Une commande inconnue ne déclenche rien et est signalée.

## Bloquée par

- Phase 1

---

## Phase 3 : Copilote — langage naturel, annonce et voix

**User stories** : US-2, US-11, US-12

### Ce qu'on livre

Le pilote ne tape plus un mot-clé : il formule librement (« rentre le train », « sort le train », « train up »). L'assistant choisit la bonne action, ou répond qu'il ne sait pas faire quand la phrase sort du périmètre — sans jamais rien déclencher au hasard. Et il se comporte en copilote : avant d'agir il **annonce et explique en français le geste qu'il fait dans le cockpit** (« Ce que je fais : … »), puis donne le **résultat**. La commande « aide » liste les commandes et leur explication. Enfin, il **parle** : ces mêmes phrases sont prononcées par une voix locale (sans réseau), activable par une case à cocher.

### Critères d'acceptation

- [x] 10 formulations françaises différentes de la même intention → au moins 8 déclenchent la bonne action.
- [x] Une phrase hors sujet → réponse explicite, aucune action déclenchée.
- [ ] Délai moyen entre la validation de la saisie et l'action observée ≤ 1,5 s.
- [x] La formulation libre n'introduit pas de régression sur les commandes de la phase 2.
- [x] Avant chaque action, le journal affiche une explication en français du geste effectué dans le cockpit.
- [x] « aide » liste les commandes connues avec leur explication.
- [x] Chaque action du catalogue porte une explication (aucune action muette).
- [x] Un ordre reconnu l'emporte sur une demande d'aide mêlée (« aide-moi à rentrer le train » exécute l'ordre).
- [x] Le copilote **parle** : chaque phrase journalisée (annonce, résultat, aide) est aussi prononcée, via une voix locale, sans réseau.
- [x] Une case « Le copilote parle » active/coupe la voix ; le réglage survit au redémarrage.
- [x] Si aucune voix n'est disponible, l'application écrit sans parler et l'explique — jamais de plantage.

## Bloquée par

- Phase 2

---

## Phase 4 : Script dans le jeu + bouton « mise à jour » + télémétrie

**User stories** : US-6

### Ce qu'on livre

Un script posé dans le dossier du jeu remonte l'état réel de l'appareil (au minimum : position du train, altitude, vitesse, nombre de Mach), que l'application affiche. Un bouton « Mettre à jour le script » le (ré)installe à neuf, et un indicateur signale si le script est présent, absent ou périmé.

> La fréquence radio a été **retirée** du périmètre : l'API d'export de DCS ne
> l'expose pas (`LoGetRadioBeaconsStatus` ne rend que le verrouillage de balises,
> pas une fréquence). Plutôt que d'afficher une valeur fausse, la télémétrie ne
> porte que des grandeurs réellement lisibles.

### Critères d'acceptation

- [x] Le script s'installe dans le dossier du jeu sans manipulation manuelle.
- [x] L'application affiche l'état **lu depuis le jeu**, distinct d'une valeur supposée.
- [x] Après suppression ou écrasement du script, le bouton le restaure et les informations reviennent.
- [x] L'indicateur de script distingue présent / absent / périmé.

> Vérifié par `tests/test_game_script.py` (46 contrôles) et installation réelle
> dans `Saved Games\DCS\Scripts`. L'affichage en vol demande une mission pour
> recevoir de vraies valeurs.

## Bloquée par

- Phase 2

---

## Phase 5 : Catalogue de commandes F/A-18C + liste visible

**User stories** : US-7

### Ce qu'on livre

Le catalogue d'actions s'étoffe : au moins cinq commandes du F/A-18C (train, volets, aérofreins, phares, feux de navigation, pilote automatique, radio…), toutes exécutables réellement, toutes compréhensibles en langage naturel. La liste des commandes supportées est consultable dans l'application.

### Critères d'acceptation

- [ ] Au moins **5 commandes** sont listées et exécutables depuis l'application.
- [ ] **20 essais** sur l'ensemble du catalogue → **au moins 18 réussites** (≥ 90 %).
- [ ] Chaque commande du catalogue répond à au moins une formulation en langage naturel.
- [ ] La liste visible correspond exactement à ce qui est exécutable (aucune commande fantôme).

## Bloquée par

- Phase 2
- Phase 4

---

## Phase 6 : Lancer / fermer le simulateur + lire et modifier la config

**User stories** : US-4, US-5

### Ce qu'on livre

Des boutons pour lancer et fermer le simulateur depuis l'application, et un écran pour lire puis modifier des réglages du jeu, avec sauvegarde persistante. Le lancement aboutit à une session jouable sans navigation manuelle.

### Critères d'acceptation

- [ ] Lancer depuis l'application → session jouable en **≤ 30 s** (le passage par l'écran d'accueil du simulateur est géré ou court-circuité).
- [ ] Fermer le simulateur depuis l'application fonctionne proprement.
- [ ] L'application affiche au moins un réglage du jeu et permet de le modifier, la modification étant persistée.
- [ ] Une sauvegarde de la config existante est conservée avant modification.

## Bloquée par

- Phase 1

---

## Phase 7 : Erreurs et états limites

**User stories** : US-9

### Ce qu'on livre

Tous les chemins d'échec produisent un message français explicite et non technique : simulateur absent, avion non chargé en vol, commande inconnue, script cassé par une mise à jour, action refusée par le jeu. Aucun cas ne fait planter ni bloquer l'application.

### Critères d'acceptation

- [ ] Chacun des cas d'échec listés produit un message clair et non technique.
- [ ] Aucun de ces cas ne fait planter ou figer l'application.
- [ ] Les erreurs restent consultables dans le journal.
- [ ] Après une erreur, l'application reste utilisable sans redémarrage.

## Bloquée par

- Phase 5

---

## Phase 8 : Voix (V2)

**User stories** : US-10

### Ce qu'on livre

La même application, pilotée à la voix : le pilote maintient un bouton de son manche et parle ; l'assistant exécute puis répond vocalement. Le texte reste disponible en parallèle.

### Critères d'acceptation

- [ ] Maintenir le bouton et prononcer un ordre du catalogue → l'action s'exécute dans le simulateur.
- [x] La réponse de l'assistant est restituée vocalement.
- [ ] Délai entre le relâchement du bouton et l'action ≤ 1,5 s pour les commandes du catalogue.
- [x] Le mode texte continue de fonctionner à l'identique.

> Amorçage fait : **Shift+A maintenu** (hook clavier global, actif seulement
> quand le simulateur est au premier plan ; la touche ne part pas vers le
> cockpit) → écoute Vosk française hors-ligne → même pipeline que le texte.
> Démarrage : `copilote/hotkey.py` et `copilote/voice.py`, vérifiés par
> `tests/test_voice_ptt.py`. Restent à confirmer en vol : l'exécution réelle
> et le délai. Le catalogue doit aussi atteindre 5 commandes (Phase 5) pour
> que la voix ait de quoi piloter.

## Bloquée par

- Phase 5
- Phase 7
