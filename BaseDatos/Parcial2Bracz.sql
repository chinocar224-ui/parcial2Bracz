/* =====================================================================
   Parcial 2 - Canal de televisión
   Base de datos : Parcial2Bracz
   Usuario       : usrparcial2 / 12345678
   Ejecutar en SSMS conectado a (localdb)\MSSQLLocalDB (o su instancia SQL Server)
   ===================================================================== */
USE master;
GO

IF DB_ID('Parcial2Bracz') IS NULL
    CREATE DATABASE Parcial2Bracz;
GO

USE Parcial2Bracz;
GO

/* ---------------------------- TABLAS ---------------------------- */
IF OBJECT_ID('dbo.HorarioPrograma', 'U') IS NOT NULL DROP TABLE dbo.HorarioPrograma;
IF OBJECT_ID('dbo.Canal', 'U') IS NOT NULL DROP TABLE dbo.Canal;
GO

CREATE TABLE Canal (
    id         INT IDENTITY(1, 1) PRIMARY KEY,
    nombre     VARCHAR(50)  NOT NULL,
    frecuencia VARCHAR(20)  NOT NULL,
    estado     SMALLINT     NOT NULL DEFAULT 1   -- 1: Activo, 0: Inactivo, -1: Eliminado
);
GO

CREATE TABLE HorarioPrograma (
    id                 INT IDENTITY(1, 1) PRIMARY KEY,
    idCanal            INT          NOT NULL,
    titulo             VARCHAR(100) NOT NULL,
    horaInicio         TIME         NOT NULL,      -- Ej: 09:00
    horaFin            TIME         NOT NULL,      -- Ej: 10:15
    diaSemana          VARCHAR(20)  NOT NULL,
    conductorPrincipal VARCHAR(100) NOT NULL,
    estado             SMALLINT     NOT NULL DEFAULT 1,   -- 1: Activo, 0: Inactivo, -1: Eliminado
    CONSTRAINT fk_HorarioPrograma_Canal FOREIGN KEY (idCanal) REFERENCES Canal (id)
);
GO

/* ------------------------ DATOS DE EJEMPLO ----------------------- */
INSERT INTO Canal (nombre, frecuencia, estado) VALUES
    ('Bolivisión', '4 VHF',   1),
    ('Unitel',     '9 VHF',   1),
    ('Red Uno',    '5 VHF',   1);
GO

INSERT INTO HorarioPrograma (idCanal, titulo, horaInicio, horaFin, diaSemana, conductorPrincipal, estado) VALUES
    (1, 'Buenos Días Bolivia',  '07:00', '09:00', 'Lunes',     'Carlos Mendoza', 1),
    (1, 'Noticiero Central',    '20:00', '21:00', 'Lunes',     'Ana Rojas',      1),
    (2, 'Mañanas de Unitel',    '06:30', '08:30', 'Martes',    'Luis Salvatierra', 1),
    (2, 'Cine en Casa',         '21:00', '23:15', 'Viernes',   'Marcela Flores', 1),
    (3, 'Deportes al Día',      '13:00', '14:00', 'Miércoles', 'Diego Vargas',   1);
GO

/* ------------------- USUARIO PARA LA APLICACIÓN ------------------ */
USE master;
GO
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'usrparcial2')
    CREATE LOGIN usrparcial2 WITH PASSWORD = '12345678', CHECK_POLICY = OFF;
GO

USE Parcial2Bracz;
GO
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'usrparcial2')
    CREATE USER usrparcial2 FOR LOGIN usrparcial2;
GO
ALTER ROLE db_datareader ADD MEMBER usrparcial2;
ALTER ROLE db_datawriter ADD MEMBER usrparcial2;
GO

/* Prueba rápida */
SELECT * FROM Canal;
SELECT * FROM HorarioPrograma;
GO
