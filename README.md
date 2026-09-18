# PhotoTrip

> Piattaforma di scoperta turistica organizzata per regione: luoghi con foto e recensioni della community.
> Progetto scolastico, team di 3 persone, durata 2 settimane.
>
> Questo README funge da specifica di riferimento per lo sviluppo.
> Rispecchia le decisioni prese in fase di progettazione: tenerlo aggiornato se l'ambito cambia.

## 1. Panoramica

Gli utenti esplorano regioni, ognuna delle quali raccoglie una serie di luoghi turistici. Ogni luogo ha una foto, una descrizione e delle recensioni lasciate liberamente (senza login) dagli utenti, con voto da 1 a 5.

Obiettivo didattico: dimostrare l'uso integrato di Azure Functions, Azure Blob Storage, di un database relazionale gestito con Entity Framework Core e di un front-end Angular.

## 2. Stack tecnologico

| Componente | Scelta | Note |
|---|---|---|
| Linguaggio | C# | .NET |
| Compute | Azure Functions v4, **modello isolated worker** | `Microsoft.Azure.Functions.Worker`. Non usare il modello in-process: il supporto termina il 10 novembre 2026 |
| Storage file | Azure Blob Storage | SDK `Azure.Storage.Blobs` |
| Database | Database relazionale via EF Core | Azure SQL Database |
| ORM | Entity Framework Core | Migration per lo schema, `AddDbContext` in DI |
| Emulazione locale storage | Azurite | evita di consumare risorse Azure reali durante lo sviluppo |
| Front-end | Angular (standalone) | progetto `PhotoTrip.PL.ANGULAR`, consuma le Functions via HTTP |

## 3. Architettura

```
Angular (PhotoTrip.PL.ANGULAR)
        │  HTTP (proxy dev su localhost:7071)
        ▼
Azure Functions (HTTP trigger)
        │                     │
        ▼                     ▼
   Database (EF Core)    Blob Storage
                         container "places-originals"
                               │
                               ▼ (Blob Trigger)
                         Function di elaborazione
                               │
                               ▼
                         Blob Storage
                         container "places-thumbnails"
```

> **Nota:** **tutte** le chiamate API sono esposte tramite Azure Functions (HTTP trigger): non ci sono controller ASP.NET Core per l'API, né sul front-end (nessun MVC). (Il requisito era usare almeno una Function; il team ha deciso di usare Functions per tutti gli endpoint, confermato con il prof.)

Flusso tipico di creazione di un luogo:
1. Il client Angular invia `POST /api/places` con i dati del luogo e il file immagine.
2. La Function salva l'immagine originale nel container `places-originals` e crea il record `Place` nel database con l'URL dell'immagine.
3. Il salvataggio del blob attiva automaticamente una Function che genera una thumbnail e la salva nel container `places-thumbnails`.
4. (Facoltativo, se semplice da implementare) la Function di thumbnail aggiorna il campo `ThumbnailUrl` del `Place` corrispondente nel database.

## 4. Modello dati

Tre entità, due relazioni uno-a-molti: `Region` → `Place` → `Review`.

```csharp
public class Region
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Place> Places { get; set; } = new List<Place>();
}

public class Place
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int RegionId { get; set; }
    public Region Region { get; set; } = null!;
    public string ImageUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}

public class Review
{
    public int Id { get; set; }
    public int PlaceId { get; set; }
    public Place Place { get; set; } = null!;
    public string AuthorName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Rating { get; set; }          // 1-5
    public DateTime Date { get; set; }       // UTC, valorizzato dal server
}
```

Note:
- Le **Regioni** sono dati di seed (caricate una volta, es. tramite `HasData` in una migration), non serve un endpoint per crearle.
- Nessuna tabella utenti: `AuthorName` è un campo libero, non una relazione.

## 5. Ambito del progetto — MVP (le due settimane)

Queste sono le funzionalità da implementare. Non aggiungere altro senza discuterne prima (vedi sezione 8, fuori scope).

