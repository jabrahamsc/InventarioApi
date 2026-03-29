# InventarioApi
Sistema web para la gestion de inventario contruido con: ASP.NET core 8, siguiendo una arquitectura limpia por capas y un Front MVC.

# Arquitectura
InventarioApi.sln
-InventarioApi -> Api Rest Backend
-InventarioApi.Web -> Front MVC con Razor Views
-InventarioApi.Tests -> Pruebas unitarias

# Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (v17.8 o superior)
- SQL Server LocalDB

# Instrucciones de ejecución

# 1. Clonar el repositorio

# 2. Restaurar paquetes NuGet

Abrir consola y dirigirse hacia ruta donde se descargo el repositorio, posterior ejecutar el comando: dotnet restore

# 3. Ejecutar la API

Dentro de la carpeta raiz del proyecto en consola ejecutar: cd InventarioApi -> dotnet run --launch-profile https

La API estara disponible en:
- Swagger UI: https://localhost:7001/swagger
- HTTP: http://localhost:5001/swagger

La base de datos se crea y migra automaticamente al iniciar.

# 4. Ejecutar el FrontEnd

Dentro de la carpeta raiz del proyecto en consola ejecutar: cd InventarioApi.Web -> dotnet run --launch-profile https

El frontend estara disponible en:
- HTTPS: https://localhost:7002/Account/Login
- HTTP: http://localhost:5002

# 5. En caso de querer iniciar todo el proyecto desde visual studio 2022 seguir los siguientes pasos:

1. Clic derecho sobre la solucion → "Configurar proyectos de inicio"
2. Seleccionar "Varios Proyectos de Inicio""
3. Colocar la opcion Inicio en  "InventarioApi" e "InventarioApi.Web".
4. Presionar f5

# Credenciales de prueba

1. Usuario: admin, Contraseña: Admin123!, Rol: Administrador
2. Usuario: empleado, Contraseña: Empleado123!, Rol: Empleado

# Permisos para cada rol

Administrador: Ver, editar eliminar productos, Registrar ventas, Ver historial de ventas, ver reposte de stock bajo, registrar usuarios.
Empleado: Ver productos, Registrar ventas, Ver reporte de stock bajo.

# Endpoints de la API

# Autenticacion

Mediante el metodo post /api/auth/login se obtiene token JWT.
Mediante el metodo post /api/auth/register se pueden registrar nuevos usuarios, esto solo funciona si el usuario que intenta ejecutar el metodo
es administrador.


# Productos

Metodo get /api/products retorna lista de todos los productos.
Metodo get /api/products/{id} obtiene un producto por su ID.
Metodo post /api/products crea un producto nuevo, solo funciona si el usuario que intenta ejecutar el metodo es administrador.
Metodo put /api/products/{id} actualiza un producto mediante su ID, solo funciona si el usuario que intenta ejecutar el metodo es administrador.
Metodo delete /api/products/{id} elimina un producto mediante su ID, solo funciona si el usuario que intenta ejecutar el metodo es administrador.
Metodo /api/products/low-stock obtiene todos los productos con existencia por bajo de su establecido como minimo.

# Ventas

Metodo get /api/sales retorna lista de todas las ventas, solo funciona si el usuario que intenta ejecutar el metodo es administrador.
Metodo get /api/sales{id} obtiene la data de una venta mediante su ID, solo funciona si el usuario que intenta ejecutar el metodo es administrador.
Metodo post /api/sales registra una nueva venta.

# Reportes

Metodo get /api/reports/low-stock retorna reporte de productos con existencia por bajo de su establecido como minimo.

# Pruebas unitarias

Las pruebas desarrolladas validan el funcionamiento de los servicios mediante diferentes escenarios usando Moq, explicados mas adelante.
Para ejecutar las mismas seguir estos pasos:

1. Ingresar al directorio raiz del proyecto desde consola
2. Ejecutar comandos: cd InventarioApi.Tests -> dotnet tests

Si se desea ejecutar desde Visual Studio, en la barra de herramientas dar clic en Prueba y en opcion Ejecutar todas las pruebas.

# Pruebas incluidas

-ProductServiceTests: Obtener producto existente retorna datos correctos 
-ProductServiceTests: Obtener producto inexistente retorna KeyNotFoundException 
-ProductServiceTests: Crear producto guarda y retorna respuesta correcta 
-ProductServiceTests: Obtener stock bajo retorna solo productos con deficit
-ProductServiceTests: Eliminar producto inexistente retorna KeyNotFoundException 
-SaleServiceTests: Venta con stock suficiente descuenta stock y crea sale 
-SaleServiceTests: Venta con stock insuficiente retorna InvalidOperationException 
-SaleServiceTests: Venta con producto inexistente retorna KeyNotFoundException


# Decisiones Tecnicas para el proyecto
1. Separar en dos proyectos independientes la funcionalidad de la API y la funcionalidad de la web, de esta manera ninguno de los dos se interesa en como funciona el otro.
2. Arquitectura en capas, cada capa tiene sus funciones especificas y no hay manera de saltarse alguna de ellas.
3. Se manejan las excepciones de tal manera que se busque el evitar que el sistema se quede colgado en algun punto si presenta un error.
4. Capa tests realizada para probar los endpoints de la api sin necesidad de tocar la data en la BD.
5. El front en si nunca llama a la API directamente, por el contrario siempre pasa por el ApiClient lo que permite agregar validaciones a futuro que afecten a todo el sistema automaticamente.
6. Validaciones expecificas para los roles que tienen los usuarios y verificacion por medio de JWT con un loguot de 10 minutos automatico, que en dado caso redirige al login.
