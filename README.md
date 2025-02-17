# Usuarios Backend

Este repositorio contiene el backend de una aplicación para la gestión de usuarios. Está desarrollado con .NET y C#, y permite realizar operaciones relacionadas con la gestión de usuarios desde la creación hasta el alta del mismo.

## Características
- API RESTful para la gestión de Usuarios.
- Conexión a base de datos PostgreSQL integrado desde SupaBase.
- Integración con SMTP Brevo para la gestión de envíos de correos electrónicos.

## Requisitos previos
- .NET SDK 8.0 o superior instalado.
- Una cuenta en [Supabase](https://supabase.com/).
- Una base de datos PostgreSQL.
- Cuenta en [Brevo](https://login.brevo.com/).
- Git para clonar el repositorio.

## Instalación y configuración

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/KEV4142/Usuarios_Backend.git
   cd Usuarios_Backend
   ```

2. **Configurar las variables de entorno:**
   Crear un archivo `.env` en el directorio raíz del proyecto y agregar las siguientes variables:
   ```env
   DB_CONNECTION= [enlace de conexion brindado por Supabase]
   TOKEN_KEY= [Campo tipo string aleatorio]
   SMTPSERVER= [Campo brindado por Brevo]
   PORT_MAIL= [Campo brindado por Brevo]
   SENDEREMAIL= [Campo brindado por Brevo]
   SENDERNAME= [Campo brindado por Brevo]
   USERNAME= [Campo brindado por Brevo]
   PASSWORD= [Campo brindado por Brevo]
   AES_KEY = [Campo tipo string aleatorio 32 bits]
   AES_IV = [Campo tipo string aleatorio 16 bits]
   FRONTEND_ORIGIN=*
   BACKEND_ORIGIN=*
   #PORT=5000
   ```

   > **Nota:** Cambia estas variables de entorno según tus necesidades y evita compartir credenciales sensibles en repositorios públicos. Adicional las ultimas 2 variables es para habilitar la funcion CORS y qué encabezados de host (Host) son permitidos al realizar solicitudes al servidor. La variable PORT dependera del servicio donde se despliegue.

3. **Restaurar las dependencias:**
   Ejecuta el siguiente comando para restaurar los paquetes necesarios:
   ```bash
   dotnet restore
   ```

4. **Ejecutar las migraciones:**
   Asegúrate de que la conexión a la base de datos esté configurada correctamente y ejecuta:
   ```bash
   cd Repositorio
   dotnet ef database update
   ```

5. **Iniciar el servidor:**
   Inicia el backend localmente:
   ```bash
   cd WebApi
   dotnet run
   ```

   El servidor estará disponible en `http://localhost:5000` por defecto (o `https://localhost:5001` para HTTPS).

## Uso
- Usa herramientas como [Postman](https://www.postman.com/) para probar los endpoints de la API.
- Integra el backend con el frontend especificando el origen permitido en `FRONTEND_ORIGIN`.

## Despliegue
Puedes desplegar este proyecto en cualquier servicio compatible, como Azure App Service, AWS, o Heroku. Recuerda configurar las variables de entorno necesarias en tu plataforma de despliegue.

## Tecnologías utilizadas
- **.NET 8**: Framework principal para el backend.
- **Supabase**: Almacenamiento para motor de base de datos.
- **PostgreSQL**: Base de datos relacional.
- **Brevo**: Servicio de correo electrónico.
- **JWT**: JWT con segunda capa de seguridad.


## Licencia
Este proyecto está bajo la Licencia MIT. Consulta el archivo `LICENSE` para más detalles.