### 5.1 Regioni
- Elenco di tutte le regioni.
- Elenco dei luoghi appartenenti a una regione.

### 5.2 Luoghi
- Creazione di un luogo (nome, descrizione, regione di appartenenza, foto).
- Lettura del dettaglio di un singolo luogo (inclusa valutazione media e numero di recensioni).
- Elenco dei luoghi, filtrabile per regione.
- Cancellazione di un luogo (le recensioni associate vengono eliminate a cascata dal DB).

### 5.3 Foto e Blob Storage
- Upload della foto originale del luogo su Blob Storage, salvataggio dell'URL nel database.
- Generazione automatica di una thumbnail alla creazione del blob originale.

### 5.4 Recensioni
- Aggiunta di una recensione a un luogo (nome autore libero, testo, voto 1-5).
- Elenco delle recensioni di un luogo.
- Calcolo della valutazione media di un luogo (calcolata al volo nella query, non serve un campo salvato).
- Cancellazione di una recensione.

### 5.5 Front-end (Angular)
- App Angular in `PhotoTrip.PL.ANGULAR`, unica interfaccia utente del progetto (sostituisce il vecchio progetto MVC).
- Consuma esclusivamente le Azure Functions via HTTP; nessun accesso diretto a DB o BLL.
- In sviluppo usa il proxy del dev server verso `http://localhost:7071` (vedi `proxy.conf.json`), quindi nessuna configurazione CORS in locale; in Azure va configurato CORS sul Function App.

## 6. Contratto API

Tutte le route sono sotto `/api`, in inglese. Payload e risposte in JSON, tranne l'upload foto (`multipart/form-data`).

| Metodo | Route | Descrizione | Body richiesta | Risposta |
|---|---|---|---|---|
| GET | `/api/regions` | Elenco regioni | — | `200` → `[{ id, name }]` |
| GET | `/api/regions/{regionId}/places` | Luoghi di una regione | — | `200` → `[{ id, name, thumbnailUrl, averageRating }]` |
| GET | `/api/places/{id}` | Dettaglio luogo | — | `200` → `{ id, name, description, regionId, regionName, imageUrl, thumbnailUrl, averageRating, reviewCount }` · `404` se non esiste |
| POST | `/api/places` | Crea un luogo | `multipart/form-data`: `name`, `description`, `regionId`, `photo` (file) | `201` → luogo creato · `400` se validazione fallisce |
| GET | `/api/places/{placeId}/reviews` | Recensioni di un luogo | — | `200` → `[{ id, authorName, text, rating, date }]` |
| POST | `/api/places/{placeId}/reviews` | Aggiunge una recensione | `{ authorName, text, rating }` | `201` → recensione creata · `400` se validazione fallisce · `404` se il luogo non esiste |
| DELETE | `/api/places/{id}` | Cancella un luogo (e le sue recensioni, a cascata) | — | `204` · `404` se non esiste |
| DELETE | `/api/places/{placeId}/reviews/{reviewId}` | Cancella una recensione | — | `204` · `404` se non esiste |

### Regole di validazione
- `name` (luogo): obbligatorio, max 100 caratteri.
- `description`: obbligatoria, max 1000 caratteri.
- `photo`: obbligatoria alla creazione, solo `.jpg`/`.jpeg`/`.png`, max 5 MB.
- `authorName`: obbligatorio, max 80 caratteri.
- `text` (recensione): obbligatorio, max 1000 caratteri.
- `rating`: intero tra 1 e 5 inclusi.

## 7. Struttura del progetto

```
PhotoTrip.slnx
  PhotoTrip.DAL/            # entità, DbContext, Repository + UnitOfWork
  PhotoTrip.BLL/            # servizi + modelli, AutoMapper
  PhotoTrip.Functions/      # API (HTTP trigger) + Blob Trigger thumbnail
  PhotoTrip.PL.ANGULAR/     # front-end Angular (fuori dalla solution .NET)
  PhotoTrip.PL.MVC/         # DEPRECATO: da rimuovere previo accordo del team
```

