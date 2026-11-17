# 📚 Documentation Utilisateur - ErAtlas

## Table des matières
- [Vue d'ensemble](#vue-densemble)
- [Installation et première utilisation](#installation-et-première-utilisation)
- [Interface principale](#interface-principale)
- [Guide des pages](#guide-des-pages)
- [Gestion des trajets](#gestion-des-trajets)
- [Gestion des utilisateurs](#gestion-des-utilisateurs)
- [Paramètres et sécurité](#paramètres-et-sécurité)
- [Dépannage](#dépannage)
- [Foire aux questions](#foire-aux-questions)

---

## Vue d'ensemble

### Qu'est-ce qu'ErAtlas ?

**ErAtlas** est une application de gestion de trajets et de transports destinée aux organisations Erasmus. Elle permet :

✅ **Aux utilisateurs classiques** :
- Consulter leurs trajets assignés
- Voir les détails de chaque trajet (départ, arrivée, horaires, type de transport)
- Gérer leurs paramètres personnels
- Se déconnecter

✅ **Aux gestionnaires** :
- Créer, modifier et supprimer les trajets
- Gérer les utilisateurs (création, modification, suppression)
- Gérer les lieux et les transports
- Superviser l'ensemble des données

### Caractéristiques principales

- **Interface intuitive** : navigation facile entre les différentes sections
- **Sécurité** : connexion sécurisée avec mot de passe haché
- **Gestion d'accès** : deux rôles (utilisateur et gestionnaire)
- **Données centralisées** : base de données SQL Server pour la persistance

---

## Installation et première utilisation

### Prérequis système

Avant de commencer, assurez-vous que votre ordinateur dispose de :
- **Windows 10 ou 11**
- **.NET Runtime 9.0** ou supérieur
- **SQL Server** (local ou distant) avec une base de données `ErAtlas` configurée
- **SSMS** (SQL Server Management Studio) - recommandé pour la gestion de la base

### Lancer l'application

1. **Télécharger/Extraire** le fichier d'installation ou la solution du projet
2. **Ouvrir** le dossier du projet
3. **Localiser et exécuter** le fichier `ErAtlas.exe` dans le dossier `bin`
   - Ou depuis Visual Studio/Rider : cliquer sur le bouton **▶ Démarrer**

> ⚠️ **Note** : L'application nécessite que SQL Server soit démarré et accessible avant le lancement.

### Premier démarrage

À l'ouverture de l'application, l'écran de **connexion** s'affichera. Vous devrez entrer :
- **Login** : votre identifiant
- **Mot de passe** : votre mot de passe

Demandez vos identifiants à l'administrateur système si vous n'en disposez pas.

---

## Interface principale

### Structure générale

L'application se divise en plusieurs zones :

```
┌─────────────────────────────────────┐
│         Barre de navigation         │
│  [Logo]  [Menu]  [Paramètres/Déco] │
├─────────────────────────────────────┤
│                                     │
│         ZONE DE CONTENU             │
│    (Change selon la page active)    │
│                                     │
└─────────────────────────────────────┘
```

### Le menu de navigation

Une fois connecté, vous pouvez accéder aux pages via :

**Pour les utilisateurs classiques** :
- 🏠 **Accueil** : tableau de bord principal
- ✈️ **Mes trajets** : vos trajets assignés
- ⚙️ **Paramètres** : gérer votre profil et vous déconnecter

**Pour les gestionnaires** (rôle Gestionnaire = Oui) :
- 🏠 **Accueil** : tableau de bord principal
- ✈️ **Mes trajets** : vos trajets assignés
- 📋 **Gestion des trajets** : créer/modifier/supprimer les trajets
- 👥 **Gestion des utilisateurs** : gérer les comptes utilisateurs
- ⚙️ **Paramètres** : gérer votre profil et vous déconnecter

---

## Guide des pages

### 1️⃣ Écran de connexion

**Objectif** : Accéder à l'application en toute sécurité

#### Étapes :

1. Entrez votre **login** (identifiant)
2. Entrez votre **mot de passe**
3. Cliquez sur le bouton **Connexion** (ou appuyez sur Entrée)

#### Cas d'erreur :

- ❌ **"Identifiants invalides"** : Vérifiez que votre login et mot de passe sont corrects
- ❌ **"Impossible de se connecter à la base"** : Vérifiez que SQL Server est en ligne
- ❌ **"Utilisateur non trouvé"** : Demandez vos identifiants à l'administrateur

#### 💡 Conseil sécurité

- Ne partagez jamais votre mot de passe
- Déconnectez-vous si vous quittez l'ordinateur
- N'écrivez jamais votre mot de passe sur le clavier d'un ordinateur public

---

### 2️⃣ Écran d'accueil

**Objectif** : Vue d'ensemble de l'application

#### Contenu affiché :

- Bienvenue avec votre prénom
- Nombre de trajets à venir (si applicable)
- Accès rapide aux fonctionnalités principales
- Statut de la connexion

#### Actions disponibles :

- Accédez directement à vos trajets via les boutons de raccourci
- Naviguez vers la gestion (si vous êtes gestionnaire)

---

### 3️⃣ Page "Mes trajets"

**Objectif** : Consulter tous vos trajets assignés

#### Affichage des trajets

La liste montre pour chaque trajet :

| Élément | Signification |
|---------|---------------|
| **Départ** | Lieu et date/heure de départ |
| **Arrivée** | Lieu et date/heure d'arrivée |
| **Transport** | Type et détails du moyen de transport |
| **Statut** | État du trajet (À venir, En cours, Terminé) |

#### Exemple d'affichage

```
🚌 Bus · Capacité: 45 | Immat: AB-123-CD

Lyon → Paris
📍 Départ : 13 Mai 2026 à 08:30
📍 Arrivée : 13 Mai 2026 à 12:45
Status: À venir
```

#### Actions possibles

- **Cliquer sur un trajet** : voir ses détails complets
- **Actualiser** : mettre à jour la liste (si bouton disponible)

---

### 4️⃣ Page "Détails du trajet"

**Objectif** : Voir les informations complètes d'un trajet

#### Informations affichées

- **Lieux** :
  - 📍 Lieu de départ (adresse complète)
  - 📍 Lieu d'arrivée (adresse complète)

- **Dates et heures** :
  - 🕐 Date et heure de départ exactes
  - 🕐 Date et heure d'arrivée exactes

- **Transport** :
  - 🚌 Type du transport (Bus, Train, Bateau, Avion)
  - 👥 Capacité (nombre de places)
  - 🔖 Immatriculation
  - 📝 Description additionnelle

- **Statut du trajet** :
  - Validé / À venir / En cours / Terminé / Annulé

#### Actions possibles

- **Retour** : revenir à la liste des trajets
- **Partager** : partager les détails (si disponible)

---

### 5️⃣ Page "Gestion des trajets" (Gestionnaires seulement)

**Objectif** : Créer, modifier et supprimer les trajets

#### Affichage

- **Tableau de trajets** avec :
  - ID du trajet
  - Dates de départ/arrivée
  - Heures
  - Lieux
  - Transport utilisé
  - Statut

#### Créer un nouveau trajet

1. Cliquez sur **➕ Ajouter un trajet** (ou bouton similaire)
2. Remplissez le formulaire :

   | Champ | Description | Exemple |
   |-------|-------------|---------|
   | **Date de départ** | Date formatée (JJ/MM/AAAA) | 15/05/2026 |
   | **Heure de départ** | Heure (HH:MM) | 14:30 |
   | **Date d'arrivée** | Date d'arrivée | 15/05/2026 |
   | **Heure d'arrivée** | Heure d'arrivée | 18:45 |
   | **Lieu de départ** | Sélectionner dans liste | Paris |
   | **Lieu d'arrivée** | Sélectionner dans liste | Berlin |
   | **Transport** | Sélectionner un transport | Bus |
   | **Statut** | État du trajet | À venir |

3. Cliquez sur **Créer** ou **Enregistrer**

#### Modifier un trajet

1. Sélectionnez le trajet dans la liste
2. Cliquez sur **✏️ Modifier** (ou double-clic)
3. Modifiez les informations nécessaires
4. Cliquez sur **Enregistrer** pour valider

#### Supprimer un trajet

1. Sélectionnez le trajet
2. Cliquez sur **🗑️ Supprimer**
3. Confirmez la suppression

> ⚠️ **Attention** : La suppression est définitive et ne peut pas être annulée

---

### 6️⃣ Page "Gestion des utilisateurs" (Gestionnaires seulement)

**Objectif** : Gérer les comptes utilisateurs

#### Affichage

Liste de tous les utilisateurs avec :
- ID Utilisateur
- Nom et Prénom
- Email
- Téléphone
- Adresse
- Statut (Gestionnaire : Oui/Non)

#### Créer un nouvel utilisateur

1. Cliquez sur **➕ Ajouter un utilisateur**
2. Remplissez le formulaire :

   | Champ | Description | Requis | Exemple |
   |-------|-------------|--------|---------|
   | **Nom** | Nom de famille | ✅ | Dupont |
   | **Prénom** | Prénom | ✅ | Marie |
   | **Email** | Adresse email | ✅ | marie.dupont@example.com |
   | **Login** | Identifiant unique | ✅ | mdupont |
   | **Mot de passe** | Sécurisé (8+ caractères) | ✅ | P@ssw0rd123 |
   | **Téléphone** | Numéro | ❌ | 0612345678 |
   | **Adresse** | Adresse complète | ❌ | 123 Rue de Paris |
   | **Code Postal** | Numéro postal | ❌ | 75001 |
   | **Ville** | Ville de résidence | ❌ | Paris |
   | **Gestionnaire** | Accès gestionnaire | ❌ | ☐ (cocher si oui) |

3. Cliquez sur **Créer**

#### Modifier un utilisateur

1. Sélectionnez l'utilisateur dans la liste
2. Cliquez sur **✏️ Modifier** ou double-cliquez
3. Modifiez les informations
4. Sauvegardez les changements

#### Supprimer un utilisateur

1. Sélectionnez l'utilisateur
2. Cliquez sur **🗑️ Supprimer**
3. Confirmez la suppression

> ⚠️ **Important** : La suppression d'un utilisateur supprime aussi ses données associées

---

### 7️⃣ Page "Paramètres"

**Objectif** : Gérer votre compte et vos préférences

#### Actions disponibles

**Informations personnelles** :
- Voir votre nom et prénom
- Voir votre email
- Voir votre numéro de téléphone (si complété)
- Voir votre adresse et ville

**Sécurité** :
- 🔄 Réinitialiser le mot de passe (si fonctionnalité disponible)

**Actions** :
- 🚪 **Déconnexion** : quitter l'application de manière sécurisée
  - Cliquez sur le bouton **Déconnexion**
  - Vous serez ramené à l'écran de connexion
  - Votre session sera fermée

---

## Gestion des trajets

### Cycle de vie d'un trajet

```
Créé → À venir → En cours → Terminé → Archivé
         ↓
       Modifié
         ↓
      Supprimé (si nécessaire)
```

### Statuts des trajets

| Statut | Signification | Color Icon |
|--------|---------------|-----------|
| **À venir** | Trajet programmé, non encore commencé | 🟢 |
| **En cours** | Trajet actuellement en déplacement | 🟡 |
| **Terminé** | Trajet complété avec succès | 🔵 |
| **Annulé** | Trajet cancelled pour une raison quelconque | 🔴 |

### Types de transports supportés

| Type | Capacité typique | Utilisation |
|------|------------------|-------------|
| 🚌 **Bus** | 45-80 places | Transport groupé routier |
| 🚂 **Train** | 200-400 places | Trajets ferroviaires longue distance |
| 🚢 **Bateau** | 100-1000 places | Trajets maritimes |
| ✈️ **Avion** | 150-350 places | Trajets aériens internationaux |

---

## Gestion des utilisateurs

### Rôles et permissions

#### Utilisateur classique
- Consultation de ses trajets
- Modification de son profil (limité)
- Déconnexion

#### Gestionnaire
- Création de trajets
- Modification de trajets
- Suppression de trajets
- Création d'utilisateurs
- Modification d'utilisateurs
- Suppression d'utilisateurs
- Accès à toutes les données

### Champs utilisateur obligatoires vs optionnels

**Obligatoires** ✅ :
- Nom
- Prénom
- Email (doit être valide)
- Login (unique dans le système)
- Mot de passe (minimum 8 caractères recommandé)

**Optionnels** ❌ :
- Numéro de téléphone
- Adresse
- Code postal
- Ville

### Bonnes pratiques

✅ **À faire** :
- Utiliser des mots de passe forts (au moins 8 caractères, avec majuscules, chiffres, caractères spéciaux)
- Vérifier que les emails sont uniques
- Documenter les logins utilisés
- Archiver les utilisateurs inactifs au lieu de les supprimer

❌ **À éviter** :
- Réutiliser le même mot de passe pour plusieurs utilisateurs
- Utiliser des logins génériques (ex: "user1", "user2")
- Conserver des comptes de test en production
- Donner l'accès gestionnaire à tous les utilisateurs

---

## Paramètres et sécurité

### Sécurité de connexion

L'application utilise :
- **Hachage SHA-256** : les mots de passe sont hachés et non stockés en clair
- **Connexion vérifiée** : chaque tentative de connexion est validée contre la base
- **Session sécurisée** : votre session est isolée et protégée

### Recommandations de sécurité

1. **Mots de passe** :
   - Changez votre mot de passe tous les 3-6 mois
   - N'utilisez jamais un mot de passe faible
   - Ne le partagez avec personne

2. **Déconnexion** :
   - Déconnectez-vous toujours après utilisation
   - Fermer la fenêtre ne suffit pas

3. **Confidentialité** :
   - Les données de trajets sont confidentielles
   - Ne transférez pas les données en dehors du système

4. **Accès gestionnaire** :
   - Accordez ce rôle avec prudence
   - Les gestionnaires ont accès à toutes les données

---

## Dépannage

### Problèmes de connexion

#### 🔴 "Impossible de se connecter à la base de données"

**Causes possibles** :
- SQL Server n'est pas démarré
- La base de données "ErAtlas" n'existe pas
- La chaîne de connexion est incorrecte
- Problème réseau/serveur

**Solutions** :
1. Vérifiez que SQL Server est en cours d'exécution
2. Vérifiez la base "ErAtlas" existe dans SSMS
3. Vérifiez les droits d'accès au serveur SQL
4. Redémarrez l'application

---

#### 🔴 "Identifiants invalides"

**Causes possibles** :
- Login inexistant dans la base
- Mot de passe incorrect
- Compte désactivé

**Solutions** :
1. Vérifiez l'orthographe du login (sensible à la casse)
2. Vérifiez le mot de passe (majuscules/minuscules)
3. Demandez un réinitialisation de mot de passe
4. Contactez l'administrateur

---

### Problèmes d'affichage

#### 🟡 "Aucun trajet ne s'affiche"

**Causes possibles** :
- Aucun trajet assigné à l'utilisateur
- Les données ne sont pas chargées
- Problème de connexion à la base

**Solutions** :
1. Vérifiez que des trajets existent en base (SSMS)
2. Actualisez la page (F5 ou bouton actualiser)
3. Déconnectez-vous et reconnectez-vous
4. Vérifiez les permissions utilisateur

---

#### 🟡 "Les informations utilisateur s'affichent partiellement"

**Causes possibles** :
- Champs optionnels non remplis
- Problème de base de données
- Cache application

**Solutions** :
1. Complétez les champs manquants dans Paramètres
2. Actualisez l'application
3. Vérifiez l'intégrité des données en base

---

### Problèmes de performance

#### 🟡 "L'application est lente / gelée"

**Causes possibles** :
- SQL Server surchargé
- Trop de données à charger
- Problème réseau

**Solutions** :
1. Attendez quelques secondes (requête en cours)
2. Redémarrez l'application
3. Vérifiez la connexion réseau
4. Redémarrez SQL Server

---

### Erreurs lors de la création/modification

#### 🔴 "Erreur lors de la sauvegarde"

**Causes possibles** :
- Champ obligatoire vide
- Données invalides (format incorrect)
- Violation de contrainte unique (ex: login dupliqué)
- Erreur base de données

**Solutions** :
1. Vérifiez que tous les champs obligatoires sont remplis
2. Vérifiez le format des données (dates, emails, etc.)
3. Vérifiez que le login/email n'existe pas déjà
4. Relancez l'opération

---

## Foire aux questions

### ❓ Comment changer mon mot de passe ?

In the **Paramètres**, cherchez l'option **Réinitialiser le mot de passe** (si disponible). Sinon, contactez un gestionnaire.

---

### ❓ Qui peut créer de nouveaux trajets ?

Seuls les **gestionnaires** (utilisateurs avec le rôle Gestionnaire activé) peuvent créer, modifier et supprimer des trajets.

---

### ❓ Comment puis-je voir les trajets de quelqu'un d'autre ?

Vous pouvez voir les trajets :
- ✅ Vos propres trajets : dans **Mes trajets**
- ✅ Tous les trajets (si gestionnaire) : dans **Gestion des trajets**

Les utilisateurs classiques ne peuvent voir que leurs propres trajets.

---

### ❓ Qu'advient-il de mes données si je supprime mon compte ?

La suppression d'un compte supprime aussi :
- ✅ Le profil utilisateur
- ✅ Les liens vers les trajets (mais les trajets restent)
- ✅ Les données personnelles

> ⚠️ **Attention** : Cette opération est définitive !

---

### ❓ Puis-je exporter mes trajets ?

À l'heure actuelle, l'export n'est pas disponible via l'interface. Contactez un gestionnaire ou un administrateur si vous avez besoin d'exporter les données.

---

### ❓ Que faire si je me déconnecte accidentellement ?

Reconnectez-vous avec vos identifiants. Vos données ne sont pas supprimées, la déconnexion ferme simplement votre session.

---

### ❓ Peut-on modifier les lieux et transports ?

**Oui, si vous êtes gestionnaire** :
- Accédez à **Gestion des trajets**
- Les lieux et transports sont gérés via la base de données directement (SSMS)
- Ou demandez à un administrateur

---

### ❓ Comment savoir si je suis gestionnaire ?

1. Ouvrez **Paramètres**
2. Regardez si le statut indique **Gestionnaire : Oui**
3. Navigez ; si vous voyez **Gestion des trajets** et **Gestion des utilisateurs**, vous êtes gestionnaire

---

### ❓ Peut-on avoir plusieurs sessions simultanément ?

Non, selon la configuration, une seule session par utilisateur peut être active. La connexion depuis un autre appareil fermera la session précédente.

---

## Contacts et support

Pour toute question ou problème :

- 📧 **Email administrateur** : [À ajouter par l'équipe IT]
- 📞 **Téléphone support** : [À ajouter par l'équipe IT]
- 🔧 **Support technique** : Contactez votre gestionnaire d'application

---

## Historique des versions

| Version | Date | Changements |
|---------|------|-----------|
| 1.0 | 13/05/2026 | Version initiale avec fonctionnalités de base |

---

**Documentation mise à jour le : 13/05/2026**

*Cette documentation est destinée aux utilisateurs finaux. Pour les informations techniques de développement, veuillez consulter le README.md.*

