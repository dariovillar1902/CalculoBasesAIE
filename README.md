# CalculoBasesAIE API

Una API REST robusta construida con .NET que proporciona toda la lógica de cálculo y verificación de fundaciones en hormigón. Permite que aplicaciones clientes realicen cálculos estructurales complejos, almacenen datos en base de datos y generen reportes en múltiples formatos.

## 📋 Tabla de Contenidos

- [¿Qué es CalculoBasesAIE API?](#qué-es-calculobasesaie-api)
- [Características](#características)
- [Capacidades](#capacidades)
- [Guía para Desarrolladores](#guía-para-desarrolladores)
- [Endpoints Principales](#endpoints-principales)
- [Contribuir](#contribuir)
- [Soporte](#soporte)
- [Licencia](#licencia)

---

## ¿Qué es CalculoBasesAIE API?

CalculoBasesAIE API es el motor de cálculo detrás de la aplicación web CalculoBasesAIEFE. Proporciona todos los servicios necesarios para:

- **Almacenar diseños** de fundaciones en una base de datos
- **Calcular automáticamente** dimensiones, armaduras y costos
- **Verificar la seguridad** de las fundaciones según normas
- **Exportar reportes** en Excel, CSV y PDF
- **Procesar cargas complejas** con múltiples direcciones

### ¿Para quién es?

- Equipos de desarrollo que construyen aplicaciones de ingeniería
- Plataformas web que necesitan lógica de cálculo estructural
- Sistemas que requieren generar reportes técnicos automáticos
- Empresas que desean ofrecer cálculos como servicio

## ✨ Características

**🔧 Cálculos Complejos**
- Calcula dimensiones óptimas de fundaciones
- Determina armadura requerida automáticamente
- Analiza distribución de esfuerzos
- Verifica capacidad portante del suelo

**💾 Gestión de Datos**
- Almacena todos los diseños en base de datos PostgreSQL
- Permite crear, leer, actualizar y eliminar diseños
- Mantiene histórico de modificaciones
- Sincronización automática con clientes

**📊 Generación de Reportes**
- Exporta a Excel con tablas, gráficos y cálculos
- Genera CSV para importar en otras aplicaciones
- Crea PDF profesionales con planos y verificaciones
- Reportes automáticos listos para presentar a clientes

**🔐 Seguridad y Confiabilidad**
- Autenticación y validación de datos
- Cálculos verificados según normas de ingeniería
- Manejo de errores robusto
- CORS configurado para múltiples dominios

**⚡ Rendimiento**
- Caché de resultados para consultas frecuentes
- Optimización de consultas a base de datos
- API rápida y responsive
- Escalable para múltiples usuarios simultáneos

## 🚀 Capacidades

La API puede:

1. **Recibir parámetros** de una fundación (cargas, suelo, hormigón)
2. **Calcular dimensiones** que cumplan con todas las normas
3. **Determinar armadura** con precisión ingenieril
4. **Verificar seguridad** contra múltiples modos de falla
5. **Estimar costos** de materiales y excavación
6. **Generar reportes** profesionales en varios formatos
7. **Almacenar proyectos** para consultas posteriores

---

# 👨‍💻 Guía para Desarrolladores

Esta sección está dirigida a desarrolladores que deseen configurar, ejecutar o contribuir a la API.

## 🔧 Requisitos Previos

- **.NET 8.0** o superior
- **PostgreSQL 13** o superior
- **Git**
- Editor: Visual Studio, Visual Studio Code o Rider

## 📥 Instalación y Configuración

### 1. Clonar el Repositorio

```bash
git clone <URL-del-repositorio>
cd CalculoBasesAIE
```

### 2. Configurar Variables de Entorno

Crea un archivo `appsettings.Development.json` con tu configuración local:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Username=postgres;Password=yourpassword;Database=CalculoBasesAIEDB"
  }
}
```

O configura la variable de entorno `DATABASE_URL`:

```bash
$env:DATABASE_URL = "postgresql://user:password@localhost:5432/CalculoBasesAIEDB"
```

### 3. Aplicar Migraciones de Base de Datos

```bash
dotnet ef database update
```

### 4. Ejecutar la API

```bash
dotnet run
```

La API estará disponible en `https://localhost:7079`

Swagger UI estará en: `https://localhost:7079/swagger`

### 5. Compilar para Producción

```bash
dotnet publish -c Release
```

## 📁 Estructura del Proyecto

```
CalculoBasesAIE/
├── Controllers/                         # Controladores ASP.NET Core
│   ├── BasesHormigonController.cs      # CRUD y cálculos
│   └── BasesHormigonIOController.cs    # Exportación de datos
│
├── Models/                              # Modelos de datos
│   ├── BaseHormigon.cs                 # Modelo principal
│   ├── BaseHormigonDimensiones.cs      # Cálculo de dimensiones
│   ├── BaseHormigonEsfuerzos.cs        # Análisis de esfuerzos
│   ├── BaseHormigonVerificaciones.cs   # Verificaciones de seguridad
│   ├── BaseHormigonArmadura.cs         # Cálculo de armadura
│   ├── BaseHormigonCuantia.cs          # Cuantía de acero
│   ├── BaseHormigonComputo.cs          # Computación de materiales
│   ├── BaseHormigonVerificacionCorte.cs# Verificación de corte
│   ├── BaseHormigonVerificacionPunzonado.cs# Verificación de punzonamiento
│   ├── BaseHormigonContext.cs          # DbContext de Entity Framework
│   └── ValueUnitPair.ts                # Pares valor-unidad
│
├── Services/                            # Lógica de negocios
│   ├── BaseHormigonService/
│   │   ├── IBaseHormigonService.cs     # Interfaz del servicio
│   │   └── BaseHormigonService.cs      # Implementación
│   └── BaseHormigonIOService/
│       ├── IBaseHormigonIOService.cs   # Interfaz I/O
│       └── BaseHormigonIOService.cs    # Exportación de datos
│
├── Repositories/                        # Acceso a datos
│   └── BaseHormigonRepository/
│       ├── IBaseHormigonRepository.cs  # Interfaz del repositorio
│       └── BaseHormigonRepository.cs   # Implementación
│
├── Migrations/                          # Migraciones de Entity Framework
│
├── Properties/                          # Configuración del proyecto
│
├── Program.cs                           # Configuración de la aplicación
├── appsettings.json                     # Configuración general
├── appsettings.Development.json         # Configuración local
├── CalculoBasesAIE.csproj              # Archivo de proyecto
└── Dockerfile                           # Para containerización
```

## 🌐 Endpoints Principales

### Base Hormigón - CRUD

**GET** `/api/baseshormigon`
- Obtiene todas las bases registradas
- Retorna: Lista de BaseHormigon

**GET** `/api/baseshormigon/{id}`
- Obtiene una base específica por ID
- Retorna: BaseHormigon o 404

**POST** `/api/baseshormigon`
- Crea una nueva base
- Body: Objeto BaseHormigon
- Retorna: BaseHormigon creada con ID

**PUT** `/api/baseshormigon/{id}`
- Actualiza una base existente
- Body: Objeto BaseHormigon modificado
- Retorna: 204 No Content

**DELETE** `/api/baseshormigon/{id}`
- Elimina una base
- Retorna: 204 No Content

### Cálculos

**GET** `/api/baseshormigon/{id}/dimensionesBase`
- Calcula dimensiones de la base
- Retorna: BaseHormigonDimensiones

**GET** `/api/baseshormigon/{id}/esfuerzosBase`
- Calcula esfuerzos internos
- Retorna: BaseHormigonEsfuerzos

**GET** `/api/baseshormigon/{id}/verificacionesBase`
- Ejecuta verificaciones de seguridad
- Retorna: BaseHormigonVerificaciones

**GET** `/api/baseshormigon/{id}/calculoCuantia`
- Calcula cuantía de acero
- Retorna: BaseHormigonCuantia

**GET** `/api/baseshormigon/{id}/calculoArmadura`
- Calcula armadura requerida
- Retorna: BaseHormigonArmadura

### Exportación de Datos

**POST** `/api/baseshormigonIO/exportExcel/{baseId}`
- Genera archivo Excel con todos los datos
- Retorna: Archivo .xlsx descargable

**POST** `/api/baseshormigonIO/exportCsv/{baseId}`
- Exporta datos en formato CSV
- Retorna: Archivo .csv descargable

**POST** `/api/baseshormigonIO/exportPdf/{baseId}`
- Genera informe PDF profesional
- Retorna: Archivo .pdf descargable

**POST** `/api/baseshormigonIO/importExcel`
- Importa datos de archivo Excel
- Body: FormData con archivo
- Retorna: BaseHormigon importada

## 🏗️ Stack Tecnológico

| Tecnología | Propósito | Versión |
|-----------|----------|---------|
| **.NET** | Framework principal | 8.0+ |
| **ASP.NET Core** | Framework web | 8.0+ |
| **Entity Framework Core** | ORM para acceso a datos | 8.0+ |
| **PostgreSQL** | Base de datos | 13+ |
| **Npgsql** | Driver PostgreSQL para .NET | 8.0+ |
| **Swagger/OpenAPI** | Documentación de API | 3.0 |

## 🧪 Pruebas

Para ejecutar las pruebas unitarias:

```bash
dotnet test
```

Para ejecutar con cobertura:

```bash
dotnet test /p:CollectCoverage=true
```

## 📝 Convenciones de Código

### Estructura de Controladores

```csharp
[Route("api/[controller]")]
[ApiController]
public class BasesHormigonController : ControllerBase
{
    // Métodos GET, POST, PUT, DELETE
}
```

### Convenciones de Nombres

- **Clases**: PascalCase (ej: `BaseHormigonService`)
- **Métodos**: PascalCase (ej: `GetAllBasesAsync()`)
- **Variables**: camelCase (ej: `baseHormigon`)
- **Constantes**: UPPER_SNAKE_CASE (ej: `MAX_DIMENSION`)

### Métodos Async

Todos los métodos de acceso a datos deben ser asincronos:

```csharp
public async Task<IEnumerable<BaseHormigon>> GetAllBasesAsync()
{
    return await _context.BaseHormigones.ToListAsync();
}
```

## 🐳 Docker

Para ejecutar en un contenedor Docker:

```bash
docker build -t calculobasesaie .
docker run -p 8080:80 --env DATABASE_URL="postgresql://..." calculobasesaie
```

## 🔒 Seguridad

### CORS

La API está configurada para aceptar solicitudes desde:
- `http://localhost:5173` (desarrollo frontend)
- `https://calculo-bases-aie.vercel.app` (producción)

Modifica `Program.cs` para agregar más orígenes permitidos.

### Validación de Entrada

Todos los endpoints validan los datos de entrada:
- Rangos válidos de cargas
- Tipos de datos correctos
- Valores requeridos no nulos

## 📊 Base de Datos

### Diagrama de Entidades

La tabla `BaseHormigones` contiene:
- ID (clave primaria)
- Nombre del proyecto
- Parámetros de carga (axil, corte X/Y, momentos)
- Parámetros del suelo
- Parámetros del hormigón y acero
- Costos unitarios
- Timestamps (creación, modificación)

### Migraciones

Las migraciones están versionadas en la carpeta `Migrations/`. Para crear una nueva migración:

```bash
dotnet ef migrations add NombreDeLaMigracion
dotnet ef database update
```

## 🚀 Despliegue

### En Vercel (recomendado)

1. Crea una cuenta en Vercel
2. Conecta tu repositorio de GitHub
3. Configura las variables de entorno en Vercel:
   - `DATABASE_URL`: Tu conexión a PostgreSQL
4. Vercel compilará y desplegará automáticamente

### En Heroku

```bash
heroku create tu-app
heroku addons:create heroku-postgresql:hobby-dev
git push heroku main
```

## 🤝 Contribuir

Si deseas contribuir al proyecto:

1. **Fork** el repositorio
2. **Crea una rama** para tu feature: `git checkout -b feature/MejoraCálculo`
3. **Realiza tus cambios** y haz commit: `git commit -m 'Add improved calculation'`
4. **Sube a la rama**: `git push origin feature/MejoraCálculo`
5. **Abre un Pull Request**

Por favor, asegúrate de:
- Escribir código limpio y documentado
- Incluir pruebas para nuevas funcionalidades
- Actualizar la documentación si es necesario

## 📞 Soporte

Para reportar bugs o sugerir mejoras, utiliza el [Issue Tracker](https://github.com/tuusuario/CalculoBasesAIE/issues) del proyecto.

## 📄 Licencia

Este proyecto está licenciado bajo la Licencia MIT. Ve el archivo [LICENSE](LICENSE) para más detalles.
