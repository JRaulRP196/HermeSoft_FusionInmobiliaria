>> Hola, empezamos a trabajar nuestro proyecto en C:\Users\kenni\source\repos\Proyecto Graduacion\HermeSoft_FusionInmobiliaria\Hermesoft\Hermesoft, lo estoy trabajando en visual studio y como ves ya trabajamos la funcionalidad de Lotes, me gustaria que revises el código para entender lo que hemos construido hasta el momento. Es una aplicación para un emprendimiento de Costa Rica.



>> Tambien quiero que veas el script de la base de datos que creamos para que tengas en mente como esta construido todo actualmente.



Base de datos:

CREATE DATABASE FUSION;

USE FUSION;



CREATE TABLE BANCOS(

idBanco int not null primary key auto\_increment,

nombre varchar(50) not null,

enlace varchar(250) not null,

maxCredito decimal not null,

honorarioAbogado decimal not null,

comision decimal not null,

tipoCambio varchar(30) not null

);



CREATE TABLE USUARIOS(

idusuario int not null primary key auto\_increment,

nombre varchar(50) not null,

apellido1 varchar(50) not null,

apellido2 varchar(50) not null,

correo varchar(70) not null,

password varchar(300) not null,

estado bool not null,

rol varchar(50) not null

);



CREATE TABLE PRIMAS(

idPrima int not null primary key auto\_increment,

fechaInicio date not null,

fechaCierre date not null,

porcentaje decimal not null,

total decimal not null

);



CREATE TABLE DESGLOSES\_PRIMAS(

idDesglosePrima int not null primary key auto\_increment,

idPrima int not null,

fechaCobro date not null,

monto decimal not null,

estado varchar(50) not null,

fechaPagado date,

constraint FK\_idPrima\_Desglose foreign key (idPrima) references PRIMAS(idPrima) 

);



CREATE TABLE VENTAS(

numContrato int not null primary key auto\_increment,

correoCliente varchar(70) not null,

gastoFormalizacion decimal not null,

codLote varchar(30) not null,

estado varchar(50) not null,

motivoNulidad varchar(300),

fechaDeRegistro date not null,

idPrima int not null,

idBanco int not null,

idUsuario int not null,

constraint FK\_idPrima\_Ventas foreign key (idPrima) references PRIMAS(idPrima),

constraint FK\_idBanco\_Ventas foreign key (idBanco) references BANCOS(idBanco),

constraint FK\_idUsuario\_Ventas foreign key (idUsuario) references USUARIOS(idUsuario)  

);



CREATE TABLE TIPOS\_ASALARIADOS(

idTipoAsalariado int not null primary key auto\_increment,

nombre varchar(50) not null

);



CREATE TABLE ENDEUDAMIENTOS\_MAXIMOS(

idEndeudamiento int not null primary key auto\_increment,

porcEndeudamiento decimal not null,

idBanco int not null,

idTipoAsalariado int not null,

constraint FK\_idBanco\_Endeudamiento foreign key (idBanco) references BANCOS(idBanco),

constraint FK\_idTipoAsalariado\_Endeudamiento foreign key (idTipoAsalariado) references TIPOS\_ASALARIADOS(idTipoAsalariado)    

);



CREATE TABLE SEGUROS(

idSeguro int not null primary key auto\_increment,

nombre varchar(50) not null,

porcSeguro decimal not null

);



CREATE TABLE SEGUROS\_BANCOS(

idSeguroBanco int not null primary key auto\_increment,

idBanco int not null,

idSeguro int not null,

constraint FK\_idBanco\_Seguro foreign key (idBanco) references BANCOS(idBanco),

constraint FK\_idSeguro\_Seguro foreign key (idSeguro) references SEGUROS(idSeguro)

);



CREATE TABLE INDICADORES\_BANCARIOS(

idIndicador int not null primary key auto\_increment,

nombre varchar(50) not null,

porcSeguro decimal not null,

fechaVigente date not null

);



CREATE TABLE INDICADORES\_BANCOS(

idIdicadorBanco int not null primary key auto\_increment,

idBanco int not null,

idIndicador int not null,

constraint FK\_idBanco\_Indicador foreign key (idBanco) references BANCOS(idBanco),

constraint FK\_idIndicador\_Indicador foreign key (idIndicador) references INDICADORES\_BANCARIOS(idIndicador)

);



CREATE TABLE TASAS\_INTERES(

idTasaInteres int not null primary key auto\_increment,

nombre varchar(50) not null

);



