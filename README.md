# Blazor Frontend

This is frontend for backend API (either [**Clean Architecture**](https://github.com/skrasekmichael/CleanArchitecture) or [**ModularMonolith**](https://github.com/skrasekmichael/ModularMonolith)) as part of my [**Master's Thesis**](https://github.com/skrasekmichael/ModularInformationSystemThesis) at [BUT FIT](https://www.fit.vut.cz/.en).

Application is implemented using the **Blazor** framework , Blazor Fluent UI library and custom implementation of cache-aside pattern levering browser's local storage.

### Run

Application expects backend API at address *https://localhost:7089* by default, to change API address or port, change `BackendUrl` value in both `src/TeamUp/appsetting.json` and `src/TeamUp.Client/wwwroot/appsetting.json`

### Screenshots

![team](images/team-page.png)
![invite](images/team-invite-panel.png)
![create event](images/create-event-page.png)
![team events](images/team-events-page.png)
![events](images/events-page.png)
![my invitations](images/invitations-page.png)
