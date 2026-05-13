# ErAtlas

Application **.NET MAUI** de gestion de trajets, lieux, transports et utilisateurs.

## 1. Présentation rapide

ErAtlas est une application Windows basée sur :
- **.NET 9 / MAUI**
- **SQL Server**
- **Stored procedures** pour toutes les opérations de lecture / création / modification / suppression
- **MVVM** avec `CommunityToolkit.Mvvm`

L’application contient notamment :
- une page de connexion,
- un tableau de bord d’accueil,
- une page “Mes trajets”,
- une page de gestion des trajets,
- une page de gestion des utilisateurs,
- une page paramètres / déconnexion.

---

## 2. Prérequis

Avant d’installer le projet, il faut disposer de :

### Logiciels requis
- **Windows 10 / 11**
- **.NET SDK 9.0**
- **Visual Studio 2022** avec le workload **.NET MAUI**
  - ou **JetBrains Rider** avec support MAUI
- **SQL Server** (local ou distant)
- **SQL Server Management Studio (SSMS)** recommandé pour créer la base et les procédures

### Packages NuGet utilisés par le projet
Le projet utilise principalement :
- `CommunityToolkit.Mvvm`
- `Microsoft.Data.SqlClient`
- `Microsoft.Maui.Controls`
- `Microsoft.Extensions.Logging.Debug`

---

## 3. Télécharger le projet

### Option A — depuis un dépôt Git
Si le projet est hébergé sur Git, clonez-le :

```bash
https://github.com/yalac/application-erasmus.git
```

### Option B — depuis une archive ZIP
Si vous avez téléchargé une archive :
1. Décompressez le ZIP dans un dossier de travail.
2. Conservez la structure du dossier complète.
3. Ouvrez ensuite le fichier solution `ErAtlas.sln`.

---

## 4. Importer le projet dans l’IDE

### Avec Visual Studio 2022
1. Ouvrir Visual Studio.
2. Cliquer sur **Ouvrir un projet ou une solution**.
3. Sélectionner le fichier :
   - `ErAtlas.sln`
4. Attendre le chargement complet de la solution.
5. Restaurer les packages NuGet si Visual Studio ne le fait pas automatiquement.

### Avec Rider
1. Ouvrir Rider.
2. Choisir **Open**.
3. Sélectionner `ErAtlas.sln`.
4. Laisser Rider charger et restaurer les dépendances.

---

## 5. Restaurer les dépendances

Si nécessaire, vous pouvez restaurer les packages depuis le dossier de la solution :

```powershell
cd "C:\Users\yanis\RiderProjects\application-erasmus\ErAtlas"
dotnet restore .\ErAtlas.sln
```

---

## 6. Structure du projet

Le projet est organisé comme suit :

- `Database/DatabaseService.cs` : accès SQL Server et appels aux procédures stockées
- `Model/` : modèles de données
  - `Utilisateur`
  - `Lieu`
  - `Transport`
  - `Trajet`
- `View/` : pages MAUI
- `ViewModels/` : logique MVVM
- `Resources/` : images, styles, polices
- `Platforms/` : fichiers spécifiques à Windows, Android, iOS, etc.

---

## 7. Base de données

### Nom de la base
Le projet utilise une base SQL Server nommée :

```text
ErAtlas
```

### Chaîne de connexion actuelle
La chaîne de connexion est définie dans :

- `ErAtlas/Database/DatabaseService.cs`

Elle est actuellement de la forme :

```csharp
Server=localhost;Database=ErAtlas;User ID=sa;Password=Info76240#;TrustServerCertificate=True;
```

### Important
- Si votre mot de passe SQL Server est différent, **modifiez la chaîne de connexion** dans `DatabaseService.cs`.
- Si votre serveur SQL n’est pas sur `localhost`, remplacez également la valeur `Server=`.

---

## 8. Schéma attendu de la base de données

Le code attend les tables suivantes.

### Table `Utilisateur`
Colonnes attendues :
- `IDUtilisateur`
- `Nom`
- `Prenom`
- `Email`
- `Login`
- `MotDePasse`
- `NumeroTelephone`
- `Adresse`
- `CodePostal`
- `Ville`
- `Gestionnaire`

