/* =====================================================================
   ErAtlas — création de la base de données
   ---------------------------------------------------------------------
   Script d'origine : export SSMS du 16/11/2026 (6 tables, 25 procédures,
   1 compte de démonstration). Réécrit ici pour être importable sur
   n'importe quel poste, avec une seule modification de fond :

   1. CREATE DATABASE ne fixe plus de chemins de fichiers. L'original
      imposait C:\SQL2022\MSSQL16.MSSQLSERVER\MSSQL\DATA\ErAtlas.mdf,
      qui n'existe que sur le poste où la base a été créée : sur tout
      autre PC, l'import échoue dès la première instruction.
   2. Base, tables, clés étrangères et données sont protégées par des
      tests d'existence : le script peut être relancé sans erreur.
   3. Les deux clés étrangères de la table voyager sont en ON DELETE
      CASCADE, sinon PS_SupprimerUtilisateur et PS_SupprimerTrajet
      échouent (erreur 547) dès qu'un utilisateur ou un trajet est lié.
   4. Un bloc de données de démonstration est ajouté à la fin.

   Exécution : SSMS, connecté en "sa", une seule fois, script complet (F5).
   ===================================================================== */

/* Pour repartir de zéro, décommenter ces 6 lignes puis exécuter :
IF DB_ID(N'ErAtlas') IS NOT NULL
BEGIN
    ALTER DATABASE [ErAtlas] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [ErAtlas];
END
GO
*/

USE [master];
GO

IF DB_ID(N'ErAtlas') IS NULL
    CREATE DATABASE [ErAtlas];
GO

ALTER DATABASE [ErAtlas] SET COMPATIBILITY_LEVEL = 160;
GO

USE [ErAtlas];
GO

/* ============================== TABLES ============================== */

