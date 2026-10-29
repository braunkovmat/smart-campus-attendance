-- Исходное состояние БД до релиза v1.9.0 (нужно только для проверки скриптов)
CREATE TABLE IF NOT EXISTS attendance_records (
    id           uuid PRIMARY KEY,
    lesson_id    uuid         NOT NULL,
    student_name varchar(200) NOT NULL,          -- формат "Фамилия Имя"
    status       varchar(20)  NOT NULL DEFAULT 'present',
    marked_at    timestamp    NOT NULL DEFAULT now()
);