### Table `Lieu`
Colonnes attendues :
- `IDLieu`
- `Nom`
- `Adresse`
- `CodePostal`
- `Ville`
- `Pays`

### Table `Transport`
Colonnes attendues :
- `IDTransport`
- `TypeTransport`
- `Capacite`
- `Immatriculation`
- `Description`

### Table `Trajet`
Colonnes attendues :
- `IDTrajet`
- `DateDepart`
- `HeureDepart`
- `DateArrivee`
- `HeureArrivee`
- `Statut`
- `IDLieuArrivee`
- `IDLieuDepart`
- `IDTransport`

### Données complémentaires pour les trajets
Les procédures liées aux voyages utilisent aussi des informations comme :
- `VilleDepart`
- `VilleArrivee`
- `TypeTransport`

---

## 9. Procédures stockées à créer

Le projet fonctionne avec des **stored procedures**.  
Vous trouverez ci-dessous la liste des procédures attendues.

### 9.1 Connexion utilisateur
```sql
USE [ErAtlas]
GO

/****** Object:  StoredProcedure [dbo].[PS_VerificationConnexion]    Script Date: 13/05/2026 11:18:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[PS_VerificationConnexion]
/* Procèdure pour vérifier la connexion correspond à un utilisateur dans la base de données */
    @username VARCHAR(50)
    AS
BEGIN
		/* Renvoie une ligne pour l'identifiant */
SELECT TOP 1 IDUtilisateur, Login, MotDePasse, Gestionnaire FROM utilisateur WHERE Login = @username
END

exec PS_VerificationConnexion 'Test'

INSERT INTO utilisateur(Login, MotDePasse, Gestionnaire)
VALUES ('Test', '9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08', 0)
GO
```

### 9.2 Utilisateurs
```sql
USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_CreationUtilisateur]    Script Date: 13/05/2026 11:33:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_CreationUtilisateur]
/* Procedure pour creer un utilisateur */
	/* Parametre */
    @Nom VARCHAR(50),
    @Prenom VARCHAR(50),
    @Email VARCHAR(100),
    @Login VARCHAR(50),
    @MotDePasse VARCHAR(255),
    @NumeroTelephone INT,
    @Adresse VARCHAR(100),
	@CodePostal INT,
	@Ville VARCHAR(100),
	@Gestionnaire BIT
	AS
BEGIN
	/* Insertion dans les colonnes des valeurs */
INSERT INTO utilisateur(Nom, Prenom, Email, Login, MotDePasse, NumeroTelephone, Adresse, CodePostal, Ville, Gestionnaire)
VALUES(@Nom, @Prenom, @Email, @Login, @MotDePasse, @NumeroTelephone, @Adresse, @CodePostal, @Ville, @Gestionnaire)
END

USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_LireUtilisateur]    Script Date: 13/05/2026 11:35:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_LireUtilisateur]
/* Procedure pour lire un utilisateur */
	/* Parametre */
    @IDUtilisateur VARCHAR(50)
	AS
BEGIN
SELECT Nom, Prenom, Email, NumeroTelephone, Adresse, CodePostal, Ville, Gestionnaire
FROM utilisateur
WHERE IDUtilisateur = @IDUtilisateur
END

USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_LireUtilisateurs]    Script Date: 13/05/2026 11:36:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_LireUtilisateurs]
/* Procedure pour lire tous utilisateur */
	AS
BEGIN
SELECT IDUtilisateur ,Nom, Prenom, Email, NumeroTelephone, Adresse, CodePostal, Ville, Gestionnaire
FROM utilisateur
END

USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_SupprimerUtilisateur]    Script Date: 13/05/2026 11:36:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_SupprimerUtilisateur]
/* Procedure pour supprimer un utilisateur */
	/* Parametre */
    @IDUtilisateur INT
	AS
BEGIN
    /* Supprime un utilisateur */
DELETE FROM utilisateur
WHERE IDUtilisateur = @IDUtilisateur
END

USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_ModifierUtilisateur]    Script Date: 13/05/2026 11:38:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_ModifierUtilisateur]
/* Procedure pour modifier un utilisateur */
	/* Parametre */
	@IDUtilisateur INT,
    @Nom VARCHAR(50),
    @Prenom VARCHAR(50),
    @Email VARCHAR(100),
    @Login VARCHAR(50),
    @MotDePasse VARCHAR(255),
    @NumeroTelephone INT,
    @Adresse VARCHAR(100),
	@CodePostal INT,
	@Ville VARCHAR(100),
	@Gestionnaire BIT
	AS
BEGIN
    
	/* Mettre a jour la table utilisateur avec les nouvelles valeurs */
UPDATE utilisateur
SET
    Nom = @Nom,
    Prenom = @Prenom,
    Email = @Email,
    Login = @Login,
    MotDePasse = @MotDePasse,
    NumeroTelephone = @NumeroTelephone,
    Adresse = @Adresse,
    CodePostal = @CodePostal,
    Ville = @Ville,
    Gestionnaire = @Gestionnaire
WHERE IDUtilisateur = @IDUtilisateur
END
```

