# WhatsappNet API

WhatsappNet API es un proyecto ASP.NET Core (.NET 6+) diseñado para interactuar con la **API de WhatsApp Cloud**. Permite la verificación de webhooks, la recepción de mensajes entrantes (texto y respuestas interactivas) y el envío de diversos tipos de mensajes (texto, imágenes, audio, video, documentos, ubicación y botones interactivos) como respuesta.

Este proyecto está estructurado para utilizar Azure Key Vault para el almacenamiento seguro de credenciales como el `phoneNumberId` y el `accessToken` necesarios para la API de WhatsApp Cloud, aunque la implementación específica de `KeyVaultHelper` no se incluye en los archivos proporcionados.

## Tabla de Contenidos

- Descripción
- Características
- Arquitectura Básica
- Requisitos Previos
- Configuración
  - Azure Key Vault
  - Token de Verificación del Webhook
  - Configuración del Webhook en Meta Developers Portal
- Instalación y Ejecución
- API Endpoints
  - Verificación del Webhook
  - Recepción de Mensajes (Webhook)
  - Endpoint de Prueba de Envío
- Uso y Lógica de Respuesta
- Estructura del Proyecto (Clases Clave)
- Consideraciones Importantes y Mejoras
- Contribuciones
- Licencia

## Descripción

Este proyecto implementa una API backend que actúa como un intermediario entre tu aplicación y la API de WhatsApp Cloud. Está construido con ASP.NET Core y C#, facilitando la comunicación bidireccional con usuarios de WhatsApp.

**Funcionalidades principales:**

1.  **Verificación de Webhook:** Implementa el endpoint `GET` requerido por WhatsApp para verificar la URL de callback.
2.  **Recepción de Mensajes:** Configura un endpoint `POST` para recibir notificaciones en tiempo real de WhatsApp, incluyendo:
    *   Mensajes de texto.
    *   Respuestas de botones interactivos (`button_reply`).
    *   Respuestas de listas interactivas (`list_reply`).
3.  **Envío de Mensajes:** Proporciona lógica para enviar diversos tipos de mensajes a los usuarios de WhatsApp, incluyendo:
    *   Texto
    *   Imágenes (desde URL)
    *   Audio (desde URL)
    *   Video (desde URL)
    *   Documentos (desde URL)
    *   Ubicación (coordenadas y detalles fijos)
    *   Botones interactivos (con opciones predefinidas)
4.  **Manejo Básico de Conversaciones:** El `WhtasappController` incluye una lógica simple para responder a mensajes entrantes específicos con diferentes tipos de mensajes predefinidos, basados en palabras clave.

## Características

*   Integración con la API de WhatsApp Cloud.
*   Verificación de webhook para la configuración inicial con WhatsApp.
*   Recepción de mensajes de texto y respuestas interactivas.
*   Envío de mensajes de texto, multimedia (imagen, audio, video, documento), ubicación y botones.
*   Estructura modular con servicios (`IWhasappCloudSendMessage`) y utilidades (`IUtil`) para la creación y envío de mensajes.
*   Modelos de datos (`WhatsAppCloudModel`) para la deserialización de payloads de webhook.
*   Diseñado para usar Azure Key Vault para la gestión segura de secretos (ID de número de teléfono y token de acceso).

## Arquitectura Básica

1.  **`WhtasappController.cs`**:
    *   Punto de entrada para las solicitudes HTTP.
    *   Maneja la verificación del webhook (`GET /api/whatsapp`).
    *   Procesa los mensajes entrantes recibidos del webhook de WhatsApp (`POST /api/whatsapp`).
    *   Contiene un endpoint de prueba para enviar un mensaje (`GET /api/whatsapp/test`).
    *   Utiliza `IUtil` para formatear los mensajes de respuesta y `IWhasappCloudSendMessage` para enviarlos.
2.  **`WhatsAppCloudModel.cs`**:
    *   Define la estructura de los objetos C# que se corresponden con el JSON recibido del webhook de WhatsApp. Esto permite una fácil deserialización de los datos entrantes.
3.  **`Util/IUtil.cs` y `Util/Util.cs`**:
    *   `IUtil`: Interfaz que define los métodos para crear diferentes tipos de objetos de mensaje.
    *   `Util`: Implementación que construye los objetos JSON (como tipos anónimos) necesarios para enviar diferentes tipos de mensajes a través de la API de WhatsApp Cloud. Las URLs para contenido multimedia y los detalles de ubicación están actualmente hardcodeados aquí.
