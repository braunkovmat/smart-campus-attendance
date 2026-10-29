-- CAMPUS-103, Фаза 1: EXPAND (расширение)
-- Только ДОБАВЛЯЕМ новые колонки. Старая student_name остаётся, приложение не ломается.
-- Колонки допускают NULL и без DEFAULT, поэтому PostgreSQL добавляет их мгновенно
-- и не блокирует таблицу надолго.

BEGIN;

ALTER TABLE attendance_records
    ADD COLUMN IF NOT EXISTS first_name varchar(100),
    ADD COLUMN IF NOT EXISTS last_name  varchar(100);

COMMIT;

-- После этого скрипта выкатывается версия сервиса v1.9.0 с двойной записью
-- (см. src/AttendanceService.cs, метод MarkAsync): она пишет и в student_name, и в новые поля.
