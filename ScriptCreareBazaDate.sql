-- Script pentru crearea bazei de date si a tabelelor in SQL Server (SSMS)
-- Rulați acest script în SQL Server Management Studio

CREATE DATABASE PlanAlimentarDB;
GO

USE PlanAlimentarDB;
GO

-- Tabel pentru utilizatori și setările lor
CREATE TABLE Utilizatori (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nume NVARCHAR(100),
    GreutateActuala DECIMAL(5,2) NOT NULL, -- kg
    Inaltime DECIMAL(5,2) NOT NULL, -- cm
    Varsta INT NOT NULL,
    Sex CHAR(1) NOT NULL CHECK (Sex IN ('M', 'F')),
    GreutateTinta DECIMAL(5,2) NOT NULL, -- kg
    DataInregistrare DATETIME DEFAULT GETDATE()
);
GO

-- Tabel pentru alimente (date importate de la provideri)
CREATE TABLE Alimente (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nume NVARCHAR(200) NOT NULL,
    CaloriiPer100g DECIMAL(6,2) NOT NULL, -- kcal
    ProteinePer100g DECIMAL(6,2) NOT NULL, -- g
    CarbohidratiPer100g DECIMAL(6,2) NOT NULL, -- g
    GrasimiPer100g DECIMAL(6,2) NOT NULL, -- g
    Provider NVARCHAR(100) NOT NULL, -- Sursa datelor
    DataImport DATETIME DEFAULT GETDATE(),
    ImagineHash NVARCHAR(64), -- Hash pentru detectarea duplicatelor
    CaleImagine NVARCHAR(500) -- Calea către imagine (stocare pe server)
);
GO

-- Tabel pentru planuri alimentare generate
CREATE TABLE PlanuriAlimentare (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UtilizatorId INT FOREIGN KEY REFERENCES Utilizatori(Id),
    ZiuaSaptamanii INT NOT NULL CHECK (ZiuaSaptamanii BETWEEN 1 AND 7), -- 1=Luni, 7=Duminica
    MicDejun NVARCHAR(500),
    Pranz NVARCHAR(500),
    Cina NVARCHAR(500),
    Gustare NVARCHAR(500),
    TotalCalorii DECIMAL(8,2),
    DataGenerare DATETIME DEFAULT GETDATE()
);
GO

-- Tabel pentru anunțuri/alerte (ex: oferte speciale alimente)
CREATE TABLE Anunturi (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Titlu NVARCHAR(200) NOT NULL,
    Descriere NVARCHAR(MAX),
    ImagineHash NVARCHAR(64),
    CaleImagine NVARCHAR(500),
    DataPublicare DATETIME DEFAULT GETDATE(),
    Activ BIT DEFAULT 1
);
GO

-- Tabel pentru log-ul de notificări și emailuri
CREATE TABLE LogNotificari (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UtilizatorId INT FOREIGN KEY REFERENCES Utilizatori(Id),
    Tip NVARCHAR(50) NOT NULL, -- 'Email', 'Notificare'
    Subiect NVARCHAR(200),
    Continut NVARCHAR(MAX),
    DataTrimitere DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(20) DEFAULT 'Pending' -- Pending, Sent, Failed
);
GO

-- Index pentru căutări rapide după hash-ul imaginii
CREATE INDEX IX_Alimente_ImagineHash ON Alimente(ImagineHash);
CREATE INDEX IX_Anunturi_ImagineHash ON Anunturi(ImagineHash);
GO

-- Procedură stocată pentru adăugarea unui utilizator
CREATE PROCEDURE AdaugaUtilizator
    @Nume NVARCHAR(100),
    @GreutateActuala DECIMAL(5,2),
    @Inaltime DECIMAL(5,2),
    @Varsta INT,
    @Sex CHAR(1),
    @GreutateTinta DECIMAL(5,2)
AS
BEGIN
    INSERT INTO Utilizatori (Nume, GreutateActuala, Inaltime, Varsta, Sex, GreutateTinta)
    VALUES (@Nume, @GreutateActuala, @Inaltime, @Varsta, @Sex, @GreutateTinta);
    
    SELECT SCOPE_IDENTITY() AS IdUtilizator;
END
GO

-- Procedură stocată pentru obținerea alimentelor pentru un plan
CREATE PROCEDURE GetAlimentePentruPlan
    @CaloriiNecesare DECIMAL(8,2)
AS
BEGIN
    -- Selectează alimente variate, ordonate aleatoriu pentru diversitate
    SELECT TOP 20 * FROM Alimente
    ORDER BY NEWID();
END
GO

PRINT 'Baza de date PlanAlimentarDB a fost creată cu succes!';
GO
