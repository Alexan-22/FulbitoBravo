-- 1. Tabla Cliente 
CREATE TABLE Cliente ( IdCliente INT IDENTITY(1,1) PRIMARY KEY, Nombre VARCHAR(100) NOT NULL, Apellido VARCHAR(100) NOT NULL, Telefono VARCHAR(20) NOT NULL, Correo VARCHAR(100) NULL ); 
GO

-- 2. Tabla Cancha 
CREATE TABLE Cancha (
    IdCancha INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(150) NULL,
    Estado BIT DEFAULT 1
);
-- 1: Disponible, 0: Mantenimiento/Inactiva ); GO

-- 3. Tabla Horario 
CREATE TABLE Horario ( IdHorario INT IDENTITY(1,1) PRIMARY KEY, HoraInicio TIME NOT NULL, HoraFin TIME NOT NULL ); 
GO

-- 4. Tabla Reserva 
CREATE TABLE Reserva ( IdReserva INT IDENTITY(1,1) PRIMARY KEY, IdCliente INT FOREIGN KEY REFERENCES Cliente(IdCliente), IdCancha INT FOREIGN KEY REFERENCES Cancha(IdCancha), FechaReserva DATE NOT NULL, IdHorario INT FOREIGN KEY REFERENCES Horario(IdHorario), EstadoReserva VARCHAR(30) DEFAULT 'Confirmada' ); 
GO

-- 5. Tabla Pago 
CREATE TABLE Pago ( IdPago INT IDENTITY(1,1) PRIMARY KEY, IdReserva INT FOREIGN KEY REFERENCES Reserva(IdReserva), Monto DECIMAL(10,2) NOT NULL, FechaPago DATETIME DEFAULT GETDATE(), EstadoPago VARCHAR(30) DEFAULT 'Pagado' );
GO

-- ========================================== -- INSERCIÓN DE 5 VALORES POR CADA TABLA -- ==========================================

-- Insertar Clientes 
INSERT INTO Cliente (Nombre, Apellido, Telefono, Correo) VALUES ('Carlos', 'Pérez', '987654321', 'carlos.perez@email.com'), ('María', 'Gómez', '912345678', 'maria.gomez@email.com'), ('José', 'Rodríguez', '998877665', 'jose.rodriguez@email.com'), ('Ana', 'Torres', '923456789', 'ana.torres@email.com'), ('Luis', 'Fernández', '934567891', 'luis.fernandez@email.com'); 
GO

-- Insertar Canchas 
INSERT INTO Cancha (Nombre, Descripcion, Estado) VALUES ('Cancha 1 - Principal', 'Grass sintético FIFA de última generación techada', 1), ('Cancha 2 - Estándar', 'Grass sintético al aire libre con iluminación LED', 1), ('Cancha 3 - Junior', 'Cancha reducida ideal para fulbito 5 vs 5', 1), ('Cancha 4 - VIP', 'Grass natural reforzado con área de vestidores exclusiva', 1), ('Cancha 5 - Mantenimiento', 'Cancha en proceso de renovación de grass', 0); 
GO

-- Insertar Horarios 
INSERT INTO Horario (HoraInicio, HoraFin) VALUES ('08:00:00', '09:00:00'), ('09:00:00', '10:00:00'), ('18:00:00', '19:00:00'), ('19:00:00', '20:00:00'), ('20:00:00', '21:00:00');
GO

-- Insertar Reservas 
INSERT INTO Reserva (IdCliente, IdCancha, FechaReserva, IdHorario, EstadoReserva) VALUES (1, 1, '2026-07-01', 1, 'Confirmada'), (2, 2, '2026-07-01', 2, 'Confirmada'), (3, 3, '2026-07-02', 3, 'Confirmada'), (4, 4, '2026-07-02', 4, 'Confirmada'), (5, 1, '2026-07-03', 5, 'Cancelada');
GO

-- Insertar Pagos
INSERT INTO Pago (IdReserva, Monto, FechaPago, EstadoPago) VALUES (1, 80.00, GETDATE(), 'Pagado'), (2, 70.00, GETDATE(), 'Pagado'), (3, 60.00, GETDATE(), 'Pagado'), (4, 100.00, GETDATE(), 'Pagado'), (5, 80.00, GETDATE(), 'Reembolsado');
GO

SELECT @@VERSION;

-- ==========================================
-- 1. PROCEDIMIENTOS ALMACENADOS: CLIENTE
-- ==========================================

-- Listar Clientes con Paginación y Búsqueda
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
    WHERE Nombre LIKE '%' + @Buscar + '%'
       OR Apellido LIKE '%' + @Buscar + '%';

    SELECT
        IdCliente,
        Nombre,
        Apellido,
        Telefono,
        Correo
    FROM Cliente
    WHERE Nombre LIKE '%' + @Buscar + '%'
       OR Apellido LIKE '%' + @Buscar + '%'
    ORDER BY IdCliente DESC
    OFFSET (@Pagina - 1) * @TamanoPagina ROWS
    FETCH NEXT @TamanoPagina ROWS ONLY;
