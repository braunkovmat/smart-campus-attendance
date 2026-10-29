# Branch Protection Rules для ветки `main`

Настройка в GitHub: **Settings → Branches → Add branch ruleset** (или Add rule), шаблон ветки: `main`.

| Настройка | Значение | Зачем |
|---|---|---|
| Require a pull request before merging | включено | Прямой `git push` в `main` запрещён, всё идёт через Pull Request |
| Required approvals | **2** | Каждое изменение смотрят два человека |
| Dismiss stale pull request approvals when new commits are pushed | включено | После новых коммитов одобрение сбрасывается, нельзя «подменить» код после ревью |
| Require review from Code Owners | включено | Файл `CODEOWNERS` назначает ревьюеров для папок `sql/` и `src/` |
| Require status checks to pass before merging | включено | Слияние только при зелёном CI |
| Required status checks | `build`, `unit-tests`, `sql-lint`, `security-scan` | Сборка, тесты, проверка SQL-миграций, поиск уязвимостей и секретов |
| Require branches to be up to date before merging | включено | Ветка должна быть свежей относительно `main` |
| Require conversation resolution before merging | включено | Все комментарии ревью закрыты |
| Require linear history | включено | Только squash или rebase, история без лишних merge-коммитов |
| Require signed commits | включено | Видно, кто автор коммита |
| Do not allow bypassing the above settings | включено | Правила действуют и для администраторов |
| Allow force pushes / Allow deletions | **выключено** | `main` нельзя переписать или удалить |

## Проверка названия коммита

В CI добавлен шаг, который проверяет, что сообщение коммита начинается с номера задачи:

```
^\[CAMPUS-[0-9]+\] .+
```

Пример: `[CAMPUS-103] Add tracing tags and business logic`. Так по цифровому следу всегда видно связь «задача → коммит → релиз → трасса».
