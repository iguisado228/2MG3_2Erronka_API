# Hasteko Gida

Gida honek gure APIa erabiltzen hasteko beharrezkoak diren pausoak azaltzen ditu. Jarraitu argibideak zure aplikazioa gure zerbitzuekin konektatzeko.

## 1. Autentifikazioa

APIaren baliabide gehienak babestuta daude eta autentifikazioa behar dute. Autentifikatzeko, `/api/Login` amaierako puntua erabili behar duzu zure langile-kodea eta pasahitza bidaliz.

Adibidez, `LoginRequest` bat bidaliko duzu:

```json
{
    "Langile_kodea": 12345,
    "Pasahitza": "zurePasahitza"
}
```

Erantzun gisa, `LoginErantzuna` bat jasoko duzu, saioa hasi den ala ez adierazten duena. Saioa zuzena bada, `LangileaDto` bat ere jasoko duzu erabiltzailearen datuekin.

```json
{
    "Ok": true,
    "Code": "ok",
    "Message": "Login zuzena",
    "Data": {
        "Id": 1,
        "Izena": "Jon",
        "Erabiltzaile_izena": "jon.doe",
        "Langile_kodea": 12345,
        "Gerentea": false,
        "TpvSarrera": true
    }
}
```

> **Garrantzitsua:** Gorde segurtasunez jasotako autentifikazio-datuak eta erabili hurrengo eskaeretan.

## 2. API Deiak Egiten

Autentifikatu ondoren, APIaren beste amaierako puntuak erabil ditzakezu. Adibidez, erreserbak lortzeko:

```
GET /api/Erreserbak
```

Eskariak sortzeko:

```
POST /api/Eskariak
```

Kontsultatu dagokion kontrolatzailearen dokumentazioa amaierako puntu bakoitzaren xehetasunak ikusteko.

## 3. Erroreak Kudeatzen

APIak errore-egoerak itzul ditzake. Erantzun bakoitzaren `Ok`, `Code` eta `Message` propietateak egiaztatu behar dituzu erroreak kudeatzeko.

*   `Ok: false`: Errore bat gertatu da.
*   `Code`: Errore mota adierazten duen kodea (adibidez, "not_found", "bad_password", "forbidden").
*   `Message`: Erabiltzaileari erakusteko errore-mezu deskriptiboa.

> **Aholkua:** Erabili HTTP bezero bat (adibidez, Postman, Insomnia edo zure programazio-lengoaiaren liburutegi bat) API deiak probatzeko.

## Hurrengo Pausoak

Orain oinarrizko funtzionamendua ulertzen duzula, hurrengo ataletara jo dezakezu APIaren funtzionalitate espezifikoak sakontzeko.