### 9.3 Lieux
```sql
USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_LireLieux]    Script Date: 13/05/2026 11:39:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_LireLieux]
AS
BEGIN
SELECT IDLieu, Nom, Adresse, CodePostal, Ville, Pays
FROM Lieu;
END


USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_CreationLieu]    Script Date: 13/05/2026 11:38:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[PS_CreationLieu]
    @Nom NVARCHAR(100),
    @Adresse NVARCHAR(200),
    @CodePostal INT,
    @Ville NVARCHAR(100),
    @Pays NVARCHAR(100)
AS
BEGIN
INSERT INTO Lieu (Nom, Adresse, CodePostal, Ville, Pays)
VALUES (@Nom, @Adresse, @CodePostal, @Ville, @Pays);

SELECT IDLieu, Nom, Adresse, CodePostal, Ville, Pays
FROM Lieu
WHERE IDLieu = SCOPE_IDENTITY();
END

USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_ModifierLieu]    Script Date: 13/05/2026 11:39:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_ModifierLieu]
    @IDLieu INT,
    @Nom NVARCHAR(100),
    @Adresse NVARCHAR(200),
    @CodePostal INT,
    @Ville NVARCHAR(100),
    @Pays NVARCHAR(100)
AS
BEGIN
UPDATE Lieu
SET Nom = @Nom,
    Adresse = @Adresse,
    CodePostal = @CodePostal,
    Ville = @Ville,
    Pays = @Pays
WHERE IDLieu = @IDLieu;

SELECT IDLieu, Nom, Adresse, CodePostal, Ville, Pays
FROM Lieu
WHERE IDLieu = @IDLieu;
END

USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_SupprimerLieu]    Script Date: 13/05/2026 11:39:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_SupprimerLieu]
    @IDLieu INT
AS
BEGIN
DELETE FROM Lieu
WHERE IDLieu = @IDLieu;
END
```

### 9.4 Transports
```sql
USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_LireTransports]    Script Date: 13/05/2026 11:40:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_LireTransports]
AS
BEGIN
SELECT IDTransport, TypeTransport, Capacite, Immatriculation, Description
FROM Transport;
END

USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_CreationTransport]    Script Date: 13/05/2026 11:40:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_CreationTransport]
    @TypeTransport NVARCHAR(100),
    @Capacite INT,
    @Immatriculation NVARCHAR(50),
    @Description NVARCHAR(255)
AS
BEGIN
INSERT INTO Transport (TypeTransport, Capacite, Immatriculation, Description)
VALUES (@TypeTransport, @Capacite, @Immatriculation, @Description);

SELECT IDTransport, TypeTransport, Capacite, Immatriculation, Description
FROM Transport
WHERE IDTransport = SCOPE_IDENTITY();
END

USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_ModifierTransport]    Script Date: 13/05/2026 11:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_ModifierTransport]
    @IDTransport INT,
    @TypeTransport NVARCHAR(100),
    @Capacite INT,
    @Immatriculation NVARCHAR(50),
    @Description NVARCHAR(255)
AS
BEGIN
UPDATE Transport
SET TypeTransport = @TypeTransport,
    Capacite = @Capacite,
    Immatriculation = @Immatriculation,
    Description = @Description
WHERE IDTransport = @IDTransport;

SELECT IDTransport, TypeTransport, Capacite, Immatriculation, Description
FROM Transport
WHERE IDTransport = @IDTransport;
END

USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_SupprimerTransport]    Script Date: 13/05/2026 11:41:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_SupprimerTransport]
    @IDTransport INT
AS
BEGIN
DELETE FROM Transport
WHERE IDTransport = @IDTransport;
END
```