IF OBJECT_ID(N'dbo.lieu', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.lieu
    (
        IDLieu      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_lieu PRIMARY KEY,
        Nom         VARCHAR(50)  NULL,
        Adresse     VARCHAR(100) NULL,
        CodePostal  INT          NULL,
        Ville       VARCHAR(50)  NULL,
        Pays        VARCHAR(50)  NULL
    );
END
GO

IF OBJECT_ID(N'dbo.Pays', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Pays
    (
        IDPays      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_pays PRIMARY KEY,
        NomPays     VARCHAR(50) NULL,
        Continent   VARCHAR(50) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.trajet', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.trajet
    (
        IDTrajet      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_trajet PRIMARY KEY,
        DateDepart    DATE      NULL,
        HeureDepart   TIME(7)   NULL,
        DateArrivee   DATE      NULL,
        HeureArrivee  TIME(7)   NULL,
        Statut        VARCHAR(50) NULL,
        IDLieuArrivee INT       NULL,
        IDLieuDepart  INT       NULL,
        IDTransport   INT       NULL
    );
END
GO

IF OBJECT_ID(N'dbo.transport', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.transport
    (
        IDTransport     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_transport PRIMARY KEY,
        TypeTransport   VARCHAR(50)  NULL,
        Capacite        INT          NULL,
        Immatriculation VARCHAR(50)  NULL,
        Description     VARCHAR(255) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.utilisateur', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.utilisateur
    (
        IDUtilisateur   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_utilisateur PRIMARY KEY,
        Nom             VARCHAR(50)  NULL,
        Prenom          VARCHAR(50)  NULL,
        Email           VARCHAR(100) NULL,
        Login           VARCHAR(50)  NULL,
        MotDePasse      VARCHAR(255) NULL,
        NumeroTelephone INT          NULL,
        Adresse         VARCHAR(100) NULL,
        CodePostal      INT          NULL,
        Ville           VARCHAR(50)  NULL,
        Gestionnaire    BIT          NULL
    );
END
GO

IF OBJECT_ID(N'dbo.voyager', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.voyager
    (
        IDTrajet      INT NOT NULL,
        IDUtilisateur INT NOT NULL,
        CONSTRAINT PK_voyager PRIMARY KEY (IDTrajet, IDUtilisateur)
    );
END
GO

/* ========================= CLÉS ÉTRANGÈRES ========================== */

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_trajet_lieu_arrivee')
    ALTER TABLE dbo.trajet WITH CHECK ADD CONSTRAINT FK_trajet_lieu_arrivee
        FOREIGN KEY (IDLieuArrivee) REFERENCES dbo.lieu (IDLieu);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_trajet_lieu_depart')
    ALTER TABLE dbo.trajet WITH CHECK ADD CONSTRAINT FK_trajet_lieu_depart
        FOREIGN KEY (IDLieuDepart) REFERENCES dbo.lieu (IDLieu);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_trajet_transport')
    ALTER TABLE dbo.trajet WITH CHECK ADD CONSTRAINT FK_trajet_transport
        FOREIGN KEY (IDTransport) REFERENCES dbo.transport (IDTransport);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_voyager_trajet')
    ALTER TABLE dbo.voyager WITH CHECK ADD CONSTRAINT FK_voyager_trajet
        FOREIGN KEY (IDTrajet) REFERENCES dbo.trajet (IDTrajet) ON DELETE CASCADE;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_voyager_utilisateur')
    ALTER TABLE dbo.voyager WITH CHECK ADD CONSTRAINT FK_voyager_utilisateur
        FOREIGN KEY (IDUtilisateur) REFERENCES dbo.utilisateur (IDUtilisateur) ON DELETE CASCADE;
GO

/* ========================== LIEUX ET PAYS =========================== */

CREATE OR ALTER PROCEDURE dbo.PS_CreationLieu
    @Nom        NVARCHAR(100),
    @Adresse    NVARCHAR(200),
    @CodePostal INT,
    @Ville      NVARCHAR(100),
    @Pays       NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.lieu (Nom, Adresse, CodePostal, Ville, Pays)
    VALUES (@Nom, @Adresse, @CodePostal, @Ville, @Pays);

    SELECT IDLieu, Nom, Adresse, CodePostal, Ville, Pays
    FROM dbo.lieu
    WHERE IDLieu = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_ModifierLieu
    @IDLieu     INT,
    @Nom        NVARCHAR(100),
    @Adresse    NVARCHAR(200),
    @CodePostal INT,
    @Ville      NVARCHAR(100),
    @Pays       NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.lieu
    SET Nom = @Nom,
        Adresse = @Adresse,
        CodePostal = @CodePostal,
        Ville = @Ville,
        Pays = @Pays
    WHERE IDLieu = @IDLieu;

    SELECT IDLieu, Nom, Adresse, CodePostal, Ville, Pays
    FROM dbo.lieu
    WHERE IDLieu = @IDLieu;
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_SupprimerLieu
    @IDLieu INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.lieu WHERE IDLieu = @IDLieu;
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_LireLieux
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IDLieu, Nom, Adresse, CodePostal, Ville, Pays
    FROM dbo.lieu
    ORDER BY Nom;
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_CreationPays
    @NomPays   VARCHAR(50),
    @Continent VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Pays (NomPays, Continent)
    VALUES (@NomPays, @Continent);
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_ModifierPays
    @IDPays    INT,
    @NomPays   VARCHAR(50),
    @Continent VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Pays
    SET NomPays = @NomPays,
        Continent = @Continent
    WHERE IDPays = @IDPays;
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_SupprimerPays
    @IDPays INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Pays WHERE IDPays = @IDPays;
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_LirePays
    @NomPays VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT NomPays, Continent
    FROM dbo.Pays;
END
GO

/* ============================ TRANSPORTS ============================ */

CREATE OR ALTER PROCEDURE dbo.PS_CreationTransport
    @TypeTransport   NVARCHAR(100),
    @Capacite        INT,
    @Immatriculation NVARCHAR(50),
    @Description     NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.transport (TypeTransport, Capacite, Immatriculation, Description)
    VALUES (@TypeTransport, @Capacite, @Immatriculation, @Description);

    SELECT IDTransport, TypeTransport, Capacite, Immatriculation, Description
    FROM dbo.transport
    WHERE IDTransport = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_ModifierTransport
    @IDTransport     INT,
    @TypeTransport   NVARCHAR(100),
    @Capacite        INT,
    @Immatriculation NVARCHAR(50),
    @Description     NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.transport
    SET TypeTransport = @TypeTransport,
        Capacite = @Capacite,
        Immatriculation = @Immatriculation,
        Description = @Description
    WHERE IDTransport = @IDTransport;

    SELECT IDTransport, TypeTransport, Capacite, Immatriculation, Description
    FROM dbo.transport
    WHERE IDTransport = @IDTransport;
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_SupprimerTransport
    @IDTransport INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.transport WHERE IDTransport = @IDTransport;
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_LireTransports
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IDTransport, TypeTransport, Capacite, Immatriculation, Description
    FROM dbo.transport
    ORDER BY TypeTransport;
END
GO

/* ============================== TRAJETS ============================= */

CREATE OR ALTER PROCEDURE dbo.PS_CreationTrajet
    @DateDepart    DATE,
    @HeureDepart   TIME(7),
    @DateArrivee   DATE,
    @HeureArrivee  TIME(7),
    @Statut        VARCHAR(50),
    @IDLieuArrivee INT,
    @IDLieuDepart  INT,
    @IDTransport   INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.trajet
        (DateDepart, HeureDepart, DateArrivee, HeureArrivee, Statut,
         IDLieuArrivee, IDLieuDepart, IDTransport)
    VALUES
        (@DateDepart, @HeureDepart, @DateArrivee, @HeureArrivee, @Statut,
         @IDLieuArrivee, @IDLieuDepart, @IDTransport);

    SELECT IDTrajet, DateDepart, HeureDepart, DateArrivee, HeureArrivee, Statut,
           IDLieuArrivee, IDLieuDepart, IDTransport
    FROM dbo.trajet
    WHERE IDTrajet = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_ModifierTrajet
    @IDTrajet      INT,
    @DateDepart    DATE,
    @HeureDepart   TIME(7),
    @DateArrivee   DATE,
    @HeureArrivee  TIME(7),
    @Statut        VARCHAR(50),
    @IDLieuArrivee INT,
    @IDLieuDepart  INT,
    @IDTransport   INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.trajet
    SET DateDepart = @DateDepart,
        HeureDepart = @HeureDepart,
        DateArrivee = @DateArrivee,
        HeureArrivee = @HeureArrivee,
        Statut = @Statut,
        IDLieuArrivee = @IDLieuArrivee,
        IDLieuDepart = @IDLieuDepart,
        IDTransport = @IDTransport
    WHERE IDTrajet = @IDTrajet;

    SELECT IDTrajet, DateDepart, HeureDepart, DateArrivee, HeureArrivee, Statut,
           IDLieuArrivee, IDLieuDepart, IDTransport
    FROM dbo.trajet
    WHERE IDTrajet = @IDTrajet;
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_SupprimerTrajet
    @IDTrajet INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.trajet WHERE IDTrajet = @IDTrajet;
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_LireTrajets
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IDTrajet, DateDepart, HeureDepart, DateArrivee, HeureArrivee, Statut,
           IDLieuArrivee, IDLieuDepart, IDTransport
    FROM dbo.trajet
    ORDER BY DateDepart;
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_LireTrajet
    @IdTrajet INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT lieuDepart.Nom  AS NomLieuDepart,
           lieuArrivee.Nom AS NomLieuArrivee,
           T.DateDepart,
           T.HeureDepart
    FROM dbo.trajet AS T
    INNER JOIN dbo.lieu AS lieuDepart  ON T.IDLieuDepart  = lieuDepart.IDLieu
    INNER JOIN dbo.lieu AS lieuArrivee ON T.IDLieuArrivee = lieuArrivee.IDLieu
    WHERE T.IDTrajet = @IdTrajet
    ORDER BY NomLieuDepart;
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_LireTypeTransport
    @IdTrajet INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TSP.TypeTransport
    FROM dbo.transport AS TSP
    INNER JOIN dbo.trajet AS TJT ON TSP.IDTransport = TJT.IDTransport
    WHERE TJT.IDTrajet = @IdTrajet;
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_LireVoyage
    @IdUtilisateur INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TJT.IDTrajet,
           TJT.DateDepart,
           TJT.HeureDepart,
           TJT.DateArrivee,
           TJT.HeureArrivee,
           TJT.Statut,
           lieuDepart.Ville  AS VilleDepart,
           lieuArrivee.Ville AS VilleArrivee,
           TSP.TypeTransport
    FROM dbo.trajet AS TJT
    INNER JOIN dbo.voyager   AS V  ON TJT.IDTrajet = V.IDTrajet
    INNER JOIN dbo.utilisateur AS U ON U.IDUtilisateur = V.IDUtilisateur
    INNER JOIN dbo.transport AS TSP ON TSP.IDTransport = TJT.IDTransport
    INNER JOIN dbo.lieu AS lieuDepart  ON TJT.IDLieuDepart  = lieuDepart.IDLieu
    INNER JOIN dbo.lieu AS lieuArrivee ON TJT.IDLieuArrivee = lieuArrivee.IDLieu
    WHERE U.IDUtilisateur = @IdUtilisateur
    ORDER BY TJT.DateDepart, TJT.HeureDepart;
END
GO

/* ============================ UTILISATEURS ========================== */

CREATE OR ALTER PROCEDURE dbo.PS_CreationUtilisateur
    @Nom             VARCHAR(50),
    @Prenom          VARCHAR(50),
    @Email           VARCHAR(100),
    @Login           VARCHAR(50),
    @MotDePasse      VARCHAR(255),
    @NumeroTelephone INT,
    @Adresse         VARCHAR(100),
    @CodePostal      INT,
    @Ville           VARCHAR(100),
    @Gestionnaire    BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.utilisateur
        (Nom, Prenom, Email, Login, MotDePasse, NumeroTelephone,
         Adresse, CodePostal, Ville, Gestionnaire)
    VALUES
        (@Nom, @Prenom, @Email, @Login, @MotDePasse, @NumeroTelephone,
         @Adresse, @CodePostal, @Ville, @Gestionnaire);
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_ModifierUtilisateur
    @IDUtilisateur   INT,
    @Nom             VARCHAR(50),
    @Prenom          VARCHAR(50),
    @Email           VARCHAR(100),
    @Login           VARCHAR(50),
    @MotDePasse      VARCHAR(255),
    @NumeroTelephone INT,
    @Adresse         VARCHAR(100),
    @CodePostal      INT,
    @Ville           VARCHAR(100),
    @Gestionnaire    BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.utilisateur
    SET Nom = @Nom,
        Prenom = @Prenom,
        Email = @Email,
        Login = @Login,
        MotDePasse = @MotDePasse,
        NumeroTelephone = @NumeroTelephone,
        Adresse = @Adresse,
        CodePostal = @CodePostal,
        Ville = @Ville,
        Gestionnaire = @Gestionnaire
    WHERE IDUtilisateur = @IDUtilisateur;
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_SupprimerUtilisateur
    @IDUtilisateur INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.utilisateur WHERE IDUtilisateur = @IDUtilisateur;
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_LireUtilisateurs
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IDUtilisateur, Nom, Prenom, Email, NumeroTelephone,
           Adresse, CodePostal, Ville, Gestionnaire
    FROM dbo.utilisateur;
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_LireUtilisateur
    @IDUtilisateur VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Nom, Prenom, Email, NumeroTelephone, Adresse, CodePostal, Ville, Gestionnaire
    FROM dbo.utilisateur
    WHERE IDUtilisateur = @IDUtilisateur;
END
GO

CREATE OR ALTER PROCEDURE dbo.PS_VerificationConnexion
    @username VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 IDUtilisateur, Login, MotDePasse, Gestionnaire
    FROM dbo.utilisateur
    WHERE Login = @username;
END
GO

/* ====================== DONNÉES DE DÉMONSTRATION ==================== */

-- Compte de démonstration : Login "Test", mot de passe "test".
-- SHA-256("test") = 9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08
IF NOT EXISTS (SELECT 1 FROM dbo.utilisateur WHERE Login = N'Test')
BEGIN
    INSERT INTO dbo.utilisateur (Login, MotDePasse, Gestionnaire)
    VALUES ('Test', '9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08', 0);
END
GO

-- Données de trajets (facultatif) : sans elles, l'onglet "Mes Trajets" est vide.
IF NOT EXISTS (SELECT 1 FROM dbo.lieu)
BEGIN
    INSERT INTO dbo.lieu (Nom, Adresse, CodePostal, Ville, Pays) VALUES
        ('Paris Gare de Lyon', 'Place Louis-Armand', 75012, 'Paris', 'France'),
        ('Lyon Part-Dieu', '5 Place Charles-Beraudier', 69003, 'Lyon', 'France'),
        ('Barcelone Sants', 'Placa dels Paisos Catalans', 8014, 'Barcelone', 'Espagne');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.transport)
BEGIN
    INSERT INTO dbo.transport (TypeTransport, Capacite, Immatriculation, Description) VALUES
        ('Train', 300, 'TR-456-EF', 'TER regional'),
        ('Avion', 180, 'AV-789-GH', 'Vol court-courrier'),
        ('Bus', 50, 'AB-123-CD', 'Autocar de ligne');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Pays)
BEGIN
    INSERT INTO dbo.Pays (NomPays, Continent) VALUES
        ('France', 'Europe'),
        ('Espagne', 'Europe');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.trajet)
BEGIN
    INSERT INTO dbo.trajet
        (DateDepart, HeureDepart, DateArrivee, HeureArrivee, Statut,
         IDLieuDepart, IDLieuArrivee, IDTransport)
    SELECT '2026-12-01', '08:15', '2026-12-01', '12:40', 'Valide',
           (SELECT TOP 1 IDLieu FROM dbo.lieu WHERE Ville = 'Paris'),
           (SELECT TOP 1 IDLieu FROM dbo.lieu WHERE Ville = 'Lyon'),
           (SELECT TOP 1 IDTransport FROM dbo.transport WHERE TypeTransport = 'Train')
    UNION ALL
    SELECT '2026-12-15', '06:30', '2026-12-15', '09:05', 'En attente',
           (SELECT TOP 1 IDLieu FROM dbo.lieu WHERE Ville = 'Lyon'),
           (SELECT TOP 1 IDLieu FROM dbo.lieu WHERE Ville = 'Barcelone'),
           (SELECT TOP 1 IDTransport FROM dbo.transport WHERE TypeTransport = 'Avion');
END
GO

-- Rattachement des trajets au compte de démonstration
INSERT INTO dbo.voyager (IDTrajet, IDUtilisateur)
SELECT T.IDTrajet, U.IDUtilisateur
FROM dbo.trajet AS T
CROSS JOIN dbo.utilisateur AS U
WHERE U.Login = N'Test'
  AND NOT EXISTS (
      SELECT 1 FROM dbo.voyager AS V
      WHERE V.IDTrajet = T.IDTrajet AND V.IDUtilisateur = U.IDUtilisateur
  );
GO

/* ============================== CONTRÔLE ============================ */

SELECT 'Tables' AS Controle, COUNT(*) AS Nombre FROM sys.tables
UNION ALL
SELECT 'Procedures', COUNT(*) FROM sys.procedures
UNION ALL
SELECT 'Utilisateurs', COUNT(*) FROM dbo.utilisateur
UNION ALL
SELECT 'Trajets', COUNT(*) FROM dbo.trajet;
GO

-- Doit renvoyer une ligne (Login "Test")
EXEC dbo.PS_VerificationConnexion @username = 'Test';
GO
