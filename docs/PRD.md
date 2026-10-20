# PRD — Copilote vocal DCS World

## Problème

Un pilote de simulateur de combat a besoin de ses deux mains sur le manche et les gaz. Passer par des menus, des raccourcis clavier ou une tablette pour piloter l'avion ou lancer une mission casse l'immersion et fait perdre du temps en plein vol. Il n'existe aujourd'hui aucun assistant qui comprenne une phrase en français, agisse dans le simulateur, lui réponde, et fonctionne entièrement sur sa machine sans dépendre du réseau.

## Solution

Une application Windows qui donne au pilote un copilote virtuel. Le pilote s'adresse à lui en langage naturel ; l'assistant exécute l'action dans le simulateur et lui répond. Il peut aussi lancer le simulateur, lire et modifier sa configuration, et commander le cockpit. La première version se pilote au clavier/texte ; la voix viendra ensuite sur la même base.

## Utilisateur cible

Un passionné de simulation de combat aérien (DCS World), jouant en solo sur PC Windows avec un manche/HOTAS. Il parle français, n'est pas informaticien, et joue en sessions de 1 à 3 h, souvent mains occupées.

## User Stories

- **US-1** — En tant que pilote, je veux saisir un ordre en texte, afin de le voir exécuté dans le simulateur.
- **US-2** — En tant que pilote, je veux formuler mon ordre en français naturel, afin de ne pas mémoriser de commandes exactes.
- **US-3** — En tant que pilote, je veux voir ce que l'assistant a compris et fait, afin de garder confiance dans le système.
- **US-4** — En tant que pilote, je veux lancer et fermer le simulateur depuis l'assistant, afin de gérer ma session sans naviguer.
- **US-5** — En tant que pilote, je veux lire et modifier la configuration du jeu, afin d'adapter mon poste de pilotage.
- **US-6** — En tant que pilote, je veux réinstaller le script du jeu en un clic, afin de ne jamais rester bloqué après une mise à jour du simulateur.
- **US-7** — En tant que pilote, je veux consulter la liste des actions supportées, afin de savoir ce que je peux demander.
- **US-8** — (état vide) Au premier lancement, je veux savoir si le simulateur est détecté, afin de comprendre comment démarrer.
- **US-9** — (erreur) Si une commande échoue, je veux un message clair, afin de pouvoir la corriger.
- **US-10** — (V2) En tant que pilote, je veux parler au lieu de taper, afin de garder les mains sur le manche.
- **US-11** — En tant que pilote, je veux que le copilote annonce et explique en français le geste qu'il fait dans le cockpit, afin de comprendre ce qui se passe dans l'avion sans quitter le vol des yeux.
- **US-12** — En tant que pilote, je veux que le copilote **me parle** (synthèse vocale locale), afin de l'entendre sans lire, les mains sur le manche et les yeux sur l'écran.

## Critères de succès

- Sur **1 avion**, **5 à 10 commandes clés** s'exécutent réellement dans le simulateur depuis une saisie texte, avec **≥ 90 % de réussite sur 20 essais**.
- Le **lancement du simulateur** depuis l'application aboutit à une session jouable en **≤ 30 s**.
- Le **bouton de mise à jour du script** restaure des commandes fonctionnelles après une mise à jour du simulateur, **sans édition manuelle**.
- L'application **démarre sans droits administrateur**.
- Délai moyen **texte → action observée ≤ 1,5 s**.

## Hors périmètre

- La **dictée vocale** (parler à l'assistant) : réservée à la V2. (La **synthèse vocale** — l'assistant qui parle — passe en V1, cf. US-12.)
- **Plusieurs avions** : un seul en V1.
- **Multijoueur / réseau**, missions dynamiques.
- Autre langue que le **français**.
- **Chaque interrupteur** du cockpit : on couvre un sous-ensemble choisi.
- Tout autre jeu que **DCS World**.
- **macOS / Linux / mobile**.

## Décisions d'implémentation

- Une **seule fenêtre**, en français : zone de saisie + journal des échanges.
- Une **liste visible** des commandes supportées.
- Un bouton **« Mettre à jour le script »**.
- Un indicateur d'**état du simulateur** (détecté / absent).
- **Aucune confirmation** demandée avant une action : le pilote a choisi le contrôle total (y compris eject et quitter).
- Réponses affichées dans la fenêtre, avec **option d'affichage dans le jeu**.
- Le copilote **annonce et explique ses actions** : chaque action porte une explication en français du geste cockpit, journalisée avant le résultat ; une commande « aide » liste les actions et leur explication.
- Le copilote **parle** : synthèse vocale **locale** (voix Windows, aucun réseau), activable par une case « Le copilote parle », retenue d'une session à l'autre. Une phrase = journalisée **et** prononcée.
- **Aucune connexion internet** requise.

## Notes complémentaires

- **Risques** : le script du jeu peut casser à chaque mise à jour (mitigé par le bouton) ; sans confirmation, un ordre mal compris (tir, eject) peut faire perdre la mission — risque **assumé** par le pilote.
- **Dépendances** : le simulateur installé + l'avion retenu.
- **Hypothèse** : le modèle d'IA local tourne assez vite sur la machine cible.
- **Référence** : vidéo YouTube `XsmTIRjkpCA` (inspiration ; contenu non consulté, YouTube inaccessible).