### 9.5 Trajets
```sql
USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_LireTrajet]    Script Date: 13/05/2026 11:43:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_LireTrajet]
/* Procédure pour lire les trajets */
	/* Paramètres */
    @IdTrajet INT
AS

BEGIN
SELECT lieuDepart.Nom AS NomLieuDepart,
       lieuArrivee.Nom AS NomLieuArrivee,
       T.DateDepart,
       T.HeureDepart
FROM trajet AS T INNER JOIN lieu AS lieuDepart  ON T.IDLieuDepart  = lieuDepart.IDLieu
                 INNER JOIN lieu AS lieuArrivee ON T.IDLieuArrivee = lieuArrivee.IDLieu
WHERE T.IDTrajet = @IdTrajet
END

USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_LireTrajets]    Script Date: 13/05/2026 11:43:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_LireTrajets]
AS
BEGIN
SELECT IDTrajet, DateDepart, HeureDepart, DateArrivee, HeureArrivee, Statut, IDLieuArrivee, IDLieuDepart, IDTransport
FROM trajet;
END

USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_CreationTrajet]    Script Date: 13/05/2026 11:43:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_CreationTrajet]
    @DateDepart DATE,
    @HeureDepart TIME(7),
    @DateArrivee DATE,
    @HeureArrivee TIME(7),
    @Statut VARCHAR(50),
    @IDLieuArrivee INT,
    @IDLieuDepart INT,
    @IDTransport INT
AS
BEGIN
INSERT INTO Trajet (DateDepart, HeureDepart, DateArrivee, HeureArrivee, Statut, IDLieuArrivee, IDLieuDepart, IDTransport)
VALUES (@DateDepart, @HeureDepart, @DateArrivee, @HeureArrivee, @Statut, @IDLieuArrivee, @IDLieuDepart, @IDTransport);

SELECT IDTrajet, DateDepart, HeureDepart, DateArrivee, HeureArrivee, Statut, IDLieuArrivee, IDLieuDepart, IDTransport
FROM trajet
WHERE IDTrajet = SCOPE_IDENTITY();
END

USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_ModifierTrajet]    Script Date: 13/05/2026 11:44:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_ModifierTrajet]
    @IDTrajet INT,
    @DateDepart DATE,
    @HeureDepart TIME(7),
    @DateArrivee DATE,
    @HeureArrivee TIME(7),
    @Statut VARCHAR(50),
    @IDLieuArrivee INT,
    @IDLieuDepart INT,
    @IDTransport INT
AS
BEGIN
UPDATE trajet
SET DateDepart = @DateDepart,
    HeureDepart = @HeureDepart,
    DateArrivee = @DateArrivee,
    HeureArrivee = @HeureArrivee,
    Statut = @Statut,
    IDLieuArrivee = @IDLieuArrivee,
    IDLieuDepart = @IDLieuDepart,
    IDTransport = @IDTransport
WHERE IDTrajet = @IDTrajet;

SELECT IDTrajet, DateDepart, HeureDepart, DateArrivee, HeureArrivee, Statut, IDLieuArrivee, IDLieuDepart, IDTransport
FROM trajet
WHERE IDTrajet = @IDTrajet;
END

USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_SupprimerTrajet]    Script Date: 13/05/2026 11:44:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_SupprimerTrajet]
    @IDTrajet INT
AS
BEGIN
DELETE FROM trajet
WHERE IDTrajet = @IDTrajet;
END

USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_LireTypeTransport]    Script Date: 13/05/2026 11:40:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_LireTypeTransport]
/* Procédure pour lire les types de transports */
	/* Paramètres */
    @IdTrajet INT
AS

BEGIN
SELECT TypeTransport
FROM transport AS TSP INNER JOIN trajet AS TJT ON TSP.IDTransport = TJT.IDTransport
WHERE IDTrajet = @IdTrajet
END

USE [ErAtlas]
GO
/****** Object:  StoredProcedure [dbo].[PS_LireVoyage]    Script Date: 13/05/2026 11:45:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[PS_LireVoyage]
/* Procédure pour lire les voyages */
	/* Paramètres */
    @IdUtilisateur INT
AS

BEGIN
SELECT TJT.IDTrajet, DateDepart, HeureDepart, DateArrivee, HeureArrivee, Statut, lieuDepart.Ville AS VilleDepart, lieuArrivee.Ville AS VilleArrivee, TSP.TypeTransport
FROM trajet AS TJT INNER JOIN voyager AS V ON TJT.IDTrajet = V.IDTrajet
                   INNER JOIN utilisateur AS U ON U.IDUtilisateur = V.IDUtilisateur
                   INNER JOIN transport AS TSP ON TSP.IDTransport = TJT.IDTransport
                   INNER JOIN lieu AS lieuDepart  ON TJT.IDLieuDepart  = lieuDepart.IDLieu
                   INNER JOIN lieu AS lieuArrivee ON TJT.IDLieuArrivee = lieuArrivee.IDLieu
WHERE U.IDUtilisateur = @IdUtilisateur
END
```

