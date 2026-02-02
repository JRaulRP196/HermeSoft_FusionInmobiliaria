-- parte 1: agregar columna logo a tabla bancos
-- si da error 1060, la columna ya existe, ignorar y continuar
ALTER TABLE BANCOS ADD COLUMN logo VARCHAR(500) DEFAULT NULL;

-- parte 2: agregar columna porcseguro a seguros bancos
-- si da error 1060, la columna ya existe, ignorar y continuar
ALTER TABLE SEGUROS_BANCOS ADD COLUMN porcSeguro DECIMAL(18,2) NOT NULL DEFAULT 0;

-- parte 3: crear tablas de catalogos
CREATE TABLE IF NOT EXISTS TIPOS_ASALARIADOS (
    idTipoAsalariado INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS SEGUROS (
    idSeguro INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    porcSeguro DECIMAL(18,2) NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS INDICADORES_BANCARIOS (
    idIndicador INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    porcIndicador DECIMAL(18,2) NOT NULL DEFAULT 0,
    fechaVigente DATE
);

CREATE TABLE IF NOT EXISTS TASAS_INTERES (
    idTasaInteres INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL
);

-- parte 4: crear tablas de relaciones
CREATE TABLE IF NOT EXISTS ENDEUDAMIENTOS_MAXIMOS (
    idEndeudamiento INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    porcEndeudamiento DECIMAL(18,2) NOT NULL,
    idBanco INT NOT NULL,
    idTipoAsalariado INT NOT NULL,
    FOREIGN KEY (idBanco) REFERENCES BANCOS(idBanco) ON DELETE CASCADE,
    FOREIGN KEY (idTipoAsalariado) REFERENCES TIPOS_ASALARIADOS(idTipoAsalariado)
);

CREATE TABLE IF NOT EXISTS SEGUROS_BANCOS (
    idSeguroBanco INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    idBanco INT NOT NULL,
    idSeguro INT NOT NULL,
    porcSeguro DECIMAL(18,2) NOT NULL DEFAULT 0,
    FOREIGN KEY (idBanco) REFERENCES BANCOS(idBanco) ON DELETE CASCADE,
    FOREIGN KEY (idSeguro) REFERENCES SEGUROS(idSeguro)
);

CREATE TABLE IF NOT EXISTS INDICADORES_BANCOS (
    idIndicadorBanco INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    idBanco INT NOT NULL,
    idIndicador INT NOT NULL,
    FOREIGN KEY (idBanco) REFERENCES BANCOS(idBanco) ON DELETE CASCADE,
    FOREIGN KEY (idIndicador) REFERENCES INDICADORES_BANCARIOS(idIndicador)
);

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

-- parte 5: insertar datos en catalogos (solo si esta vacio)
INSERT IGNORE INTO TIPOS_ASALARIADOS (idTipoAsalariado, nombre) VALUES
(1, 'Publico'),
(2, 'Privado'),
(3, 'Profesional Independiente'),
(4, 'Trabajador Independiente');

INSERT IGNORE INTO TASAS_INTERES (idTasaInteres, nombre) VALUES
(1, 'Tasa Variable'),
(2, 'Tasa Escalonada');

INSERT IGNORE INTO SEGUROS (idSeguro, nombre) VALUES
(1, 'Desempleo'),
(2, 'Vida');

SELECT 'base de datos actualizada correctamente' AS resultado;
