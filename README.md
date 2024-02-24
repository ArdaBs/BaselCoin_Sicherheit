
# NoSQL Ski Service Manager

- [NoSQL Ski Service Manager](#nosql-ski-service-manager)
- [Projektstruktur](#projektstruktur)
  - [Überblick](#überblick)
  - [Systemvoraussetzungen](#systemvoraussetzungen)
  - [MongoDB Konfiguration](#mongodb-konfiguration)
  - [Projekt-Setup](#projekt-setup)
  - [Verwendung](#verwendung)
  - [Tests](#tests)
  - [Automatisierung und Skripting](#automatisierung-und-skripting)
  - [Zusätzliche anforderung statistische Auswertungsabfragen](#zusätzliche-anforderung-statistische-auswertungsabfragen)
    - [Backup-Automatisierung](#backup-automatisierung)

# Projektstruktur

Das NoSQL Ski Service Manager-Projekt ist sorgfältig in mehrere Verzeichnisse gegliedert, die für verschiedene Aspekte der Anwendung stehen:

- [`/Controllers`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/NoSQLSkiServiceManager/Controllers): Dieses Verzeichnis enthält die Controller für die Web-API, die Anfragen an die Endpunkte steuern.

- [`/Models`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/NoSQLSkiServiceManager/Models): Hier werden die Datenmodelle gespeichert, die die Struktur der in der Anwendung verwendeten Daten definieren.

- [`/Services`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/NoSQLSkiServiceManager/Services): In diesem Ordner finden Sie die Services, die Geschäftslogik implementieren und zwischen Controllern und Datenmodellen vermitteln.

- [`/DTOs`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/NoSQLSkiServiceManager/DTOs): Data Transfer Objects (DTOs) dienen zur Datenübertragung zwischen verschiedenen Schichten der Anwendung.

- [`/Middlewares`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/NoSQLSkiServiceManager/Middlewares): Middleware-Komponenten, die zwischen der Anfrage und der Antwort innerhalb der HTTP-Anforderungspipeline operieren.

- [`/Profiles`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/NoSQLSkiServiceManager/Profiles): Hier sind die AutoMapper-Profile abgelegt, welche die Konfiguration für das Objekt-Mapping definieren.

- [`/Interfaces`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/NoSQLSkiServiceManager/Interfaces): Dieses Verzeichnis beinhaltet Schnittstellen, die für die Definition von Verträgen innerhalb der Anwendung verwendet werden.

- [`/Docs`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/Docs): Hier befindet sich die Dokumentation, einschliesslich relevanter Dokumente wie der `Postman Collection V2.1` und anderer Testprotokolle.
  
- [`/MongoDbScripts`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/MongoDbScripts):  Enthält Skripte für die Erstellung der Datenbank mit Validierung sowie für Backup- und Restore-Funktionalitäten.

Schauen Sie im Ordner [`/Docs`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/Docs) vorbei, um mehr über die Nutzung der Anwendung und die Konfiguration der Postman Collection zu erfahren.

## Überblick

Der NoSQL Ski Service Manager ist eine Backend-Anwendung, entwickelt mit .NET 8.0, die Serviceaufträge für Skiservices verwaltet. Dieses Projekt nutzt MongoDB als NoSQL-Datenbank und bietet eine RESTful API für die Interaktion mit Frontend-Anwendungen.

## Systemvoraussetzungen

- .NET 8.0 SDK
- MongoDB Server (Version 7.0 oder höher)
- Optional: Postman für API-Tests

## MongoDB Konfiguration

Verwenden Sie die `mongod.conf` Datei, um den MongoDB Server zu konfigurieren. Die Konfigurationsdatei befindet sich in der Regel unter `C:\Program Files\MongoDB\Server\7.0\bin\mongod.cfg`, wenn MongoDB für alle Benutzer installiert wurde.

Es ist mir bewusst, dass man in einer produktiven Umgebung dies nicht auskommentiert.

```yaml
# Sicherheitseinstellungen (auskommentiert für Entwicklungszwecke)
# security:
#   authorization: enabled
```

Stellen Sie sicher, dass der MongoDB-Service läuft und auf Port 27017 hört.

## Projekt-Setup

1. Klonen Sie das Repository in Ihr lokales System.
2. Stellen Sie sicher, dass MongoDB läuft und die Konfigurationen korrekt sind.
3. Öffnen Sie das Projekt in Ihrer bevorzugten IDE (z.B. Visual Studio oder VS Code).
4. Stellen Sie sicher, dass alle NuGet-Pakete installiert sind. Wenn NuGet-Pakete nicht automatisch heruntergeladen werden, führen Sie `dotnet restore` aus.
5. Starten Sie das Backend über den Befehl `dotnet run`. Die erforderlichen Collections und Daten werden automatisch initialisiert.

## Verwendung

Die API kann über Endpunkte wie `https://localhost:7095/api/ServiceOrder` oder `https://localhost:7095/api/Employee` angesprochen werden. Für die vollständige Liste der verfügbaren Endpunkte, sehen Sie sich die Controller im Projekt an.

## Tests

Zur Durchführung von API-Tests können Sie die bereitgestellte Postman-Collection verwenden. Diese Sammlung ist mit Collection-Variablen konfiguriert, was bedeutet, dass Sie die Tests flexibel und wiederholt ausführen können, solange das Backend aktiv ist. Importieren Sie einfach die Collection in Postman und führen Sie die Tests nach Bedarf durch.

## Automatisierung und Skripting

Obwohl dieses Projekt so konfiguriert ist, dass alle erforderlichen Komponenten automatisch beim Start der Anwendung erstellt werden, wurden zusätzliche Skripting-Tools bereitgestellt, um die Anforderungen des Dozenten zu erfüllen und um weitere Aspekte der Datenbankintegration zu demonstrieren.

- **PowerShell-Skripte**: Für die Automatisierung von Backup-Prozessen wurden PowerShell-Skripte entwickelt. Diese Skripte sind im Verzeichnis [`/MongoDbScripts`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/MongoDbScripts) zu finden und können nach Bedarf ausgeführt werden, um manuelle Backups der Datenbank zu erstellen und wiederherzustellen.

- **JavaScripts für Datenbank-Integration**: Im gleichen Verzeichnis befinden sich auch JavaScript-Dateien, die verwendet werden können, um Datenbank-Collections und Validierungsregeln direkt im MongoDB-Server zu initialisieren. Diese Skripte dienen als Ergänzung zur automatischen Initialisierung durch das Backend und können für Lehrzwecke oder zur manuellen Einrichtung der Datenbank verwendet werden.

- **Automatisierter Backup im Projekt**: Zusätzlich zu den Skripten ist die Backup-Funktionalität direkt in das Projekt integriert, um eine nahtlose Datensicherung während des Betriebs zu ermöglichen. Dies stellt sicher, dass alle Daten sicher gespeichert werden, ohne dass der Benutzer manuell eingreifen muss.

Diese Skripting-Optionen bieten zusätzliche Flexibilität und ermöglichen es dem Benutzer, die Datenbank-Konfigurationen zu verstehen und nach Belieben anzupassen. Sie dienen auch als praktische Beispiele für die Anwendung von Skripten zur Datenbankverwaltung.

## Zusätzliche anforderung statistische Auswertungsabfragen
Die Abfragen können hier gefunden werden: [`/Statistische_Auswertungsabfragen`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/MongoDbScripts/Statistische_Auswertungsabfragen)

### Backup-Automatisierung

Für die Automatisierung des Backup-Prozesses nutzt dieses Projekt integrierte Executables (`mongodump.exe` und `mongorestore.exe`), die im Verzeichnis [`/MongoTools`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/NoSQLSkiServiceManager/MongoTools) enthalten sind. Diese Herangehensweise gewährleistet, dass Nutzer des Backends keine zusätzlichen Schritte zur Einrichtung von Umgebungsvariablen oder zur Konfiguration von Planungstools wie dem Windows Task Scheduler durchführen müssen.

- **Integrierte Tools**: Die `mongodump.exe` und `mongorestore.exe` Tools sind direkt in das Projekt eingebunden und werden durch projekteigene Prozesse aufgerufen. Dadurch wird ein nahtloses Backup und Restore ermöglicht, das vollständig vom Backend aus gesteuert wird.

- **Vermeidung von manuellen Einstellungen**: Indem wir diese Tools in das Projekt einbinden, vermeiden wir die Notwendigkeit für den Nutzer, `mongodump` oder `mongorestore` zu den Umgebungsvariablen hinzuzufügen oder sich mit externen Planern vertraut zu machen.

- **Produktionsumgebung**: Während in einer Produktionsumgebung oft externe Planungstools wie der Windows Task Scheduler für regelmässige Backups verwendet werden, zielt diese Lösung darauf ab, die Einrichtung und Verwendung des Backends so einfach und direkt wie möglich zu gestalten, ohne auf externe Abhängigkeiten zurückgreifen zu müssen.

Diese Strategie stellt sicher, dass jeder, der das Backend verwendet, von einer integrierten und wartungsarmen Backup-Lösung profitiert, die ohne zusätzliche Konfiguration oder Einrichtung funktioniert.
