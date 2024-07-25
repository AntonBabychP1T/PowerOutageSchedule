# PowerOutageSchedule

PowerOutageSchedule - це API для управління розкладом відключень електроенергії для різних груп. API дозволяє імпортувати, експортувати, переглядати та редагувати розклади відключень.

## Вимоги

- .NET SDK 8.0
- IDE (Visual Studio або Visual Studio Code)

## Налаштування проекту

### Клонування репозиторію

```bash
git clone https://github.com/yourusername/PowerOutageSchedule.git
cd PowerOutageSchedule
```

### Налаштування середовища
Відкрийте проект у вашому IDE.
Переконайтеся, що у вас встановлені необхідні пакети NuGet. Якщо ні, додайте їх вручну через командний рядок або через менеджер пакетів у вашому IDE.
Структура проекту
```bash
/PowerOutageSchedule
  /Controllers
    - OutageController.cs
  /DTOs
    - EditScheduleDto.cs
    - ImportScheduleDto.cs
    - TimeIntervalDto.cs
  /Models
    - OutageSchedule.cs
    - TimeInterval.cs
    - DataStore.cs
  /Services
    /Interfaces
      - IOutageImportService.cs
      - IOutageExportService.cs
      - IOutageReadService.cs
      - IOutageEditService.cs
    /Implementations
      - OutageImportExportService.cs
      - OutageReadEditService.cs
  - Program.cs
  - PowerOutageSchedule.csproj
## Запуск проекту
###Команди для запуску
```bash
dotnet build
dotnet run --project PowerOutageSchedule
```
## API Endpoint'и
### Імпорт розкладу відключень
POST /api/outage/import

- Опис: Імпортує розклади відключень з файлу.
- Параметри:
 - - file: Файл з розкладами відключень (.txt)
- Приклад запиту:
```http
POST /api/outage/import HTTP/1.1
Content-Type: multipart/form-data; boundary=---------------------------974767299852498929531610575
Content-Length: 123
-----------------------------974767299852498929531610575
Content-Disposition: form-data; name="file"; filename="schedules.txt"
Content-Type: text/plain

-----------------------------974767299852498929531610575--
```
- Отримання поточних відключень
GET /api/outage/current

Опис: Повертає список груп з поточними відключеннями.
Приклад відповіді:
```json
[
  {
    "groupNumber": 1,
    "outageIntervals": [
      { "start": "08:00", "end": "10:00" }
    ]
  }
]
```
Отримання розкладу для конкретної групи
GET /api/outage/group/{groupNumber}

Опис: Повертає розклад відключень для вказаної групи.
Приклад відповіді:
```json
{
  "groupNumber": 1,
  "outageIntervals": [
    { "start": "08:00", "end": "10:00" }
  ]
}
```
Редагування розкладу для конкретної групи
PUT /api/outage/group/{groupNumber}

Опис: Редагує розклад відключень для вказаної групи.
Параметри:
groupNumber: Номер групи
Тіло запиту:
```json

{
  "outageIntervals": [
    { "start": "08:00", "end": "10:00" }
  ]
}
```
Приклад відповіді:
```http
HTTP/1.1 204 No Content
Експорт розкладу відключень
GET /api/outage/export
```
Опис: Експортує всі розклади відключень у JSON файл.
Приклад відповіді:
```http

HTTP/1.1 200 OK
Content-Disposition: attachment; filename="schedules.json"
Content-Type: application/octet-stream
```

```json
[
    {
        "GroupNumber": 1,
        "OutageIntervals": [
            {
                "Start": "11:00",
                "End": "12:00"
            }
        ]
    },
    {
        "GroupNumber": 2,
        "OutageIntervals": [
            {
                "Start": " 10:00",
                "End": "13:00"
            },
            {
                "Start": " 15:00",
                "End": "18:00"
            },
            {
                "Start": " 20:00",
                "End": "22:00"
            }
        ]
    },
    {
        "GroupNumber": 3,
        "OutageIntervals": [
            {
                "Start": " 11:00",
                "End": "14:00"
            },
            {
                "Start": " 17:00",
                "End": "19:00 "
            }
        ]
    },
    {
        "GroupNumber": 4,
        "OutageIntervals": [
            {
                "Start": " 12:00",
                "End": "15:00"
            },
            {
                "Start": " 17:00",
                "End": "18:00"
            },
            {
                "Start": " 20:00",
                "End": "22:00"
            }
        ]
    },
    {
        "GroupNumber": 5,
        "OutageIntervals": [
            {
                "Start": " 06:00",
                "End": "08:00"
            },
            {
                "Start": " 14:00",
                "End": "17:00 "
            }
        ]
    },
    {
        "GroupNumber": 6,
        "OutageIntervals": [
            {
                "Start": " 10:00",
                "End": "13:00"
            },
            {
                "Start": " 15:00",
                "End": "18:00"
            },
            {
                "Start": " 20:00",
                "End": "22:00"
            }
        ]
    },
    {
        "GroupNumber": 7,
        "OutageIntervals": [
            {
                "Start": " 09:00",
                "End": "12:00"
            },
            {
                "Start": " 15:00",
                "End": "18:00 "
            }
        ]
    },
    {
        "GroupNumber": 8,
        "OutageIntervals": [
            {
                "Start": " 10:00",
                "End": "13:00"
            },
            {
                "Start": " 15:00",
                "End": "18:00"
            },
            {
                "Start": " 20:00",
                "End": "22:00"
            }
        ]
    }
]
```
