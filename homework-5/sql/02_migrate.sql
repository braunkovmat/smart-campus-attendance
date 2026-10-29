-- CAMPUS-103, Фаза 2: MIGRATE (перенос исторических данных)
-- Заполняем first_name / last_name для старых записей, где они ещё пустые.
-- Делаем пачками по 1000 строк, чтобы не держать долгую блокировку.
-- Запускать БЕЗ внешней транзакции (COMMIT внутри DO-блока требует PostgreSQL 11+).

DO $$
DECLARE
    updated integer;
BEGIN
    LOOP
        UPDATE attendance_records
        SET last_name  = split_part(btrim(student_name), ' ', 1),
            first_name = btrim(substr(btrim(student_name),
                                      length(split_part(btrim(student_name), ' ', 1)) + 1))
        WHERE id IN (SELECT id
                     FROM attendance_records
                     WHERE last_name IS NULL OR first_name IS NULL
                     LIMIT 1000);

        GET DIAGNOSTICS updated = ROW_COUNT;
        EXIT WHEN updated = 0;
        COMMIT;
    END LOOP;
END $$;

-- Проверка: обе выборки должны вернуть 0
SELECT count(*) AS not_migrated
FROM attendance_records
WHERE last_name IS NULL OR first_name IS NULL;

SELECT count(*) AS mismatched
FROM attendance_records
WHERE btrim(last_name || ' ' || first_name) <> btrim(student_name);
