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