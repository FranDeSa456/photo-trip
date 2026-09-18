# PhotoTrip

> Piattaforma di scoperta turistica organizzata per regione: luoghi con foto e recensioni della community.
> Progetto scolastico backend, team di 3 persone, durata 2 settimane.
>
> Questo README funge da specifica di riferimento per lo sviluppo.
> Rispecchia le decisioni prese in fase di progettazione: tenerlo aggiornato se l'ambito cambia.

## 1. Panoramica

Gli utenti esplorano regioni, ognuna delle quali raccoglie una serie di luoghi turistici. Ogni luogo ha una foto, una descrizione e delle recensioni lasciate liberamente (senza login) dagli utenti, con voto da 1 a 5.

Obiettivo didattico: dimostrare l'uso integrato di Azure Functions, Azure Blob Storage e di un database relazionale gestito con Entity Framework Core.

## 2. Stack tecnologico

| Componente | Scelta | Note |
|---|---|---|
| Linguaggio | C# | .NET |
| Compute | Azure Functions v4, **modello isolated worker** | `Microsoft.Azure.Functions.Worker`. Non usare il modello in-process: il supporto termina il 10 novembre 2026 |
| Storage file | Azure Blob Storage | SDK `Azure.Storage.Blobs` |
| Database | Database relazionale via EF Core | Azure SQL Database |
| ORM | Entity Framework Core | Migration per lo schema, `AddDbContext` in DI |
| Emulazione locale storage | Azurite | evita di consumare risorse Azure reali durante lo sviluppo |

## 3. Architettura

```
Client (Postman / pagina demo)
        │  HTTP
        ▼
Azure Functions (HTTP trigger)
        │                     │
        ▼                     ▼
   Database (EF Core)    Blob Storage
                          container "luoghi-originali"
                                │
                                ▼ (Blob Trigger)
                          Function di elaborazione
                                │
                                ▼
                          Blob Storage
                          container "luoghi-thumbnail"
```

> **Nota:** **tutte** le chiamate API sono esposte tramite Azure Functions (HTTP trigger): non ci sono controller ASP.NET Core per l'API. (Il requisito era usare almeno una Function; il team ha deciso di usare Functions per tutti gli endpoint, confermato con il prof.)

Flusso tipico di creazione di un luogo:
1. Il client invia `POST /api/luoghi` con i dati del luogo e il file immagine.
2. La Function salva l'immagine originale nel container `luoghi-originali` e crea il record `Luogo` nel database con l'URL dell'immagine.
3. Il salvataggio del blob attiva automaticamente una Function che genera una thumbnail e la salva nel container `luoghi-thumbnail`.
4. (Facoltativo, se semplice da implementare) la Function di thumbnail aggiorna il campo `ThumbnailUrl` del `Luogo` corrispondente nel database.

## 4. Modello dati

Tre entità, due relazioni uno-a-molti: `Regione` → `Luogo` → `Recensione`.

```csharp
public class Regione
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;

    public ICollection<Luogo> Luoghi { get; set; } = new List<Luogo>();
}

public class Luogo
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descrizione { get; set; } = string.Empty;
    public int RegioneId { get; set; }
    public Regione Regione { get; set; } = null!;
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }

    public ICollection<Recensione> Recensioni { get; set; } = new List<Recensione>();
}

public class Recensione
{
    public int Id { get; set; }
    public int LuogoId { get; set; }
    public Luogo Luogo { get; set; } = null!;
    public string NomeAutore { get; set; } = string.Empty;
    public string Testo { get; set; } = string.Empty;
    public int Voto { get; set; }          // 1-5
    public DateTime Data { get; set; }     // UTC, valorizzato dal server
}
```

Note:
- Le **Regioni** sono dati di seed (caricate una volta, es. tramite `HasData` in una migration), non serve un endpoint per crearle.
- Nessuna tabella utenti: `NomeAutore` è un campo libero, non una relazione.

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

## 6. Contratto API

Tutte le route sono sotto `/api`. Payload e risposte in JSON, tranne l'upload foto (`multipart/form-data`).