CREATE TABLE ESCENARIOS\_TASAS\_INTERES(

idEscenario int not null primary key auto\_increment,

nombre varchar(50) not null,

porcAdicional decimal not null,

porcDatoBancario decimal not null,

plazo int not null,

idBanco int not null,

idTasaInteres int not null,

constraint FK\_idBanco\_Escenario foreign key (idBanco) references BANCOS(idBanco),

constraint FK\_idTasaInteres\_Escenario foreign key (idTasaInteres) references TASAS\_INTERES(idTasaInteres)

);



CREATE TABLE MAPAS(

idMapa int not null primary key auto\_increment,

direccion varchar(250) not null,

condominio varchar(100) not null 

);



CREATE TABLE COORDENADAS(

idCoordenada int not null primary key auto\_increment,

x varchar(100) not null,

y varchar(100) not null,

lote varchar(100) not null,

idMapa int not null,

constraint FK\_idMapa\_Coordenadas foreign key(idMapa) references MAPAS(idMapa) 

);



>> Una vez hayas analizado estas cosas, quiero que lleves acabo estas dos historias de usuario:



#### Visualización del listado de bancos

Como usuario, quiero ver una lista de todos los bancos registrados, para conocer las entidades bancarias vigentes en la plataforma.



Escenario 1: Lista con bancos registrados

* Dado que existen bancos registrados en el sistema
* Cuando el usuario selecciona la opción **“Bancos”** del menú principal
* Entonces el sistema debe mostrar una lista en formato de tarjetas
* Y cada tarjeta debe incluir el logo, nombre y la opción **“Detalle”**
* Y mostrar la opción para registrar un nuevo banco





Escenario 2: Sin bancos registrados

* Dado que no existen bancos registrados
* Cuando el usuario selecciona la opción **“Bancos”**
* Entonces el sistema debe mostrar únicamente la opción de registrar un nuevo banco
* Y un mensaje indicando que no hay bancos registrados





#### Visualización del detalle de un banco

Como usuario, quiero ver el detalle completo de un banco, para verificar que la información registrada esté actualizada.



Escenario 1: Visualización del detalle de un banco existente

* Dado que el usuario selecciona un banco válido
* Cuando presiona el botón “Detalle”
* Entonces el sistema debe mostrar una vista con la información del banco:

1. Nombre
2. Logo
3. Enlace web
4. Plazo máximo de crédito en años
5. Porcentaje de endeudamiento máximo según tipo de asalariado
6. Tasas de interés



Escenario 2: Banco eliminado o inexistente

* Dado que el banco seleccionado no existe en la base de datos
* Cuando presiona el botón “Detalle”
* Entonces el sistema debe redirigir al listado de bancos
* Y mostrar un mensaje indicando que el banco fue eliminado o no existe



>> Y tengo los siguientes puntos importantes que debes tener en cuenta a la hora de programar:

1. Cuando programes no sobre uses los comentarios, no quiero que uses emojis, ni símbolos raros, y cuando tengas que hacer comentarios para explicar alguna parte del código, quiero que sea un comentario simple, sin mayúsculas, sin tildes y en español. (Lo que se vea visualmente en la aplicacion si debe tener sus respectivas mayusculas y tildes).
2. Deberas tomar en cuenta la estructura actual de la base de datos para construir lo que queremos, sin embargo si necesitas hacer ajustes a la base de datos para que funcione con esta implementación puedes hacerlo, en ese caso debes crear un .sql con las modificaciones para simplemente abrirlo en MySQL workbench y correr el file, y que eso haga los cambios, esto solo si necesitas hacer cambios para llevar a cabo lo que te estoy pidiendo.
3. Como ves, en nuestra aplicación ya tenemos bancos agregados como ejemplo hardcoded, esto era para tener un ejemplo visual de cómo debería quedar tanto el listado, los inputs y el detalle. Asi que puedes usar los bancos que ya están hardcoded como un ejemplo de como queremos que esto termine funcionando al agregar un banco con la funcionalidad bien implementada.
4. Al programar por favor enfocate en las dos historias de usuario que te solicite especificamente, no hagas cosas ajenas a lo que necesito.
5. Ten en cuenta que estamos abriendo la solucion en visual studio y por eso quiero que primero analices lo que ya esta construido para que te guies en como debes construir todo esta parte.
6. Pon atencion a como esta construido el formulario, no debes alterarlo ni hacer cosas complicadas. intenta por favor no usar codigo complicado si no mas bien, lograr lo que necesito de la manera mas simple posible. Que el formulario se maneje de una forma simple y se quede como esta en este momento, recuerda que puedes crear un .sqñ file que pueda correr en mysql workbench si es necesario modificar la base de datos de como fue construida originalmente
7. Como veras, actualmente hay 3 bancos que aparecen listados como ejemplo hardcoded, por favor asegurate de removerlas para que ahora en el listado solo aparezvcan los bancos que se hayan publicado en la base de datos con tu nueva funcionalidad ya aplicada 
