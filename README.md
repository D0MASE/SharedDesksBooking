
English Version


Shared Desks Booking System


    A full-stack web application designed for booking shared office desks. This project was developed as a technical task for an internship developer position at the IDT team.

Key Features


    Interactive Desk Grid: View all desks with color-coded statuses: Open, Reserved, and Maintenance.

    Smart Date Range Booking: Reserve desks for a specific period with an integrated date range picker.

    Flexible Cancellation: Cancel a reservation for a single day or the entire date range.

    User Profile: View personal reservation history, separated into active and past bookings.

    Hover Context: Detailed information (reserver names or maintenance notes) visible on hover.


Tech Stack & Tools


    Frontend: React with Bootstrap for responsive UI.
    Backend: ASP.NET Core Web API with EF Core In-Memory Database.
    Testing: xUnit for backend unit tests.

    Tools: VS Code, Git/GitHub, Swagger UI (API testing), Axios, and React-datepicker.


Architectural Decisions


    DTO Pattern: Custom Data Transfer Objects (DeskResponseDto, UserProfileDto) for type safety and clean architecture.

    Backend Logic Centralization: All availability calculations and business logic are implemented in the back-end.

    Dependency Injection: Database context is injected into controllers following design patterns.

    Database Seeding: Automatic data seeding is implemented for immediate system testing.


Setup Instructions


    Backend: Navigate to SharedDesksBooking folder, run dotnet run. API: https://localhost:7277.

    Frontend: Navigate to client folder, run npm install and npm start.

    Tests: Run dotnet test in the root directory to execute xUnit tests.



------------------------------------------------------------------------------

Lietuviška versija


Bendro naudojimo stalų rezervavimo sistema


    Pilno ciklo žiniatinklio programa, skirta biuro stalų rezervacijai. Projektas parengtas kaip IDT komandos praktikos atrankos techninė užduotis.


Pagrindinės funkcijos


    Interaktyvus tinklelis: Stalų peržiūra su spalviniais statusais: Laisvas, Rezervuotas ir Priežiūra.

    Pažangus rezervavimas: Galimybė pasirinkti rezervacijos pradžios ir pabaigos datas.

    Lankstus atšaukimas: Rezervacijos atšaukimas pasirinktai dienai arba visam laikotarpiui.
    
    Vartotojo profilis: Rezervacijų istorija, suskirstyta į aktyvias ir praėjusias rezervacijas.

    Kontekstinė informacija: Užvedus pelę matomi rezervavusių asmenų vardai arba priežiūros informacija.


Technologijos ir įrankiai


    Front-end: React su Bootstrap biblioteka.

    Back-end: ASP.NET Core Web API ir EF Core In-Memory duomenų bazė.

    Testavimas: xUnit biblioteka backend unit testams.

    Įrankiai: VS Code, Git/GitHub, Swagger UI, Axios, React-datepicker.


Architektūriniai sprendimai
    
    DTO šablonas: Panaudoti specifiniai duomenų perdavimo objektai (DeskResponseDto, UserProfileDto) duomenų saugumui užtikrinti.

    Logikos sutelkimas serveryje: Visi užimtumo skaičiavimai ir verslo logika atliekami backend dalyje.

    Priklausomybių injekcija (DI): Duomenų bazės kontekstas injekuojamas į kontrolerius laikantis projektavimo šablonų.

    Duomenų seeder: Įdiegtas automatinis pradinių duomenų užpildymas greitam sistemos išbandymui.


Paleidimo instrukcija

    Backend dalis: SharedDesksBooking kataloge paleiskite dotnet run. API: https://localhost:7277.

    Frontend dalis: Kliento kataloge paleiskite npm install ir npm start.

    Testai: Šakniniame kataloge rašykite dotnet test.