| Metodo | Route | Descrizione | Body richiesta | Risposta |
|---|---|---|---|---|
| GET | `/api/regioni` | Elenco regioni | — | `200` → `[{ id, nome }]` |
| GET | `/api/regioni/{regioneId}/luoghi` | Luoghi di una regione | — | `200` → `[{ id, nome, thumbnailUrl, valutazioneMedia }]` |
| GET | `/api/luoghi/{id}` | Dettaglio luogo | — | `200` → `{ id, nome, descrizione, regioneId, regioneNome, imageUrl, thumbnailUrl, valutazioneMedia, numeroRecensioni }` · `404` se non esiste |
| POST | `/api/luoghi` | Crea un luogo | `multipart/form-data`: `nome`, `descrizione`, `regioneId`, `foto` (file) | `201` → luogo creato · `400` se validazione fallisce |
| GET | `/api/luoghi/{luogoId}/recensioni` | Recensioni di un luogo | — | `200` → `[{ id, nomeAutore, testo, voto, data }]` |
| POST | `/api/luoghi/{luogoId}/recensioni` | Aggiunge una recensione | `{ nomeAutore, testo, voto }` | `201` → recensione creata · `400` se validazione fallisce · `404` se il luogo non esiste |
| DELETE | `/api/luoghi/{id}` | Cancella un luogo (e le sue recensioni, a cascata) | — | `204` · `404` se non esiste |
| DELETE | `/api/luoghi/{luogoId}/recensioni/{recensioneId}` | Cancella una recensione | — | `204` · `404` se non esiste |

### Regole di validazione
- `nome` (luogo): obbligatorio, max 100 caratteri.
- `descrizione`: obbligatoria, max 1000 caratteri.
- `foto`: obbligatoria alla creazione, solo `.jpg`/`.jpeg`/`.png`, max 5 MB.
- `nomeAutore`: obbligatorio, max 80 caratteri.
- `testo` (recensione): obbligatorio, max 1000 caratteri.
- `voto`: intero tra 1 e 5 inclusi.

## 7. Struttura del progetto proposta

```
/src
  Program.cs                     # setup DI: DbContext, BlobServiceClient
  host.json
  local.settings.json            # NON committare: contiene connection string locali
  /Functions
    RegioniFunctions.cs          # GET /api/regioni, GET /api/regioni/{id}/luoghi
    LuoghiFunctions.cs           # GET/POST /api/luoghi...
    RecensioniFunctions.cs       # GET/POST .../recensioni
    ThumbnailFunction.cs         # Blob Trigger
  /Models
    Regione.cs
    Luogo.cs
    Recensione.cs
  /Data
    AppDbContext.cs
    /Migrations
  /Services
    IBlobStorageService.cs / BlobStorageService.cs
    IThumbnailService.cs / ThumbnailService.cs
```

### Backend a layer
L'applicazione NON deve usare il pattern DTO. Utilizzeremo direttamente i Model di dominio e di entità condivisi tra le sezioni dell'applicazione. La struttura deve seguire la separazione a tre livelli:
- Data Access Layer (DAL): Gestione persistenza Microsoft SQL Server, Repository Pattern, DbContext (Entity Framework Core o Dapper), migrazioni e interazione raw con Microsoft SQL Server.
- Business Logic Layer (BLL): entità e logica di normalizzazione dati grezzi.
- Presentation Layer / API (PL): Azure Functions API per esporre le funzionalità.

## 8. Fuori scope per ora (da valutare solo se avanza tempo)

Non implementare queste funzionalità finché l'MVP non è completo e testato:
- Login/autenticazione utenti.
- Ricerca testuale sui luoghi.
- Front-End

## 9. Setup ambiente di sviluppo

1. Installare Azure Functions Core Tools v4 e .NET SDK (versione ≥ 8).
2. Installare e avviare **Azurite** per emulare il Blob Storage in locale.
3. Predisporre un'istanza SQL Server locale (LocalDB o container Docker).
4. Copiare un `local.settings.json` con:
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
5. Applicare le migration: `dotnet ef database update`.
6. Avviare le Function in locale: `func start`.

## 10. Team e roadmap (2 settimane)

| Persona | Responsabilità |
|---|---|
| A | Setup progetto, Function HTTP, logica di business |
| B | Entità, migration EF Core, query su regioni/luoghi/recensioni |
| C | Integrazione Blob Storage, Blob Trigger per le thumbnail, test end-to-end |

**Settimana 1:** setup del progetto e del repo, database e prime migration, endpoint CRUD di base (regioni, luoghi).
**Settimana 2:** integrazione Blob Storage e trigger thumbnail, recensioni e valutazione media, test end-to-end e rifinitura per la demo.

## 11. Note

- Restare aderenti all'ambito descritto nella sezione 5: non introdurre funzionalità della sezione 8 senza che venga chiesto esplicitamente.
- Usare sempre il modello **isolated worker** per le Azure Functions, mai quello in-process.
- Preferire soluzioni semplici: è un progetto scolastico di 2 settimane per 3 persone, non un prodotto in produzione — evitare over-engineering (no pattern architetturali complessi, no microservizi, no eccessiva astrazione).
