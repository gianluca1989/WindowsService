# WindowsService

Servizio windows che ogni 10 secondi esegue un Job.
Il job preleva un dato da una coda di RabbitMQ. Il dato è mandato e ricevuto in formato json, (ovviamente è un dato di test).

In questo file vi sono due progetti:
- RabbitApp che inserisce un dato in coda (appunto su RabbitMQ).
  Il dato deve essere inserito in una coda preesistente, (volendo ho lasciato commentato il modo di creare una nuova coda).
- QuartzApp che invece fa partire il servizio e mi permette appunto di prelevare dati inseriti grazie all'altro progetto.
