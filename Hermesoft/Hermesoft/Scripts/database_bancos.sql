-- Script para crear las tablas de bancos y datos de ejemplo
-- Ejecutar en MySQL Workbench en la base de datos FUSION

USE FUSION;

-- Agregar columna logo a BANCOS si no existe
SELECT COUNT(*) INTO @columnExists
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = 'FUSION'
AND TABLE_NAME = 'BANCOS'
AND COLUMN_NAME = 'logo';

SET @sql = CASE WHEN @columnExists = 0 THEN 'ALTER TABLE BANCOS ADD COLUMN logo varchar(500) DEFAULT NULL' ELSE 'SELECT ''La columna logo ya existe'' AS Mensaje' END;

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Crear tabla TIPOS_ASALARIADOS si no existe
CREATE TABLE IF NOT EXISTS TIPOS_ASALARIADOS (
    idTipoAsalariado int NOT NULL AUTO_INCREMENT,
    nombre varchar(50) NOT NULL,
    PRIMARY KEY (idTipoAsalariado)
);

-- Crear tabla ENDEUDAMIENTOS_MAXIMOS si no existe
CREATE TABLE IF NOT EXISTS ENDEUDAMIENTOS_MAXIMOS (
    idEndeudamiento int NOT NULL AUTO_INCREMENT,
    porcEndeudamiento decimal(18,2) NOT NULL,
    idBanco int NOT NULL,
    idTipoAsalariado int NOT NULL,
    PRIMARY KEY (idEndeudamiento),
    FOREIGN KEY (idBanco) REFERENCES BANCOS(idBanco),
    FOREIGN KEY (idTipoAsalariado) REFERENCES TIPOS_ASALARIADOS(idTipoAsalariado)
);

-- Crear tabla SEGUROS si no existe
CREATE TABLE IF NOT EXISTS SEGUROS (
    idSeguro int NOT NULL AUTO_INCREMENT,
    nombre varchar(50) NOT NULL,
    porcSeguro decimal(18,2) NOT NULL,
    PRIMARY KEY (idSeguro)
);

-- Crear tabla SEGUROS_BANCOS si no existe
CREATE TABLE IF NOT EXISTS SEGUROS_BANCOS (
    idSeguroBanco int NOT NULL AUTO_INCREMENT,
    idBanco int NOT NULL,
    idSeguro int NOT NULL,
    PRIMARY KEY (idSeguroBanco),
    FOREIGN KEY (idBanco) REFERENCES BANCOS(idBanco),
    FOREIGN KEY (idSeguro) REFERENCES SEGUROS(idSeguro)
);

-- Crear tabla TASAS_INTERES si no existe
CREATE TABLE IF NOT EXISTS TASAS_INTERES (
    idTasaInteres int NOT NULL AUTO_INCREMENT,
    nombre varchar(50) NOT NULL,
    PRIMARY KEY (idTasaInteres)
);

-- Crear tabla ESCENARIOS_TASAS_INTERES si no existe
CREATE TABLE IF NOT EXISTS ESCENARIOS_TASAS_INTERES (
    idEscenario int NOT NULL AUTO_INCREMENT,
    nombre varchar(50) NOT NULL,
    porcAdicional decimal(18,2) NOT NULL,
    porcDatoBancario decimal(18,2) NOT NULL,
    plazo int NOT NULL,
    idBanco int NOT NULL,
    idTasaInteres int NOT NULL,
    PRIMARY KEY (idEscenario),
    FOREIGN KEY (idBanco) REFERENCES BANCOS(idBanco),
    FOREIGN KEY (idTasaInteres) REFERENCES TASAS_INTERES(idTasaInteres)
);

-- Insertar tipos de asalariados si no existen
INSERT INTO TIPOS_ASALARIADOS (nombre) VALUES
('Publico'), ('Privado'), ('Profesional Independiente'), ('Trabajador Independiente')
ON DUPLICATE KEY UPDATE nombre = VALUES(nombre);

-- Insertar tasas de interes si no existen
INSERT INTO TASAS_INTERES (nombre) VALUES
('Tasa Variable'), ('Tasa Escalonada')
ON DUPLICATE KEY UPDATE nombre = VALUES(nombre);

-- Insertar seguros si no existen
INSERT INTO SEGUROS (nombre, porcSeguro) VALUES
('Vida', 0), ('Desempleo', 0)
ON DUPLICATE KEY UPDATE nombre = VALUES(nombre);

-- Insertar datos de ejemplo solo si no existen
INSERT INTO BANCOS (nombre, enlace, maxCredito, honorarioAbogado, comision, tipoCambio, logo)
SELECT 'BAC San Jose', 'https://www.baccredomatic.com/es-cr', 35.00, 7.00, 5.00, '510,10', '/assets/images/bancos/BAC.png'
WHERE NOT EXISTS (SELECT 1 FROM BANCOS WHERE nombre = 'BAC San Jose');

INSERT INTO BANCOS (nombre, enlace, maxCredito, honorarioAbogado, comision, tipoCambio, logo)
SELECT 'Banco Nacional', 'https://www.bncr.fi.cr', 30.00, 6.50, 4.50, '510,10', '/assets/images/bancos/BNCR.png'
WHERE NOT EXISTS (SELECT 1 FROM BANCOS WHERE nombre = 'Banco Nacional');

INSERT INTO BANCOS (nombre, enlace, maxCredito, honorarioAbogado, comision, tipoCambio, logo)
SELECT 'Scotiabank', 'https://www.scotiabank.com.co', 35.00, 7.00, 5.00, '510,10', '/assets/images/bancos/Scotiabank.png'
WHERE NOT EXISTS (SELECT 1 FROM BANCOS WHERE nombre = 'Scotiabank');

SELECT 'Proceso completado correctamente' AS Resultado;
