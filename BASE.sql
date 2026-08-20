USE master;
GO

-- 0. ELIMINAR BASE DE DATOS SI YA EXISTE
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'DBGrassSintetico')
BEGIN
    ALTER DATABASE DBGrassSintetico SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE DBGrassSintetico;
END;
GO

-- 1. CREAR BASE DE DATOS
CREATE DATABASE DBGrassSintetico;
GO

USE DBGrassSintetico;
GO

-- ==========================================
-- CREACIÓN DE TABLAS (CON DNI INCLUIDO)
-- ==========================================

-- Tabla Cliente
CREATE TABLE Cliente ( 
    IdCliente INT IDENTITY(1,1) PRIMARY KEY, 
    DNI VARCHAR(8) NOT NULL UNIQUE,
    Nombre VARCHAR(100) NOT NULL, 
    Apellido VARCHAR(100) NOT NULL, 
    Telefono VARCHAR(20) NOT NULL, 
    Correo VARCHAR(100) NULL 
); 
GO

-- Tabla Cancha
CREATE TABLE Cancha (
    IdCancha INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(150) NULL,
    Estado BIT DEFAULT 1
);
GO

-- Tabla Horario
CREATE TABLE Horario ( 
    IdHorario INT IDENTITY(1,1) PRIMARY KEY, 
    HoraInicio TIME NOT NULL, 
    HoraFin TIME NOT NULL 
); 
GO

-- Tabla Reserva
CREATE TABLE Reserva ( 
    IdReserva INT IDENTITY(1,1) PRIMARY KEY, 
    IdCliente INT FOREIGN KEY REFERENCES Cliente(IdCliente), 
    IdCancha INT FOREIGN KEY REFERENCES Cancha(IdCancha), 
    FechaReserva DATE NOT NULL, 
    IdHorario INT FOREIGN KEY REFERENCES Horario(IdHorario), 
    EstadoReserva VARCHAR(30) DEFAULT 'Confirmada' 
); 
GO

-- Tabla Pago
CREATE TABLE Pago ( 
    IdPago INT IDENTITY(1,1) PRIMARY KEY, 
    IdReserva INT FOREIGN KEY REFERENCES Reserva(IdReserva), 
    Monto DECIMAL(10,2) NOT NULL, 
    FechaPago DATETIME DEFAULT GETDATE(), 
    EstadoPago VARCHAR(30) DEFAULT 'Pagado' 
);
GO

