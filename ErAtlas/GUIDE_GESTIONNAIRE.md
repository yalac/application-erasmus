# 👑 Guide du gestionnaire - ErAtlas

*Ce guide est destiné aux utilisateurs avec le rôle "Gestionnaire"*

---

## 📋 Table des matières

- [Responsabilités du gestionnaire](#responsabilités-du-gestionnaire)
- [Gestion des trajets](#gestion-des-trajets)
- [Gestion des utilisateurs](#gestion-des-utilisateurs)
- [Meilleures pratiques](#meilleures-pratiques)
- [Scénarios courants](#scénarios-courants)
- [Checklist d'audit](#checklist-daudit)

---

## Responsabilités du gestionnaire

En tant que gestionnaire, vous avez accès à l'ensemble des fonctionnalités de l'application :

✅ **Pouvoirs de gestion** :
- Créer, modifier et supprimer les trajets
- Créer, modifier et supprimer les utilisateurs
- Gérer les lieux et transports (via base de données directement)
- Accéder à toutes les données de l'organisation
- Superviser les trajets assignés aux utilisateurs

⚠️ **Responsabilités** :
- Maintenir l'intégrité des données
- Respecter la confidentialité des informations
- Gérer l'accès gestionnaire de manière responsable
- Documenter les changements importants
- Supporter les utilisateurs en cas de problème

---

## Gestion des trajets

### Cycle de vie complet d'un trajet

#### Phase 1 : Création
```
1. Gestion des trajets → Ajouter un trajet
2. Saisir les informations :
   - Dates de départ/arrivée
   - Heures de départ/arrivée
   - Lieux (départ et arrivée)
   - Transport utilisé
   - Statut initial
3. Valider la création
```

**Données obligatoires** :
- Date de départ (format JJ/MM/AAAA)
- Heure de départ (HH:MM)
- Date d'arrivée
- Heure d'arrivée
- Lieu de départ (doit exister)
- Lieu d'arrivée (doit exister)
- Transport (doit exister)
- Statut

**Validation** :
```
Heure arrivée > Heure départ (même date) ✅
OU
Date arrivée > Date départ ✅
```

#### Phase 2 : Modification
```
1. Sélectionner le trajet dans la liste
2. Cliquer sur "Modifier"
3. Éditer les champs nécessaires
4. Enregistrer les modifications
```

**Historique** :
- Les modifications sont actualisées immédiatement
- Assurez-vous de notifier les utilisateurs de changements importants
- (Idéalement via email ou notification système)

#### Phase 3 : Suppression
```
1. Sélectionner le trajet
2. Cliquer sur "Supprimer"
3. Confirmer la suppression
```

⚠️ **ATTENTION** :
- **Définitif** : impossible d'annuler
- **Archive d'abord** : envisagez d'archiver au lieu de supprimer
- **Notifiez les utilisateurs** : si des gens sont assignés

### Gestion des statuts de trajet

#### Statuts disponibles et transitions

```
CRÉATION
   ↓
À venir (par défaut)
   ↓
En cours (avant le départ)
   ↓
Terminé (après l'arrivée)
   
OU

À venir
   ↓
Annulé (avant le départ si problème)
```

#### Recommandations par statut

| Statut | Quand l'utiliser | Actions possibles |
|--------|-----------------|-------------------|
| **À venir** | Trajet créé, date à l'avenir | Modifier, Passer en cours, Annuler |
| **En cours** | Trajet a commencé | Modifier (horaires), Terminer |
| **Terminé** | Trajet complété | Archiver,  Conserver pour historique |
| **Annulé** | Trajet ne se fera pas | Conserver pour audit, Historique |

### Gestion des lieux

#### Créer un lieu (via Base de données)

Vous devez créer les lieux dans la base de données **avant** de les utiliser dans un trajet.

Structure minimale :
```sql
Nom : "Paris"
Adresse : "75001 Paris, France" (ou détail)
CodePostal : 75001
Ville : "Paris"
Pays : "France"
```

#### Exemples de lieux

```
Lieu 1 :
  Nom : Centre Erasmus Paris
  Adresse : 123 Rue de Rivoli
  CodePostal : 75001
  Ville : Paris
  Pays : France

Lieu 2 :
  Nom : Campus Universitaire Berlin
  Adresse : Unter den Linden 6
  CodePostal : 10117
  Ville : Berlin
  Pays : Allemagne
```

### Gestion des transports

#### Types de transports disponibles

Vous devez configurer les transports dans la base avant de les utiliser.

**Structure** :
```
TypeTransport : "Bus" | "Train" | "Bateau" | "Avion"
Capacité : nombre de places
Immatriculation : plaque d'immatriculation
Description : infos supplémentaires
```

**Exemples** :

```
Transport 1 (Bus) :
  Type : Bus
  Capacité : 45
  Immatriculation : AB-123-CD
  Description : Bus climatisé, WiFi disponible

Transport 2 (Train) :
  Type : Train
  Capacité : 200
  Immatriculation : SNCF-5847
  Description : TGV Paris-Berlin, 1ère classe
```

---

## Gestion des utilisateurs

### Création d'un nouvel utilisateur

#### Étapes

1. **Accédez à** Gestion des utilisateurs → Ajouter un utilisateur
2. **Remplissez les champs obligatoires** :

   | Champ | Règles | Exemple |
   |-------|--------|---------|
   | **Nom** | sans accents | Dupont |
   | **Prénom** | sans accents | Marie |
   | **Email** | format valide | marie.dupont@erasmus.eu |
   | **Login** | unique, pas d'espaces | mdupont2024 |
   | **Mot de passe** | 8+ caractères min | SecurePass123! |

3. **Remplissez optionnellement** :
   - Téléphone
   - Adresse
   - Code postal
   - Ville

4. **Définissez le rôle** :
   - ☐ Gestionnaire = Non (user classique)
   - ☑ Gestionnaire = Oui (accès admin)

5. **Cliquez Créer**

#### Vérification après création

```
✅ L'utilisateur apparaît dans la liste
✅ Le statut Gestionnaire est correct
✅ Vous pouvez le modifier/supprimer
```

### Modification d'un utilisateur

**Cas courants** :

#### Réinitialiser le mot de passe

1. Sélectionnez l'utilisateur
2. Cliquez **Modifier**
3. Changez le mot de passe
4. Enregistrez
5. **Notifiez l'utilisateur** du nouveau mot de passe (par email sécurisé)

#### Promouvoir en gestionnaire

1. Sélectionnez l'utilisateur
2. Cliquez **Modifier**
3. Cochez **Gestionnaire = Oui**
4. Enregistrez

#### Rétrograder un gestionnaire

1. Sélectionnez l'utilisateur
2. Cliquez **Modifier**
3. Décochez **Gestionnaire = Oui**
4. Enregistrez

⚠️ **Attention** : L'utilisateur perdra l'accès aux pages de gestion

#### Désactiver un utilisateur

Actuellement, il n'y a pas de statut "inactif". Vous pouvez :
- ✅ Supprimer complètement l'utilisateur
- ✅ Changer son login en le marquant comme inactif (ex: `mdupont_INACTIF`)

### Suppression d'un utilisateur

#### Étapes

1. Sélectionnez l'utilisateur
2. Cliquez **Supprimer**
3. **Confirmez** (opération irréversible)

#### Avant de supprimer

✅ **Vérification** :
- L'utilisateur ne doit pas avoir de trajets assignés actifs
- Informez l'utilisateur de la suppression
- Archivez ses données si nécessaire
- Notifiez les autres gestionnaires

#### Alternatives à la suppression

Au lieu de supprimer définitivement :

**Option 1 : Archivage**
- Créez un login archive : `mdupont_ARCHIVE`
- Copiez les données
- Bloquez le login original

**Option 2 : Désactivation**
- Changez le login : `INACTIF_mdupont`
- Changez le mot de passe à une valeur aléatoire
- Gardez le compte accessible pour l'audit

---

## Meilleures pratiques

### 📋 Intégrité des données

#### ✅ À faire

- Vérifier les données **avant** de les créer
- Utiliser des formats **cohérents** (ex: noms en capitales)
- **Documenter** les changements majeurs
- Faire des **sauvegardes régulières** de la base de données
- Valider les informations auprès de la source avant création

#### ❌ À ne pas faire

- Créer des doublons d'utilisateurs
- Utiliser des lieux ou transports fantaisistes
- Laisser des trajets orphelins (sans utilisateur assigné)
- Changer les dates sans communiquer
- Garder des données obsolètes

### 🔐 Sécurité

#### Mots de passe

✅ **Bonnes pratiques** :
```
Longueur : 12+ caractères
Composition : majuscules + minuscules + chiffres + symboles
Format : !@#$%^&*
Exemple sûr : ErAtlas@2024!Secure
```

❌ **À éviter** :
```
Mots de passe faibles : 123456, password, admin
Mots de passe réutilisés : même mot pour tous
Mots de passe partagés : communiqués par email
```

#### Gestion des accès

✅ **À faire** :
- Donner l'accès gestionnaire **uniquement** aux personnes de confiance
- Vérifier régulièrement **qui** a ce rôle
- Retirer l'accès **immédiatement** si le personnel quitte
- Documenter les **raisons** des accès

❌ **À ne pas faire** :
- Donner l'accès gestionnaire "juste au cas où"
- Laisser des comptes gestionnaires inactifs
- Partager les identifiants de gestion
- Ignorer les accès non utilisés

### 📊 Reporting et audit

#### Points de contrôle mensuels

```
□ Nombre total de trajets
□ Nombre total d'utilisateurs
□ Trajets obsolètes à nettoyer
□ Utilisateurs inactifs
□ Changements effectués ce mois
□ Anomalies détectées
```

#### Documentation

Maintenez un journal des changements importants :

```
Date : 15/05/2026
Changement : Création trajet Paris-Berlin
Par : Jean Martin (gestionnaire)
Détails : Trajet groupe 45 personnes, Bus

Date : 14/05/2026
Changement : Création utilisateur Sophie Bernard
Par : Marie Dupont
Détails : Nouvel utilisateur, gestionnaire=Non
```

---

## Scénarios courants

### Scénario 1 : Un trajet doit être annulé

**Étapes** :
1. Accédez à **Gestion des trajets**
2. Trouvez le trajet
3. Cliquez **Modifier**
4. Changez le statut à **"Annulé"**
5. Enregistrez
6. **Notifiez les utilisateurs** du changement

**Email suggested** :
```
Sujet : Annulation du trajet Paris-Berlin du 20/05/2026

Le trajet Paris-Berlin prévu le 20 mai est annulé en raison de [raison].
Un trajet de substitution sera programmé pour le [date].

Merci de la compréhension.
```

### Scénario 2 : Un utilisateur oublie son mot de passe

**Étapes** :
1. Allez à **Gestion des utilisateurs**
2. Trouvez l'utilisateur
3. Cliquez **Modifier**
4. Générez un nouveau mot de passe (complexe, aléatoire)
5. Enregistrez
6. **Envoyez le nouveau mot de passe** par email sécurisé
7. Demandez à l'utilisateur de le **changer** au prochain login

**Mot de passe temporaire suggested** :
```
Format : ErA[randomChars]2024!
Exemple : ErA7kQ9mP2024!
```

### Scénario 3 : Créer un trajet pour un groupe

**Étapes** :
1. Créez le trajet avec dates/heures/transport correct
2. Créez ou modifiez les trajets associés
3. Vérifiez les capacités (transport + nombre d'utilisateurs)
4. Notifiez les utilisateurs concernés
5. Confirmez les assignations

**Checklist** :
```
□ Dates et heures vérifiées
□ Transport a assez de places
□ Lieux existent en base
□ Statut = "À venir"
□ Utilisateurs notifiés
```

### Scénario 4 : Nettoyer les données obsolètes

**Mensuellement** :
1. Allez à **Gestion des trajets**
2. Identifiez les trajets avec statut "Terminé" > 6 mois
3. **Archivez** ou **supprimez** (selon politique)
4. Supprimez les trajets "Annulés" non pertinents
5. Notifiez les utilisateurs des suppressions

### Scénario 5 : Un nouvel utilisateur arrive

**Étapes** :
1. Créez un compte utilisateur
2. Envoyez login + mot de passe temporaire
3. Demandez à l'utilisateur de :
   - Se connecter
   - Aller à **Paramètres**
   - Changer le mot de passe
4. Assignez les trajets pertinents
5. Confirmez qu'il a accès

---

## Checklist d'audit

À effectuer mensuellement ou trimestriellement.

### Utilisateurs

```
□ Nombre total d'utilisateurs enregistrés : _____
□ Nombre de gestionnaires : _____
□ Utilisateurs inactifs depuis 1 mois : _____
□ Comptes à supprimer : _____
□ Logins uniques vérifiés : ✅
□ Emails valides vérifiés : ✅
```

### Trajets

```
□ Nombre total de trajets : _____
□ Trajets "À venir" (prochains 30j) : _____
□ Trajets "Terminés" (> 90j) : _____
□ Trajets "Annulés" : _____
□ Trajets sans transport assigné : _____
□ Trajets sans lieu assigné : _____
□ Trajets en doublon (suspectés) : _____
```

### Sécurité

```
□ Gestionnaires justifiés : ✅
□ Mots de passe faibles identifiés : ___
□ Accès non utilisés : ___
□ Changements non documentés : ___
□ Données sensibles sécurisées : ✅
```

### Actions

```
□ Suppression des doublons effectuée
□ Utilisateurs inactifs avertis
□ Trajets obsolètes archivés
□ Mises à jour de sécurité appliquées
□ Rapport généré et archivé
```

---

## Contacts et ressources

### Support technique
- **Documentation utilisateur** : DOCUMENTATION_UTILISATEUR.md
- **Guide de démarrage rapide** : GUIDE_DEMARRAGE_RAPIDE.md
- **README technique** : README.md

### Pour des questions spécifiques
- Administrateur système : [À définir]
- DBA (base de données) : [À définir]
- Responsable Erasmus : [À définir]

---

**Vous avez terminé cette lecture ? Vous êtes maintenant un gestionnaire certifié ErAtlas ! 🎉**

*Document mis à jour le : 13/05/2026*

