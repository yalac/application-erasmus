# ErAtlas — application de suivi des mobilités Erasmus

Application de bureau **Windows** (C# / .NET MAUI) connectée à **SQL Server** : gestion des trajets, des lieux, des transports et des utilisateurs.

Projet réalisé dans le cadre du **BTS SIO (option SLAM)**.

## Documentation

| Fichier | Contenu |
| --- | --- |
| [`ErAtlas/README.md`](ErAtlas/README.md) | documentation technique complète |
| [`ErAtlas/GUIDE_DEMARRAGE_RAPIDE.md`](ErAtlas/GUIDE_DEMARRAGE_RAPIDE.md) | démarrage en quelques étapes |
| [`ErAtlas/GUIDE_GESTIONNAIRE.md`](ErAtlas/GUIDE_GESTIONNAIRE.md) | utilisation pour un gestionnaire |
| [`ErAtlas/DOCUMENTATION_UTILISATEUR.md`](ErAtlas/DOCUMENTATION_UTILISATEUR.md) | documentation utilisateur |
| `ErAtlas/INDEX_DOCUMENTATION.md` | index des documents |

## Démarrage rapide

### 1. Base de données

Le script `Document/sql/ErAtlas_baseDeDonnee.sql` crée la base `ErAtlas` : 6 tables, 25 procédures stockées, un compte de démonstration et deux trajets. Il ne fixe aucun chemin de fichier et peut être relancé sans erreur.

1. Ouvrir **SSMS 22** et se connecter à `localhost\SQLEXPRESS` (authentification SQL Server, compte `sa`).
2. `Fichier` → `Ouvrir` → `Fichier…` → choisir le script → `F5`.
3. Vérifier en fin d'exécution : **6 tables, 25 procédures, 1 utilisateur, 2 trajets**.

### 2. Chaîne de connexion

Dans `ErAtlas/ErAtlas/Database/DatabaseService.cs`, la constante `ConnectionString` doit correspondre à ton installation :

```csharp
private const string ConnectionString = "Server=localhost\\SQLEXPRESS;Database=ErAtlas;User ID=sa;Password=<MOT_DE_PASSE_SA>;TrustServerCertificate=True;";
```

- `localhost\\SQLEXPRESS` pour une instance nommée (installation Express standard) ; `localhost` seul si tu as choisi l'instance par défaut.
- `<MOT_DE_PASSE_SA>` : le mot de passe du compte `sa` défini à l'installation de SQL Server.

### 3. Lancer

Ouvrir `ErAtlas/ErAtlas.sln` dans Visual Studio, choisir la cible **Windows Machine**, puis `F5`.

## Compte de démonstration

| Identifiant | Mot de passe |
| --- | --- |
| `Test` | `test` |

Le mot de passe est stocké en SHA-256 dans la base, jamais en clair. Identifiants **fictifs**, réservés aux tests locaux.

## Prérequis

Windows 10 (1809) ou 11, SQL Server 2025 Express (ou Developer), SSMS 22, Visual Studio 2026 avec la charge **.NET MAUI** et le SDK .NET 11 (ou .NET 10 LTS, en alignant la cible du projet).
