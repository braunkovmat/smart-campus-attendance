-- CAMPUS-103, Фаза 3: CONTRACT (сжатие)
-- Запускать ТОЛЬКО когда все клиенты и сервисы перешли на first_name / last_name
-- (в логах за неделю нет запросов, которые читают student_name).

BEGIN;

-- 1. Страховка: если остались незаполненные строки, скрипт остановится с ошибкой
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM attendance_records
               WHERE last_name IS NULL OR first_name IS NULL) THEN
        RAISE EXCEPTION 'Есть непереехавшие строки, сначала выполните 02_migrate.sql';
    END IF;
END $$;

-- 2. Новые поля становятся обязательными
ALTER TABLE attendance_records
    ALTER COLUMN last_name  SET NOT NULL,
    ALTER COLUMN first_name SET NOT NULL;

-- 3. Удаляем старую колонку
ALTER TABLE attendance_records DROP COLUMN student_name;

COMMIT;