-- ==========================================
-- TABLA USUARIO (LOGIN ADMIN / CLIENTE)
-- ==========================================
-- Rol: 'Admin' o 'Cliente'.
-- IdCliente enlaza la cuenta con su ficha de Cliente (NULL para Admin).
-- El usuario Admin por defecto (admin / Admin123!) se crea automáticamente
-- la primera vez que arranca la aplicación (ver Program.cs), para que el
-- hash de contraseña siempre sea generado por el mismo algoritmo que usa
-- la app en tiempo de ejecución.
CREATE TABLE Usuario (
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Rol VARCHAR(20) NOT NULL DEFAULT 'Cliente', -- Admin | Cliente
    IdCliente INT NULL FOREIGN KEY REFERENCES Cliente(IdCliente),
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- ==========================================
-- INSERCIÓN DE DATOS DE PRUEBA
-- ==========================================

INSERT INTO Cliente (DNI, Nombre, Apellido, Telefono, Correo) VALUES 
('70123456', 'Carlos', 'Pérez', '987654321', 'carlos.perez@email.com'), 
('70234567', 'María', 'Gómez', '912345678', 'maria.gomez@email.com'), 
('70345678', 'José', 'Rodríguez', '998877665', 'jose.rodriguez@email.com'), 
('70456789', 'Ana', 'Torres', '923456789', 'ana.torres@email.com'), 
('70567890', 'Luis', 'Fernández', '934567891', 'luis.fernandez@email.com'); 
GO

INSERT INTO Cancha (Nombre, Descripcion, Estado) VALUES 
('Cancha 1 - Principal', 'Grass sintético FIFA de última generación techada', 1), 
('Cancha 2 - Estándar', 'Grass sintético al aire libre con iluminación LED', 1), 
('Cancha 3 - Junior', 'Cancha reducida ideal para fulbito 5 vs 5', 1), 
('Cancha 4 - VIP', 'Grass natural reforzado con área de vestidores exclusiva', 1), 
('Cancha 5 - Mantenimiento', 'Cancha en proceso de renovación de grass', 0); 
GO

INSERT INTO Horario (HoraInicio, HoraFin) VALUES 
('08:00:00', '09:00:00'), 
('09:00:00', '10:00:00'), 
('18:00:00', '19:00:00'), 
('19:00:00', '20:00:00'), 
('20:00:00', '21:00:00');
GO

INSERT INTO Reserva (IdCliente, IdCancha, FechaReserva, IdHorario, EstadoReserva) VALUES 
(1, 1, '2026-07-01', 1, 'Confirmada'), 
(2, 2, '2026-07-01', 2, 'Confirmada'), 
(3, 3, '2026-07-02', 3, 'Confirmada'), 
(4, 4, '2026-07-02', 4, 'Confirmada'), 
(5, 1, '2026-07-03', 5, 'Cancelada');
GO

INSERT INTO Pago (IdReserva, Monto, FechaPago, EstadoPago) VALUES 
(1, 80.00, GETDATE(), 'Pagado'), 
(2, 70.00, GETDATE(), 'Pagado'), 
(3, 60.00, GETDATE(), 'Pagado'), 
(4, 100.00, GETDATE(), 'Pagado'), 
(5, 80.00, GETDATE(), 'Reembolsado');
GO

-- ==========================================
-- PROCEDIMIENTOS ALMACENADOS
-- ==========================================

-- 1. CLIENTE: Listar con Paginación y Búsqueda (incluye DNI)
CREATE OR ALTER PROCEDURE dbo.sp_ListarClientesPaginado
    @Buscar VARCHAR(100) = NULL,
    @Pagina INT = 1,
    @TamanoPagina INT = 5,
    @TotalRegistros INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SET @Buscar = ISNULL(@Buscar, '');

    SELECT @TotalRegistros = COUNT(1)
    FROM Cliente
    WHERE DNI LIKE '%' + @Buscar + '%'
       OR Nombre LIKE '%' + @Buscar + '%'
       OR Apellido LIKE '%' + @Buscar + '%';

    SELECT
        IdCliente,
        DNI,
        Nombre,
        Apellido,
        Telefono,
        Correo
    FROM Cliente
    WHERE DNI LIKE '%' + @Buscar + '%'
       OR Nombre LIKE '%' + @Buscar + '%'
       OR Apellido LIKE '%' + @Buscar + '%'
    ORDER BY IdCliente DESC
    OFFSET (@Pagina - 1) * @TamanoPagina ROWS
    FETCH NEXT @TamanoPagina ROWS ONLY;
END;
GO

-- 2. CLIENTE: Obtener por ID
CREATE OR ALTER PROCEDURE sp_ObtenerClientePorId
    @IdCliente INT
AS
BEGIN
    SELECT IdCliente, DNI, Nombre, Apellido, Telefono, Correo
    FROM Cliente
    WHERE IdCliente = @IdCliente;
END;
GO

-- 3. CLIENTE: Insertar
CREATE OR ALTER PROCEDURE sp_InsertarCliente
    @DNI VARCHAR(8),
    @Nombre VARCHAR(100),
    @Apellido VARCHAR(100),
    @Telefono VARCHAR(20),
    @Correo VARCHAR(100)
AS
BEGIN
    INSERT INTO Cliente (DNI, Nombre, Apellido, Telefono, Correo)
    VALUES (@DNI, @Nombre, @Apellido, @Telefono, @Correo);
    
    SELECT SCOPE_IDENTITY() AS IdCliente;
END;
GO

-- 4. CLIENTE: Actualizar
CREATE OR ALTER PROCEDURE sp_ActualizarCliente
    @IdCliente INT,
    @DNI VARCHAR(8),
    @Nombre VARCHAR(100),
    @Apellido VARCHAR(100),
    @Telefono VARCHAR(20),
    @Correo VARCHAR(100)
AS
BEGIN
    UPDATE Cliente
    SET DNI = @DNI,
        Nombre = @Nombre,
        Apellido = @Apellido,
        Telefono = @Telefono,
        Correo = @Correo
    WHERE IdCliente = @IdCliente;
END;
GO

-- 5. CLIENTE: Eliminar
CREATE OR ALTER PROCEDURE sp_EliminarCliente
    @IdCliente INT
AS
BEGIN
    DELETE FROM Cliente WHERE IdCliente = @IdCliente;
END;
GO

-- 6. CANCHA: Listar
CREATE OR ALTER PROCEDURE sp_ListarCanchas
AS
BEGIN
    SELECT IdCancha, Nombre, Descripcion, Estado FROM Cancha;
END;
GO

-- 7. HORARIO: Listar
CREATE OR ALTER PROCEDURE sp_ListarHorarios
AS
BEGIN
    SELECT IdHorario, HoraInicio, HoraFin FROM Horario;
END;
GO

-- 8. TRANSACCIONAL: Registrar Reserva + Pago
CREATE OR ALTER PROCEDURE sp_RegistrarReservaConPago
    @IdCliente INT,
    @IdCancha INT,
    @FechaReserva DATE,
    @IdHorario INT,
    @Monto DECIMAL(10,2),
    @IdReservaGenerado INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        IF EXISTS (
            SELECT 1 FROM Reserva 
            WHERE IdCancha = @IdCancha 
              AND FechaReserva = @FechaReserva 
              AND IdHorario = @IdHorario 
              AND EstadoReserva = 'Confirmada'
        )
        BEGIN
            RAISERROR('La cancha ya se encuentra reservada en este horario.', 16, 1);
        END

        INSERT INTO Reserva (IdCliente, IdCancha, FechaReserva, IdHorario, EstadoReserva)
        VALUES (@IdCliente, @IdCancha, @FechaReserva, @IdHorario, 'Confirmada');

        SET @IdReservaGenerado = SCOPE_IDENTITY();

        INSERT INTO Pago (IdReserva, Monto, FechaPago, EstadoPago)
        VALUES (@IdReservaGenerado, @Monto, GETDATE(), 'Pagado');

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- 9. REPORTE: Listar Reservas Paginado (incluye DNI)
CREATE OR ALTER PROCEDURE sp_ListarReservasReporte
    @FechaInicio DATE = NULL,
    @FechaFin DATE = NULL,
    @Pagina INT = 1,
    @TamanoPagina INT = 10,
    @TotalRegistros INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @TotalRegistros = COUNT(1)
    FROM Reserva R
    WHERE (@FechaInicio IS NULL OR R.FechaReserva >= @FechaInicio)
      AND (@FechaFin IS NULL OR R.FechaReserva <= @FechaFin);

    SELECT
        R.IdReserva,
        R.IdCliente,
        C.DNI,
        C.Nombre + ' ' + C.Apellido AS NombreCliente,
        R.IdCancha,
        CA.Nombre AS NombreCancha,
        R.FechaReserva,
        R.IdHorario,
        H.HoraInicio,
        H.HoraFin,
        R.EstadoReserva,
        P.Monto,
        P.EstadoPago
    FROM Reserva R
    INNER JOIN Cliente C ON R.IdCliente = C.IdCliente
    INNER JOIN Cancha CA ON R.IdCancha = CA.IdCancha
    INNER JOIN Horario H ON R.IdHorario = H.IdHorario
    LEFT JOIN Pago P ON R.IdReserva = P.IdReserva
    WHERE (@FechaInicio IS NULL OR R.FechaReserva >= @FechaInicio)
      AND (@FechaFin IS NULL OR R.FechaReserva <= @FechaFin)
    ORDER BY R.IdReserva DESC
    OFFSET (@Pagina - 1) * @TamanoPagina ROWS
    FETCH NEXT @TamanoPagina ROWS ONLY;
END;
GO

-- 10. RESERVA: Listar por Cliente (para el panel "Mis Reservas")
CREATE OR ALTER PROCEDURE sp_ListarReservasPorCliente
    @IdCliente INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.IdReserva,
        R.IdCliente,
        C.DNI,
        C.Nombre + ' ' + C.Apellido AS NombreCliente,
        R.IdCancha,
        CA.Nombre AS NombreCancha,
        R.FechaReserva,
        R.IdHorario,
        H.HoraInicio,
        H.HoraFin,
        R.EstadoReserva,
        P.Monto,
        P.EstadoPago
    FROM Reserva R
    INNER JOIN Cliente C ON R.IdCliente = C.IdCliente
    INNER JOIN Cancha CA ON R.IdCancha = CA.IdCancha
    INNER JOIN Horario H ON R.IdHorario = H.IdHorario
    LEFT JOIN Pago P ON R.IdReserva = P.IdReserva
    WHERE R.IdCliente = @IdCliente
    ORDER BY R.FechaReserva DESC, R.IdReserva DESC;
END;
GO
