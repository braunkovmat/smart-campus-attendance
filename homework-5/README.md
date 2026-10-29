# Практическая работа №5, вариант 3: «Учет посещаемости пар»

Задача: **CAMPUS-103**, разделение поля `StudentName` на `FirstName` и `LastName`.

| Блок | Файлы |
|---|---|
| 1. Цифровой след | `src/AttendanceController.cs`, `src/Program.cs` |
| 2. Релиз и Change Management | `docs/change-request-v1.9.0.md`, `docs/branch-protection.md` |
| 3. Миграция БД (Expand and Contract) | `sql/01_expand.sql`, `sql/02_migrate.sql`, `sql/03_contract.sql`, двойная запись в `src/AttendanceService.cs` |
| 4. Sanity-тест и документация | `tests/ProductionSanityTests.cs`, `docs/adr/0009-split-student-name.md`, `CHANGELOG.md` |

`sql/00_initial_schema.sql` нужен только чтобы проверить скрипты на пустой базе.
