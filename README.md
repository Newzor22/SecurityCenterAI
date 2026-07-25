# Security Center AI

Backend inicial del MVP para un copiloto de ciberseguridad. Incluye registro,
login con JWT, perfil básico de usuario, dashboard protegido y PostgreSQL.

## Requisitos

- .NET SDK 8
- Docker Desktop

## Ejecutar en desarrollo

1. Copia `.env.example` como `.env` y cambia sus secretos.
2. Inicia PostgreSQL:

   ```powershell
   docker compose up -d postgres
   ```

3. Crea o actualiza la base de datos:

   ```powershell
   dotnet ef database update --project src/SecurityCenterAI.Infrastructure --startup-project src/SecurityCenterAI.Api
   ```

4. Ejecuta la API:

   ```powershell
   dotnet run --project src/SecurityCenterAI.Api
   ```

Swagger estará disponible en la URL mostrada por .NET, agregando `/swagger`.

## Contrato inicial para el frontend

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/dashboard` (requiere `Authorization: Bearer <token>`)
- `GET /health`

Los secretos de producción deben configurarse mediante variables de entorno:
`ConnectionStrings__PostgreSQL` y `Jwt__Key`. Nunca deben subirse al repositorio.

## Flujo Git recomendado

- `main`: versiones estables.
- `develop`: integración.
- `feature/backend-auth`: trabajo de autenticación.
- `feature/frontend-auth`: pantallas e integración del frontend.
