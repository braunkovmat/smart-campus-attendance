# Changelog

Формат основан на [Keep a Changelog](https://keepachangelog.com/ru/1.1.0/),
версии следуют [Semantic Versioning](https://semver.org/lang/ru/).

## [1.9.0] - 2026-11-01

### Added
- [CAMPUS-103] Новые поля `FirstName` и `LastName` в записи посещаемости (таблица `attendance_records`).
- [CAMPUS-103] Двойная запись: сервис сохраняет данные и в старое поле `StudentName`, и в новые поля.
- [CAMPUS-103] Трассировка OpenTelemetry: теги `jira.issue` и `campus.attendance_id` в методах контроллера.
- [CAMPUS-103] Методы `GET /api/attendance/{id}` и `DELETE /api/attendance/{id}`.
- [CAMPUS-103] Production sanity-тест с очисткой тестовых данных.
- ADR-0009 и Change Request CR-103.

### Fixed
- [CAMPUS-103] Старые записи теперь можно сортировать и искать по фамилии (раньше имя хранилось одной строкой).

### Breaking Changes
- Поле `StudentName` объявлено устаревшим (deprecated). Клиентам нужно перейти на `firstName` и `lastName`.
- Поле `student_name` будет удалено из БД и API в версии v2.0.0 (фаза Contract, `03_contract.sql`). В v1.9.0 всё работает как раньше.

## [1.8.2] - 2026-10-15

### Fixed
- Исправлена ошибка при отметке посещаемости на парах с несколькими подгруппами.