---

## 10. Ordre conseillé d’installation de la base de données

1. Créer la base SQL Server `ErAtlas`.
2. Créer les tables.
3. Créer les procédures stockées.
4. Ajouter au moins un utilisateur de test.
5. Vérifier que le mot de passe est bien stocké **haché en SHA-256**.
6. Tester la connexion dans l’application.

---

## 11. Exemple de préparation SQL

Voici un squelette de fichier SQL que vous pouvez utiliser :

```sql
USE ErAtlas;
GO

-- =====================================================
-- TABLES
-- =====================================================

USE [ErAtlas]
GO

/****** Object:  Table [dbo].[utilisateur]    Script Date: 13/05/2026 12:06:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[utilisateur](
    [IDUtilisateur] [int] IDENTITY(1,1) NOT NULL,
    [Nom] [varchar](50) NULL,
    [Prenom] [varchar](50) NULL,
    [Email] [varchar](100) NULL,
    [Login] [varchar](50) NULL,
    [MotDePasse] [varchar](255) NULL,
    [NumeroTelephone] [int] NULL,
    [Adresse] [varchar](100) NULL,
    [CodePostal] [int] NULL,
    [Ville] [varchar](50) NULL,
    [Gestionnaire] [bit] NULL,
    CONSTRAINT [PK_utilisateur] PRIMARY KEY CLUSTERED
(
[IDUtilisateur] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
    GO



    USE [ErAtlas]
GO

/****** Object:  Table [dbo].[lieu]    Script Date: 13/05/2026 12:04:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[lieu](
    [IDLieu] [int] IDENTITY(1,1) NOT NULL,
    [Nom] [varchar](50) NULL,
    [Adresse] [varchar](100) NULL,
    [CodePostal] [int] NULL,
    [Ville] [varchar](50) NULL,
    [Pays] [varchar](50) NULL,
    CONSTRAINT [PK_lieu] PRIMARY KEY CLUSTERED
(
[IDLieu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
    GO


    USE [ErAtlas]
    GO

/****** Object:  Table [dbo].[transport]    Script Date: 13/05/2026 12:05:56 ******/
    SET ANSI_NULLS ON
    GO

    SET QUOTED_IDENTIFIER ON
    GO

CREATE TABLE [dbo].[transport](
    [IDTransport] [int] IDENTITY(1,1) NOT NULL,
    [TypeTransport] [varchar](50) NULL,
    [Capacite] [int] NULL,
    [Immatriculation] [varchar](50) NULL,
    [Description] [varchar](255) NULL,
    CONSTRAINT [PK_transport] PRIMARY KEY CLUSTERED
(
[IDTransport] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
    GO


    USE [ErAtlas]
    GO

/****** Object:  Table [dbo].[trajet]    Script Date: 13/05/2026 12:05:35 ******/
    SET ANSI_NULLS ON
    GO

    SET QUOTED_IDENTIFIER ON
    GO

CREATE TABLE [dbo].[trajet](
    [IDTrajet] [int] IDENTITY(1,1) NOT NULL,
    [DateDepart] [date] NULL,
    [HeureDepart] [time](7) NULL,
    [DateArrivee] [date] NULL,
    [HeureArrivee] [time](7) NULL,
    [Statut] [varchar](50) NULL,
    [IDLieuArrivee] [int] NULL,
    [IDLieuDepart] [int] NULL,
    [IDTransport] [int] NULL,
    CONSTRAINT [PK_trajet] PRIMARY KEY CLUSTERED
(
[IDTrajet] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
    GO

ALTER TABLE [dbo].[trajet]  WITH CHECK ADD  CONSTRAINT [FK_trajet_lieu_arrivee] FOREIGN KEY([IDLieuArrivee])
    REFERENCES [dbo].[lieu] ([IDLieu])
    GO

ALTER TABLE [dbo].[trajet] CHECK CONSTRAINT [FK_trajet_lieu_arrivee]
    GO

ALTER TABLE [dbo].[trajet]  WITH CHECK ADD  CONSTRAINT [FK_trajet_lieu_depart] FOREIGN KEY([IDLieuDepart])
    REFERENCES [dbo].[lieu] ([IDLieu])
    GO

ALTER TABLE [dbo].[trajet] CHECK CONSTRAINT [FK_trajet_lieu_depart]
    GO

ALTER TABLE [dbo].[trajet]  WITH CHECK ADD  CONSTRAINT [FK_trajet_transport] FOREIGN KEY([IDTransport])
    REFERENCES [dbo].[transport] ([IDTransport])
    GO

ALTER TABLE [dbo].[trajet] CHECK CONSTRAINT [FK_trajet_transport]
    GO




-- =====================================================
-- PROCEDURES STOCKÉES
-- =====================================================

-- Créer toutes les procèdures stockées (21)
```