END;
GO

-- Obtener Cliente por ID
CREATE OR ALTER PROCEDURE sp_ObtenerClientePorId
    @IdCliente INT
AS
BEGIN
    SELECT IdCliente, Nombre, Apellido, Telefono, Correo
    FROM Cliente
    WHERE IdCliente = @IdCliente;
END;
GO

-- Registrar Cliente
CREATE OR ALTER PROCEDURE sp_InsertarCliente
    @Nombre VARCHAR(100),
    @Apellido VARCHAR(100),
    @Telefono VARCHAR(20),
    @Correo VARCHAR(100)
AS
BEGIN
    INSERT INTO Cliente (Nombre, Apellido, Telefono, Correo)
    VALUES (@Nombre, @Apellido, @Telefono, @Correo);
    
    SELECT SCOPE_IDENTITY() AS IdCliente;
END;
GO

-- Actualizar Cliente
CREATE OR ALTER PROCEDURE sp_ActualizarCliente
    @IdCliente INT,
    @Nombre VARCHAR(100),
    @Apellido VARCHAR(100),
    @Telefono VARCHAR(20),
    @Correo VARCHAR(100)
AS
BEGIN
    UPDATE Cliente
    SET Nombre = @Nombre,
        Apellido = @Apellido,
        Telefono = @Telefono,
        Correo = @Correo
    WHERE IdCliente = @IdCliente;
END;
GO

-- Eliminar Cliente
CREATE OR ALTER PROCEDURE sp_EliminarCliente
    @IdCliente INT
AS
BEGIN
    DELETE FROM Cliente WHERE IdCliente = @IdCliente;
END;
GO

-- ==========================================
-- 2. PROCEDIMIENTOS ALMACENADOS: CANCHA
-- ==========================================

CREATE OR ALTER PROCEDURE sp_ListarCanchas
AS
BEGIN
    SELECT IdCancha, Nombre, Descripcion, Estado FROM Cancha;
END;
GO

-- ==========================================
-- 3. PROCEDIMIENTOS ALMACENADOS: HORARIO
-- ==========================================

CREATE OR ALTER PROCEDURE sp_ListarHorarios
AS
BEGIN
    SELECT IdHorario, HoraInicio, HoraFin FROM Horario;
END;
GO

-- ==========================================
-- 4. SP TRANSACCIONAL: REGISTRAR RESERVA + PAGO
-- ==========================================
-- (Satisface la rúbrica de Carrito/Transacción en BD)

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
        -- Validar si la cancha ya está ocupada en ese horario y fecha
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

        -- 1. Insertar Reserva
        INSERT INTO Reserva (IdCliente, IdCancha, FechaReserva, IdHorario, EstadoReserva)
        VALUES (@IdCliente, @IdCancha, @FechaReserva, @IdHorario, 'Confirmada');

        SET @IdReservaGenerado = SCOPE_IDENTITY();

        -- 2. Insertar Pago correspondiente
        INSERT INTO Pago (IdReserva, Monto, FechaPago, EstadoPago)
        VALUES (@IdReservaGenerado, @Monto, GETDATE(), 'Pagado');

        -- Confirmar ambos registros
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- En caso de error, deshacer todos los cambios
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- ==========================================
-- 5. PROCEDIMIENTO: REPORTE DE RESERVAS (PAGINADO)
-- ==========================================

CREATE OR ALTER PROCEDURE sp_ListarReservasReporte
    @FechaInicio DATE = NULL,
    @FechaFin DATE = NULL,
    @Pagina INT = 1,
    @TamanoPagina INT = 10,
    @TotalRegistros INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener total de registros
    SELECT @TotalRegistros = COUNT(1)
    FROM Reserva R
    WHERE (@FechaInicio IS NULL OR R.FechaReserva >= @FechaInicio)
      AND (@FechaFin IS NULL OR R.FechaReserva <= @FechaFin);

    -- Obtener reservas paginadas
    SELECT
        R.IdReserva,
        R.IdCliente,
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
    INNER JOIN Cliente C
        ON R.IdCliente = C.IdCliente
    INNER JOIN Cancha CA
        ON R.IdCancha = CA.IdCancha
    INNER JOIN Horario H
        ON R.IdHorario = H.IdHorario
    LEFT JOIN Pago P
        ON R.IdReserva = P.IdReserva
    WHERE (@FechaInicio IS NULL OR R.FechaReserva >= @FechaInicio)
      AND (@FechaFin IS NULL OR R.FechaReserva <= @FechaFin)
    ORDER BY R.IdReserva DESC
    OFFSET (@Pagina - 1) * @TamanoPagina ROWS
    FETCH NEXT @TamanoPagina ROWS ONLY;
END;
GO

SELECT IdCancha, Nombre, Descripcion, Estado
FROM Cancha
WHERE IdCancha = 1;