Dettaglio `PhotoTrip.Functions`:
```
Program.cs                       # setup DI: DbContext, BlobServiceClient
host.json
local.settings.json              # NON committare: contiene connection string locali
/Functions
  RegionFunctions.cs             # GET /api/regions, GET /api/regions/{id}/places
  PlaceFunctions.cs              # GET/POST /api/places...
  ReviewFunctions.cs             # GET/POST .../reviews
  ThumbnailFunction.cs           # Blob Trigger
/Configurations
  MappingProfile.cs              # profilo AutoMapper
```

Dettaglio `PhotoTrip.PL.ANGULAR`:
```
proxy.conf.json                  # proxy dev /api → localhost:7071
src/environments/environment*.ts # apiBaseUrl
src/app/models/                  # interfacce TypeScript dei modelli API
src/app/services/                # client HTTP verso le Functions
```

### Backend a layer
L'applicazione NON deve usare il pattern DTO. Utilizzeremo direttamente i Model di dominio e di entità condivisi tra le sezioni dell'applicazione. La struttura deve seguire la separazione a tre livelli:
- Data Access Layer (DAL): Gestione persistenza Microsoft SQL Server, Repository Pattern, DbContext (Entity Framework Core o Dapper), migrazioni e interazione raw con Microsoft SQL Server.
- Business Logic Layer (BLL): entità e logica di normalizzazione dati grezzi.
- Presentation Layer / API (PL): Azure Functions API per esporre le funzionalità; Angular per l'interfaccia utente.

## 8. Fuori scope per ora (da valutare solo se avanza tempo)

Non implementare queste funzionalità finché l'MVP non è completo e testato:
- Login/autenticazione utenti.
- Ricerca testuale sui luoghi.

## 9. Setup ambiente di sviluppo

1. Installare Azure Functions Core Tools v4, .NET SDK (versione ≥ 8) e Node.js LTS (≥ 20).
2. Installare e avviare **Azurite** per emulare il Blob Storage in locale.
3. Predisporre un'istanza SQL Server locale (LocalDB o container Docker).
4. Copiare un `local.settings.json` in `PhotoTrip.Functions/` con:
   ```json
   {
     "IsEncrypted": false,
     "Values": {
       "AzureWebJobsStorage": "UseDevelopmentStorage=true",
       "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
       "SqlConnectionString": "<connection string locale>"
     }
   }
   ```
5. Applicare le migration: `dotnet ef migrations add <Nome> --project PhotoTrip.DAL --startup-project PhotoTrip.Functions`, poi `dotnet ef database update --project PhotoTrip.DAL --startup-project PhotoTrip.Functions`.
6. Avviare le Function in locale: `func start` dentro `PhotoTrip.Functions/`.
7. Avviare il front-end: `npm install && npm start` dentro `PhotoTrip.PL.ANGULAR/`, poi aprire `http://localhost:4200`.

## 10. Team e roadmap (2 settimane)

| Persona | Responsabilità |
|---|---|
| A | Setup progetto, Function HTTP, logica di business |
| B | Entità, migration EF Core, query su regioni/luoghi/recensioni |
| C | Integrazione Blob Storage, Blob Trigger per le thumbnail, test end-to-end |

**Settimana 1:** setup del progetto e del repo, database e prime migration, endpoint CRUD di base (regioni, luoghi).
**Settimana 2:** integrazione Blob Storage e trigger thumbnail, recensioni e valutazione media, front-end Angular, test end-to-end e rifinitura per la demo.

## 11. Note

- Restare aderente all'ambito descritto nella sezione 5: non introdurre funzionalità della sezione 8 senza che venga chiesto esplicitamente.
- Usare sempre il modello **isolated worker** per le Azure Functions, mai quello in-process.
- Route API in inglese (`/api/regions`, `/api/places`, `/api/reviews`); codice in inglese, documentazione in italiano.
- Preferire soluzioni semplici: è un progetto scolastico di 2 settimane per 3 persone, non un prodotto in produzione — evitare over-engineering (no pattern architetturali complessi, no microservizi, no eccessiva astrazione).
