# Contribuir a NexoVida

¡Bienvenido! Nos alegra que estés aquí. Esta guía explica cómo contribuir a
NexoVida de una manera que mantenga el repositorio sano, revisable y seguro de
publicar.

NexoVida es un monorepo que contiene:

- **`backend/`** — API REST en ASP.NET Core 8 (C#).
- **`mobile/`** — Aplicación Flutter (Dart) (escritorio Linux / Android).

Si buscas instrucciones para ejecutar el proyecto localmente, consulta
[`README.md`](README.md).

## En pocas palabras

> Todo cambio pasa por un Pull Request. Nunca hagas push directo a `main` ni a
> `develop`. Crea tu rama a partir de `develop`, nómbrala
> `feature/<scope>/<descripcion>`, escribe tus commits usando
> [Conventional Commits](#convencion-de-mensajes-de-commit), abre un PR hacia
> `develop` y espera a que pasen la CI y al menos una revisión antes de fusionar.
> Las versiones se preparan en `develop`, se promueven a `main` con un PR de
> release y se etiquetan con una versión.

## Índice

- [Código de conducta](#codigo-de-conducta)
- [Estructura del repositorio](#estructura-del-repositorio)
- [Entorno de desarrollo](#entorno-de-desarrollo)
- [Estrategia de ramas](#estrategia-de-ramas)
- [Convención de nombres de ramas](#convencion-de-nombres-de-ramas)
- [Convención de mensajes de commit](#convencion-de-mensajes-de-commit)
- [Flujo de Pull Request](#flujo-de-pull-request)
- [Plantilla de Pull Request](#plantilla-de-pull-request)
- [Code review](#code-review)
- [Pruebas y CI](#pruebas-y-ci)
- [Reportar problemas](#reportar-problemas)
- [Definición de terminado](#definicion-de-terminado)
- [Releases y versionado](#releases-y-versionado)

## Código de conducta

Sé respetuoso, constructivo e inclusivo. Los desacuerdos pasan; discute el
código, no a la persona. Buscamos un entorno de baja fricción donde cualquiera
pueda contribuir sin importar su experiencia.

## Estructura del repositorio

```
.
├── .github/workflows/   # Pipelines de CI/CD (GitHub Actions)
├── backend/             # API REST en ASP.NET Core 8
│   ├── WebApi/          # Proyecto de la API (controllers, middleware, DTOs)
│   ├── Services/        # Implementaciones de la capa de servicios
│   ├── WebApi.Models/   # Modelos de dominio
│   ├── Scripts/         # Esquema SQL y scripts de seed
│   └── README.md
├── mobile/              # Aplicación Flutter
│   ├── lib/             # Código Dart (UI, servicios, modelos, config)
│   └── README.md
├── docs/                # Documentación del proyecto
└── README.md
```

Las notas de arquitectura del backend están en
[`backend/README.md`](backend/README.md); las de la app Flutter, en
[`mobile/README.md`](mobile/README.md).

## Entorno de desarrollo

Antes de contribuir, debes poder compilar y ejecutar el proyecto localmente:

1. **Backend (ASP.NET Core 8)**
   - `dotnet build backend/WebApi/WebApi.csproj`
   - Ejecuta contra un SQL Server local.
2. **Mobile (Flutter)**
   - `flutter analyze`
   - `flutter run -d linux` (o el dispositivo que uses)

Si algo falla al configurarse, pide ayuda antes de empezar a trabajar: un entorno
local roto dificulta la revisión para todos.

## Estrategia de ramas

NexoVida sigue un modelo de **Git Flow adaptado**, porque las versiones se
publican como entregas discretas y no como despliegues continuos.

### Ramas protegidas

La protección de push aplica a **`main`** y **`develop`**:

- Sin push directo.
- Sin force push.
- Sin borrado de ramas.
- Todos los cambios llegan mediante Pull Request.
- Los checks obligatorios (CI) deben pasar.
- Al menos **una aprobación** antes de fusionar (dos para PRs de release).

### Rol de las ramas

| Rama | Propósito | Nace de | Se fusiona en |
| --- | --- | --- | --- |
| `main` | Producción: todo commit en `main` es (o será) una versión publicada. **Siempre publicable.** | — | — |
| `develop` | Rama de integración: donde el trabajo diario converge y los conflictos aparecen antes. Se mantiene usable (compila, pasa pruebas). | — | `main` (en el release) |
| `feature/*` | Un único trabajo, corto y enfocado. | `develop` | `develop` |
| `release/*` | Congelar y estabilizar una versión próxima (solo fixes, docs y bump de versión). | `develop` | `main` **y** `develop` |
| `hotfix/*` | Fix de emergencia para algo ya publicado. Alcance mínimo. | `main` | `main` **y** `develop` |

### Trabajar en una funcionalidad

```bash
git fetch origin
git switch develop
git pull origin develop
git switch -c feature/api/rate-limit-login
# ...trabaja, commitea, trabaja, commitea...
git push -u origin feature/api/rate-limit-login
```

Abre un Pull Request hacia **`develop`**.

### Publicar una versión en `main`

1. Cuando `develop` esté completa para una versión, corta una rama de release:
   `git switch -c release/v1.2.0 develop`.
2. Congela el alcance: solo correcciones de bugs, documentación y bump de versión.
3. Ejecuta la suite completa de pruebas contra la rama de release.
4. Abre un PR de release **hacia `main`**, pide revisión, fusiona y etiqueta:

```bash
git tag v1.2.0
git push origin v1.2.0
```

5. Fusiona la rama de release de vuelta en `develop` para que los fixes lleguen
   al siguiente ciclo.
6. Borra la rama de release.

### Hotfix a producción

```bash
git fetch origin
git switch main
git switch -c hotfix/critical-password-hash-bug
# fix mínimo
git push -u origin hotfix/critical-password-hash-bug
```

Abre un PR hacia `main` (etiqueta el fix) y luego **haz back-merge a `develop`** —
saltarte este paso es como los fixes desaparecen silenciosamente de la siguiente
versión.

## Convención de nombres de ramas

Los nombres de rama van **en inglés**, en minúsculas, **kebab-case** y con un
prefijo de tipo. Usa un alcance cuando ayude (`api`, `db`, `mobile`, `auth`, ...).

| Tipo | Formato | Ejemplo |
| --- | --- | --- |
| Funcionalidad | `feature/<scope>/<descripcion>` | `feature/auth/rate-limit-login` |
| Fix | `fix/<scope>/<descripcion>` | `fix/db/metrica-table-name` |
| Refactor | `refactor/<scope>/<descripcion>` | `refactor/api/dto-validation` |
| Docs | `docs/<descripcion>` | `docs/contributing-guide` |
| Chore | `chore/<descripcion>` | `chore/ci/add-mobile-workflow` |
| Release | `release/v<mayor>.<menor>.<patch>` | `release/v1.2.0` |
| Hotfix | `hotfix/<descripcion-corta>` | `hotfix/jwt-audience-validation` |

Mantén las ramas cortas y enfocadas en **un solo** cambio lógico (a lo mucho unos
días). Si una rama dura más, divide el trabajo.

## Convención de mensajes de commit

Usamos **[Conventional Commits](https://www.conventionalcommits.org/)**, en
**inglés**, para que el historial sirva de changelog y sea compatible con
herramientas de automatización.

Formato:

```
<type>(<scope>): <resumen corto>

<cuerpo: por qué, qué, trade-offs>

<pie: BREAKING CHANGE / referencias a issues>
```

### Tipos

| Tipo | Cuándo usarlo |
| --- | --- |
| `feat` | Funcionalidad nueva visible para el usuario |
| `fix` | Corrección de un bug |
| `docs` | Solo documentación |
| `style` | Formato, espacios en blanco (sin cambio de lógica) |
| `refactor` | Cambio de código que no es ni fix ni feature |
| `test` | Agregar o actualizar pruebas |
| `perf` | Mejora de rendimiento |
| `ci` | Cambios en el pipeline de CI/CD |
| `chore` | Mantenimiento (dependencias, build, tooling) |
| `revert` | Revertir un commit anterior |

### Alcance (opcional)

Usa un alcance para señalar el área afectada: `api`, `db`, `mobile`, `auth`,
`models`, `ui`, etc.

```
feat(api): add login rate limiting
fix(db): use Recordatorios table name in metrics
feat(mobile): add role-based navigation for professionals
```

### Reglas

- Modo imperativo: "Add", "Fix", no "Added", "Fixes".
- Primera línea de ≤ 72 caracteres.
- Sin punto final en el resumen.
- Explica el **por qué** en el cuerpo, no solo el qué (el diff ya muestra el qué).
- Un commit = un cambio coherente. Si el mensaje necesita dos "y", sepáralo.
- Marca los cambios que rompen compatibilidad con `BREAKING CHANGE:` en el pie
  o con un `!` tras el tipo:

```
feat(api): require issuer/audience on JWT validation

BREAKING CHANGE: tokens without iss/aud are now rejected.
```

### Ejemplos

```
fix(auth): use constant-time password comparison
feat(mobile): let professionals edit patient reminders
test(api): cover BOLA/IDOR scoping by role
docs(contributing): document release flow
```

## Flujo de Pull Request

1. **Mantén los PRs pequeños.** Ideal: 200–400 líneas; razonablemente, menos de
   ~800. Un PR es un cambio autocontenido con sus pruebas. Divide el trabajo
   grande en PRs apilados.
2. **Apunta a la rama correcta**: `feature/*` → `develop`; `release/*`/`hotfix/*` → `main`.
3. **Revísate primero.** Lee tu propio diff antes de pedir revisión.
4. **Asegúrate de que la CI pase** antes de pedir review — los revisores nunca
   deberían gastar tiempo en código que falla el lint, el build o las pruebas.
5. **Responde a los comentarios rápido** en la misma rama; haz rebase en vez de
   force-push sobre ramas compartidas. Usa `--force-with-lease` si debes reescribir
   tu propia rama de feature.
6. **Borra la rama** después de que se fusione.

### Estrategia de merge

- `feature/*` → `develop`: **squash merge**, con el título del squash redactado
  como un buen mensaje de Conventional Commit.
- `release/*` y `hotfix/*` → `main`: **merge commit**, para que el momento del
  release sea visible en el historial y fácil de leer con `git log --merges`.

## Plantilla de Pull Request

Todo PR debe responder cinco preguntas:

```markdown
## Why       (Por qué)
¿Qué problema resuelve este cambio? Enlaza el issue/ticket.

## What      (Qué)
Descripción breve de lo que cambió.

## How tested (Cómo se probó)
Pruebas ejecutadas, estado de CI, verificaciones manuales.

## Risk      (Riesgo)
¿Qué podría romperse? ¿Hay algo importante que revisar?

## Rollback  (Revertir)
¿Cómo deshacemos esto si fuera necesario?
```

Si un PR es tan grande que no puedes llenar esto rápidamente, probablemente es
demasiado grande.

## Code review

### Para el autor

- Revisa el diff una vez más antes de pedir revisión.
- Sin outputs de depuración, TODOs, o código muerto.
- Sin secretos, tokens ni cadenas de conexión commiteados.
- Las pruebas cubren el comportamiento nuevo, no solo el camino feliz.

### Para el revisor

- Revisa en ~24h; no seas el cuello de botella.
- Sé específico y constructivo: "Esto podría ser más claro si..." mejor que
  "Esto está confuso".
- Separa los problemas que **bloquean** de los detalles (*nits*).
- Aprueba cuando esté lo suficientemente bien — no exijas perfección.
- Pon especial atención a la **seguridad** en este proyecto: autenticación,
  validación de JWT, scoping por rol (BOLA/IDOR), saneamiento de entradas y
  rate limiting. Ver [`backend/README.md`](backend/README.md) para los controles
  de OWASP API Security.

## Pruebas y CI

Antes de empujar un PR, corre los checks relevantes localmente:

| Proyecto | Comandos |
| --- | --- |
| Backend | `dotnet build backend/WebApi/WebApi.csproj` |
| Backend | `dotnet test` (cuando existan pruebas) |
| Mobile | `flutter analyze` |
| Mobile | `flutter test` (cuando existan pruebas) |

`.github/workflows/backend-ci.yml` corre en `push`/`pull_request` a `main` y
`develop`. La CI es una **puerta**, no una sugerencia — un check que falla
bloquea el merge.

## Reportar problemas

Antes de abrir un issue, busca en los existentes para evitar duplicados. Incluye:

- **Reportes de bug**: comportamiento esperado vs. real, pasos para reproducirlo,
  entorno (SO, versiones de Flutter/Dotnet) y logs relevantes.
- **Solicitudes de funcionalidad / mejoras**: el problema que resolvéis y un
  enfoque sugerido.

## Definición de terminado

Una contribución está *terminada* cuando:

- [ ] La rama está bien nombrada y tiene el scope correcto, basada en `develop`
      (o `main` para hotfixes).
- [ ] Los commits siguen Conventional Commits en inglés.
- [ ] El código compila sin errores ni warnings nuevos.
- [ ] `flutter analyze` pasa limpio en cambios de mobile.
- [ ] Pruebas agregadas/actualizadas y pasando (cuando aplique).
- [ ] Sin secretos commiteados; sin cambios de archivos innecesarios.
- [ ] La descripción del PR responde *Why / What / How tested / Risk / Rollback*.
- [ ] La CI está en verde y al menos un revisor aprobó.
- [ ] El PR apunta a la rama correcta (`develop` por defecto).

## Releases y versionado

Usamos [Semantic Versioning](https://semver.org/): `MAJOR.MINOR.PATCH`.

- `MAJOR` — cambios que rompen compatibilidad (`BREAKING CHANGE` en commits).
- `MINOR` — funcionalidades nuevas (`feat`).
- `PATCH` — correcciones de bugs (`fix`).

Cada release se fusiona en `main`, se etiqueta `vX.Y.Z` y se regresa a `develop`.
Una versión que recorrió el camino completo `develop → release/* → main → tag`
es algo a lo que podemos regresar con confianza.

## Referencias

- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
- [Atlassian — Gitflow Workflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow)
- [GitHub Docs — Setting guidelines for repository contributors](https://docs.github.com/en/communities/setting-up-your-project-for-healthy-contributions/setting-guidelines-for-repository-contributors)
- [OWASP API Security Top 10](https://owasp.org/API-Security/editions/2023/en/0x11-t10/)