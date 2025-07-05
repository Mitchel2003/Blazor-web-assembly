# Azure MCP Credentials

Por favor, completa los siguientes campos con la información de tu entorno de Azure. **No compartas este archivo públicamente.**

| Variable                | Valor (rellena aquí)         | Descripción                                                                 |
|-------------------------|------------------------------|-----------------------------------------------------------------------------|
| AZURE_SUBSCRIPTION_ID   |7a48be01-5691-41b0-8915-811ee178b597 | ID de tu suscripción de Azure.                                               |
| AZURE_TENANT_ID         |2bac32fd-d9a2-40d9-a272-3a35920f5607 | ID de tu tenant de Azure Active Directory.                                   |
| AZURE_CLIENT_ID         |(no uso authenticacion de aplicacion) | ID de la aplicación registrada en Azure AD (si usas autenticación de app).  |
| AZURE_CLIENT_SECRET     |(no uso authenticacion de aplicacion) | Secreto de la aplicación registrada (si usas autenticación de app).         |
| AZURE_SQL_SERVER        |prueba-2.database.windows.net        | Nombre del servidor SQL (ej: myserver.database.windows.net).                 |
| AZURE_SQL_DATABASE      |prueba-2                             | Nombre de la base de datos SQL.                                              |
| AZURE_SQL_USER          |prueba_2                             | Usuario de SQL (si aplica).                                                  |
| AZURE_SQL_PASSWORD      |P123456@                             | Contraseña de SQL (si aplica).                                               |

---

## ¿Cómo obtener estos valores?

- **AZURE_SUBSCRIPTION_ID**: Ve al portal de Azure > Suscripciones > selecciona tu suscripción > copia el ID.
- **AZURE_TENANT_ID**: Ve al portal de Azure > Azure Active Directory > Información general > copia el ID de tenant.
- **AZURE_CLIENT_ID** y **AZURE_CLIENT_SECRET**: Si usas autenticación de aplicación, registra una app en Azure AD y copia estos valores.
- **AZURE_SQL_SERVER**: Nombre DNS de tu servidor SQL.
- **AZURE_SQL_DATABASE**: Nombre de la base de datos.
- **AZURE_SQL_USER** y **AZURE_SQL_PASSWORD**: Credenciales de acceso a la base de datos (si usas autenticación SQL).

> **Importante:** No compartas este archivo fuera de tu entorno seguro.


## CSR (Client-Side Rendering):
 `En CSR, el navegador descarga una página HTML vacía y luego ejecuta JavaScript para renderizar el contenido. Esto puede resultar en una experiencia más interactiva y rápida una vez que la página está cargada, pero puede tener un tiempo de carga inicial más lento y afectar negativamente al SEO.`

 `Es adecuado para aplicaciones web altamente interactivas donde la velocidad de carga inicial no es tan crítica.`

## SSR (Server-Side Rendering):
 `En SSR, el servidor genera el HTML completo de la página antes de enviarlo al navegador. Esto resulta en un tiempo de carga inicial más rápido y una mejor indexación por los motores de búsqueda (SEO), pero puede requerir más recursos del servidor y puede ser más complejo de implementar.`

 `Es beneficioso para sitios web donde el SEO y la velocidad de carga inicial son importantes.`

## SSG (Static Site Generation): 
 `En SSG, las páginas HTML se generan durante la compilación y se sirven como archivos estáticos. Esto resulta en tiempos de carga extremadamente rápidos y una excelente experiencia de usuario, pero no es adecuado para sitios web con contenido altamente dinámico o que cambia con frecuencia.`

 `Ideal para sitios web con contenido estático, como blogs, documentación, etc.`

## En resumen: 
 `CSR: Rápido después de la carga inicial, ideal para aplicaciones interactivas.`
 `SSR: Carga rápida inicial, buen SEO, pero puede ser más complejo.`
 `SSG: Carga muy rápida, ideal para contenido estático, pero no para contenido dinámico.`

### ---------------------------------------------------------------------------------------------------- ###