---

## 12. Exécution du projet

### Depuis Visual Studio / Rider
1. Ouvrir la solution `ErAtlas.sln`.
2. Vérifier que le projet de démarrage est `ErAtlas`.
3. Sélectionner la configuration **Debug**.
4. Lancer l’application sur **Windows Machine**.

### En ligne de commande
Depuis le dossier racine de la solution :

```powershell
cd "C:\Users\yanis\RiderProjects\application-erasmus\ErAtlas"
dotnet build .\ErAtlas.sln -c Debug
dotnet run --project .\ErAtlas\ErAtlas.csproj
```

---

## 13. Connexion et utilisation

### À l’ouverture
- L’application démarre sur l’écran de connexion.
- Une fois connecté, le menu principal apparaît.

### Rôles
- **Utilisateur classique** : accès à ses trajets et à ses paramètres.
- **Gestionnaire** : accès aux pages de gestion des trajets et des utilisateurs.

### Déconnexion
Quand l’utilisateur se déconnecte, l’application revient à l’écran de connexion.

---

## 14. Dépannage

### L’application se ferme au lancement
Vérifiez :
- que la base `ErAtlas` existe,
- que SQL Server est démarré,
- que la chaîne de connexion est correcte,
- que les procédures stockées existent bien,
- que les tables ont les bons noms de colonnes.

### Erreur de connexion SQL
Vérifiez :
- le serveur dans `Server=...`
- le mot de passe SQL Server
- le port éventuel si vous n’utilisez pas l’instance locale standard

### Aucun trajet / aucune donnée affichée
Vérifiez :
- les données présentes dans les tables,
- les jointures utilisées par les procédures stockées,
- les identifiants `IDLieu`, `IDTransport`, `IDUtilisateur`.

---

## 15. Notes importantes

- Le mot de passe utilisateur est comparé après hachage SHA-256 dans l’application.
- Les images transport utilisées dans l’interface sont stockées dans `Resources/AppIcon`.
- Les styles partagés de l’application sont dans `Resources/Styles`.

---

## 16. Améliorations possibles

- Externaliser la chaîne de connexion dans un fichier de configuration.
- Ajouter un jeu de données de test SQL.
- Ajouter un script SQL complet de création de la base.
- Ajouter une page ou un journal d’erreur pour le diagnostic de démarrage.

