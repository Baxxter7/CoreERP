# CoreERP

Sistema ERP modular basado en microservicios, construido con **.NET 8** y arquitectura de APIs independientes con frontend MVC.

## Arquitectura

```
CoreERP
├── ClientesAPI        → Gestión de clientes
├── ComprasAPI         → Gestión de compras
├── InventariosAPI     → Gestión de inventarios
├── ProductosAPI       → Gestión de productos
├── ProveedoresAPI     → Gestión de proveedores
├── ReportesAPI        → Generación de reportes (PDF)
├── SucursalesAPI      → Gestión de sucursales
├── UsuariosAPI        → Autenticación y gestión de usuarios
├── VentasAPI          → Gestión de ventas
└── WebApp             → Frontend MVC (ASP.NET Core MVC)
```

## Stack Tecnológico

| Componente | Tecnología |
|---|---|
| Framework | .NET 8 |
| Base de datos | SQL Server (via Entity Framework Core 8) |
| Autenticación | JWT Bearer + ASP.NET Core Identity |
| Documentación APIs | Swagger / OpenAPI |
| Generación PDF | QuestPDF |
| Serialización | Newtonsoft.Json |
| Mapeo de objetos | AutoMapper |
| Análisis estático | SonarAnalyzer.CSharp |
| Gestión de paquetes | Central Package Management |

## Puertos de los servicios

| Proyecto | HTTP | HTTPS |
|---|---|---|
| ClientesAPI | 5214 | 7150 |
| ComprasAPI | 5019 | 7024 |
| InventariosAPI | 5156 | 7123 |
| ProductosAPI | 5170 | 7045 |
| ProveedoresAPI | 5031 | 7070 |
| ReportesAPI | 5029 | 7173 |
| SucursalesAPI | 5260 | 7239 |
| UsuariosAPI | 5010 | 7287 |
| VentasAPI | 5179 | 7034 |
| WebApp | 5101 | 7107 |

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB o instancia completa)
- Visual Studio 2022 v17.8+ o Rider 2024.1+
