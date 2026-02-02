-- script para actualizar la estructura de la base de datos
-- este script es seguro para ejecutar en una base de datos ya existente

USE FUSION;

-- agregar columna logo a tabla bancos si no existe
SELECT COUNT(*) INTO @columnExists
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = 'FUSION'
AND TABLE_NAME = 'BANCOS'
AND COLUMN_NAME = 'logo';

IF @columnExists = 0 THEN
    ALTER TABLE BANCOS ADD COLUMN logo VARCHAR(500) DEFAULT NULL;
END IF;

-- tabla tipos asalariados (catalogo con datos fijos)
CREATE TABLE IF NOT EXISTS TIPOS_ASALARIADOS (
    idTipoAsalariado INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL
);

-- verificar si ya hay datos en tipos asalariados
SELECT COUNT(*) INTO @tipoAsalariadoCount FROM TIPOS_ASALARIADOS;

IF @tipoAsalariadoCount = 0 THEN
    INSERT INTO TIPOS_ASALARIADOS (idTipoAsalariado, nombre) VALUES
    (1, 'Publico'),
    (2, 'Privado'),
    (3, 'Profesional Independiente'),
    (4, 'Trabajador Independiente')
    ON DUPLICATE KEY UPDATE nombre = VALUES(nombre);
END IF;

-- tabla endeudamiento maximos
CREATE TABLE IF NOT EXISTS ENDEUDAMIENTOS_MAXIMOS (
    idEndeudamiento INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    porcEndeudamiento DECIMAL(18,2) NOT NULL,
    idBanco INT NOT NULL,
    idTipoAsalariado INT NOT NULL,
    FOREIGN KEY (idBanco) REFERENCES BANCOS(idBanco) ON DELETE CASCADE,
    FOREIGN KEY (idTipoAsalariado) REFERENCES TIPOS_ASALARIADOS(idTipoAsalariado)
);

-- tabla seguros
CREATE TABLE IF NOT EXISTS SEGUROS (
    idSeguro INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    porcSeguro DECIMAL(18,2) NOT NULL DEFAULT 0
);

-- tabla seguros bancos
CREATE TABLE IF NOT EXISTS SEGUROS_BANCOS (
    idSeguroBanco INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    idBanco INT NOT NULL,
    idSeguro INT NOT NULL,
    FOREIGN KEY (idBanco) REFERENCES BANCOS(idBanco) ON DELETE CASCADE,
    FOREIGN KEY (idSeguro) REFERENCES SEGUROS(idSeguro)
);

-- tabla indicadores bancarios
CREATE TABLE IF NOT EXISTS INDICADORES_BANCARIOS (
    idIndicador INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    porcIndicador DECIMAL(18,2) NOT NULL DEFAULT 0,
    fechaVigente DATE
);

-- tabla indicadores bancos
CREATE TABLE IF NOT EXISTS INDICADORES_BANCOS (
    idIndicadorBanco INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    idBanco INT NOT NULL,
    idIndicador INT NOT NULL,
    FOREIGN KEY (idBanco) REFERENCES BANCOS(idBanco) ON DELETE CASCADE,
    FOREIGN KEY (idIndicador) REFERENCES INDICADORES_BANCARIOS(idIndicador)
);

-- tabla tasas interes (catalogo con datos fijos)
CREATE TABLE IF NOT EXISTS TASAS_INTERES (
    idTasaInteres INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL
);

-- verificar si ya hay datos en tasas interes
SELECT COUNT(*) INTO @tasaInteresCount FROM TASAS_INTERES;

IF @tasaInteresCount = 0 THEN
    INSERT INTO TASAS_INTERES (idTasaInteres, nombre) VALUES
    (1, 'Tasa Variable'),
    (2, 'Tasa Escalonada')
    ON DUPLICATE KEY UPDATE nombre = VALUES(nombre);
END IF;

-- tabla escenarios tasas interes
CREATE TABLE IF NOT EXISTS ESCENARIOS_TASAS_INTERES (
    idEscenario INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    porcAdicional DECIMAL(18,2) NOT NULL DEFAULT 0,
    porcDatoBancario DECIMAL(18,2) NOT NULL DEFAULT 0,
    plazo INT NOT NULL DEFAULT 0,
    idBanco INT NOT NULL,
    idTasaInteres INT NOT NULL,
    FOREIGN KEY (idBanco) REFERENCES BANCOS(idBanco) ON DELETE CASCADE,
    FOREIGN KEY (idTasaInteres) REFERENCES TASAS_INTERES(idTasaInteres)
);

SELECT 'base de datos actualizada correctamente' AS Resultado;
