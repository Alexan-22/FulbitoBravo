Base de Datos DBGrassSintetico

IF DB_ID('DBGrassSintetico') IS NULL
    CREATE DATABASE DBGrassSintetico;
GO
USE DBGrassSintetico;
GO

-- 1. Tabla Cliente
CREATE TABLE Cliente (
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Telefono VARCHAR(20) NOT NULL,
    Correo VARCHAR(100) NULL
);
GO

-- 2. Tabla Cancha
CREATE TABLE Cancha (
    IdCancha INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(150) NULL,
    Estado BIT DEFAULT 1 -- 1: Disponible, 0: Mantenimiento/Inactiva
);
GO

-- 3. Tabla Horario
CREATE TABLE Horario (
    IdHorario INT IDENTITY(1,1) PRIMARY KEY,
    HoraInicio TIME NOT NULL,
    HoraFin TIME NOT NULL
);
GO

-- 4. Tabla Reserva (Con validación lógica para evitar duplicados en misma cancha, fecha y horario)
CREATE TABLE Reserva (
    IdReserva INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente INT FOREIGN KEY REFERENCES Cliente(IdCliente),
    IdCancha INT FOREIGN KEY REFERENCES Cancha(IdCancha),
    FechaReserva DATE NOT NULL,
    IdHorario INT FOREIGN KEY REFERENCES Horario(IdHorario),
    EstadoReserva VARCHAR(30) DEFAULT 'Confirmada' -- Confirmada, Cancelada
);
GO

-- 5. Tabla Pago
CREATE TABLE Pago (
    IdPago INT IDENTITY(1,1) PRIMARY KEY,
    IdReserva INT FOREIGN KEY REFERENCES Reserva(IdReserva),
    Monto DECIMAL(10,2) NOT NULL,
    FechaPago DATETIME DEFAULT GETDATE(),
    EstadoPago VARCHAR(30) DEFAULT 'Pagado'
);
GO






IF DB_ID('DBGrassSintetico') IS NULL
    CREATE DATABASE DBGrassSintetico;
GO
USE DBGrassSintetico;
GO

-- 1. Tabla Cliente
CREATE TABLE Cliente (
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Telefono VARCHAR(20) NOT NULL,
    Correo VARCHAR(100) NULL
);
GO

-- 2. Tabla Cancha
CREATE TABLE Cancha (
    IdCancha INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(150) NULL,
    Estado BIT DEFAULT 1 -- 1: Disponible, 0: Mantenimiento/Inactiva
);
GO

-- 3. Tabla Horario
CREATE TABLE Horario (
    IdHorario INT IDENTITY(1,1) PRIMARY KEY,
    HoraInicio TIME NOT NULL,
    HoraFin TIME NOT NULL
);
GO

-- 4. Tabla Reserva
CREATE TABLE Reserva (
    IdReserva INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente INT FOREIGN KEY REFERENCES Cliente(IdCliente),
    IdCancha INT FOREIGN KEY REFERENCES Cancha(IdCancha),
    FechaReserva DATE NOT NULL,
    IdHorario INT FOREIGN KEY REFERENCES Horario(IdHorario),
    EstadoReserva VARCHAR(30) DEFAULT 'Confirmada'
);
GO

-- 5. Tabla Pago
CREATE TABLE Pago (
    IdPago INT IDENTITY(1,1) PRIMARY KEY,
    IdReserva INT FOREIGN KEY REFERENCES Reserva(IdReserva),
    Monto DECIMAL(10,2) NOT NULL,
    FechaPago DATETIME DEFAULT GETDATE(),
    EstadoPago VARCHAR(30) DEFAULT 'Pagado'
);
GO

-- ==========================================
-- INSERCIÓN DE 5 VALORES POR CADA TABLA
-- ==========================================

-- Insertar Clientes
INSERT INTO Cliente (Nombre, Apellido, Telefono, Correo) VALUES
('Carlos', 'Pérez', '987654321', 'carlos.perez@email.com'),
('María', 'Gómez', '912345678', 'maria.gomez@email.com'),
('José', 'Rodríguez', '998877665', 'jose.rodriguez@email.com'),
('Ana', 'Torres', '923456789', 'ana.torres@email.com'),
('Luis', 'Fernández', '934567891', 'luis.fernandez@email.com');
GO

-- Insertar Canchas
INSERT INTO Cancha (Nombre, Descripcion, Estado) VALUES
('Cancha 1 - Principal', 'Grass sintético FIFA de última generación techada', 1),
('Cancha 2 - Estándar', 'Grass sintético al aire libre con iluminación LED', 1),
('Cancha 3 - Junior', 'Cancha reducida ideal para fulbito 5 vs 5', 1),
('Cancha 4 - VIP', 'Grass natural reforzado con área de vestidores exclusiva', 1),
('Cancha 5 - Mantenimiento', 'Cancha en proceso de renovación de grass', 0);
GO

-- Insertar Horarios
INSERT INTO Horario (HoraInicio, HoraFin) VALUES
('08:00:00', '09:00:00'),
('09:00:00', '10:00:00'),
('18:00:00', '19:00:00'),
('19:00:00', '20:00:00'),
('20:00:00', '21:00:00');
GO

-- Insertar Reservas
INSERT INTO Reserva (IdCliente, IdCancha, FechaReserva, IdHorario, EstadoReserva) VALUES
(1, 1, '2026-07-01', 1, 'Confirmada'),
(2, 2, '2026-07-01', 2, 'Confirmada'),
(3, 3, '2026-07-02', 3, 'Confirmada'),
(4, 4, '2026-07-02', 4, 'Confirmada'),
(5, 1, '2026-07-03', 5, 'Cancelada');
GO

-- Insertar Pagos
INSERT INTO Pago (IdReserva, Monto, FechaPago, EstadoPago) VALUES
(1, 80.00, GETDATE(), 'Pagado'),
(2, 70.00, GETDATE(), 'Pagado'),
(3, 60.00, GETDATE(), 'Pagado'),
(4, 100.00, GETDATE(), 'Pagado'),
(5, 80.00, GETDATE(), 'Reembolsado');
GO