4.  **`Services/WhatsappCloud/SendMessages/IWhasappCloudSendMessage.cs`**:
    *   Interfaz para el servicio responsable de enviar mensajes a la API de WhatsApp Cloud.
    *   Define el método `Task<bool> Execute(object model)`.
5.  **`Services/WhatsappCloud/SendMessages/WhasappCloudSendMessage.cs`** (No proporcionado, pero su existencia se infiere):
    *   Implementación de `IWhasappCloudSendMessage`.
    *   Sería responsable de realizar la llamada HTTP POST a la API de Graph de Facebook (endpoint de WhatsApp Cloud).
    *   Se espera que obtenga las credenciales (`phoneNumberId`, `accessToken`) de Azure Key Vault (a través de un `KeyVaultHelper` o similar).

## Requisitos Previos

*   .NET SDK (el proyecto parece ser .NET 6+ por el uso de `Program.cs` implícito y características modernas de C#).
*   Una cuenta de Azure con una instancia de Azure Key Vault configurada.
*   Una cuenta de Desarrollador de Facebook (Meta) con una aplicación configurada para la API de WhatsApp Cloud:
    *   Un número de teléfono registrado y verificado para usar con la API.
    *   Permisos necesarios para enviar y recibir mensajes.
    *   Un Token de Acceso (permanente o temporal).
    *   El ID del Número de Teléfono.
*   Una URL pública para tu API (necesaria para que WhatsApp envíe eventos de webhook). Puedes usar ngrok durante el desarrollo local.

## Configuración

### Azure Key Vault

Asegúrate de que tu instancia de Azure Key Vault contenga los siguientes secretos:

1.  `phoneNumberId`: El ID de tu número de teléfono de WhatsApp Business.
2.  `accessToken`: Tu token de acceso para la API de WhatsApp Cloud.

La clase `WhasappCloudSendMessage` (no proporcionada) necesitará acceder a estos secretos. Asegúrate de que la identidad con la que se ejecuta tu aplicación (ej. tu usuario local, Identidad Administrada de Azure) tenga permisos de "Get" para estos secretos en tu Key Vault.

**Nota:** Si la clase `WhasappCloudSendMessage` o su `KeyVaultHelper` tienen el nombre del Key Vault hardcodeado, deberás actualizarlo o, preferiblemente, configurarlo a través de `appsettings.json` o variables de entorno.

### Token de Verificación del Webhook

El token de verificación del webhook está actualmente **hardcodeado** en `WhtasappController.cs`:

Este es el token que configurarás en el portal de Meta for Developers para verificar tu endpoint de webhook.
**Recomendación:** Mueve este token a `appsettings.json` (usando `UserSecrets` para desarrollo local) o a variables de entorno por seguridad y flexibilidad.

### Configuración del Webhook en Meta Developers Portal

En la configuración de tu aplicación de WhatsApp en el Meta for Developers portal:

1.  Ve a la sección "WhatsApp" -> "Configuración de API".
2.  En la sección "Webhooks", haz clic en "Editar".
3.  **URL de Callback:** `https://[TU_URL_PUBLICA]/api/whatsapp` (reemplaza `[TU_URL_PUBLICA]` con tu URL accesible públicamente, ej. la de ngrok).
4.  **Token de Verificación:** Ingresa el valor que tienes en `WhtasappController.cs` (actualmente `54caf5-86b-4962-826f-b861d8e350f8`).
5.  **Suscribirse a los campos del Webhook:** Haz clic en "Administrar" y asegúrate de suscribirte al menos al campo `messages`.

## Instalación y Ejecución

1.  **Clona el repositorio:**

    *(Reemplaza `https://github.com/yeferson-nova/WhatsappNet.git` con la URL real de tu repositorio)*

2.  **Configura Azure Key Vault:**
    *   Asegúrate de que la identidad de tu aplicación tenga acceso a los secretos necesarios.
    *   Si es necesario, actualiza la configuración de acceso al Key Vault en el código (ej. nombre del Key Vault).

3.  **(Recomendado)** Mueve el token de verificación del webhook de `WhtasappController.cs` a `appsettings.json` o variables de entorno y actualiza el código para leerlo desde allí.

4.  **Restaura las dependencias del proyecto:**
    
    
5.  **Construye el proyecto:**

6.  **Ejecuta el proyecto:**
    ## API Endpoints

La ruta base para estos endpoints es `/api/whatsapp`.

### Verificación del Webhook

*   **Endpoint:** `GET /api/whatsapp`
*   **Propósito:** Utilizado por WhatsApp para verificar tu endpoint de webhook durante la configuración inicial.
*   **Query Parameters Requeridos por WhatsApp:**
    *   `hub.mode`: Debe ser `subscribe`.
    *   `hub.verify_token`: Tu token de verificación secreto (el que está hardcodeado o configurado).
    *   `hub.challenge`: Un string aleatorio que tu endpoint debe devolver.
*   **Respuesta Exitosa:** `200 OK` con el valor de `hub.challenge` en el cuerpo de la respuesta.
*   **Respuesta Fallida:** `400 Bad Request` si el token no coincide o faltan parámetros.

### Recepción de Mensajes (Webhook)

*   **Endpoint:** `POST /api/whatsapp`
*   **Propósito:** Recibe notificaciones de eventos de WhatsApp, principalmente mensajes entrantes de usuarios.
*   **Cuerpo de la Solicitud (Request Body):** Un objeto JSON que se mapea a la clase `WhatsAppCloudModel`.
*   **Lógica Principal:**
    1.  Deserializa el cuerpo de la solicitud JSON en un objeto `WhatsAppCloudModel`.
    2.  Extrae el número de teléfono del remitente (`Message.From`).
    3.  Extrae el texto del mensaje del usuario usando el método `GetUserText(Message)`. Este método maneja:
        *   Mensajes de texto (`Message.Text.Body`).
        *   Respuestas de listas interactivas (`Message.Interactive.List_Reply.Title`).
        *   Respuestas de botones interactivos (`Message.Interactive.Button_Reply.Title`).
    4.  Utiliza una instrucción `switch` basada en el texto del usuario (convertido a mayúsculas) para determinar qué tipo de mensaje enviar como respuesta.
    5.  Construye el objeto de mensaje de respuesta utilizando los métodos de la clase `Util` (ej. `_util.TextMessage(...)`, `_util.ImageMessage(...)`).
    6.  Envía el mensaje de respuesta utilizando `_whasappCloudSendMessage.Execute(objectMesage)`.
*   **Respuesta:** `200 OK` con el cuerpo de texto `"EVENT_RECEIVED"`. (Nota: Devuelve esto incluso si ocurre una excepción dentro del `try-catch`, lo cual podría ocultar errores).

### Endpoint de Prueba de Envío

*   **Endpoint:** `GET /api/whatsapp/test`
*   **Propósito:** Permite enviar un mensaje de texto de prueba predefinido para verificar la funcionalidad de envío de mensajes.
*   **Lógica:**
    *   Crea un objeto de mensaje de texto hardcodeado:
        *   Destinatario: `573012811778`
        *   Mensaje: `"hola app"`
    *   Envía este mensaje utilizando `_whasappCloudSendMessage.Execute()`.
*   **Respuesta:** `200 OK` con un booleano indicando el resultado de la operación de envío.

## Uso y Lógica de Respuesta

Una vez que la API esté configurada y el webhook esté activo:

1.  Envía un mensaje desde un número de WhatsApp al número de teléfono de tu empresa configurado con la API de WhatsApp Cloud.
2.  La API procesará el mensaje entrante. Si el texto del mensaje (o el título de la respuesta interactiva) coincide con una de las siguientes palabras clave (insensible a mayúsculas/minúsculas), se enviará una respuesta predefinida:
    *   `"TEXT"`: Envía un mensaje de texto de ejemplo.
    *   `"IMAGE"`: Envía una imagen desde una URL hardcodeada.
    *   `"AUDIO"`: Envía un archivo de audio desde una URL hardcodeada.
    *   `"VIDEO"`: Envía un video desde una URL hardcodeada.
    *   `"DOCUMENT"`: Envía un documento PDF desde una URL hardcodeada.
    *   `"LOCATION"`: Envía un mensaje de ubicación con coordenadas y detalles hardcodeados (Museo del Oro, Bogotá).
    *   `"BUTTON"`: Envía un mensaje interactivo con dos botones ("Aprobar", "Rechazar").
    *   Cualquier otro texto: Envía un mensaje de texto `"NINGUNA OPCION1"`.

Puedes probar la funcionalidad de envío directamente (sin esperar un mensaje entrante) accediendo a `GET /api/whatsapp/test` en tu navegador o con una herramienta como Postman. (Recuerda que el número de destino y el mensaje están hardcodeados en este endpoint).

## Estructura del Proyecto (Clases Clave)

*   `WhatsappNet.Api/`
    *   `Controllers/WhtasappController.cs`: Controlador principal para manejar las solicitudes HTTP.
    *   `Models/WhatsappCloud/WhatsAppCloudModel.cs`: Define los modelos de datos para deserializar las cargas útiles del webhook de WhatsApp.
    *   `Services/WhatsappCloud/SendMessages/`:
        *   `IWhasappCloudSendMessage.cs`: Interfaz para el servicio de envío de mensajes.
        *   `WhasappCloudSendMessage.cs` (Asumido): Implementación del servicio de envío.
    *   `Util/`:
        *   `IUtil.cs`: Interfaz para las utilidades de formato de mensajes.
        *   `Util.cs`: Implementación que crea los objetos de mensaje para enviar a WhatsApp.
    *   `Program.cs` (y `Startup.cs` si es < .NET 6): Configuración de la aplicación ASP.NET Core, inyección de dependencias (como `AddScoped<IUtil, Util>()`, `AddScoped<IWhasappCloudSendMessage, WhasappCloudSendMessage>()`).

## Consideraciones Importantes y Mejoras

*   **Valores Hardcodeados:**
    *   **Token de Verificación del Webhook:** En `WhtasappController.cs`. **Mover a configuración segura.**
    *   **Número de Teléfono de Prueba:** En `WhtasappController.cs` (endpoint `/test`). Hacer configurable si se usa más allá de pruebas básicas.
    *   **URLs de Contenido Multimedia y Detalles de Ubicación:** En `Util.cs`. Estos son solo ejemplos. Para una aplicación real, estos deberían ser dinámicos o configurables.
    *   **Nombre de Azure Key Vault:** Si está hardcodeado en `WhasappCloudSendMessage.cs` o su helper, hacerlo configurable.
*   **Manejo de Errores:** El endpoint `POST /api/whatsapp` devuelve `"EVENT_RECEIVED"` incluso si ocurre una excepción interna. Implementar un logging más robusto y, potencialmente, diferentes códigos de estado HTTP para errores.
*   **Seguridad de Secretos:** Asegurar que el acceso a Azure Key Vault esté restringido y siga el principio de mínimo privilegio.
*   **Validación de Entradas:** Considerar añadir más validaciones a los datos recibidos del webhook, aunque WhatsApp firma las solicitudes.
*   **Escalabilidad y Robustez del Servicio de Envío:** La implementación de `WhasappCloudSendMessage.cs` debe manejar reintentos, timeouts y errores de la API de Graph de forma adecuada.
*   **Flexibilidad de `Util.cs`:** Actualmente, `Util.cs` devuelve `object` (tipos anónimos). Para mayor mantenibilidad y claridad, considera definir clases DTO (Data Transfer Objects) específicas para cada tipo de mensaje que se envía a la API de WhatsApp.
*   **Pruebas Unitarias e Integración:** Añadir pruebas para asegurar la fiabilidad del código.

## Contribuciones

¡Las contribuciones son bienvenidas! Si deseas contribuir, por favor:

1.  Haz un Fork del proyecto.
2.  Crea tu Feature Branch (`git checkout -b feature/AmazingFeature`).
3.  Realiza tus cambios y haz Commit (`git commit -m 'Add some AmazingFeature'`).
4.  Haz Push a la Branch (`git push origin feature/AmazingFeature`).
5.  Abre un Pull Request.

Por favor, asegúrate de que tu código sigue las guías de estilo del proyecto y considera añadir pruebas para tus cambios.

## Licencia

Este proyecto está bajo la Licencia MIT. Consulta el archivo `LICENSE` para más detalles.


---

**Nota Importante sobre el uso de APIs de WhatsApp:**
Asegúrate de cumplir con los Términos de Servicio de WhatsApp Business y la Política de la Plataforma de WhatsApp Business. El uso no autorizado o la violación de estos términos puede llevar a la suspensión de cuentas o acciones legales.

---

    