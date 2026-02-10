CREATE DATABASE FUSION;
USE FUSION;

CREATE TABLE TIPO_CAMBIO(
idTipoCambio int not null primary key auto_increment,
tipoCambio double not null
);

CREATE TABLE BANCOS(
idBanco int not null primary key auto_increment,
nombre varchar(50) not null,
enlace varchar(250) not null,
maxCredito decimal not null,
honorarioAbogado decimal not null,
comision decimal not null,
logo VARCHAR(500) not null,
idTipoCambio int not null,
constraint FK_idTipoCambio_Banco foreign key (idTipoCambio) references TIPO_CAMBIO(idTipoCambio) 
);

CREATE TABLE USUARIOS(
idusuario int not null primary key auto_increment,
nombre varchar(50) not null,
apellido1 varchar(50) not null,
apellido2 varchar(50) not null,
correo varchar(70) not null,
password varchar(300) not null,
estado bool not null,
rol varchar(50) not null
);

CREATE TABLE PRIMAS(
idPrima int not null primary key auto_increment,
fechaInicio date not null,
fechaCierre date not null,
porcentaje decimal not null,
total decimal not null
);

CREATE TABLE DESGLOSES_PRIMAS(
idDesglosePrima int not null primary key auto_increment,
idPrima int not null,
fechaCobro date not null,
monto decimal not null,
estado varchar(50) not null,
fechaPagado date,
constraint FK_idPrima_Desglose foreign key (idPrima) references PRIMAS(idPrima) 
);

CREATE TABLE VENTAS(
numContrato int not null primary key auto_increment,
correoCliente varchar(70) not null,
gastoFormalizacion decimal not null,
codLote varchar(30) not null,
estado varchar(50) not null,
motivoNulidad varchar(300),
fechaDeRegistro date not null,
idPrima int not null,
idBanco int not null,
idUsuario int not null,
constraint FK_idPrima_Ventas foreign key (idPrima) references PRIMAS(idPrima),
constraint FK_idBanco_Ventas foreign key (idBanco) references BANCOS(idBanco),
constraint FK_idUsuario_Ventas foreign key (idUsuario) references USUARIOS(idUsuario)  
);

CREATE TABLE TIPOS_ASALARIADOS(
idTipoAsalariado int not null primary key auto_increment,
nombre varchar(50) not null
);

CREATE TABLE ENDEUDAMIENTOS_MAXIMOS(
idEndeudamiento int not null primary key auto_increment,
porcEndeudamiento decimal not null,
idBanco int not null,
idTipoAsalariado int not null,
constraint FK_idBanco_Endeudamiento foreign key (idBanco) references BANCOS(idBanco),
constraint FK_idTipoAsalariado_Endeudamiento foreign key (idTipoAsalariado) references TIPOS_ASALARIADOS(idTipoAsalariado)    
);

CREATE TABLE SEGUROS(
idSeguro int not null primary key auto_increment,
nombre varchar(50) not null
);

CREATE TABLE SEGUROS_BANCOS(
idSeguroBanco int not null primary key auto_increment,
porcSeguro decimal not null,
idBanco int not null,
idSeguro int not null,
constraint FK_idBanco_Seguro foreign key (idBanco) references BANCOS(idBanco),
constraint FK_idSeguro_Seguro foreign key (idSeguro) references SEGUROS(idSeguro)
);

CREATE TABLE INDICADORES_BANCARIOS(
idIndicador int not null primary key auto_increment,
nombre varchar(50) not null,
porcSeguro double not null,
fechaVigente date not null
);

CREATE TABLE TASAS_INTERES(
idTasaInteres int not null primary key auto_increment,
nombre varchar(50) not null
);

CREATE TABLE ESCENARIOS_TASAS_INTERES(
idEscenario int not null primary key auto_increment,
nombre varchar(50) not null,
idBanco int not null,
idTasaInteres int not null,
constraint FK_idBanco_Escenario foreign key (idBanco) references BANCOS(idBanco),
constraint FK_idTasaInteres_Escenario foreign key (idTasaInteres) references TASAS_INTERES(idTasaInteres)
);

CREATE TABLE MAPAS(
idMapa int not null primary key auto_increment,
direccion varchar(250) not null,
condominio varchar(100) not null 
);

CREATE TABLE COORDENADAS(
idCoordenada int not null primary key auto_increment,
x varchar(100) not null,
y varchar(100) not null,
lote varchar(100) not null,
idMapa int not null,
constraint FK_idMapa_Coordenadas foreign key(idMapa) references MAPAS(idMapa) 
);

CREATE TABLE PLAZOS_ESCENARIOS(
idPlazoEscenario int not null primary key auto_increment,
porcAdicional double not null,
plazo int not null,
idIndicador int not null,
idEscenario int not null,
constraint FK_idIndicador_Plazos foreign key(idIndicador) references INDICADORES_BANCARIOS(idIndicador),
constraint FK_idEscenario_Plazos foreign key(idEscenario) references ESCENARIOS_TASAS_INTERES(idEscenario)
);

CREATE TABLE HISTORICO_CAMBIOS_BANCARIOS (
    idHistorico INT AUTO_INCREMENT PRIMARY KEY,
    fechaCambio DATE NOT NULL,
    usuarioNombre VARCHAR(100) NOT NULL,
    usuarioCorreo VARCHAR(150) NOT NULL,
    tablaAfectada VARCHAR(60) NOT NULL,
    informacionAnterior LONGTEXT NOT NULL,
    informacionNueva LONGTEXT NOT NULL
);

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

select * from fusion.historico_cambios_bancarios;